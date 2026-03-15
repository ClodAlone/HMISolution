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