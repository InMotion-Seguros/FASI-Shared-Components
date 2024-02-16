Imports InMotionGIT.Assembly.Model.Data
Imports System.Text
Imports System.Xml

Namespace Generators

    'InMotionGIT.Assembly.Model.Generators.Utilitarios.MakeModel("C:\factory\mapfre.sugese\modelos.sugese\bin\Debug\modelos.sugese.dll", "C:\factory\modelos.sugese.xml")
    'Dim s As String = (New InMotionGIT.Assembly.Model.Generators.Utilitarios).Generate("C:\factory\modelos.sugese.xml", "modelos.sugese.CuentaTecnica.ModeloCuentaTecnica", "_CuentaTecnica")

    Public Class Utilitarios

        Private entities As EntityCollection

        Private others As New StringBuilder

        Public Shared Sub MakeModel(assemblyName As String, outputxmlfile As String)
            Dim repository As New InMotionGIT.Assembly.Model.assemblyLoader

            repository.Load(assemblyName, Enumerations.EnumLoadMode.Class + Enumerations.EnumLoadMode.Member)
            InMotionGIT.Common.Helpers.Serialize.SerializeToFile(Of EntityCollection)(repository.Class, outputxmlfile, True)
        End Sub

        'fullname = modelos.sugese.Produccion.ModeloProduccion"
        'name =  _Produccion
        Public Function Generate(xmlfile As String, fullname As String, name As String) As String

            entities = InMotionGIT.Common.Helpers.Serialize.DeserializeFromFile(Of EntityCollection)(xmlfile)

            Dim s As String = Gen(entities.GetItemByFullName(fullname), name, 0, 0)
            s &= others.ToString

            others = New StringBuilder
            Dim t As String = Gen2(entities.GetItemByFullName(fullname), name, 0, 0)
            t &= others.ToString

            Return s & t
        End Function

        Private Function Gen(entityItem As Entity, name As String, level As Integer, childLevel As Integer) As String
            Dim builder As New StringBuilder
            Dim childItem As Entity

            If level = 0 Then
                builder.AppendLine("Private Sub ModelInit()")
                builder.AppendFormat("{0} = New {1}.{2}", name, entityItem.namespaceIdentify, entityItem.name)
                builder.AppendLine()
            End If

            builder.AppendFormat("With {0}", name)
            builder.AppendLine()

            For Each prop As Element In entityItem.elements
                If prop.type = InMotionGIT.Assembly.Model.Enumerations.EnumMemberType.Property Then
                    If prop.isComplexType Then
                        If prop.iscollection Then
                            builder.AppendFormat("  .{0} = New {1}(Of {2})", prop.name, prop.basetypefullname, prop.ItemTypeFullName)
                        ElseIf prop.basetypefullname.EndsWith("[]") Then
                            builder.AppendFormat("  .{0}(0) = New {1}", prop.name, prop.basetypefullname.Replace("[]", String.Empty))
                        Else

                            builder.AppendFormat("  .{0} = New {1}", prop.name, prop.basetypefullname)
                        End If

                    ElseIf prop.isenum Then
                        childItem = entities.GetItemByFullName(prop.basetypefullname)

                        builder.AppendFormat("  .{0} = {1}.{2}", prop.name, childItem.FullName, ((From t In childItem.elements Where t.type = InMotionGIT.Assembly.Model.Enumerations.EnumMemberType.Enumeration Select t).FirstOrDefault).name)
                    Else

                        If Not prop.basetypefullname.EndsWith("[]") Then


                            builder.AppendFormat("  .{0} = ", prop.name)

                            Select Case prop.basetypefullname
                                Case "System.String"
                                    builder.Append("String.Empty")
                                Case "System.String[]"
                                    builder.Append("New System.String()")
                                Case "System.Decimal"
                                    builder.Append("0")
                                Case "System.Boolean"
                                    builder.Append("False")

                                Case Else
                                    builder.Append("NONE")
                            End Select
                        Else
                            builder.AppendFormat(" '.{0} = BAD", prop.name)
                        End If
                    End If

                    builder.AppendLine()
                End If

            Next

            builder.AppendFormat("End With")
            builder.AppendLine()

            For Each prop As Element In entityItem.elements
                If prop.type = InMotionGIT.Assembly.Model.Enumerations.EnumMemberType.Property AndAlso prop.isComplexType Then
                    If Not prop.basetypefullname.EndsWith("[]") AndAlso Not prop.iscollection Then
                        childItem = entities.GetItemByFullName(prop.basetypefullname)
                        builder.AppendLine(Gen(childItem, String.Format("{0}.{1}", name, prop.name), level + 1, childLevel))
                    ElseIf Not prop.iscollection Then
                        Dim child As New StringBuilder
                        childItem = entities.GetItemByFullName(prop.basetypefullname.Replace("[]", String.Empty))

                        child.AppendLine()
                        child.AppendFormat("Public Shared Function Add{0}(items As {1}.{2}()) As {1}.{2}", prop.name, childItem.namespaceIdentify, childItem.name)

                        child.AppendLine()

                        child.AppendFormat(" Dim result As New {1}.{2}", name, childItem.namespaceIdentify, childItem.name)
                        child.AppendLine()

                        child.AppendFormat("  ReDim Preserve items(UBound(items) + 1)", name, prop.name)
                        child.AppendLine()
                        child.AppendFormat("  items(UBound(items)) = result", name, prop.name)
                        child.AppendLine()

                        child.AppendLine(Gen(childItem, "result", level, childLevel + 1))

                        child.AppendLine(" Return result")
                        child.AppendLine("End Function")

                        others.AppendLine(child.ToString)
                    End If
                End If

            Next

            If level = 0 Then
                builder.AppendLine("End Sub")
            End If

            Return builder.ToString
        End Function

        Private Function Gen2(entityItem As Entity, name As String, level As Integer, childLevel As Integer) As String
            Dim builder As New StringBuilder
            Dim childItem As Entity

            If level = 0 Then
                builder.AppendLine("Private Sub ModelInit2()")

            End If

            builder.AppendFormat("With {0}", name)
            builder.AppendLine()

            For Each prop As Element In entityItem.elements
                If prop.type = InMotionGIT.Assembly.Model.Enumerations.EnumMemberType.Property Then
                    If prop.isComplexType Then

                    ElseIf prop.isenum Then
                        childItem = entities.GetItemByFullName(prop.basetypefullname)

                        builder.AppendFormat("  .{0} = sheet.EnumValue(Of {1})(RowNumber, 1, witherror)", prop.name, prop.basetypefullname)
                        builder.AppendLine()
                        builder.AppendLine("  If witherror Then")
                        builder.AppendFormat("    Helpers.XmlRespose.AddError(Response, ""error"", String.Format(""El {0} '{{0}}' no es valido."", sheet.Cell(RowNumber, 1).Value))", prop.name)
                        builder.AppendLine()
                        builder.AppendLine("  End If")
                    Else

                        If Not prop.basetypefullname.EndsWith("[]") Then


                            builder.AppendFormat("  .{0} = ", prop.name)

                            Select Case prop.basetypefullname
                                Case "System.String"
                                    builder.Append("sheet.StringValue(RowNumber, 1)")
                                Case "System.String[]"
                                    builder.Append("New System.String()")
                                Case "System.Decimal"
                                    builder.Append("sheet.FormatDecimalValue(RowNumber, 1, ""0.00"")")
                                Case "System.Boolean"
                                    builder.Append("False")

                                Case Else
                                    builder.Append("NONE")
                            End Select
                        Else
                            builder.AppendFormat(" '.{0} = BAD", prop.name)
                        End If
                    End If

                    builder.AppendLine()
                End If

            Next

            builder.AppendFormat("End With")
            builder.AppendLine()

            For Each prop As Element In entityItem.elements
                If prop.type = InMotionGIT.Assembly.Model.Enumerations.EnumMemberType.Property AndAlso prop.isComplexType Then
                    If Not prop.basetypefullname.EndsWith("[]") AndAlso Not prop.iscollection Then
                        childItem = entities.GetItemByFullName(prop.basetypefullname)
                        builder.AppendLine(Gen2(childItem, String.Format("{0}.{1}", name, prop.name), level + 1, childLevel))
                    ElseIf Not prop.iscollection Then
                        Dim child As New StringBuilder
                        childItem = entities.GetItemByFullName(prop.basetypefullname.Replace("[]", String.Empty))

                        child.AppendLine()
                        child.AppendFormat("Public Shared Function Add{0}2(items As {1}.{2}()) As {1}.{2}", prop.name, childItem.namespaceIdentify, childItem.name)

                        child.AppendLine()





                        child.AppendLine(Gen2(childItem, "result", level, childLevel + 1))


                        child.AppendLine("End Function")

                        others.AppendLine(child.ToString)
                    End If
                End If

            Next

            If level = 0 Then
                builder.AppendLine("End Sub")
            End If

            Return builder.ToString
        End Function

        Private _doc As XmlDocument
        Private _xnm As XmlNamespaceManager
        Private _builder As New StringBuilder

        Public Sub Generate3(xmlfile As String, fullname As String)

            entities = InMotionGIT.Common.Helpers.Serialize.DeserializeFromFile(Of EntityCollection)(xmlfile)

            _doc = New XmlDocument
            _doc.Load("C:\SSS\Modelos con cambios a partir del 2020 v2\ModeloSaldosContables.XSD")
            _xnm = New XmlNamespaceManager(_doc.NameTable)
            _xnm.AddNamespace("xs", "http://www.w3.org/2001/XMLSchema")
            _xnm.AddNamespace("res", "urn:cap:ties:ressugesetypes:v1")


            _builder = New StringBuilder
            Cuentas(entities.GetItemByFullName(fullname), "")


            _builder = New StringBuilder
            SCTotal(entities.GetItemByFullName(fullname), "")

            _builder = New StringBuilder
            SCAccount(entities.GetItemByFullName(fullname), "")




        End Sub

        Private Function Cuentas(entityItem As Entity, parentCode As String) As String
            Dim childItem As Entity
            Dim lista As String = String.Empty
            Dim currentElement As Element = Nothing
            For Each prop As Element In entityItem.elements
                If prop.type = InMotionGIT.Assembly.Model.Enumerations.EnumMemberType.Property AndAlso prop.isComplexType Then
                    If Not prop.basetypefullname.EndsWith("[]") AndAlso Not prop.iscollection Then
                        childItem = entities.GetItemByFullName(prop.basetypefullname)

                        If isFlat(childItem) Then
                            currentElement = Nothing
                            For Each prop2 As Element In childItem.elements
                                If prop2.name.StartsWith("Total") Then
                                    currentElement = prop2
                                    Exit For
                                End If
                            Next
                            If currentElement.IsNotEmpty Then
                                If lista.Length > 0 Then
                                    lista &= ","
                                End If
                                lista &= prop.name & "." & currentElement.name
                                For Each prop2 As Element In childItem.elements
                                    If Not prop2.name.EndsWith("Specified") AndAlso Not prop2.name.StartsWith("Total") Then
                                        _builder.AppendFormat("{1}", currentElement.name, prop2.name.Replace("cta_", String.Empty))
                                        _builder.AppendLine()
                                    End If
                                Next
                            End If
                        Else
                            Dim currentCode As New StringBuilder
                            If parentCode.Length > 0 Then
                                currentCode.Append(parentCode)
                                currentCode.Append(" AndAlso ")
                            End If
                            currentCode.AppendFormat("{0}.IsNotEmpty", prop.basetypefullname.Replace("Cta_", ".cta_").Replace("modelos.sugese.SaldosContables.ModeloSaldosContablesDatosModelo", "_SaldosContables.Datos.Modelo"))
                            Dim aqui As String = Cuentas(childItem, currentCode.ToString)
                            If aqui.IsNotEmpty Then
                                currentElement = Nothing
                                For Each prop2 As Element In childItem.elements
                                    If prop2.name.StartsWith("Total") Then
                                        currentElement = prop2
                                        Exit For
                                    End If
                                Next
                                If currentElement.IsNotEmpty Then
                                    If lista.Length > 0 Then
                                        lista &= ","
                                    End If
                                    lista &= prop.name & "." & currentElement.name
                                    For Each nameTotal As String In aqui.Split(",")

                                        '_builder.AppendFormat("                If .{0}.IsNotEmpty AndAlso .{1}.IsNotEmpty Then", nameTotal.Split(".")(0), nameTotal)
                                        '_builder.AppendLine()
                                        '_builder.AppendFormat("                    .{0} += .{1}", currentElement.name, nameTotal)
                                        '_builder.AppendLine()
                                        '_builder.AppendFormat("                End If")
                                        '_builder.AppendLine()

                                    Next

                                End If
                            End If
                        End If
                    End If
                End If

            Next
            Return lista
        End Function



        Private Function SCTotal(entityItem As Entity, parentCode As String) As String
            Dim childItem As Entity
            Dim lista As String = String.Empty
            Dim currentElement As Element = Nothing
            For Each prop As Element In entityItem.elements
                If prop.type = InMotionGIT.Assembly.Model.Enumerations.EnumMemberType.Property AndAlso prop.isComplexType Then
                    If Not prop.basetypefullname.EndsWith("[]") AndAlso Not prop.iscollection Then
                        childItem = entities.GetItemByFullName(prop.basetypefullname)

                        If isFlat(childItem) Then
                            currentElement = Nothing
                            For Each prop2 As Element In childItem.elements
                                If prop2.name.StartsWith("Total") Then
                                    currentElement = prop2
                                    Exit For
                                End If
                            Next
                            If currentElement.IsNotEmpty Then
                                If lista.Length > 0 Then
                                    lista &= ","
                                End If
                                lista &= prop.name & "." & currentElement.name
                                _builder.AppendFormat("        If _SaldosContables.IsNotEmpty AndAlso _SaldosContables.Datos.IsNotEmpty AndAlso _SaldosContables.Datos.Modelo.IsNotEmpty AndAlso {0} AndAlso {1}.IsNotEmpty Then", parentCode, prop.basetypefullname.Replace("Cta_", ".cta_").Replace("modelos.sugese.SaldosContables.ModeloSaldosContablesDatosModelo", "_SaldosContables.Datos.Modelo"))
                                _builder.AppendLine()
                                _builder.AppendFormat("            With {0}", prop.basetypefullname.Replace("Cta_", ".cta_").Replace("modelos.sugese.SaldosContables.ModeloSaldosContablesDatosModelo", "_SaldosContables.Datos.Modelo"))
                                _builder.AppendLine()
                                _builder.AppendFormat("                .{0} = 0", currentElement.name)
                                _builder.AppendLine()
                                For Each prop2 As Element In childItem.elements
                                    If Not prop2.name.EndsWith("Specified") AndAlso Not prop2.name.StartsWith("Total") Then
                                        _builder.AppendFormat("                If .{0}.IsNotEmpty Then", prop2.name)
                                        _builder.AppendLine()
                                        _builder.AppendFormat("                    .{0} += .{1}", currentElement.name, prop2.name)
                                        _builder.AppendLine()
                                        _builder.AppendFormat("                End If")
                                        _builder.AppendLine()
                                    End If
                                Next
                                _builder.AppendFormat("            End With")
                                _builder.AppendLine()
                                _builder.AppendFormat("        End If")
                                _builder.AppendLine()
                            End If
                        Else
                            Dim currentCode As New StringBuilder
                            If parentCode.Length > 0 Then
                                currentCode.Append(parentCode)
                                currentCode.Append(" AndAlso ")
                            End If
                            currentCode.AppendFormat("{0}.IsNotEmpty", prop.basetypefullname.Replace("Cta_", ".cta_").Replace("modelos.sugese.SaldosContables.ModeloSaldosContablesDatosModelo", "_SaldosContables.Datos.Modelo"))
                            Dim aqui As String = SCTotal(childItem, currentCode.ToString)
                            If aqui.IsNotEmpty Then
                                currentElement = Nothing
                                For Each prop2 As Element In childItem.elements
                                    If prop2.name.StartsWith("Total") Then
                                        currentElement = prop2
                                        Exit For
                                    End If
                                Next
                                If currentElement.IsNotEmpty Then
                                    If lista.Length > 0 Then
                                        lista &= ","
                                    End If
                                    lista &= prop.name & "." & currentElement.name
                                    _builder.AppendFormat("        If _SaldosContables.IsNotEmpty AndAlso _SaldosContables.Datos.IsNotEmpty AndAlso _SaldosContables.Datos.Modelo.IsNotEmpty AndAlso {0} AndAlso {1}.IsNotEmpty Then", parentCode, prop.basetypefullname.Replace("Cta_", ".cta_").Replace("modelos.sugese.SaldosContables.ModeloSaldosContablesDatosModelo", "_SaldosContables.Datos.Modelo"))
                                    _builder.AppendLine()
                                    _builder.AppendFormat("            With {0}", prop.basetypefullname.Replace("Cta_", ".cta_").Replace("modelos.sugese.SaldosContables.ModeloSaldosContablesDatosModelo", "_SaldosContables.Datos.Modelo"))
                                    _builder.AppendLine()
                                    _builder.AppendFormat("                .{0} = 0", currentElement.name)
                                    _builder.AppendLine()
                                    For Each nameTotal As String In aqui.Split(",")

                                        _builder.AppendFormat("                If .{0}.IsNotEmpty AndAlso .{1}.IsNotEmpty Then", nameTotal.Split(".")(0), nameTotal)
                                        _builder.AppendLine()
                                        _builder.AppendFormat("                    .{0} += .{1}", currentElement.name, nameTotal)
                                        _builder.AppendLine()
                                        _builder.AppendFormat("                End If")
                                        _builder.AppendLine()

                                    Next
                                    _builder.AppendFormat("            End With")
                                    _builder.AppendLine()
                                    _builder.AppendFormat("        End If")
                                    _builder.AppendLine()
                                End If
                            End If
                        End If
                    End If
                End If

            Next
            Return lista
        End Function

        Private Sub SCAccount(entityItem As Entity, parentCode As String)
            Dim childItem As Entity

            For Each prop As Element In entityItem.elements
                If prop.type = InMotionGIT.Assembly.Model.Enumerations.EnumMemberType.Property AndAlso prop.isComplexType Then
                    If Not prop.basetypefullname.EndsWith("[]") AndAlso Not prop.iscollection Then
                        childItem = entities.GetItemByFullName(prop.basetypefullname)

                        If isFlat(childItem) Then
                            For Each prop2 As Element In childItem.elements

                                If Not prop2.name.EndsWith("Specified") AndAlso Not prop2.name.StartsWith("Total") Then
                                    _builder.AppendFormat("            Case ""{0}""", prop2.name.Replace("cta_", ""))
                                    _builder.AppendLine()
                                    _builder.AppendLine(parentCode)
                                    _builder.AppendFormat("                If {0}.IsEmpty Then", prop.basetypefullname.Replace("Cta_", ".cta_").Replace("modelos.sugese.SaldosContables.ModeloSaldosContablesDatosModelo", "_SaldosContables.Datos.Modelo"))
                                    _builder.AppendLine()
                                    _builder.AppendFormat("                    {0} = New {1}", prop.basetypefullname.Replace("Cta_", ".cta_").Replace("modelos.sugese.SaldosContables.ModeloSaldosContablesDatosModelo", "_SaldosContables.Datos.Modelo"), prop.basetypefullname)
                                    _builder.AppendLine()
                                    _builder.AppendFormat("                End If")
                                    _builder.AppendLine()
                                    _builder.AppendFormat("                With {0}", prop.basetypefullname.Replace("Cta_", ".cta_").Replace("modelos.sugese.SaldosContables.ModeloSaldosContablesDatosModelo", "_SaldosContables.Datos.Modelo"))
                                    _builder.AppendLine()
                                    If prop2.basetypefullname.StartsWith("System.Nullable") Then
                                        _builder.AppendFormat("                    .{0} = New System.Nullable(Of System.Decimal)", prop2.name)
                                        _builder.AppendLine()
                                    ElseIf Not prop2.basetypefullname.StartsWith("System.Decimal") Then
                                        _builder.AppendFormat("                    .{0} = New System.Nullable(Of System.Decimal)", prop2.name)
                                    End If

                                    _builder.AppendFormat("                    .{0} = sheet.FormatDecimalValue", prop2.name)

                                    Select Case RestrictionTypeName(prop2.name)
                                        Case "res:MontoPositivo"
                                            _builder.Append("OnlyPositive")
                                        Case "res:MontoNegativo"
                                            _builder.Append("OnlyNegative")
                                        Case "res:Monto"
                                            _builder.Append("")
                                        Case Else
                                            _builder.Append("")
                                    End Select

                                    _builder.AppendFormat("(rowNumber, ""B"", ""0.00"", Response, sheetName", prop2.name)
                                    If haveSpecified(childItem, prop2) Then
                                        _builder.AppendFormat(", .{0}Specified", prop2.name)
                                    End If
                                    _builder.AppendFormat(")")

                                    _builder.AppendLine()
                                    _builder.AppendFormat("                End With")
                                    _builder.AppendLine()
                                End If
                            Next

                        Else
                            Dim currentCode As New StringBuilder
                            currentCode.Append(parentCode)
                            currentCode.AppendFormat("                If {0}.IsEmpty Then", prop.basetypefullname.Replace("Cta_", ".cta_").Replace("modelos.sugese.SaldosContables.ModeloSaldosContablesDatosModelo", "_SaldosContables.Datos.Modelo"))
                            currentCode.AppendLine()
                            currentCode.AppendFormat("                    {0} = New {1}", prop.basetypefullname.Replace("Cta_", ".cta_").Replace("modelos.sugese.SaldosContables.ModeloSaldosContablesDatosModelo", "_SaldosContables.Datos.Modelo"), prop.basetypefullname)
                            currentCode.AppendLine()
                            currentCode.AppendFormat("                End If")
                            currentCode.AppendLine()
                            SCAccount(childItem, currentCode.ToString)
                        End If
                    End If
                End If

            Next
        End Sub

        Private Shared Function isFlat(childItem As Entity) As Boolean
            Dim lastLevel As Boolean = False

            For Each prop2 As Element In childItem.elements
                lastLevel = lastLevel Or (Not prop2.basetypefullname.StartsWith("System."))
            Next
            Return Not lastLevel
        End Function

        Private Shared Function haveSpecified(childItem As Entity, prop As Element) As Boolean
            Dim lastLevel As Boolean = False

            For Each prop2 As Element In childItem.elements
                If prop2.name = (prop.name & "Specified") Then
                    lastLevel = True
                    Exit For
                End If
            Next
            Return lastLevel
        End Function

        Private Function RestrictionTypeName(account As String) As String
            Dim typename As String = String.Empty
            Dim node As XmlNode = _doc.SelectSingleNode("//xs:element[@name=""" & account & """]", _xnm)
            If Not IsNothing(node) Then
                typename = node.Attributes.GetNamedItem("type").Value
            End If
            Return typename
        End Function

    End Class

End Namespace
