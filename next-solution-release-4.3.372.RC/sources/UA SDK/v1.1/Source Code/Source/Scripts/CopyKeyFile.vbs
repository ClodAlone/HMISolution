Option Explicit

Dim shell, fso
Dim projectPath, rootDir, srcFile, dstFile, keyFile, versionFile, assemblyFile

Set shell = WScript.CreateObject("WScript.Shell")

If WScript.Arguments.Count > 0 Then
    projectPath = WScript.Arguments(0)
End If

If WScript.Arguments.Count > 1 Then
    rootDir = WScript.Arguments(1)
End If

Set fso = CreateObject("Scripting.FileSystemObject")

assemblyFile = projectPath & "\Properties\AssemblyInfo.cs"  

If fso.FileExists(assemblyFile) Then

  versionFile = rootDir & "\Source\Include\AssemblyVersionInfo.cs"

  If fso.FileExists(versionFile) Then

    Set srcFile = fso.GetFile(versionFile)
    
    Dim dstPath
    dstPath = projectPath & "\Properties\AssemblyVersionInfo.cs"
    
    If fso.FileExists(dstPath) Then
        Set dstFile = fso.GetFile(dstPath)
        
        If dstFile.DateLastModified = srcFile.DateLastModified Then
            dstPath = ""            
            WScript.Echo "Version File Not Updated: " & versionFile
        End If
    End If
    
    If dstPath <> "" Then
        srcFile.Copy dstPath  
        WScript.Echo "Copied Version File: " & versionFile
    End If

  End If

End If

keyFile = rootDir & "..\Keys\OPC Key Pair.master (UA).snk"

If Not fso.FileExists(keyFile) Then
   keyFile = rootDir & "..\..\Keys\OPC Key Pair.master (UA).snk"
End If

If Not fso.FileExists(keyFile) Then
   keyFile = rootDir & "\Keys\OPC Key Pair.test.snk"
End If
  
If fso.FileExists(keyFile) Then

  Set srcFile = fso.GetFile(keyFile)
  srcFile.Copy projectPath & "OPC Key Pair.snk", True  

  WScript.Echo "Copied Key File: " & keyFile

End If