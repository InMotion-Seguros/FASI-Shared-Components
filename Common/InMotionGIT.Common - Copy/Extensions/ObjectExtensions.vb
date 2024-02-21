Imports System.Runtime.CompilerServices

Namespace Extensions

    ''' <summary>
    ''' Extension methods for the object data type
    ''' </summary>
    Public Module ObjectExtensions

        <Extension()>
        Public Function IsNotEmpty(source As Dictionary(Of String, Object)) As Boolean
            Dim result As Boolean
            If source IsNot Nothing AndAlso source.Count <> 0 Then
                result = True
            End If
            Return result
        End Function

        <Extension()>
        Public Function ToStringExtended(source As Dictionary(Of String, Object)) As String
            Return String.Join(", ", source.Select(Function(kvp) String.Format("{0}= {1}", kvp.Key, kvp.Value)).ToArray())
        End Function

        ''' <summary>
        ''' Este método permite realizar una clonación completa por medio de memberwiseclone
        ''' </summary>
        ''' <typeparam name="T"></typeparam>
        ''' <param name="source"></param>
        ''' <returns></returns>
        <Extension()>
        Public Function CloneObject(Of T As Class)(source As T) As T
            If source Is Nothing Then
                Return Nothing
            End If
            Dim inst As System.Reflection.MethodInfo = source.GetType().GetMethod("MemberwiseClone", System.Reflection.BindingFlags.Instance Or System.Reflection.BindingFlags.NonPublic)
            If inst IsNot Nothing Then
                Dim result = DirectCast(inst.Invoke(source, Nothing), T)
                CloneObjectBase(source, result)
                Return result
            Else
                Return Nothing
            End If
        End Function

        Private Sub CloneObjectBase(Of T As Class)(source As T, result As T)
            For Each itemPropertyes As System.Reflection.PropertyInfo In source.GetType.GetProperties
                If IsNotCoreType(itemPropertyes.PropertyType) Then
                    Dim instanceObjectFromBase = GetPropValue(source, itemPropertyes.Name)
                    Dim inntanceObjectCloned = CloneInternal(instanceObjectFromBase)
                    If result.IsNotEmpty Then
                        Dim propertyResult As System.Reflection.PropertyInfo = result.GetType.GetProperty(itemPropertyes.Name)
                        If propertyResult.IsNotEmpty Then
                            propertyResult.SetValue(result, Convert.ChangeType(inntanceObjectCloned, propertyResult.PropertyType), Nothing)
                        End If
                    End If
                    CloneObjectBase(instanceObjectFromBase, inntanceObjectCloned)
                End If
            Next
        End Sub

        Private Function GetPropValue(src As Object, propName As String) As Object
            Return src.[GetType]().GetProperty(propName).GetValue(src, Nothing)
        End Function

        Private Function CloneInternal(Of T As Class)(source As T) As T
            If source Is Nothing Then
                Return Nothing
            End If
            Dim inst As System.Reflection.MethodInfo = source.GetType().GetMethod("MemberwiseClone", System.Reflection.BindingFlags.Instance Or System.Reflection.BindingFlags.NonPublic)
            If inst IsNot Nothing Then
                Dim result = DirectCast(inst.Invoke(source, Nothing), T)
                Return result
            Else
                Return Nothing
            End If
        End Function

        Public Function IsNotCoreType([type] As Type) As Boolean
            Return ([type] <> GetType(Object) AndAlso Type.GetTypeCode([type]) = TypeCode.Object)
        End Function

        <Extension()>
        Public Function IsEmpty(ByVal value As Object) As Boolean
            Return IsNothing(value)
        End Function

        <Extension()>
        Public Function IsNotEmpty(ByVal value As Object) As Boolean
            Return Not IsNothing(value)
        End Function

        ''' <summary>
        ''' Checks whether the property in the object/ Verifica si existe la propiedad en el objeto
        ''' </summary>
        ''' <param name="srcObject"></param>
        ''' <param name="propertyName"></param>
        ''' <returns></returns>
        <Extension()>
        Public Function ExistsProperty(srcObject As Object, propertyName As String) As Boolean
            If srcObject Is Nothing Then
                Throw New System.ArgumentNullException("srcObject")
            End If

            If (propertyName Is Nothing) OrElse (propertyName = String.Empty) OrElse (propertyName.Length = 0) Then
                Throw New System.ArgumentException("Property name cannot be empty or null.")
            End If

            Dim propInfoSrcObj As System.Reflection.PropertyInfo = srcObject.[GetType]().GetProperty(propertyName)

            Return (propInfoSrcObj IsNot Nothing)
        End Function

        <Extension()>
        Public Function GetTypeProperty(srcObject As Object, propertyName As String) As Type
            If srcObject Is Nothing Then
                Throw New System.ArgumentNullException("srcObject")
            End If

            If (propertyName Is Nothing) OrElse (propertyName = String.Empty) OrElse (propertyName.Length = 0) Then
                Throw New System.ArgumentException("Property name cannot be empty or null.")
            End If

            Dim propInfoSrcObj As System.Reflection.PropertyInfo = srcObject.[GetType]().GetProperty(propertyName)

            Return propInfoSrcObj.PropertyType
        End Function

    End Module

End Namespace