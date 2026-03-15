@echo off
setlocal enabledelayedexpansion
SET msbEXe="C:\Program Files\Microsoft Visual Studio\2022\Professional\Msbuild\Current\Bin\MSBuild.exe"
%msbEXe% UFSolution.sln /t:clean;restore;build  /property:Configuration=Debug /property:platform="Any CPU"
If %errorlevel% NEQ 0 exit %errorlevel%
For /F %%t  in ('dir /B /A-D /S ..\..\*Tests.dll') do (

	SET fileName=%%t
	SET /A SkipTest=0
	For /F "delims=" %%s in (%~dp0TestsNamesToExclude.txt)do (
		If Not "!fileName:%%s=!"=="!fileName!" SET SkipTest=1
	)
	if "!fileName:obj=!"=="%%t" (
			If !SkipTest! EQU 0 (
				cd %%~pt
				"C:\Program Files\Microsoft Visual Studio\2022\Professional\Common7\IDE\CommonExtensions\Microsoft\TestWindow\vstest.console.exe" %%~nt%%~xt
				IF ERRORLEVEL 1 exit 1
			)
		)
)