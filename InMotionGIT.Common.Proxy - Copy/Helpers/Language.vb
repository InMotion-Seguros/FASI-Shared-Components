#Region "using"

Imports System.Drawing
Imports System.Globalization
Imports System.Resources
Imports System.Threading
Imports System.Web
Imports System.Xml

#End Region

Namespace Helpers

    Public Class Language

#Region "Data Base Methods"

        Public Shared Function Languages() As List(Of Services.Contracts.Language)
            Dim result As New List(Of Services.Contracts.Language)
            Dim key As String = "LanguagesAll"
            If Common.Helpers.Caching.NotExist(key) Then
                With New DataManagerFactory(" SELECT " +
                                            " 	LOOKUP.* " +
                                            " FROM " +
                                            " 	LOOKUPMASTER " +
                                            " INNER JOIN LOOKUP ON LOOKUPMASTER.LOOKUPID = LOOKUP.LOOKUPID " +
                                            " WHERE " +
                                            " 	LOOKUPMASTER.KEY = 'Language' " +
                                            " AND LOOKUP.RECORDSTATUS = 1 ",
                                             "TabLanguage", "FrontOfficeConnectionString")

                    .Cache = Enumerations.EnumCache.CacheWithCommand
                    Dim languageList = .QueryExecuteToTable(True)
                    Dim culturalCode As String = String.Empty

                    If languageList.IsNotEmpty() AndAlso languageList.Rows.Count <> 0 Then
                        For Each item As DataRow In languageList.Rows
                            If Not IsNothing(languageList.Columns.Item("HOMOLOGOUSCODE")) Then
                                culturalCode = item.StringValue("HOMOLOGOUSCODE")
                            Else
                                culturalCode = String.Empty
                            End If
                            result.Add(New Services.Contracts.Language With {.Code = item.IntegerValue("Code"), .LanguageId = item.IntegerValue("LANGUAGEiD"), .Description = item.StringValue("DESCRIPTION"), .CulturalCode = culturalCode})
                        Next
                    End If
                    InMotionGIT.Common.Helpers.Caching.SetItem(key, result)
                End With
            Else
                result = DirectCast(InMotionGIT.Common.Helpers.Caching.GetItem(key), List(Of Services.Contracts.Language))
            End If
            Return result
        End Function

        Public Shared Function LanguagesByCultural(culturalCode As String) As List(Of Services.Contracts.Language)
            Dim resultAll = Languages()
            Dim result = (From itemDb In resultAll Where itemDb.CulturalCode.ToLower().Equals(culturalCode.ToLower()) Select itemDb).ToList()
            If result.IsNotEmpty() Then
                result = (From itemDb In resultAll Where itemDb.CulturalCode.ToLower().Equals("en".ToLower()) Select itemDb).ToList()
            End If
            Return result
        End Function

        Public Shared Function LanguagesById(languageId As Integer) As List(Of Services.Contracts.Language)
            Dim resultAll = Languages()
            Dim result = (From itemDb In resultAll Where itemDb.LanguageId = languageId Select itemDb).ToList()
            If result.IsEmpty() Then
                result = (From itemDb In resultAll Where itemDb.LanguageId = 1 Select itemDb).ToList()
            End If
            Return result
        End Function

        Public Shared Function LanguageToDictionary() As Dictionary(Of Integer, String)
            Return LanguageToDictionary(1)
        End Function

        Public Shared Function LanguageToDictionary(language As Integer) As Dictionary(Of Integer, String)
            Dim result As New Dictionary(Of Integer, String)
            Dim resultAll = Languages()
            Dim filterd = (From itemDb In resultAll Where itemDb.LanguageId = language Select itemDb).ToList()
            For Each item In filterd
                result.Add(item.Code, item.Description)
            Next
            'Try
            '    With New DataManagerFactory(String.Format(CultureInfo.InvariantCulture,
            '                                        "SELECT TabLanguage.LanguageCode, TransLanguage.Description " &
            '                                        "FROM TabLanguage " &
            '                                        "LEFT JOIN TransLanguage ON TransLanguage.LanguageCode = TabLanguage.LanguageCode AND TransLanguage.LanguageID = {0} " &
            '                                        "WHERE TabLanguage.RecordStatus = 1 ORDER BY Description", language),
            '                                    "TabLanguage", "FrontOfficeConnectionString")

            '        .Cache = Enumerations.EnumCache.CacheWithCommand
            '        languageList = .QueryExecuteToTable(True)
            '    End With
            'Catch ex As Exception

            'End Try

            'If Not IsNothing(languageList) AndAlso (Not IsNothing(languageList.Rows) AndAlso languageList.Rows.Count > 0) Then
            '    For Each languageItem As DataRow In languageList.Rows
            '        result.Add(languageItem("LanguageCode"), languageItem("Description"))
            '    Next

            '    Common.Helpers.Caching.SetItem("Languages", result, -1)
            'End If

            Return result
        End Function

        Public Shared Function SetLanguageCodeByDescription(value As String) As Integer
            Dim languagesList As Dictionary(Of Integer, String) = LanguageToDictionary()
            Dim result As Integer = 0

            For Each languageItem As KeyValuePair(Of Integer, String) In languagesList
                If String.Equals(languageItem.Value, value, StringComparison.CurrentCultureIgnoreCase) Then

                    result = languageItem.Key
                    Exit For
                End If
            Next

            Return result
        End Function

        Public Shared Function SetLanguageDescriptionByCode(value As Integer) As String
            Dim languagesList As Dictionary(Of Integer, String) = LanguageToDictionary()
            Dim result As String = String.Empty

            For Each languageItem As KeyValuePair(Of Integer, String) In languagesList
                If languageItem.Key = value Then

                    result = languageItem.Value
                    Exit For
                End If
            Next

            Return result
        End Function

        Public Shared Function DescriptionToEnumLanguage(description As String, language As Integer) As Integer
            Dim result As Integer
            Dim languagesList As Dictionary(Of Integer, String) = LanguageToDictionary(language)

            For Each languageItem As KeyValuePair(Of Integer, String) In languagesList
                If String.Equals(languageItem.Value, description, StringComparison.CurrentCultureIgnoreCase) Then

                    result = languageItem.Key

                    Exit For
                End If
            Next

            Return result
        End Function

        ''' <summary>
        ''' Extrae todas los valores de determinado key en todos los recursos./Extracts all key values determined in all resources.
        ''' </summary>
        ''' <param name="keyName"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function GetAllTraductions(keyName As String) As List(Of DataType.LookUpValue)
            Dim result As New List(Of DataType.LookUpValue)
            Dim key As String = String.Format("GetAllTraductions_{0}", keyName)
            If Common.Helpers.Caching.NotExist(key) Then

                Dim resourceManager = My.Resources.ResourceManager()
                If resourceManager.GetString(keyName).IsNotEmpty() Then
                    Dim vector = resourceManager.GetString(keyName).Split(";")
                    For Each vectorItem In vector
                        result.Add(New DataType.LookUpValue() With {.Code = vectorItem.Split(":")(0),
                                                                    .Description = vectorItem.Split(":")(1)})
                    Next
                End If

                Common.Helpers.Caching.SetItem(key, result)
            Else
                result = Common.Helpers.Caching.GetItem(key)
            End If

            Return result
        End Function

        ''' <summary>
        ''' Extrae todas los valores de determinado key en todos los recursos pero por código de traducción ./Extracts all key values determined in all resources by code lenguaje
        ''' </summary>
        ''' <param name="keyName"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function GetAllTraductionsByCodeLanguageId(keyName As String) As List(Of DataType.LookUpValue)
            Dim key As String = String.Format("GetAllTraductionsByCodeLanguageId_{0}", keyName)
            Dim result As New List(Of DataType.LookUpValue)
            Dim namePathFile As String() = System.IO.Directory.GetFiles(String.Format("{0}App_GlobalResources", System.Web.HttpContext.Current.Request.PhysicalApplicationPath), "*.resx")
            If Common.Helpers.Caching.NotExist(key) Then
                For Each itemPathFile In namePathFile
                    Dim loResource As New XmlDocument()
                    loResource.Load(itemPathFile)
                    Dim loRoot As XmlNode = loResource.SelectSingleNode(String.Format("root/data[@name='{0}']/value", keyName))
                    If Not IsNothing(loRoot) Then
                        Dim value = loRoot.InnerText
                        Dim nameFile = System.IO.Path.GetFileNameWithoutExtension(itemPathFile).Split(".")
                        Dim culturalName As String = String.Empty
                        If nameFile.Count = 1 Then
                            culturalName = "en"
                        Else
                            culturalName = nameFile(1).ToLower
                        End If

                        Dim LangCode = GetLanguageIdCurrentContext(culturalName)
                        If LangCode > 0 Then
                            result.Add(New DataType.LookUpValue() With {.Code = LangCode, .Description = value})
                        End If

                    End If
                Next
                If result.IsNotEmpty Then
                    result = result.OrderBy(Function(X) X.Code).ToList
                    Dim resullTemporal As New List(Of DataType.LookUpValue)

                    For Each Item As DataType.LookUpValue In result
                        Dim temporalIteim As DataType.LookUpValue = (From itemLocal In resullTemporal Where itemLocal.Code = Item.Code AndAlso itemLocal.Description.Equals(Item.Description) Select itemLocal).FirstOrDefault
                        If temporalIteim.IsEmpty Then
                            resullTemporal.Add(Item)
                        End If
                    Next

                    result = resullTemporal
                End If
                Common.Helpers.Caching.SetItem(key, result)
            Else
                result = Common.Helpers.Caching.GetItem(key)
            End If

            Return result
        End Function

        ''' <summary>
        ''' obtiene la traducción del valor del lenguaje/ obtains the value of language translation
        ''' </summary>
        ''' <param name="languageId"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function GetLanguageNameByLanguageId(languageId As Integer) As String
            Dim result As String = String.Empty
            Dim listValueLanguageTraductions = LookUpLanguageByCurrentInfo(languageId)
            result = (From itemLanguageEnable In listValueLanguageTraductions
                      Where itemLanguageEnable.Code.Equals(languageId.ToString)
                      Select itemLanguageEnable.Description).FirstOrDefault()
            If String.IsNullOrEmpty(result) Then
                result = (From itemLanguageEnable In listValueLanguageTraductions
                          Select itemLanguageEnable.Description).FirstOrDefault()
            End If
            Return result
        End Function

        ''' <summary>
        ''' Obtiene los posibles valores con el código de lenguaje del thread.currentinfo.name actual o sobrecargado con un id definido por el usuario/Possible values obtained with the current language code or overloaded thread.currentinfo.name a user defined id
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function LookUpLanguageByCurrentInfo() As List(Of DataType.LookUpValue)
            Return LookUpLanguageByCurrentInfo(-1)
        End Function

        ''' <summary>
        ''' Obtiene los posibles valores con el código de lenguaje del thread.currentinfo.name actual o sobrecargado con un id definido por el usuario/Possible values obtained with the current language code or overloaded thread.currentinfo.name a user defined id
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function LookUpLanguageByCurrentInfo(LenguageId As Integer) As List(Of DataType.LookUpValue)
            Dim cultureTemporal As CultureInfo = Thread.CurrentThread.CurrentCulture
            Dim listCultureInfoEnable As New List(Of DataType.LookUpValue)
            If LenguageId = -1 Then
                LenguageId = GetLanguageIdCurrentContext()
            Else
                LenguageId = ExistCode(LenguageId)
            End If
            Dim languageById = LanguagesById(LenguageId)
            If languageById.IsNotEmpty() Then
                For Each item In languageById
                    listCultureInfoEnable.Add(New DataType.LookUpValue With {.Code = item.Code, .Description = item.Description})
                Next
            End If
            Return listCultureInfoEnable
        End Function

        ''' <summary>
        ''' Obtiene los posibles valores con el código de language del thread.currentinfo.name actual o sobrecargado con un id definido por el usuario/Possible values obtained with the current language code or overloaded thread.currentinfo.name a user defined id
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function LookUpLanguageByCurrentInfoExtend() As DataType.LookUpValueCollection
            Dim value As List(Of DataType.LookUpValue) = LookUpLanguageByCurrentInfo()
            Dim result As New DataType.LookUpValueCollection
            For Each item In value
                result.Add(New DataType.LookUpValue With {.Code = item.Code, .Description = item.Description})
            Next
            Return result
        End Function

        ''' <summary>
        ''' Obtiene el id del lenguage con el actual thread.curren.curreninfo.name/Gets the id of the current language thread.curren.curreninfo.name
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function GetLanguageIdCurrentContext() As Integer
            Return GetLanguageIdCurrentContext(String.Empty)
        End Function

        ''' <summary>
        ''' Obtiene el id del lenguaje con el actual currentinfo.nama defindo por el usuarioGets the id of language to the current user currentinfo.nama defindo
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function GetLanguageIdCurrentContext(CultureInfoName As String) As Integer
            Dim cultureTemporal As CultureInfo = Thread.CurrentThread.CurrentCulture
            Dim listCultureInfoEnable = Languages()
            If listCultureInfoEnable.IsEmpty OrElse
                listCultureInfoEnable.Count = 0 Then
                Throw New InMotionGIT.Common.Exceptions.InMotionGITException("The 'frontoffice' table 'TABLANGUAGE' is empty")
            End If

            If CultureInfoName.IsEmpty Then
                If Not IsNothing(HttpContext.Current.Session("App_LanguageId")) Then
                    Return Convert.ToInt32(HttpContext.Current.Session("App_LanguageId").ToString())
                End If
                CultureInfoName = cultureTemporal.Name.ToLower()
            Else
                CultureInfoName = CultureInfoName.ToLower
            End If

            Dim foundValue As String = (From itemLanguageEnable In listCultureInfoEnable
                                        Where itemLanguageEnable.CulturalCode.ToLower().Equals(CultureInfoName)
                                        Select itemLanguageEnable.Code).FirstOrDefault()
            If String.IsNullOrEmpty(foundValue) Then
                If CultureInfoName.Split("-").Count() = 1 Then
                    foundValue = (From itemLanguageEnable In listCultureInfoEnable
                                  Where itemLanguageEnable.CulturalCode.ToLower().StartsWith(CultureInfoName)
                                  Select itemLanguageEnable.Code).FirstOrDefault()
                Else
                    foundValue = (From itemLanguageEnable In listCultureInfoEnable
                                  Where itemLanguageEnable.CulturalCode.ToLower().StartsWith(CultureInfoName.Split("-")(0))
                                  Select itemLanguageEnable.Code).FirstOrDefault()
                End If
            End If

            Return If(String.IsNullOrEmpty(foundValue), 0, Integer.Parse(foundValue))
        End Function

        Public Shared Function CulturalCodeAllowed(CultureInfoName As String) As Boolean
            Dim result As Boolean = False
            Dim listCultureInfoEnable = Languages()
            Dim foundValue As String = (From itemLanguageEnable In listCultureInfoEnable
                                        Where itemLanguageEnable.CulturalCode.ToLower().Equals(CultureInfoName)
                                        Select itemLanguageEnable.Code).FirstOrDefault()
            If foundValue.IsNotEmpty() Then
                result = True
            End If
            Return result
        End Function

        ''' <summary>
        ''' Retorna un cultureInfo.Name válido por cultureInfo.Name/Returns a valid cultureInfo.Name by cultureInfo.Name
        ''' </summary>
        ''' <param name="cultureName"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function GetLanguageNameByCultureInfo(cultureName As String) As String
            cultureName = cultureName.ToLower
            Dim result As String = String.Empty
            Dim listCultureInfoEnable = GetAllCultureInfoName()
            Dim foundValue As String = (From itemLanguageEnable In listCultureInfoEnable
                                        Where itemLanguageEnable.Description.ToLower().Equals(cultureName)
                                        Select itemLanguageEnable.Description).FirstOrDefault()
            If String.IsNullOrEmpty(foundValue) Then
                If cultureName.Split("-").Count() = 1 Then
                    foundValue = (From itemLanguageEnable In listCultureInfoEnable
                                  Where itemLanguageEnable.Description.ToLower().StartsWith(cultureName)
                                  Select itemLanguageEnable.Description).FirstOrDefault()
                Else
                    foundValue = (From itemLanguageEnable In listCultureInfoEnable
                                  Where itemLanguageEnable.Description.ToLower().StartsWith(cultureName.Split("-")(0))
                                  Select itemLanguageEnable.Description).FirstOrDefault()
                End If
            End If
            result = foundValue
            Return result
        End Function

        ''' <summary>
        ''' Obtiene un loopUp de todos los cultureinfo.name disponible en base de datos/Gets a loopUp of all available cultureinfo.name database
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function GetAllCultureInfoName() As List(Of DataType.LookUpValue)
            Dim listCultureInfoEnable As New List(Of DataType.LookUpValue)
            Dim listCultureInfoEnableAll = Languages()
            Dim fitered = (From itemDb In listCultureInfoEnableAll Select itemDb.Code, itemDb.CulturalCode Distinct).ToList()

            For Each item In fitered
                listCultureInfoEnable.Add(New DataType.LookUpValue With {.Code = item.Code, .Description = item.CulturalCode})
            Next

            Return listCultureInfoEnable
        End Function

        ''' <summary>
        ''' Verifica la existencia de un id de lenguaje con respecto a los existentes en base de datos/Checks for an id of language with respect to the existing database
        ''' </summary>
        ''' <param name="LanguageCode"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function ExistCode(LanguageCode As Integer) As Integer
            Dim result As String = String.Empty
            Dim listCultureInfoEnable = Languages()

            result = (From CultureInfoItem In listCultureInfoEnable
                      Where CultureInfoItem.LanguageId = LanguageCode
                      Select CultureInfoItem.LanguageId).FirstOrDefault()

            If result.IsEmpty Then
                result = (From CultureInfoItem In listCultureInfoEnable
                          Select CultureInfoItem.LanguageId).FirstOrDefault()
            End If
            Return result
        End Function

        ''' <summary>
        ''' Se obtiene el cultureinfo.name por medio de código /The cultureinfo.name is obtained through code
        ''' </summary>
        ''' <param name="LanguageCode"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function GetCultureInfoByCode(LanguageCode As Integer) As String
            Dim result As String = String.Empty
            Dim listCultureInfoEnable = Languages()
            result = (From CultureInfoItem In listCultureInfoEnable
                      Where CultureInfoItem.Code = LanguageCode
                      Select CultureInfoItem.CulturalCode).FirstOrDefault()

            If result.IsEmpty Then
                result = (From CultureInfoItem In listCultureInfoEnable
                          Select CultureInfoItem.CulturalCode).FirstOrDefault()
            End If
            Return result
        End Function

