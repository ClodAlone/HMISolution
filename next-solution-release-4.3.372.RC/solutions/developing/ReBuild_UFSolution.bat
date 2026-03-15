SET MSBuild_32Bit_Tool=="C:\Program Files\Microsoft Visual Studio\2022\Professional\Msbuild\Current\Bin\MSBuild.exe"
SET MSBuild_64Bit_Tool=="C:\Program Files\Microsoft Visual Studio\2022\Professional\Msbuild\Current\Bin\MSBuild.exe"

SET SignCheckFilePath=.\UFSolution\bin\Obfuscated\net48\MSZ.dll
SET CertFriendlyName=PROGEA SRL

@ECHO OFF
CLS
ECHO *******************************************************************
ECHO.    
ECHO.    Movicon.NExT Batch Compile Routine
ECHO.
ECHO *******************************************************************
ECHO.
ECHO 1   Platform.NExT : Left actual build number and compile
ECHO.
ECHO 2   Drivers.NExT : Left actual build number and compile
ECHO.
ECHO 3   Platform.NExT : Increment the build number and compile
ECHO.
ECHO 4   Drivers.NExT : Increment the build number and compile
ECHO.
ECHO 5   Connext : Left actual build number and compile
ECHO.
ECHO C   Exit without do anything
ECHO.
CHOICE /C 12345C /M "Choose a compile option"

IF ERRORLEVEL 5 GOTO Opt5
IF ERRORLEVEL 4 GOTO Opt4
IF ERRORLEVEL 3 GOTO Opt3
IF ERRORLEVEL 2 GOTO Opt2
IF ERRORLEVEL 1 GOTO Opt1

GOTO End

:Opt5
SET SkipIncrementAssemblyVersion=true
SET SkipObfuscation=true
GOTO Connext

:Connext

:: See if sigcheck is in the path
where sigcheck.exe 2>NUL 1>NUL
if not "%ERRORLEVEL%"=="0" echo sigcheck.exe is not in your path&& goto :End

:: Make sure the file exists
if not exist "%SignCheckFilePath%" echo "%SignCheckFilePath%" does not exist&& goto :End

:: Take the output from sigcheck, parse it and put it into a variable called VerifiedLine
for /F "delims=" %%l in ('sigcheck -nobanner "%SignCheckFilePath%"^|findstr Verified') do set VerifiedLine=%%l

:: See if the line contains "Unsigned"
if not "%VerifiedLine:Unsigned=%"=="%VerifiedLine%" SET CertFriendlyName=

IF EXIST "%PROGRAMFILES(X86)%" (GOTO 64BIT_2) ELSE (GOTO 32BIT_2)

:64BIT_2
echo 64-bit..
cmd /K %MSBuild_64Bit_Tool% /nologo /filelogger UFConnext.proj /property:SkipIncrementAssemblyVersion=%SkipIncrementAssemblyVersion%;SkipObfuscation=%SkipObfuscation%;CertFriendlyName="%CertFriendlyName%"

GOTO End

:32BIT_2
echo 32-bit...
cmd /K %MSBuild_32Bit_Tool% /nologo /filelogger UFConnext.proj /property:SkipIncrementAssemblyVersion=%SkipIncrementAssemblyVersion%;SkipObfuscation=%SkipObfuscation%;CertFriendlyName="%CertFriendlyName%"

GOTO End

:Opt4
SET SkipHgPull=true
SET SkipIncrementAssemblyVersion=false
SET SkipClean=false
GOTO Drivers

:Opt2
SET SkipHgPull=true
SET SkipIncrementAssemblyVersion=true
SET SkipClean=false
GOTO Drivers

:Drivers

IF EXIST "%PROGRAMFILES(X86)%" (GOTO 64BIT) ELSE (GOTO 32BIT)

:64BIT
echo 64-bit...
%MSBuild_64Bit_Tool% /nologo /filelogger .\Drivers\Drivers.proj /property:SkipIncrementAssemblyVersion=%SkipIncrementAssemblyVersion%;SkipClean=%SkipClean%;SkipHgPull=%SkipHgPull%

GOTO End

:32BIT
echo 32-bit...
%MSBuild_32Bit_Tool% /nologo /filelogger .\Drivers\Drivers.proj /property:SkipIncrementAssemblyVersion=%SkipIncrementAssemblyVersion%;SkipClean=%SkipClean%;SkipHgPull=%SkipHgPull%

GOTO End

:Opt3
SET SkipIncrementAssemblyVersion=false
SET SkipObfuscation=true
GOTO Execute

:Opt1
SET SkipIncrementAssemblyVersion=true
SET SkipObfuscation=true
GOTO Execute

:Execute

ECHO.
SET /P AskSkipSign=Do you want sign all files ([Y]/N)?
IF /I "%AskSkipSign%"=="N" SET CertFriendlyName=

IF EXIST "%PROGRAMFILES(X86)%" (GOTO 64BIT_2) ELSE (GOTO 32BIT_2)

:64BIT_2
echo 64-bit..
cmd /K %MSBuild_64Bit_Tool% /nologo /filelogger UFSolution.proj /property:SkipIncrementAssemblyVersion=%SkipIncrementAssemblyVersion%;SkipObfuscation=%SkipObfuscation%;CertFriendlyName="%CertFriendlyName%"

GOTO End

:32BIT_2
echo 32-bit...
cmd /K %MSBuild_32Bit_Tool% /nologo /filelogger UFSolution.proj /property:SkipIncrementAssemblyVersion=%SkipIncrementAssemblyVersion%;SkipObfuscation=%SkipObfuscation%;CertFriendlyName="%CertFriendlyName%"

:End

PAUSE