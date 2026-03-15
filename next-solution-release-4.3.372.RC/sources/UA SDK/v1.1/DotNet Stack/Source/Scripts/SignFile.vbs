Option Explicit

Dim shell, fso, environment
Dim pos
Dim StdIn, StdOut
Dim filePath, fileName, dirName, rootDir, srcFile, keyFile, cmdText, command, buffer, inChar, setupPath, archiverPath, archiveName
Dim buildNumber
Set StdIn = WScript.StdIn
Set StdOut = WScript.StdOut

Set shell = WScript.CreateObject("WScript.Shell")

If WScript.Arguments.Count > 0 Then
    filePath = WScript.Arguments(0)
End If

If WScript.Arguments.Count > 1 Then
    rootDir = WScript.Arguments(1)
End If

pos = InstrRev(filePath, "\", -1)

fileName = filePath
  WScript.Echo fileName

If pos > 0 Then
  fileName = Mid(filePath, pos+1)
  dirName  = Mid(filePath, 1, pos-1)
End If

Set fso = CreateObject("Scripting.FileSystemObject")

keyFile = rootDir & "\..\Keys\OPC Authenticode Key 2015.pfx"

If Not fso.FileExists(keyFile) Then
   keyFile = rootDir & "\..\..\Keys\OPC Authenticode Key 2015.pfx"
End If
  
SignFile shell, fso, filePath, keyFile
  
If Right(filePath, 3) = "msm" Then
    CopyFile filePath, rootDir & "Merge Modules"
End If

If Right(filePath, 3) = "msi" Then
    setupPath = dirName & "\" & "setup.exe"
    Wscript.Echo setupPath
    If fso.FileExists(setupPath) Then
       SignFile shell, fso, setupPath, keyFile
    End If
End If

If Right(filePath, 3) = "msi" Then

    Set environment = shell.Environment("Process")
    archiverPath = environment("ProgramFiles") & "\7-Zip\7z.exe"

    If fso.FileExists(archiverPath) Then

        CopyFile rootDir & "Docs\OPC UA SDK 1.01 Readme.rtf", dirName

        pos = InstrRev(filePath, ".", -1)

        archiveName = filePath

        If pos > 0 Then
          archiveName = Mid(filePath, 1, pos-1)
        End If
                
        buildNumber = LoadVersion(rootDir, fso)
        
        If Len(buildNumber) > 0 Then
            archiveName = archiveName & " " & buildNumber
            WScript.Echo "Renaming Archive To: " & archiveName
        End If
        
        cmdText = Chr(34) & archiverPath & Chr(34)
        cmdText = cmdText & " a "
        cmdText = cmdText & Chr(34) & archiveName & ".zip" & Chr(34)
        cmdText = cmdText & " *.*"
          
        Execute shell, cmdText
              
        CopyFile archiveName & ".zip", rootDir & "Published"
      
    End If

End If

Sub Execute(shell, cmdText)

  Dim command, buffer
  
  Set command = shell.Exec(cmdText)

  Do While command.Status = 0

     If Not command.StdOut.AtEndOfStream Then
        inChar = command.StdOut.Read(1)
        
        If inChar <> vbCr Then
           buffer = buffer & inChar
        End If     
     End If

  Loop

  WScript.Echo buffer
  
End Sub
        
Sub CopyFile(filePath, targetDir)

  Dim srcFile

  If fso.FileExists(filePath) Then
    set srcFile = fso.GetFile(filePath)
    srcFile.Copy targetDir & "\" & srcFile.Name, True
    Wscript.Echo "Copied " & filePath
  End If

End Sub

Sub SignFile(shell, fso, filePath, keyFile)

  Wscript.Echo "Signing file " & filePath
  Wscript.Echo "Key file " & keyFile

  Dim exeText
  exeText =  shell.Environment("Process").item("ProgramFiles") & "\Microsoft SDKs\Windows\v7.0A\Bin\signtool.exe"

  If Not fso.FileExists(exeText) Then
    exeText = shell.Environment("Process").item("ProgramFiles(x86)") & "\Microsoft SDKs\Windows\v7.0A\Bin\signtool.exe"
  End If

  If Not fso.FileExists(exeText) Then
    exeText = shell.Environment("Process").item("ProgramFiles") & "\Microsoft SDKs\Windows\v6.0A\Bin\signtool.exe"
  End If

  If Not fso.FileExists(exeText) Then
    exeText = shell.Environment("Process").item("ProgramFiles(x86)") & "\Microsoft SDKs\Windows\v6.0A\Bin\signtool.exe"
  End If

  If Not fso.FileExists(exeText) Then
    exeText = shell.Environment("Process").item("SystemDrive") & "\Program Files\Microsoft SDKs\Windows\v6.0A\Bin\signtool.exe"
  End If

  If Not fso.FileExists(exeText) Then
    Wscript.Echo "WARNING: Cannot find signtool.exe!"
  End If

  If fso.FileExists(keyFile) Then

    cmdText = Chr(34) & exeText & Chr(34)
    cmdText = cmdText & " sign /v" 
    cmdText = cmdText & " /f " & Chr(34) & keyFile & Chr(34)
    cmdText = cmdText & " /p opcua2010 /du http://www.opcfoundation.org"
    cmdText = cmdText & " /t http://timestamp.verisign.com/scripts/timestamp.dll"
    cmdText = cmdText & " " & Chr(34) & filePath & Chr(34)
  
    Execute shell, cmdText

  End If

End Sub

Function LoadVersion(rootDir, fso)

    Dim versionFile
    versionFile = rootDir & "\BuildVersion.txt"
    LoadVersion = ""
  
    If fso.FileExists(versionFile) Then
    
        Dim objFile
        Set objFile = fso.OpenTextFile(rootDir & "\BuildVersion.txt", ForReading)
        Const ForReading = 1

        Dim arrFileLines(), ii
        ii = 0
        Do Until objFile.AtEndOfStream 
            Redim Preserve arrFileLines(ii)
            arrFileLines(ii) = objFile.ReadLine
            ii = ii + 1
        Loop
        objFile.Close
        
        LoadVersion = Trim(arrFileLines(0))
        
    End If

End Function