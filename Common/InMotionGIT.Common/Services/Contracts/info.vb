Imports System.IO
Imports System.Runtime.Serialization
Imports System.Threading.Tasks

Namespace Services.Contracts

    <DebuggerDisplay("{PathFullName}")>
    <DataContract()>
    Public Class info
        Private Shared _parallelsOptions As New ParallelOptions()

        Public Sub New()

        End Sub

        <DataMember()>
        Public Property Name As String

        <DataMember()>
        Public Property LastWrite As Date

        <DataMember()>
        Public Property Length As Long

        <DataMember()>
        Public Property IsFolder As Boolean

        <DataMember()>
        Public Property CheckSum As String

        <DataMember()>
        Public Property PathFullName As String

        <DataMember()>
        Public Property Childs As List(Of info)

        Public Shared Function Process(path As String, Optional parent As info = Nothing) As info
            _parallelsOptions.MaxDegreeOfParallelism = Environment.ProcessorCount * 10
            Dim folderBase As DirectoryInfo = Nothing
            Dim root As info = Nothing
            Dim current As info = Nothing

            Try
                If Not IsNothing(parent) Then
                    root = parent
                Else
                    root = New info With {.Name = System.IO.Path.GetFileName(path),
                                          .PathFullName = path,
                                          .IsFolder = True}
                End If
                If IsNothing(root.Childs) Then
                    root.Childs = New List(Of info)
                End If

                folderBase = New DirectoryInfo(path)
                Dim files = folderBase.GetFiles()
                Parallel.ForEach(files, _parallelsOptions, Sub(file)
                                                               root.Childs.Add(
                                                             New info With {
                                                                            .Name = file.Name,
                                                                            .PathFullName = file.FullName,
                                                                            .CheckSum = InMotionGIT.Common.Helpers.MD5Helper.CheckSum(file.FullName),
                                                                            .IsFolder = False,
                                                                            .LastWrite = file.LastWriteTime.ToLocalTime,
                                                                            .Length = file.Length
                                                                           })
                                                           End Sub)

                Dim folders = folderBase.GetDirectories()

                Parallel.ForEach(folders, _parallelsOptions, Sub(directory)
                                                                 current = New info With {
                                                                    .Name = System.IO.Path.GetFileName(directory.Name),
                                                                    .PathFullName = directory.FullName,
                                                                    .IsFolder = True,
                                                                    .LastWrite = directory.LastWriteTime.ToLocalTime
                                                                }
                                                                 root.Childs.Add(current)
                                                                 Process(directory.FullName, current)
                                                             End Sub)
            Catch ex As Exception
                root.Name = "Falla: " & ex.Message
            End Try

            If root.Childs.IsNotEmpty() Then
                root.Childs = root.Childs.OrderByDescending(Function(c) c.PathFullName).ToList()
            End If

            Return root
        End Function

    End Class

End Namespace