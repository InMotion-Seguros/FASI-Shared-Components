#Region "using"

Imports System.IO
Imports System.Reflection
Imports System.Text.RegularExpressions
Imports InMotionGIT.Assembly.Model.Enumerations
Imports InMotionGIT.Assembly.Model.Data
Imports InMotionGIT.Common.Extensions
Imports InMotionGIT.Common
Imports System.Globalization

#End Region

'   Dim x As New InMotionGIT.Assembly.Model.assemblyLoader

'   x.Load("InMotionGIT.Policy.Entity.Contracts.dll", "C:\Users\Nelson\AppData\Local\DesignerWorkbench\Bin")

'   Debug.Print(x.GetElementType("InMotionGIT.Policy.Entity.Contracts.RiskInformation", "RiskInformation.BuildPolicyInstance.Agency"))


Public Class assemblyLoader

    Private Shared CurrentAssembly As String
    Private Shared DepedencyAssembly As String
    Dim _tempContainer As Dictionary(Of String, Dictionary(Of String, Dictionary(Of String, EntityHierarchy)))
    Private _xmlDocument As XDocument

    Private AssemblyLoaded As String

    Property [Class] As EntityCollection

    Public Sub New()
        [Class] = New EntityCollection
        AssemblyLoaded = String.Empty
    End Sub

    Public Sub Clear()
        [Class] = New EntityCollection
        AssemblyLoaded = String.Empty
    End Sub

    Public Sub Load(ByVal filePattern As String, ByVal excludePattern As String, ByVal mode As EnumLoadMode)
        Dim path As String = filePattern.Substring(0, filePattern.LastIndexOf("\"))
        Dim pattern As String = filePattern.Substring(filePattern.LastIndexOf("\") + 1)

        AddHandler(AppDomain.CurrentDomain.AssemblyResolve), AddressOf assemblyLoader.CurrentDomain_AssemblyResolve

        For Each fileName As String In Directory.GetFiles(path, pattern)
            If String.IsNullOrEmpty(excludePattern) OrElse Not Regex.IsMatch(fileName, excludePattern, RegexOptions.IgnoreCase) Then
                LoadAssemblyInformation(fileName, mode, String.Empty)
            End If
        Next

        RemoveHandler(AppDomain.CurrentDomain.AssemblyResolve), AddressOf assemblyLoader.CurrentDomain_AssemblyResolve
    End Sub

    Public Sub Load(ByVal assemblyName As String, ByVal mode As EnumLoadMode)
        AddHandler(AppDomain.CurrentDomain.AssemblyResolve), AddressOf assemblyLoader.CurrentDomain_AssemblyResolve

        LoadAssemblyInformation(assemblyName, mode, String.Empty)

        RemoveHandler(AppDomain.CurrentDomain.AssemblyResolve), AddressOf assemblyLoader.CurrentDomain_AssemblyResolve
    End Sub

    Public Sub LoadAssemblyList(AssemblyList As String, PathBase As String, mode As EnumLoadMode)

        AddHandler(AppDomain.CurrentDomain.AssemblyResolve), AddressOf assemblyLoader.CurrentDomain_AssemblyResolve

        For Each fileName As String In AssemblyList.Split(";")
            LoadAssemblyInformation(String.Format(CultureInfo.InvariantCulture, "{0}\{1}", PathBase, IO.Path.GetFileName(fileName)), mode, String.Empty)

            If DepedencyAssembly.IsNotEmpty Then
                Dim localDependency As String = DepedencyAssembly
                For Each fileNameItem As String In localDependency.Split(";")
                    If AssemblyLoaded.IndexOf(fileNameItem) = -1 Then
                        LoadAssemblyInformation(String.Format("{0}\{1}", PathBase, IO.Path.GetFileName(fileNameItem)), EnumLoadMode.Member, String.Empty)
                    End If
                Next
            End If
        Next

        Dim entityid As Integer = 1
        Dim elementid As Integer = 1
        Dim doContinue As Boolean = True

        Do While doContinue
            doContinue = False

            For Each itemEntity As Entity In [Class]
                If itemEntity.elements.Count = 0 Then
                    [Class].Remove(itemEntity)
                    doContinue = True

                    Exit For
                End If
            Next
        Loop

        Dim entityData As Entity = Nothing

        For Each itemEntity As Entity In [Class]
            itemEntity.classid = entityid
            entityid += 1

            For Each itemElement As Element In itemEntity.elements
                itemElement.memberid = elementid
                elementid += 1

                If itemElement.isComplexType Then
                    entityData = [Class].GetItemByFullName(itemElement.basetypefullname)

                    If Not IsNothing(entityData) Then
                        itemElement.iscollection = entityData.iscollection
                    End If
                End If
            Next
        Next

        RemoveHandler(AppDomain.CurrentDomain.AssemblyResolve), AddressOf assemblyLoader.CurrentDomain_AssemblyResolve
    End Sub

    ''' <summary>
    ''' Procesa toda la información de un dll
    ''' </summary>
    ''' <param name="assemblyFileName">Nombre del dll</param>
    ''' <param name="mode">Filtro usado para definri la información a extraer</param>
    Private Sub LoadAssemblyInformation(assemblyFileName As String, mode As EnumLoadMode, ByRef errors As String)
        Dim localAssembly As Reflection.Assembly = Nothing
        Dim entityItem As Entity
        Dim parameterList As String = String.Empty
        Dim parameterDeclare As String = String.Empty
        Dim xmlFileDocument As String = Path.Combine(Path.GetDirectoryName(assemblyFileName), (Path.GetFileNameWithoutExtension(assemblyFileName) & ".xml"))
        Dim parameterInstance As Parameter = Nothing
        Dim newElement As Element
        Dim MemberEnglishDesignerField() As Attributes.ElementEnglishDesigner
        Dim MemberSpanishDesignerField() As Attributes.ElementSpanishDesigner
        Dim MemberBehaviorField() As Attributes.ElementBehaviorAttribute
        Dim MemberRequiredField() As Attributes.ElementRequiredAttribute
        Dim MemberLookUpField() As Attributes.ElementLookupAttribute
        Dim loadEntity As Boolean = False

        Try
            If AssemblyLoaded.IndexOf(assemblyFileName) = -1 Then
                'For Each item As Reflection.Assembly In AppDomain.CurrentDomain.GetAssemblies
                '    If Not item.IsDynamic AndAlso String.Equals(IO.Path.GetFileNameWithoutExtension(item.Location), IO.Path.GetFileNameWithoutExtension(assemblyFileName), StringComparison.CurrentCultureIgnoreCase) Then
                '        localAssembly = item
                '        Exit For
                '    End If
                'Next

                If IsNothing(localAssembly) Then
                    If IO.File.Exists(assemblyFileName) Then
                        localAssembly = System.Reflection.Assembly.LoadFile(assemblyFileName)
                    Else
                        errors += String.Format(CultureInfo.InvariantCulture, "The assembly '{0}' not found{1}", assemblyFileName, vbCrLf)
                    End If
                End If

                If File.Exists(xmlFileDocument) Then
                    Try
                        _xmlDocument = XDocument.Load(xmlFileDocument)
                    Catch ex As Exception
                        _xmlDocument = Nothing
                    End Try

                Else
                    _xmlDocument = Nothing
                End If

                DepedencyAssembly = String.Empty
                CurrentAssembly = assemblyFileName

                Dim types As Type() = Nothing
                Try
                    types = localAssembly.GetTypes()
                Catch ex As Exception
                    types = Nothing
                End Try

                If types.IsNotEmpty Then

                    For Each typeInstance As Type In types
                        If (IsNothing(typeInstance.Namespace) OrElse Not typeInstance.Namespace.EndsWith(".My", StringComparison.CurrentCultureIgnoreCase)) AndAlso Not typeInstance.Name.Contains("$") Then

                            entityItem = New Entity

                            With entityItem
                                .assemblydependency = assemblyFileName

                                If DepedencyAssembly.Length > 0 Then
                                    .assemblydependency += ";" & DepedencyAssembly
                                End If
                                .namespaceIdentify = typeInstance.Namespace
                                .name = typeInstance.Name

                                For Each item As Type In typeInstance.GetInterfaces
                                    If item.FullName.IsNotEmpty AndAlso Not item.FullName.StartsWith("System.", StringComparison.CurrentCultureIgnoreCase) Then
                                        If .Interfaces.IsNotEmpty Then
                                            .Interfaces &= ","
                                        End If
                                        .Interfaces &= item.FullName
                                        .name = typeInstance.Name
                                    End If
                                Next

                                If Not IsNothing(typeInstance.BaseType) Then
                                    .basetypefullname = String.Format(CultureInfo.InvariantCulture,
                                                                      "{0}.{1}", typeInstance.BaseType.Namespace, typeInstance.BaseType.Name)
                                Else
                                    .basetypefullname = String.Empty
                                End If

                                .isenum = typeInstance.IsEnum
                                .isabstract = typeInstance.IsAbstract
                                .isinterface = typeInstance.IsInterface
                                If .basetypefullname.EndsWith("`1") Then
                                    .basetypefullname = .basetypefullname.Substring(0, .basetypefullname.Length - 2)
                                End If

                                .summary = GetDocumentation(String.Format("T:{0}.{1}", entityItem.namespaceIdentify, entityItem.name))
                                'If Not IsNothing(typeInstance.BaseType) AndAlso Not IsNothing(typeInstance.BaseType.BaseType) Then
                                '    .iscollection = (typeInstance.BaseType.BaseType.Name = "Collection`1")
                                'End If
                                'Validates if the type implements the IEnumerable interface, this will set it as a collection
                                .iscollection = (Not typeInstance.GetInterface("IEnumerable") Is Nothing)

                                If .iscollection Then
                                    If Not IsNothing(typeInstance.BaseType) AndAlso Not typeInstance.BaseType.FullName Is Nothing AndAlso typeInstance.BaseType.FullName.IndexOf("[") > -1 Then
                                        .ItemTypeFullName = typeInstance.BaseType.FullName.Substring(typeInstance.BaseType.FullName.IndexOf("["))
                                        .ItemTypeFullName = .ItemTypeFullName.Replace("[", String.Empty)
                                        .ItemTypeFullName = .ItemTypeFullName.Substring(0, .ItemTypeFullName.IndexOf(",")).Trim
                                    End If
                                End If

                                Dim ClassEnglishField() As Attributes.EntityEnglishDesigner = DirectCast(typeInstance.GetCustomAttributes(GetType(Attributes.EntityEnglishDesigner), True), Attributes.EntityEnglishDesigner())

                                If (ClassEnglishField.Length > 0) Then
                                    .EnglishTitle = ClassEnglishField(0).Title
                                End If

                                Dim ClassSpanishField() As Attributes.EntitySpanishDesigner = DirectCast(typeInstance.GetCustomAttributes(GetType(Attributes.EntitySpanishDesigner), True), Attributes.EntitySpanishDesigner())

                                If (ClassSpanishField.Length > 0) Then
                                    .SpanishTitle = ClassSpanishField(0).Title
                                End If

                                Dim EntityCommonlyUsedField() As Attributes.EntityCommonlyUsedAttribute = DirectCast(typeInstance.GetCustomAttributes(GetType(Attributes.EntityCommonlyUsedAttribute), True), Attributes.EntityCommonlyUsedAttribute())

                                If (EntityCommonlyUsedField.Length > 0) Then
                                    .commonlyused = True

                                End If

                                Dim EntityExcludeField() As Attributes.EntityExcludeAttribute = DirectCast(typeInstance.GetCustomAttributes(GetType(Attributes.EntityExcludeAttribute), True), Attributes.EntityExcludeAttribute())

                                If (EntityExcludeField.Length > 0) Then
                                    .exclude = True
                                End If

                            End With

                            If ((mode And EnumLoadMode.Member) = EnumLoadMode.Member) Then
                                'For Each propertie As PropertyInfo In typeInstance.GetProperties(BindingFlags.Instance Or BindingFlags.Public Or BindingFlags.FlattenHierarchy)
                                'For Each propertie As PropertyInfo In typeInstance.GetProperties(BindingFlags.Static Or BindingFlags.Public Or BindingFlags.NonPublic Or BindingFlags.Instance Or BindingFlags.DeclaredOnly)
                                For Each propertie As PropertyInfo In typeInstance.GetProperties(BindingFlags.Static Or BindingFlags.Instance Or BindingFlags.Public Or BindingFlags.FlattenHierarchy)
                                    loadEntity = True

                                    'If entityItem.iscollection AndAlso String.Equals(propertie.Name, "Item", StringComparison.CurrentCultureIgnoreCase) Then
                                    '    loadEntity = False
                                    'End If

                                    If loadEntity Then
                                        newElement = entityItem.elements.AddElement

                                        With newElement
                                            .type = EnumMemberType.Property
                                            .name = propertie.Name
                                            .basetypefullname = propertie.PropertyType.ToString
                                            .isenum = propertie.PropertyType.IsEnum
                                            .summary = GetDocumentation(String.Format(CultureInfo.InvariantCulture,
                                                                                      "P:{0}.{1}.{2}", entityItem.namespaceIdentify, entityItem.name, .name))
                                            .canread = propertie.CanRead
                                            .canwrite = propertie.CanWrite

                                            If Not IsNothing(propertie.GetGetMethod()) Then
                                                .IsShared = propertie.GetGetMethod().IsStatic
                                            End If

                                            If .basetypefullname.Contains("`1") And .basetypefullname.EndsWith("]") Then
                                                .ItemTypeFullName = .basetypefullname.Substring(.basetypefullname.IndexOf("["))
                                                .ItemTypeFullName = .ItemTypeFullName.Replace("[", String.Empty)
                                                .ItemTypeFullName = .ItemTypeFullName.Replace("]", String.Empty)
                                                .iscollection = True
                                                .basetypefullname = .basetypefullname.Substring(0, .basetypefullname.IndexOf("`1"))
                                            End If

                                            MemberEnglishDesignerField = DirectCast(propertie.GetCustomAttributes(GetType(Attributes.ElementEnglishDesigner), True), Attributes.ElementEnglishDesigner())

                                            If (MemberEnglishDesignerField.Length > 0) Then
                                                .EnglishCaption = MemberEnglishDesignerField(0).Caption
                                                .EnglishToolTip = MemberEnglishDesignerField(0).ToolTip
                                            End If

                                            MemberSpanishDesignerField = DirectCast(propertie.GetCustomAttributes(GetType(Attributes.ElementSpanishDesigner), True), Attributes.ElementSpanishDesigner())

                                            If (MemberSpanishDesignerField.Length > 0) Then
                                                .SpanishCaption = MemberSpanishDesignerField(0).Caption
                                                .SpanishToolTip = MemberSpanishDesignerField(0).ToolTip
                                            End If

                                            MemberBehaviorField = DirectCast(propertie.GetCustomAttributes(GetType(Attributes.ElementBehaviorAttribute), True), Attributes.ElementBehaviorAttribute())

                                            If (MemberBehaviorField.Length > 0) Then
                                                .Scale = MemberBehaviorField(0).Scale
                                                .Size = MemberBehaviorField(0).Size
                                                .Precision = MemberBehaviorField(0).Precision
                                            End If

                                            MemberRequiredField = DirectCast(propertie.GetCustomAttributes(GetType(Attributes.ElementRequiredAttribute), True), Attributes.ElementRequiredAttribute())

                                            If (MemberRequiredField.Length > 0) Then
                                                .IsRequired = MemberRequiredField(0).IsRequired
                                            Else
                                                .IsRequired = False
                                            End If

                                            MemberLookUpField = DirectCast(propertie.GetCustomAttributes(GetType(Attributes.ElementLookupAttribute), True), Attributes.ElementLookupAttribute())

                                            If (MemberLookUpField.Length > 0) Then
                                                .LookUpTableName = MemberLookUpField(0).TableName
                                                .LookUpKeyField = MemberLookUpField(0).KeyField
                                                .LookUpDescriptionField = MemberLookUpField(0).DescriptionField
                                            End If
                                        End With

                                        If newElement.basetypefullname.StartsWith("System.Collections.Generic.List") Then
                                            With VirtualEntity(entityItem, newElement)
                                                newElement.ItemTypeFullName = String.Empty
                                                newElement.basetypefullname = String.Format(CultureInfo.InvariantCulture, "{0}.{1}", .namespaceIdentify, .name)
                                                newElement.type = EnumMemberType.Property
                                            End With
                                        End If

                                        If entityItem.iscollection AndAlso
                                            String.IsNullOrEmpty(entityItem.ItemTypeFullName) AndAlso
                                            String.Equals(entityItem.elements(entityItem.elements.Count - 1).name, "item", StringComparison.CurrentCultureIgnoreCase) Then

                                            entityItem.ItemTypeFullName = entityItem.elements(entityItem.elements.Count - 1).basetypefullname
                                        End If
                                    End If
                                Next

                                'Or BindingFlags.NonPublic
                                For Each propertie As FieldInfo In typeInstance.GetFields(BindingFlags.Static Or BindingFlags.Public Or BindingFlags.Instance Or BindingFlags.DeclaredOnly)
                                    With entityItem.elements.AddElement
                                        .type = EnumMemberType.Field
                                        .name = propertie.Name
                                        .basetypefullname = propertie.FieldType.ToString
                                        .isenum = propertie.FieldType.IsEnum
                                        .summary = GetDocumentation(String.Format("P:{0}.{1}.{2}", entityItem.namespaceIdentify, entityItem.name, .name))
                                        .canread = True
                                        .canwrite = True
                                        .IsShared = propertie.IsStatic

                                        If .basetypefullname.Contains("`1") And .basetypefullname.EndsWith("]") Then
                                            .ItemTypeFullName = .basetypefullname.Substring(.basetypefullname.IndexOf("["))
                                            .ItemTypeFullName = .ItemTypeFullName.Replace("[", String.Empty)
                                            .ItemTypeFullName = .ItemTypeFullName.Replace("]", String.Empty)
                                            .iscollection = True
                                            .basetypefullname = .basetypefullname.Substring(0, .basetypefullname.IndexOf("`1"))
                                        End If

                                    End With
                                Next

                                If entityItem.isenum Then

                                    For Each field In typeInstance.GetFields(BindingFlags.Public Or BindingFlags.Static)
                                        With entityItem.elements.AddElement
                                            .type = EnumMemberType.Enumeration
                                            .name = field.Name
                                            .basetypefullname = field.MemberType.ToString
                                            .isenum = False
                                            .parameterDeclare = field.GetRawConstantValue().ToString()
                                            .summary = GetDocumentation(String.Format("F:{0}.{1}.{2}", entityItem.namespaceIdentify, entityItem.name, .name))
                                        End With

                                    Next
                                End If

                            End If
                            If ((mode And EnumLoadMode.Method) = EnumLoadMode.Method) Then

                                For Each methods As MethodInfo In typeInstance.GetMethods(BindingFlags.Static Or BindingFlags.Public Or BindingFlags.NonPublic Or BindingFlags.Instance Or BindingFlags.DeclaredOnly)
                                    If (Not methods.Name.StartsWith("get_", StringComparison.Ordinal) And Not methods.Name.StartsWith("set_", StringComparison.Ordinal)) AndAlso Not methods.Name.Contains("$") Then

                                        With entityItem.elements.AddElement
                                            .type = EnumMemberType.Method
                                            .name = methods.Name
                                            .basetypefullname = methods.ReturnType.ToString

                                            If IsNotPrimitive(methods.ReturnType) Then
                                                .iscollection = (Not methods.ReturnType.GetInterface("IEnumerable") Is Nothing)
                                                If .iscollection Then
                                                    If Not methods.ReturnType.FullName Is Nothing AndAlso
                                                       methods.ReturnType.FullName.IndexOf("[") > -1 Then
                                                        .ItemTypeFullName = methods.ReturnType.FullName.Substring(methods.ReturnType.FullName.IndexOf("["))
                                                        If .ItemTypeFullName = "[]" Then
                                                            .iscollection = False
                                                            .ItemTypeFullName = String.Empty
                                                        Else
                                                            .ItemTypeFullName = .ItemTypeFullName.Replace("[", String.Empty)
                                                            .ItemTypeFullName = .ItemTypeFullName.Substring(0, .ItemTypeFullName.IndexOf(",")).Trim
                                                        End If

                                                    ElseIf methods.ReturnType.BaseType.FullName.Contains("`1") And methods.ReturnType.BaseType.FullName.EndsWith("]") Then
                                                        .ItemTypeFullName = methods.ReturnType.BaseType.FullName.Substring(methods.ReturnType.BaseType.FullName.IndexOf("["))
                                                        If .ItemTypeFullName.Contains(",") Then
                                                            .ItemTypeFullName = .ItemTypeFullName.Substring(0, .ItemTypeFullName.IndexOf(","))
                                                        End If
                                                        .ItemTypeFullName = .ItemTypeFullName.Replace("[", String.Empty)
                                                        .ItemTypeFullName = .ItemTypeFullName.Replace("]", String.Empty)
                                                        '.basetypefullname = methods.ReturnType.BaseType.FullName.Substring(0, methods.ReturnType.BaseType.FullName.IndexOf("`1"))
                                                    Else
                                                        .iscollection = False
                                                    End If
                                                End If
                                            End If

                                            parameterList = String.Empty
                                            parameterDeclare = String.Empty

                                            .Parameters = New ParameterCollection
                                            .canread = True
                                            .canwrite = True
                                            .IsShared = methods.IsStatic
                                            .summary = GetDocumentation(String.Format(CultureInfo.InvariantCulture,
                                                                                      "M:{0}.{1}.{2}", entityItem.namespaceIdentify, entityItem.name, methods.Name))

                                            For Each param As ParameterInfo In methods.GetParameters()
                                                parameterInstance = New Parameter

                                                With parameterInstance
                                                    .IsOptional = param.IsOptional
                                                    .Name = param.Name
                                                    .TypeFullname = param.ParameterType.FullName

                                                    If .TypeFullname.IsNotEmpty AndAlso .TypeFullname.Contains("&") Then
                                                        .TypeFullname = .TypeFullname.Replace("&", "")
                                                    End If

                                                    If param.ParameterType.IsByRef Then
                                                        parameterInstance.TypePassVariable = "ByRef"
                                                    Else
                                                        parameterInstance.TypePassVariable = "ByVal"
                                                    End If

                                                    If Not IsNothing(param.DefaultValue) AndAlso Not IsDBNull(param.DefaultValue) Then
                                                        Try
                                                            parameterInstance.Default = param.DefaultValue

                                                        Catch ex As Exception
                                                            Console.WriteLine("DefaultValue fail")
                                                        End Try
                                                    End If
                                                End With

                                                .Parameters.Add(parameterInstance)

                                                parameterList += param.ParameterType.FullName

                                                If parameterDeclare.Length > 0 Then
                                                    parameterDeclare += ", "
                                                End If

                                                parameterDeclare += String.Format(CultureInfo.InvariantCulture,
                                                                                  "ByVal {0} As {1}", param.Name, param.ParameterType.FullName)
                                            Next

                                            .parameterDeclare = parameterDeclare

                                            If .summary.IsEmpty Then
                                                .summary = GetDocumentation(String.Format(CultureInfo.InvariantCulture,
                                                                                          "M:{0}.{1}.{2}({3})", entityItem.namespaceIdentify, entityItem.name, .name, parameterList))
                                            End If

                                            .Tag = methods.DeclaringType.FullName
                                            entityItem.havemethods = True
                                        End With
                                    End If
                                Next
                            End If
                            ' Use the 'client' variable to call operations on the service.

                            If entityItem.iscollection Then
                                With entityItem.elements.AddElement
                                    .type = EnumMemberType.Property

                                    If Not IsNothing(entityItem.ItemTypeFullName) Then
                                        .name = entityItem.ItemTypeFullName.Substring(entityItem.ItemTypeFullName.LastIndexOf(".") + 1) + "[0]"
                                        .basetypefullname = entityItem.ItemTypeFullName
                                    End If
                                End With
                            End If

                            [Class].Add(entityItem)
                        End If
                    Next
                End If

                AssemblyLoaded &= String.Format(CultureInfo.InvariantCulture, "<{0}>", assemblyFileName)
            End If

        Catch ex As Exception
            errors += String.Format(CultureInfo.InvariantCulture,
                                    "Failure to try to read the information on the '{0}' assembly ({1}).{2}",
                                    assemblyFileName, ex.Message, vbCrLf)
        End Try
    End Sub

    Private Function GetDocumentation(ByVal path As String) As String
        Dim result As String = String.Empty
        If Not IsNothing(_xmlDocument) Then
            Dim queryResult = From x In _xmlDocument.<doc>.<members>.<member>
                              Where x.@name = path
                              Select x.<summary>, x.<value>

            If queryResult.Count > 0 Then
                With queryResult.Single
                    result = .summary.Value
                    If String.IsNullOrEmpty(result) Then
                        result = String.Empty
                    Else
                        result = result.Replace(vbCr, String.Empty).Replace(vbLf, String.Empty).Trim
                    End If
                End With

            End If
        End If
        Return result
    End Function

    Friend Shared Function CurrentDomain_AssemblyResolve(ByVal sender As Object, ByVal args As ResolveEventArgs) As System.Reflection.Assembly
        Dim assemblyName As String = args.Name

        If assemblyName.Length > 0 And assemblyName.Contains(",") Then
            assemblyName = String.Format("{0}\{1}.dll",
                                         CurrentAssembly.Substring(0, CurrentAssembly.LastIndexOf("\")),
                                         assemblyName.Substring(0, assemblyName.IndexOf(",")))
        End If

        If assemblyName.IsNotEmpty Then

            'For Each item As Reflection.Assembly In AppDomain.CurrentDomain.GetAssemblies
            '    If String.Equals(IO.Path.GetFileNameWithoutExtension(item.Location), IO.Path.GetFileNameWithoutExtension(assemblyName), StringComparison.CurrentCultureIgnoreCase) Then
            '        Return item
            '        Exit For
            '    End If
            'Next

            If File.Exists(assemblyName) Then
                If assemblyLoader.DepedencyAssembly.Length > 0 Then
                    assemblyLoader.DepedencyAssembly += ";"
                End If
                assemblyLoader.DepedencyAssembly += assemblyName
                Dim localAssembly As Reflection.Assembly = System.Reflection.Assembly.LoadFile(assemblyName)

                Return localAssembly
                'Else

                '    Dim assemblyFileDialog As New OpenFileDialog()

                '    If assemblyName.IndexOf("\") > -1 Then
                '        assemblyName = assemblyName.Substring(assemblyName.LastIndexOf("\") + 1)

                '    End If

                '    With assemblyFileDialog
                '        .Filter = assemblyName & " |" & assemblyName   '& ";" & assemblyName & " |" & assemblyName

                '        .RestoreDirectory = True
                '        .FileName = assemblyName
                '    End With

                '    If assemblyFileDialog.ShowDialog() = DialogResult.OK Then
                '        Return Assembly.LoadFile(assemblyFileDialog.FileName)
                '    Else
                '        Throw New Exception("Assembly '" + assemblyName + "' not found")
                '    End If
            End If
        End If
        Return Nothing
    End Function

    Public Sub LoadAssemblyList(AssemblyList As String, PathBase As String, mode As EnumLoadMode, Hierarchical As Boolean)

        AddHandler(AppDomain.CurrentDomain.AssemblyResolve), AddressOf assemblyLoader.CurrentDomain_AssemblyResolve

        _tempContainer = New Dictionary(Of String, Dictionary(Of String, Dictionary(Of String, EntityHierarchy)))

        For Each fileName As String In AssemblyList.Split(";")
            LoadAssemblyInformationHierarchical(String.Format("{0}\{1}", PathBase, IO.Path.GetFileName(fileName)), mode)

            If DepedencyAssembly.IsNotEmpty Then
                Dim localDependency As String = DepedencyAssembly
                For Each fileNameItem As String In localDependency.Split(";")
                    If AssemblyLoaded.IndexOf(fileNameItem) = -1 Then
                        LoadAssemblyInformationHierarchical(String.Format("{0}\{1}", PathBase, IO.Path.GetFileName(fileNameItem)), EnumLoadMode.Member)
                    End If
                Next

            End If
        Next

        RemoveHandler(AppDomain.CurrentDomain.AssemblyResolve), AddressOf assemblyLoader.CurrentDomain_AssemblyResolve
    End Sub

    'Public Function AssemblyHierarchical() As EntityHierarchy
    '    Dim RootEntity As New EntityHierarchy With {.ParentCore = Nothing, .CellsCore = New Object() {Nothing}} ' (Nothing, New Object() {Nothing, Nothing})
    '    For Each ItemNamespace In _tempContainer.Keys
    '        Dim rootEntityNameSpace As New EntityHierarchy

    '        With rootEntityNameSpace
    '            .ParentCore = RootEntity
    '            .CellsCore = New Object() {rootEntityNameSpace, Model.Enumerations.EnumTypeElement.Assembly}
    '            .name = ItemNamespace
    '        End With

    '        Dim rootNamespaceDictonary As Dictionary(Of String, Dictionary(Of String, EntityHierarchy)) = _tempContainer(ItemNamespace)
    '        For Each ItemAssemblyRoot In rootNamespaceDictonary.Keys

    '            Dim rootEntityItemNameSpace As New EntityHierarchy
    '            With rootEntityItemNameSpace
    '                .ParentCore = rootEntityNameSpace
    '                .CellsCore = New Object() {rootEntityItemNameSpace, Model.Enumerations.EnumTypeElement.Namespace}
    '                .name = ItemAssemblyRoot
    '            End With

    '            Dim rootNamespaceItemDictonary As Dictionary(Of String, EntityHierarchy) = rootNamespaceDictonary(ItemAssemblyRoot)
    '            Dim rootEntityItemElement As EntityHierarchy
    '            For Each ItemAssembly In rootNamespaceItemDictonary.Keys
    '                Dim tempItemAssembly = rootNamespaceItemDictonary(ItemAssembly)
    '                Dim tempType As Model.Enumerations.EnumTypeElement
    '                If tempItemAssembly.isclass Then
    '                    tempType = Model.Enumerations.EnumTypeElement.Class
    '                ElseIf tempItemAssembly.isinterface Then
    '                    tempType = Model.Enumerations.EnumTypeElement.Interface
    '                ElseIf tempItemAssembly.isenum Then
    '                    tempType = Model.Enumerations.EnumTypeElement.Enumerator
    '                End If
    '                With tempItemAssembly
    '                    .ParentCore = rootEntityItemNameSpace
    '                    .CellsCore = New Object() {tempItemAssembly, tempType}
    '                End With
    '                For Each ItemElement In tempItemAssembly.elements
    '                    Dim tempTypeProperty As Model.Enumerations.EnumTypeElement
    '                    Select Case ItemElement.type
    '                        Case Model.Enumerations.EnumMemberType.Enumeration
    '                            tempTypeProperty = Model.Enumerations.EnumTypeElement.Enumerator
    '                        Case Model.Enumerations.EnumMemberType.Field
    '                            tempTypeProperty = Model.Enumerations.EnumTypeElement.Field
    '                        Case Model.Enumerations.EnumMemberType.Property
    '                            tempTypeProperty = Model.Enumerations.EnumTypeElement.Property
    '                        Case Model.Enumerations.EnumMemberType.Method
    '                            tempTypeProperty = Model.Enumerations.EnumTypeElement.Method
    '                    End Select

    '                    rootEntityItemElement = New EntityHierarchy
    '                    With rootEntityItemElement
    '                        .ParentCore = tempItemAssembly
    '                        .CellsCore = New Object() {rootEntityItemElement, tempTypeProperty}
    '                        .name = ItemElement.name
    '                    End With
    '                Next
    '            Next
    '        Next
    '    Next
    '    Return RootEntity
    'End Function

    Private Sub LoadAssemblyInformationHierarchical(ByVal assemblyFileName As String, ByVal mode As EnumLoadMode)
        Dim localAssembly As Reflection.Assembly = Nothing
        Dim entityItem As EntityHierarchy
        Dim summary As String = String.Empty
        Dim parameterList As String = String.Empty
        Dim parameterDeclare As String = String.Empty
        Dim xmlFileDocument As String = Path.Combine(Path.GetDirectoryName(assemblyFileName), (Path.GetFileNameWithoutExtension(assemblyFileName) & ".xml"))
        Dim parameterInstance As Parameter = Nothing

        If AssemblyLoaded.IndexOf(assemblyFileName) = -1 Then
            For Each item As Reflection.Assembly In AppDomain.CurrentDomain.GetAssemblies
                If Not item.IsDynamic AndAlso String.Equals(IO.Path.GetFileNameWithoutExtension(item.Location), IO.Path.GetFileNameWithoutExtension(assemblyFileName), StringComparison.CurrentCultureIgnoreCase) Then
                    localAssembly = item
                    Exit For
                End If
            Next

            If IsNothing(localAssembly) Then
                localAssembly = System.Reflection.Assembly.LoadFile(assemblyFileName)
            End If

            If File.Exists(xmlFileDocument) Then
                _xmlDocument = XDocument.Load(xmlFileDocument)
            Else
                _xmlDocument = Nothing
            End If

            DepedencyAssembly = String.Empty
            CurrentAssembly = assemblyFileName

            Dim types As Type() = localAssembly.GetTypes()
            Dim listName As New List(Of String)
            For Each ItemType As Type In types
                If (IsNothing(ItemType.Namespace) OrElse Not ItemType.Namespace.EndsWith(".My", StringComparison.CurrentCultureIgnoreCase)) AndAlso Not ItemType.Name.Contains("$") Then
                    If Not listName.Contains(ItemType.Namespace) Then
                        listName.Add(ItemType.Namespace)
                    End If
                End If
            Next
            listName.Sort()


            Dim _tempRootNameSpace As New Dictionary(Of String, Dictionary(Of String, EntityHierarchy))
            Dim _namespace As String = String.Empty
            _tempContainer.Add(localAssembly.GetName().Name, _tempRootNameSpace)
            For Each ItemNamespace As String In listName
                _tempRootNameSpace.Add(ItemNamespace, New Dictionary(Of String, EntityHierarchy))
                _namespace = ItemNamespace
                Dim typeFilter As Type() = (From itemType In localAssembly.GetTypes Where Not IsNothing(itemType.Namespace) AndAlso
                                            itemType.Namespace.Equals(_namespace) Select itemType).ToArray
                For Each typeInstance As Type In typeFilter
                    If (IsNothing(typeInstance.Namespace) OrElse Not typeInstance.Namespace.EndsWith(".My", StringComparison.CurrentCultureIgnoreCase)) AndAlso Not typeInstance.Name.Contains("$") Then

                        entityItem = New EntityHierarchy(Nothing, Nothing)

                        With entityItem
                            .assemblydependency = assemblyFileName
                            If DepedencyAssembly.Length > 0 Then
                                .assemblydependency += ";" & DepedencyAssembly
                            End If
                            .namespaceIdentify = typeInstance.Namespace
                            .name = typeInstance.Name

                            If Not IsNothing(typeInstance.BaseType) Then
                                .basetypefullname = String.Format("{0}.{1}", typeInstance.BaseType.Namespace, typeInstance.BaseType.Name)
                            Else
                                .basetypefullname = String.Empty
                            End If

                            .isenum = typeInstance.IsEnum
                            .isinterface = typeInstance.IsInterface
                            .isclass = typeInstance.IsClass
                            If .basetypefullname.EndsWith("`1") Then
                                .basetypefullname = .basetypefullname.Substring(0, .basetypefullname.Length - 2)
                            End If

                            .summary = GetDocumentation(String.Format("T:{0}.{1}", entityItem.namespaceIdentify, entityItem.name))
                            'If Not IsNothing(typeInstance.BaseType) AndAlso Not IsNothing(typeInstance.BaseType.BaseType) Then
                            '    .iscollection = (typeInstance.BaseType.BaseType.Name = "Collection`1")
                            'End If
                            'Validates if the type implements the IEnumerable interface, this will set it as a collection
                            .iscollection = (Not typeInstance.GetInterface("IEnumerable") Is Nothing)

                            If .iscollection Then
                                If Not IsNothing(typeInstance.BaseType) AndAlso Not typeInstance.BaseType.FullName Is Nothing AndAlso typeInstance.BaseType.FullName.IndexOf("[") > -1 Then
                                    .ItemTypeFullName = typeInstance.BaseType.FullName.Substring(typeInstance.BaseType.FullName.IndexOf("["))
                                    .ItemTypeFullName = .ItemTypeFullName.Replace("[", String.Empty)
                                    .ItemTypeFullName = .ItemTypeFullName.Substring(0, .ItemTypeFullName.IndexOf(",")).Trim
                                End If
                            End If

                            Dim ClassEnglishField() As Attributes.EntityEnglishDesigner = DirectCast(typeInstance.GetCustomAttributes(GetType(Attributes.EntityEnglishDesigner), True), Attributes.EntityEnglishDesigner())

                            If (ClassEnglishField.Length > 0) Then
                                .EnglishTitle = ClassEnglishField(0).Title
                            End If

                            Dim ClassSpanishField() As Attributes.EntitySpanishDesigner = DirectCast(typeInstance.GetCustomAttributes(GetType(Attributes.EntitySpanishDesigner), True), Attributes.EntitySpanishDesigner())

                            If (ClassSpanishField.Length > 0) Then
                                .SpanishTitle = ClassSpanishField(0).Title
                            End If

                            Dim EntityCommonlyUsedField() As Attributes.EntityCommonlyUsedAttribute = DirectCast(typeInstance.GetCustomAttributes(GetType(Attributes.EntityCommonlyUsedAttribute), True), Attributes.EntityCommonlyUsedAttribute())

                            If (EntityCommonlyUsedField.Length > 0) Then
                                .commonlyused = True

                            End If

                        End With

                        If ((mode And EnumLoadMode.Member) = EnumLoadMode.Member) Then
                            'For Each propertie As PropertyInfo In typeInstance.GetProperties(BindingFlags.Instance Or BindingFlags.Public Or BindingFlags.FlattenHierarchy)
                            'For Each propertie As PropertyInfo In typeInstance.GetProperties(BindingFlags.Static Or BindingFlags.Public Or BindingFlags.NonPublic Or BindingFlags.Instance Or BindingFlags.DeclaredOnly)
                            For Each propertie As PropertyInfo In typeInstance.GetProperties(BindingFlags.Static Or BindingFlags.Instance Or BindingFlags.Public Or BindingFlags.FlattenHierarchy)

                                With entityItem.elements.AddElement
                                    .type = EnumMemberType.Property
                                    .name = propertie.Name
                                    .basetypefullname = propertie.PropertyType.ToString
                                    .isenum = propertie.PropertyType.IsEnum
                                    .summary = GetDocumentation(String.Format("P:{0}.{1}.{2}", entityItem.namespaceIdentify, entityItem.name, .name))
                                    .canread = propertie.CanRead
                                    .canwrite = propertie.CanWrite
                                    If Not IsNothing(propertie.GetGetMethod()) Then
                                        .IsShared = propertie.GetGetMethod().IsStatic
                                    End If

                                    If .basetypefullname.Contains("`1") And .basetypefullname.EndsWith("]") Then
                                        .ItemTypeFullName = .basetypefullname.Substring(.basetypefullname.IndexOf("["))
                                        .ItemTypeFullName = .ItemTypeFullName.Replace("[", String.Empty)
                                        .ItemTypeFullName = .ItemTypeFullName.Replace("]", String.Empty)
                                        .iscollection = True
                                        .basetypefullname = .basetypefullname.Substring(0, .basetypefullname.IndexOf("`1"))
                                    End If

                                    Dim MemberEnglishDesignerField() As Attributes.ElementEnglishDesigner = DirectCast(propertie.GetCustomAttributes(GetType(Attributes.ElementEnglishDesigner), True), Attributes.ElementEnglishDesigner())

                                    If (MemberEnglishDesignerField.Length > 0) Then
                                        .EnglishCaption = MemberEnglishDesignerField(0).Caption
                                        .EnglishToolTip = MemberEnglishDesignerField(0).ToolTip
                                    End If

                                    Dim MemberSpanishDesignerField() As Attributes.ElementSpanishDesigner = DirectCast(propertie.GetCustomAttributes(GetType(Attributes.ElementSpanishDesigner), True), Attributes.ElementSpanishDesigner())
                                    If (MemberSpanishDesignerField.Length > 0) Then
                                        .SpanishCaption = MemberSpanishDesignerField(0).Caption
                                        .SpanishToolTip = MemberSpanishDesignerField(0).ToolTip
                                    End If

                                    Dim MemberBehaviorField() As Attributes.ElementBehaviorAttribute = DirectCast(propertie.GetCustomAttributes(GetType(Attributes.ElementBehaviorAttribute), True), Attributes.ElementBehaviorAttribute())
                                    If (MemberBehaviorField.Length > 0) Then
                                        .Scale = MemberBehaviorField(0).Scale
                                        .Size = MemberBehaviorField(0).Size
                                        .Precision = MemberBehaviorField(0).Precision
                                    End If

                                    Dim MemberRequiredField() As Attributes.ElementRequiredAttribute = DirectCast(propertie.GetCustomAttributes(GetType(Attributes.ElementRequiredAttribute), True), Attributes.ElementRequiredAttribute())
                                    If (MemberRequiredField.Length > 0) Then
                                        .IsRequired = MemberRequiredField(0).IsRequired
                                    Else
                                        .IsRequired = False
                                    End If

                                    Dim MemberLookUpField() As Attributes.ElementLookupAttribute = DirectCast(propertie.GetCustomAttributes(GetType(Attributes.ElementLookupAttribute), True), Attributes.ElementLookupAttribute())
                                    If (MemberLookUpField.Length > 0) Then
                                        .LookUpTableName = MemberLookUpField(0).TableName
                                        .LookUpKeyField = MemberLookUpField(0).KeyField
                                        .LookUpDescriptionField = MemberLookUpField(0).DescriptionField
                                    End If

                                End With

                                If entityItem.iscollection AndAlso
                                    String.IsNullOrEmpty(entityItem.ItemTypeFullName) AndAlso
                                    String.Equals(entityItem.elements(entityItem.elements.Count - 1).name, "item", StringComparison.CurrentCultureIgnoreCase) Then
                                    entityItem.ItemTypeFullName = entityItem.elements(entityItem.elements.Count - 1).basetypefullname

                                End If

                            Next

                            'Or BindingFlags.NonPublic
                            For Each propertie As FieldInfo In typeInstance.GetFields(BindingFlags.Static Or BindingFlags.Public Or BindingFlags.Instance Or BindingFlags.DeclaredOnly)
                                With entityItem.elements.AddElement
                                    .type = EnumMemberType.Field
                                    .name = propertie.Name
                                    .basetypefullname = propertie.FieldType.ToString
                                    .isenum = propertie.FieldType.IsEnum
                                    .summary = GetDocumentation(String.Format("P:{0}.{1}.{2}", entityItem.namespaceIdentify, entityItem.name, .name))
                                    .canread = True
                                    .canwrite = True
                                    .IsShared = propertie.IsStatic

                                    If .basetypefullname.Contains("`1") And .basetypefullname.EndsWith("]") Then
                                        .ItemTypeFullName = .basetypefullname.Substring(.basetypefullname.IndexOf("["))
                                        .ItemTypeFullName = .ItemTypeFullName.Replace("[", String.Empty)
                                        .ItemTypeFullName = .ItemTypeFullName.Replace("]", String.Empty)
                                        .iscollection = True
                                        .basetypefullname = .basetypefullname.Substring(0, .basetypefullname.IndexOf("`1"))
                                    End If

                                End With
                            Next

                            If entityItem.isenum Then

                                For Each field In typeInstance.GetFields(BindingFlags.Public Or BindingFlags.Static)
                                    With entityItem.elements.AddElement
                                        .type = EnumMemberType.Enumeration
                                        .name = field.Name
                                        .basetypefullname = field.MemberType.ToString
                                        .isenum = False
                                        .parameterDeclare = field.GetRawConstantValue().ToString()
                                        .summary = GetDocumentation(String.Format("F:{0}.{1}.{2}", entityItem.namespaceIdentify, entityItem.name, .name))
                                    End With

                                Next
                            End If

                        End If
                        If ((mode And EnumLoadMode.Method) = EnumLoadMode.Method) Then

                            For Each methods As MethodInfo In typeInstance.GetMethods(BindingFlags.Static Or BindingFlags.Public Or BindingFlags.NonPublic Or BindingFlags.Instance Or BindingFlags.DeclaredOnly)
                                If (Not methods.Name.StartsWith("get_", StringComparison.Ordinal) And Not methods.Name.StartsWith("set_", StringComparison.Ordinal)) AndAlso Not methods.Name.Contains("$") Then

                                    With entityItem.elements.AddElement
                                        .type = EnumMemberType.Method
                                        .name = methods.Name
                                        .basetypefullname = methods.ReturnType.ToString

                                        parameterList = String.Empty
                                        parameterDeclare = String.Empty

                                        .Parameters = New ParameterCollection
                                        .canread = True
                                        .canwrite = True
                                        .IsShared = methods.IsStatic
                                        .summary = GetDocumentation(String.Format("M:{0}.{1}.{2}", entityItem.namespaceIdentify, entityItem.name, methods.Name))

                                        For Each param As ParameterInfo In methods.GetParameters()
                                            parameterInstance = New Parameter

                                            With parameterInstance
                                                .IsOptional = param.IsOptional
                                                .Name = param.Name
                                                .TypeFullname = param.ParameterType.FullName

                                                If .TypeFullname.Contains("&") Then
                                                    .TypeFullname = .TypeFullname.Replace("&", "")
                                                End If

                                                If param.ParameterType.IsByRef Then
                                                    parameterInstance.TypePassVariable = "ByRef"
                                                Else
                                                    parameterInstance.TypePassVariable = "ByVal"
                                                End If

                                                If Not IsNothing(param.DefaultValue) AndAlso Not IsDBNull(param.DefaultValue) Then
                                                    Try
                                                        parameterInstance.Default = param.DefaultValue

                                                    Catch ex As Exception
                                                        Console.WriteLine("DefaultValue fail")
                                                    End Try
                                                End If
                                            End With

                                            .Parameters.Add(parameterInstance)

                                            parameterList += param.ParameterType.FullName

                                            If parameterDeclare.Length > 0 Then
                                                parameterDeclare += ", "
                                            End If

                                            parameterDeclare += String.Format("ByVal {0} As {1}", param.Name, param.ParameterType.FullName)
                                        Next

                                        .parameterDeclare = parameterDeclare

                                        If .summary.IsEmpty Then
                                            .summary = GetDocumentation(String.Format("M:{0}.{1}.{2}({3})", entityItem.namespaceIdentify, entityItem.name, .name, parameterList))
                                        End If

                                        .Tag = methods.DeclaringType.FullName
                                        entityItem.havemethods = True
                                    End With
                                End If
                            Next
                        End If
                        ' Use the 'client' variable to call operations on the service.

                        If entityItem.iscollection Then
                            With entityItem.elements.AddElement
                                .type = EnumMemberType.Property

                                If Not IsNothing(entityItem.ItemTypeFullName) Then
                                    .name = entityItem.ItemTypeFullName.Substring(entityItem.ItemTypeFullName.LastIndexOf(".") + 1) + "[0]"
                                    .basetypefullname = entityItem.ItemTypeFullName
                                End If
                            End With
                        End If
                        _tempContainer(localAssembly.GetName().Name)(ItemNamespace).Add(entityItem.name, entityItem)
                    End If
                Next
            Next

            AssemblyLoaded &= String.Format("<{0}>", assemblyFileName)
        End If
    End Sub

    Public Function GetElementType(baseType As String, pathElement As String) As String
        Dim result As String = String.Empty
        Dim subtype As String = String.Empty
        Dim baseElement As Entity = [Class].GetItemByFullName(baseType)
        Dim currentElement As Element

        If baseElement.IsNotEmpty Then
            If pathElement.IsNotEmpty AndAlso pathElement.IndexOf(".") > -1 Then
                pathElement = pathElement.Substring(pathElement.IndexOf(".") + 1)

                For Each item As String In pathElement.Split(".")
                    subtype = String.Empty

                    If item.IndexOf("(") > -1 Then
                        item = item.Substring(0, item.IndexOf("("))
                    End If

                    currentElement = baseElement.elements.GetItemByName(item, EnumMemberType.Property)

                    If currentElement.IsNotEmpty Then
                        result = currentElement.basetypefullname

                        If currentElement.isComplexType Then
                            baseElement = [Class].GetItemByFullName(currentElement.basetypefullname)

                            If baseElement.IsNotEmpty AndAlso baseElement.iscollection Then
                                subtype = String.Format(",{0}", baseElement.ItemTypeFullName)

                                baseElement = [Class].GetItemByFullName(baseElement.ItemTypeFullName)
                            End If
                        End If
                    End If
                Next
            Else
                result = baseElement.basetypefullname
            End If
        End If
        Return result & subtype
    End Function

    Private Function VirtualEntity(classItem As Entity, memberItem As Element) As Entity
        Dim classItemAux As New Entity

        With classItemAux
            .assemblydependency = classItem.assemblydependency
            .namespaceIdentify = classItem.namespaceIdentify

            .IEnumerableType = "System.Collections.Generic.List"

            .name = memberItem.name & "VList"
            .basetypefullname = memberItem.type
            .ItemTypeFullName = memberItem.ItemTypeFullName
            .iscollection = True
            .elements.Add(VirtualElement(.ItemTypeFullName))
        End With

        [Class].Add(classItemAux)

        Return classItemAux
    End Function

    Private Function VirtualElement(ItemTypeFullName As String) As Element
        Dim memberItem As New Element

        With memberItem
            .name = ItemTypeFullName.Substring(ItemTypeFullName.LastIndexOf(".") + 1) + "[0]"
            .type = EnumMemberType.Property
            .basetypefullname = ItemTypeFullName
            .canwrite = True
            .canread = True
        End With

        Return memberItem
    End Function

    ''' <summary>
    ''' Returns True if a non primitive type is trying to be invoked
    ''' </summary>
    ''' <param name="objectType"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Shared Function IsNotPrimitive(objectType As Type) As Boolean
        Return Not (objectType.IsValueType OrElse
                    objectType.IsPrimitive OrElse
                    New Type() {
                         GetType([String]), GetType([Decimal]),
                         GetType(Int16), GetType(Int32), GetType(Int64),
                         GetType(DateTime), GetType(DateTimeOffset),
                         GetType(TimeSpan), GetType(Guid)}.Contains(objectType) OrElse
                   Convert.GetTypeCode(objectType) <> TypeCode.[Object])
    End Function

    Public Sub LoadAssemblyListByCatalog(AssemblyList As String, PathBase As String, mode As EnumLoadMode, ByRef errors As String)
        Dim wrongFileName As String = String.Empty
        Dim isContinue As Boolean = True

        AddHandler(AppDomain.CurrentDomain.AssemblyResolve), AddressOf assemblyLoader.CurrentDomain_AssemblyResolve

        While isContinue
            isContinue = False

            For Each fileName As String In AssemblyList.Split(";")
                Try
                    wrongFileName = String.Empty

                    LoadAssemblyInformation(String.Format(CultureInfo.InvariantCulture, "{0}\{1}", PathBase, IO.Path.GetFileName(fileName)), mode, errors)

                    If DepedencyAssembly.IsNotEmpty Then
                        Dim localDependency As String = DepedencyAssembly

                        For Each fileNameItem As String In localDependency.Split(";")
                            If AssemblyLoaded.IndexOf(fileNameItem) = -1 Then
                                LoadAssemblyInformation(String.Format(CultureInfo.InvariantCulture, "{0}\{1}", PathBase, IO.Path.GetFileName(fileNameItem)), EnumLoadMode.Member, errors)
                            End If
                        Next
                    End If

                Catch ex As Exception
                    If Not IsNothing(ex.InnerException) AndAlso Not String.IsNullOrEmpty(ex.InnerException.Message) Then
                        errors += String.Format(CultureInfo.InvariantCulture, "{0}({1}){2}", ex.Message, ex.InnerException.Message, vbCrLf)
                    Else
                        errors += String.Format(CultureInfo.InvariantCulture, "{0}{1}", ex.Message, vbCrLf)
                    End If

                    wrongFileName = fileName
                    isContinue = True
                    Exit For
                End Try
            Next

            If Not String.IsNullOrEmpty(wrongFileName) Then
                ClearFailedAssemblies(AssemblyList, wrongFileName)
            End If
        End While

        Dim entityid As Integer = 1
        Dim elementid As Integer = 1
        Dim entityData As Entity = Nothing

        isContinue = True

        Do While isContinue
            isContinue = False

            For Each itemEntity As Entity In [Class]
                If itemEntity.elements.Count = 0 Then
                    [Class].Remove(itemEntity)
                    isContinue = True

                    Exit For
                End If
            Next
        Loop

        For Each itemEntity As Entity In [Class]
            itemEntity.classid = entityid
            entityid += 1

            For Each itemElement As Element In itemEntity.elements
                itemElement.memberid = elementid
                elementid += 1

                If itemElement.isComplexType Then
                    entityData = [Class].GetItemByFullName(itemElement.basetypefullname)

                    If Not IsNothing(entityData) Then
                        itemElement.iscollection = entityData.iscollection
                    End If
                End If
            Next
        Next

        RemoveHandler(AppDomain.CurrentDomain.AssemblyResolve), AddressOf assemblyLoader.CurrentDomain_AssemblyResolve
    End Sub

    Private Shared Sub ClearFailedAssemblies(ByRef assemblyList As String, wrongFileName As String)
        Dim newAssemblyList As String = String.Empty

        For Each fileName As String In assemblyList.Split(";")
            If Not String.Equals(fileName, wrongFileName, StringComparison.CurrentCultureIgnoreCase) Then
                newAssemblyList += String.Format(CultureInfo.InvariantCulture, "{0};", fileName)
            End If
        Next

        assemblyList = newAssemblyList
    End Sub

End Class