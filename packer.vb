Imports System.IO
Imports System.Collections.Generic
Imports System.Text

Module packer



    Public showOutput As Boolean = True


    Public fileListData As New Collection
    'packet file keys for output
    Public cacheKeys As New Collection
    Public fileList As New List(Of String)
    Public packFormats As String = "*"
    'paket modu css,js
    Public packMode As String = ""
    '0 = no limit
    Public packSize As Long = 0
    Public packTarget As String
    Public includeSubDirectories As Boolean = True
    Public canPack As Boolean = False

    Public headerPacked As Boolean = False

    Public packOutput As String = ""
    Public packedTotalSize As Long = 0
    Public packedSize As Long = 0

    Public outPutList As ArrayList = New ArrayList()





    Public Function GetMimeType(fileName As String) As String
        Dim mimeType As String = "application/unknown"
        Dim ext As String = System.IO.Path.GetExtension(fileName).ToLower()
        Dim regKey As Microsoft.Win32.RegistryKey = Microsoft.Win32.Registry.ClassesRoot.OpenSubKey(ext)

        If Not regKey Is Nothing And Not regKey.GetValue("Content Type") Is Nothing Then

            mimeType = regKey.GetValue("Content Type").ToString()

            Return mimeType
        End If

        Return mimeType
    End Function


    Public Sub writeConsole(txt As String)
        If showOutput = True Then
            System.Console.WriteLine(txt)
        End If


    End Sub

    Public Function readDirectory(path As String) As Boolean
        Dim fInfo() As String



        For Each foundFile As String In My.Computer.FileSystem.GetFiles(path)


            fInfo = getFileNameInfo(getFileName(foundFile))


            If packFormats = "*" Then
                fileList.Add(foundFile)


            Else


                If fInfo(1) <> "" Then
                    If packFormats.IndexOf(fInfo(1) & ",") >= 1 Then
                        fileList.Add(foundFile)
                    End If
                End If


            End If

        Next

        For Each foundDir As String In My.Computer.FileSystem.GetDirectories(path)

            writeConsole("Packing files in directory... " & foundDir)
            readDirectory(foundDir)

            '            For Each foundFileX As String In My.Computer.FileSystem.GetFiles(foundFile)
            '           fileList.Add(foundFileX)
            '      Next
        Next
        Return True
    End Function

    Public Function b64encode(data() As Byte) As String
        Return System.Convert.ToBase64String(data)
    End Function


    Public Function packFileBASE64(filePath As String) As Byte()
        Dim returns() As Byte = File.ReadAllBytes(filePath)

        Return returns
    End Function

    Public Function getFileName(pathx As String) As String
        Dim ars() As String = Split(pathx, "\")
        Return ars(UBound(ars))
    End Function

    Public Function getFileNameInfo(filename As String) As String()

        Dim retString(2) As String


        If InStr(1, filename, ".") >= 1 Then
            Dim keyss() As String = Split(filename, ".")


            retString(0) = String.Join(".", keyss, 0, UBound(keyss))
            retString(1) = keyss.Last()


        Else
            retString(0) = filename
            retString(1) = ""


        End If
        Return retString
    End Function

    Public Function packList() As Boolean


        For i = 0 To fileList.Count - 1

            Dim fil As String = fileList(i)
            Dim tRec As New clsFileData



            tRec.fileName = getFileName(fil)



            tRec.path = fil
            tRec.type = GetMimeType(fil)
            tRec.data = b64encode(packFileBASE64(fil))


            tRec.key = packKey(tRec.fileName)




            fileListData.Add(tRec)
            Dim tShot As String = Mid(tRec.data, 1, 30)

            writeConsole("Packing...(base64) " & tRec.path & " " & tRec.type & "  " & tShot)

            '    Console.WriteLine(tRec.fileName & " " & tRec.type & "  " & tShot)




        Next i
        Return True

    End Function

    Public Function getNewKey(fkey As String) As String
        Dim i As Integer

        If cacheKeys.Contains(fkey) = True Then
            For i = 0 To 1000
                If cacheKeys.Contains(fkey & i) = False Then
                    Return fkey
                End If

            Next i



            Return ""


        Else
            Return fkey

        End If
        Return ""

    End Function
    Public Function safeKey(data As String) As String
        Dim allowedChars As String = "qwertyuiopasdfghjklzxcvbnm._-0123456789"
        Dim tChar As String = ""

        For i = 1 To Len(data)
            tChar = Mid(data, i, 1)
            If InStr(1, allowedChars, tChar) < 1 Then
                data = Replace(data, tChar, "_")

            End If
        Next
        data = Replace(data, "__", "_")
        data = Replace(data, "__", "_")
        data = Replace(data, "__", "_")
        Return data

    End Function

    'pack key from filename  x1.png to  x1

    Public Function packKey(filename As String) As String
        Dim fileKeyName As String = filename
        Dim defFormat As String = ""
        Dim tempFileInfo() As String = getFileNameInfo(filename)


        If InStr(1, filename, ".") >= 1 Then
            fileKeyName = tempFileInfo(0)
            defFormat = tempFileInfo(1)
        End If

        fileKeyName = safeKey(fileKeyName)
        Dim tCheckKey As String = fileKeyName

        If tempFileInfo(1) <> "" Then
            tCheckKey = tCheckKey & "." & tempFileInfo(1)
        End If







        If cacheKeys.Contains(tCheckKey) = True Then
            Dim newKey As String = getNewKey(fileKeyName)
            If tempFileInfo(1) <> "" Then
                newKey = newKey & "." & tempFileInfo(1)
            End If

            fileKeyName = newKey
        Else

            fileKeyName = tCheckKey

        End If
        fileKeyName = Replace(fileKeyName, " ", "_")

        writeConsole("Assign key[" & fileKeyName & "] for file:" & filename)


        If fileKeyName = "" Then
            writeConsole("Cannot assign key for file:" & filename)
        End If

        Return fileKeyName


    End Function

    'add data to output
    Public Function addToPack(ByRef tFileRec As clsFileData) As Boolean
        Dim addData As String = ""
        'new packMode started
        If packMode = "" Then
            packMode = "js"

        End If


        If packMode = "js" Then
            If packOutput = "" Then
                If headerPacked = False Then
                    packOutput = "var Assets = {};"
                    headerPacked = True

                End If


            End If

        End If

        If packMode = "js" Then

            addData = "Assets['" & tFileRec.key & "'] ='data:" & tFileRec.type & ";base64," & tFileRec.data & "';" & Environment.NewLine
        Else

            addData = "." & Replace(tFileRec.key, ".", "_") & " { content: url(data:" & tFileRec.type & ";base64," & tFileRec.data & "); }" & Environment.NewLine



        End If


        packedSize = packedSize + Len(addData)

        packedTotalSize = packedTotalSize + packedSize


        'writeConsole(packedSize & "/" & packSize)

        packOutput = packOutput & addData
        Return True

    End Function
    Public Function writeToOutput() As Boolean
        Dim packIndex As Integer = 0

        Dim t As Integer


        Dim packetDump As String = ""
        Dim tRec As clsFileData


        outPutList.Add("")

        For Each tRec In fileListData




            If packSize = 0 Then
                addToPack(tRec)
            Else

                addToPack(tRec)

                outPutList(packIndex) = outPutList(packIndex) & packOutput


                If packedSize >= packSize Then
                    packIndex = packIndex + 1
                    outPutList.Add("")
                    packedSize = 0

                End If


                packOutput = ""











            End If

        Next


        writeConsole("Writing Outputs to files (" & outPutList.Count & " / " & packIndex & ") Total:" & packedTotalSize & " bytes")


        Dim outPutFileInfo As String() = getFileNameInfo(packTarget)
        Dim outPutFileName As String = getFileName(outPutFileInfo(0))
        Dim outPutFileEx As String = outPutFileInfo(1)
        Dim fi As New IO.FileInfo(packTarget)
        Dim outPath As String = fi.Directory.Root.ToString





        If packSize = 0 Then
            If System.IO.File.Exists(packTarget) = True Then
                System.IO.File.Delete(packTarget)
            End If


            writeToFile(packTarget, packOutput)
        Else

            writeConsole("Parting files by Size (" & packSize & ") (" & packIndex & ")")


            Dim outNewFile As String = outPutFileName


            For t = 0 To outPutList.Count - 1



                If outPutFileEx = "" Then
                    outNewFile = outPutFileName & t
                Else
                    outNewFile = outPutFileName & t & "." & outPutFileEx
                End If
                outNewFile = outPath & "\" & outNewFile
                If System.IO.File.Exists(outNewFile) = True Then
                    System.IO.File.Delete(outNewFile)
                End If

                writeToFile(outNewFile, outPutList(t))
            Next t

        End If


        writeConsole("Packing completed...")

        Return True
    End Function

    Public Function writeToFile(fileFullPath As String, fileData As String) As Boolean

        writeConsole("Write to file:" & fileFullPath)

        Try
            If Not System.IO.File.Exists(fileFullPath) = True Then
                Dim file As System.IO.FileStream
                file = System.IO.File.Create(fileFullPath)
                file.Close()
            End If

            My.Computer.FileSystem.WriteAllText(fileFullPath, fileData, True)

            writeConsole("wiriting to file ... " & fileFullPath)
            Return True
        Catch ex As Exception
            writeConsole(ex.ToString)

            Return False

        End Try
        Return True



    End Function


    Sub Main(args() As String)
        writeConsole("HTML5 Resource Packer by BlackStorm @ OyunPark")

        writeConsole("HTML5 Resource Packer usage: HTML5PACK <directory> <targetformat(css or js)> <packtypes(.png,.gif)> <packsize(1024)> <outputFileName> --topfolder --nooutput")


        '0 = klasor
        '1 = pack tipi css,js
        If args.Length <= 0 Then
            ReDim Preserve args(6)
        End If

        Dim commandLine As String = String.Join(" ", args)

        'include only top folder
        If InStr(1, commandLine, "--topfolder") >= 1 Then
            includeSubDirectories = False
        End If

        'write information to console
        If InStr(1, commandLine, "--nooutput") >= 1 Then
            includeSubDirectories = False
        End If


        If args.Length >= 1 Then

            If args(0) <> "" Then


                If args(0) = "" Then
                    args(0) = AppDomain.CurrentDomain.BaseDirectory & "\pack\"
                End If




                'target file type
                If args(1) <> "" Then

                    If packMode = "" Then
                        packMode = "js"

                    End If
                    packMode = LCase(args(1))
                    If packMode <> "js" And packMode <> "css" Then
                        packMode = "js"

                    End If
                End If


                'pack formats .gif,.png
                If args(2) = "" Then
                    args(2) = "*"


                Else
                    If args(2) <> "*" Then
                        packFormats = LCase(args(2)) & ","
                    End If

                End If

                    'max packed file size
                    If args(3) <> "" Then
                    If IsNumeric(args(3)) = True Then
                        packSize = CLng(args(3))
                    End If


                End If


                'target output file
                If args(4) <> "" Then
                    packTarget = args(4)
                    If packTarget = "" Then
                        packTarget = AppDomain.CurrentDomain.BaseDirectory & "\packed." & packMode

                    End If
                End If

            End If

        End If


        If args(0) = "" Then
            args(0) = AppDomain.CurrentDomain.BaseDirectory & "\pack\"
        End If



        If System.IO.Directory.Exists(args(0)) = False Then
            If System.IO.Directory.Exists(AppDomain.CurrentDomain.BaseDirectory & "\" & args(0)) Then
                args(0) = AppDomain.CurrentDomain.BaseDirectory & "\" & args(0)
            End If

            If System.IO.Directory.Exists(args(0)) = False Then
                writeConsole("ERROR: Target directory not exists!")

            Else
                canPack = True
            End If
        Else
            canPack = True


        End If



        If packMode = "" Then
            packMode = "js"
        End If



        If packTarget = "" Then
            packTarget = AppDomain.CurrentDomain.BaseDirectory & "\packed." & packMode
        End If
        If packFormats = "" Then
            packFormats = "*"

        End If

        If IsNumeric(args(3)) = True Then
            packSize = CLng(args(3))
        End If


        If canPack = True Then


            readDirectory(args(0))
            writeConsole("Packing... " & args(0) & "TargetFormat:" & packMode & " FileTypes:" & packFormats & " PackageSize:" & packSize & " Output:" & packTarget)

            packList()

            writeToOutput()
        End If

        Console.ReadLine()



    End Sub

End Module
