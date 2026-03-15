@ECHO OFF
CLS
ECHO ******************************************************
ECHO.    
ECHO.    Drivers.NExT Batch Compile Routine
ECHO.
ECHO ******************************************************
ECHO.
ECHO 1   Compile without getting lastest revision
ECHO.
ECHO 2   Compile by getting latest revision
ECHO.
ECHO 3   Compile by getting latest revision and increment the build number
ECHO.
ECHO C   Exit without do anything
ECHO.
CHOICE /C 123C /T 10 /D 1 /M "Choose a compile option"

IF ERRORLEVEL 4 GOTO End
IF ERRORLEVEL 3 GOTO Opt3
IF ERRORLEVEL 2 GOTO Opt2
IF ERRORLEVEL 1 GOTO Opt1

GOTO End

:Opt3
SET SkipHgPull=false
SET SkipIncrementAssemblyVersion=false
GOTO Execute


:Opt2
SET SkipHgPull=false
SET SkipIncrementAssemblyVersion=true
GOTO Execute

:Opt1
SET SkipHgPull=true
SET SkipIncrementAssemblyVersion=true
GOTO Execute

:Execute
cmd /K "%windir%\Microsoft.NET\Framework\v4.0.30319\MSBuild.exe" /nologo /filelogger Drivers.proj /property:SkipIncrementAssemblyVersion=%SkipIncrementAssemblyVersion%;SkipHgPull=%SkipHgPull%

:End