#End Region

#Region "Extension Methods"

        Public Shared Function GetResourceExtension(currentLanguage As Integer) As String
            Dim result As String = String.Empty
            Dim languagesAll As List(Of Services.Contracts.Language) = Languages()

            result = (From itemDb In languagesAll Where itemDb.Code = currentLanguage Select itemDb.CulturalCode).FirstOrDefault()

            Return result
        End Function

        Public Shared Function GetResourceLanguage(extension As String) As Integer
            Dim result As Integer = -1
            Dim languagesAll = Languages()
            result = (From itemDb In languagesAll Where itemDb.CulturalCode = extension Select itemDb.LanguageId).FirstOrDefault()
            Return result
        End Function

#End Region

#Region "Get Flag Image Methods"

        Public Shared Function GetLanguageLargeImage(key As Integer) As Image
            Select Case key
                Case 1
                    Return My.Resources.unitedStates

                Case 2
                    Return My.Resources.spain

                Case 3
                    Return My.Resources.portugal

                Case 4
                    Return My.Resources.netherlands

                Case Else
            End Select

            Return Nothing
        End Function

        Public Shared Function GetLanguageImage(key As Integer) As Image
            Select Case key
                Case 1
                    Return My.Resources.us

                Case 2
                    Return My.Resources.es

                Case 3
                    Return My.Resources.pt

                Case 4
                    Return My.Resources.nl
                Case Else
            End Select

            Return Nothing
        End Function

#End Region

    End Class

End Namespace