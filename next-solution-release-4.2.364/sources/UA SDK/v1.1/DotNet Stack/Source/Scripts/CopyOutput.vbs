Option Explicit

Dim shell, fso
Dim projectPath, rootDir, targetPath, targetRoot, keyFile, pos

Set shell = WScript.CreateObject("WScript.Shell")

If WScript.Arguments.Count > 0 Then
    projectPath = WScript.Arguments(0)
End If

If WScript.Arguments.Count > 1 Then
    rootDir = WScript.Arguments(1)
End If

If WScript.Arguments.Count > 2 Then
    targetPath = WScript.Arguments(2)
End If

Set fso = CreateObject("Scripting.FileSystemObject")

keyFile = projectPath & "OPC Key Pair.snk" 

If fso.FileExists(keyFile) Then
  fso.DeleteFile keyFile, True  
End If

pos = InstrRev(targetPath, ".", -1)

targetRoot = targetPath

If pos > 0 Then
  targetRoot = Mid(targetPath, 1, pos-1)
End If

CopyFile targetRoot & ".exe", rootDir
CopyFile targetRoot & ".dll", rootDir
CopyFile targetRoot & ".pdb", rootDir
CopyFile targetRoot & ".xml", rootDir
CopyFile targetRoot & ".exe.config", rootDir
CopyFile targetRoot & ".Config.xml", rootDir

Sub CopyFile(filePath, rootDir)

  Dim srcFile

  If fso.FileExists(filePath) Then
    set srcFile = fso.GetFile(filePath)
    srcFile.Copy rootDir & "\Source\Bin\" & srcFile.Name, True
    Wscript.Echo "Copied " & filePath
  End If

End Sub