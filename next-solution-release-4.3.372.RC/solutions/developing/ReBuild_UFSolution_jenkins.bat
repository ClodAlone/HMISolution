@ECHO OFF
SET MSBuild_32Bit_Tool=="C:\Program Files\Microsoft Visual Studio\2022\Professional\Msbuild\Current\Bin\MSBuild.exe"
SET MSBuild_64Bit_Tool=="C:\Program Files\Microsoft Visual Studio\2022\Professional\Msbuild\Current\Bin\MSBuild.exe"
SET SignCheckFilePath=.\UFSolution\bin\Obfuscated\net48\MSZ.dll
SET CertFriendlyName=PROGEA SRL

IF "%~1" EQU "5" GOTO Opt5
IF "%~1" EQU "4" GOTO Opt4
IF "%~1" EQU "3" GOTO Opt3
IF "%~1" EQU "2" GOTO Opt2
IF "%~1" EQU "1" GOTO Opt1

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
%MSBuild_64Bit_Tool% /nologo /filelogger UFConnext.proj /property:Revision=%~2 /property:ShortHeadId=%~3  /property:SkipIncrementAssemblyVersion=%SkipIncrementAssemblyVersion%;SkipObfuscation=%SkipObfuscation%;CertFriendlyName="%CertFriendlyName%"

GOTO End

:32BIT_2
echo 32-bit...
%MSBuild_32Bit_Tool% /nologo /filelogger UFConnext.proj /property:Revision=%~2 /property:ShortHeadId=%~3  /property:SkipIncrementAssemblyVersion=%SkipIncrementAssemblyVersion%;SkipObfuscation=%SkipObfuscation%;CertFriendlyName="%CertFriendlyName%"

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
%MSBuild_64Bit_Tool% /nologo /filelogger .\Drivers\Drivers.proj /property:Revision=%~2 /property:ShortHeadId=%~3  /property:SkipIncrementAssemblyVersion=%SkipIncrementAssemblyVersion%;SkipClean=%SkipClean%;SkipHgPull=%SkipHgPull%

GOTO End

:32BIT
echo 32-bit...
%MSBuild_32Bit_Tool% /nologo /filelogger .\Drivers\Drivers.proj /property:Revision=%~2 /property:ShortHeadId=%~3  /property:SkipIncrementAssemblyVersion=%SkipIncrementAssemblyVersion%;SkipClean=%SkipClean%;SkipHgPull=%SkipHgPull%

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
SET AskSkipSign= "%~2"
IF  "%AskSkipSign%"=="N" SET CertFriendlyName=

IF EXIST "%PROGRAMFILES(X86)%" (GOTO 64BIT_2) ELSE (GOTO 32BIT_2)

:64BIT_2
echo 64-bit..
%MSBuild_64Bit_Tool% /nologo /filelogger UFSolution.proj /property:Revision=%~3 /property:ShortHeadId=%~4 /property:SkipIncrementAssemblyVersion=%SkipIncrementAssemblyVersion%;SkipObfuscation=%SkipObfuscation%;CertFriendlyName="%CertFriendlyName%"

GOTO End

:32BIT_2
echo 32-bit...
%MSBuild_32Bit_Tool% /nologo /filelogger UFSolution.proj /property:Revision=%~3 /property:ShortHeadId=%~4  /property:SkipIncrementAssemblyVersion=%SkipIncrementAssemblyVersion%;SkipObfuscation=%SkipObfuscation%;CertFriendlyName="%CertFriendlyName%"

:End
