@ECHO OFF

SET SignCheckFilePath=.\UFSolution\bin\Obfuscated\net48\MSZ.dll

:Unsigned
SET ObfuscatedSuffix=Unsigned
GOTO Start

:Obfuscated
SET ObfuscatedSuffix=Obfuscated

:Start
SET LocalRetailVersionPath=.\UFSolution\bin\Retail\
SET ServerRetailVersionPath=\\Server2\update\Latest\mov4.2\Compiled-Files\

SET LocalRetailObfuscatedPath=.\UFSolution\bin\Retail.obfuscated\
SET ServerRetailObfuscatedPath=\\Server2\update\Latest\mov4.2\Compiled-%ObfuscatedSuffix%\

SET LocalVersionConnextPath=.\UFSolution\bin\Retail.Connext\
SET ServerVersionConnextPath=\\Server2\update\Latest\mov4.2\Connext-Files\

SET LocalObfuscatedConnextPath=.\UFSolution\bin\Retail.Connext.obfuscated\
SET ServerObfuscatedConnextPath=\\Server2\update\Latest\mov4.2\Connext-%ObfuscatedSuffix%\

SET LocalNetCoreVersionPath=.\UFSolution\bin\NetCore\
SET ServerNetCoreVersionPath=\\Server2\update\Latest\mov4.2\NetCore-Files\

SET LocalNetCoreObfuscatedPath=.\UFSolution\bin\NetCore.obfuscated\
SET ServerNetCoreObfuscatedPath=\\Server2\update\Latest\mov4.2\NetCore-%ObfuscatedSuffix%\

SET LocalNetCoreConnextVersionPath=.\UFSolution\bin\NetCore.Connext\
SET ServerNetCoreConnextVersionPath=\\Server2\update\Latest\mov4.2\NetCoreConnext-Files\

SET LocalNetCoreConnextObfuscatedPath=.\UFSolution\bin\NetCore.Connext.obfuscated\
SET ServerNetCoreConnextObfuscatedPath=\\Server2\update\Latest\mov4.2\NetCoreConnext-%ObfuscatedSuffix%\

SET LocalDeployServerVersionPath=.\UFSolution\bin\DeployServer\
SET ServerDeployServerVersionPath=\\Server2\update\Latest\mov4.2\DeployServer-Files\

SET LocalDeployServerObfuscatedPath=.\UFSolution\bin\DeployServer.obfuscated\
SET ServerDeployServerObfuscatedPath=\\Server2\update\Latest\mov4.2\DeployServer-%ObfuscatedSuffix%\

SET LocalWebClientHMIVersionPath=.\UFSolution\bin\WebClientHMI\
SET ServerWebClientHMIVersionPath=\\Server2\update\Latest\mov4.2\WebClientHMI-Files\

SET LocalWebClientHMIObfuscatedPath=.\UFSolution\bin\WebClientHMI.obfuscated\
SET ServerWebClientHMIObfuscatedPath=\\Server2\update\Latest\mov4.2\WebClientHMI-%ObfuscatedSuffix%\

SET LocalOptionalsObfuscatedPath=.\UFSolution\bin\Optionals.obfuscated\

SET ServerLibrariesPath=\\Server2\update\Latest\mov4.2\Libraries-Files\
SET ServerProEneryPath=\\Server2\update\Latest\mov4.2\ProEnergy\
SET ServerProLeanPath=\\Server2\update\Latest\mov4.2\ProLean\

SET ServerNExTBAPath=\\Server2\update\Latest\mov4.2\MoviconBA\

SET LocalLicenseRootPath=.\UFSolution\bin\NextLicMng\
SET ServerLicenseRootPath=\\Server2\update\Latest\mov4.2\NextLicMng\

SET OutputPathCompiled="\\Server2\update\Latest\mov4.2\Compiled-Resources"
SET OutputPathLibraries="\\Server2\update\Latest\mov4.2\Libraries-Resources"
SET OutputPathCultures="\\Server2\update\Latest\mov4.2\Cultures-Resources"

SET OutputPathSharedResources="\\Itmdnaspengfs01\grafica\Archivio_Grafico\Grafica Movicon NEXT\SharedResources-Projects\4.2\"

RMDIR /S /Q %ServerRetailVersionPath%
RMDIR /S /Q %ServerRetailObfuscatedPath%

RMDIR /S /Q %ServerVersionConnextPath%
RMDIR /S /Q %ServerObfuscatedConnextPath%

RMDIR /S /Q %ServerNetCoreVersionPath%
RMDIR /S /Q %ServerNetCoreObfuscatedPath%

RMDIR /S /Q %ServerNetCoreConnextVersionPath%
RMDIR /S /Q %ServerNetCoreConnextObfuscatedPath%

RMDIR /S /Q %ServerDeployServerVersionPath%
RMDIR /S /Q %ServerDeployServerObfuscatedPath%

RMDIR /S /Q %ServerWebClientHMIVersionPath%
RMDIR /S /Q %ServerWebClientHMIObfuscatedPath%

if "%ObfuscatedSuffix%"=="Obfuscated" RMDIR /S /Q %ServerProEneryPath%
if "%ObfuscatedSuffix%"=="Obfuscated" RMDIR /S /Q %ServerProLeanPath%

if "%ObfuscatedSuffix%"=="Obfuscated" RMDIR /S /Q %ServerNExTBAPath%

if "%ObfuscatedSuffix%"=="Obfuscated" RMDIR /S /Q %ServerLicenseRootPath%

if "%ObfuscatedSuffix%"=="Obfuscated" RMDIR /S /Q %OutputPathCompiled%
if "%ObfuscatedSuffix%"=="Obfuscated" RMDIR /S /Q %OutputPathLibraries%
if "%ObfuscatedSuffix%"=="Obfuscated" RMDIR /S /Q %OutputPathCultures%

if "%ObfuscatedSuffix%"=="Obfuscated" goto :Execute

:: See if sigcheck is in the path
where sigcheck.exe 2>NUL 1>NUL
if not "%ERRORLEVEL%"=="0" echo sigcheck.exe is not in your path&& goto :End

:: Make sure the file exists
if not exist "%SignCheckFilePath%" echo "%SignCheckFilePath%" does not exist&& goto :End

:: Take the output from sigcheck, parse it and put it into a variable called VerifiedLine
for /F "delims=" %%l in ('sigcheck -nobanner "%SignCheckFilePath%"^|findstr Verified') do set VerifiedLine=%%l

:: See if the line contains "Unsigned"
if not "%VerifiedLine:Unsigned=%"=="%VerifiedLine%" (goto :Execute) else (goto :Obfuscated)

:Execute
MKDIR %ServerRetailVersionPath%
MKDIR %ServerRetailObfuscatedPath%

MKDIR %ServerVersionConnextPath%
MKDIR %ServerObfuscatedConnextPath%

MKDIR %ServerNetCoreVersionPath%
MKDIR %ServerNetCoreObfuscatedPath%

MKDIR %ServerNetCoreConnextVersionPath%
MKDIR %ServerNetCoreConnextObfuscatedPath%

MKDIR %ServerDeployServerVersionPath%
MKDIR %ServerDeployServerObfuscatedPath%

MKDIR %ServerWebClientHMIVersionPath%
MKDIR %ServerWebClientHMIObfuscatedPath%

if "%ObfuscatedSuffix%"=="Obfuscated" MKDIR %ServerProEneryPath%
if "%ObfuscatedSuffix%"=="Obfuscated" MKDIR %ServerProLeanPath%

REM if "%ObfuscatedSuffix%"=="Obfuscated" MKDIR %ServerNExTBAPath%

if "%ObfuscatedSuffix%"=="Obfuscated" MKDIR %ServerLicenseRootPath%

XCOPY /S /Y /Z /EXCLUDE:DeployFilesToExclude.txt %LocalRetailVersionPath%*.* %ServerRetailVersionPath%
XCOPY /S /Y /Z /EXCLUDE:DeployFilesToExclude.txt %LocalRetailObfuscatedPath%*.* %ServerRetailObfuscatedPath%

XCOPY /S /Y /Z %LocalVersionConnextPath%*.* %ServerVersionConnextPath%
XCOPY /S /Y /Z %LocalObfuscatedConnextPath%*.* %ServerObfuscatedConnextPath%

XCOPY /S /Y /Z %LocalNetCoreConnextVersionPath%*.* %ServerNetCoreConnextVersionPath%
XCOPY /S /Y /Z %LocalNetCoreConnextObfuscatedPath%*.* %ServerNetCoreConnextObfuscatedPath%

XCOPY /S /Y /Z %LocalNetCoreVersionPath%*.* %ServerNetCoreVersionPath%
XCOPY /S /Y /Z %LocalNetCoreObfuscatedPath%*.* %ServerNetCoreObfuscatedPath%

XCOPY /S /Y /Z %LocalDeployServerVersionPath%*.* %ServerDeployServerVersionPath%
XCOPY /S /Y /Z %LocalDeployServerObfuscatedPath%*.* %ServerDeployServerObfuscatedPath%

XCOPY /S /Y /Z %LocalWebClientHMIVersionPath%*.* %ServerWebClientHMIVersionPath%
XCOPY /S /Y /Z %LocalWebClientHMIObfuscatedPath%*.* %ServerWebClientHMIObfuscatedPath%

if "%ObfuscatedSuffix%"=="Obfuscated" XCOPY /S /Y /Z %LocalLicenseRootPath%*.* %ServerLicenseRootPath%

IF EXIST "%PROGRAMFILES(X86)%" (GOTO 64BIT) ELSE (GOTO 32BIT)

:64BIT
echo 64-bit...
XCOPY /S /Y /U /D /Z "C:\Program Files (x86)\DevExpress 21.2\Components\Bin\Framework\*.dll" %ServerLibrariesPath%DevExpress
XCOPY /S /Y /U /D /Z "C:\Program Files (x86)\Polar Engineering\WinWrap Basic v10 - For WPF 4.0\REDIST\*.*" %ServerLibrariesPath%WinWrap
XCOPY /S /Y /U /D /Z "C:\Program Files (x86)\Polar Engineering\WinWrap Basic v10 - For .NET 4.0\REDIST\*.*" %ServerLibrariesPath%WinWrap
GOTO EndCheckOS

:32BIT
echo 32-bit...
XCOPY /S /Y /U /D /Z "C:\Program Files\DevExpress 21.2\Components\Bin\Frameworkk\*.dll" %ServerLibrariesPath%DevExpress
XCOPY /S /Y /U /D /Z "C:\Program Files\Polar Engineering\WinWrap Basic v10 - For WPF 4.0\REDIST\*.*" %ServerLibrariesPath%WinWrap
XCOPY /S /Y /U /D /Z "C:\Program Files\Polar Engineering\WinWrap Basic v10 - For .NET 4.0\REDIST\*.*" %ServerLibrariesPath%WinWrap
GOTO EndCheckOS

:EndCheckOS

XCOPY /Y /Z %LocalOptionalsObfuscatedPath%\net48\MoviconNextBuilder.dll %ServerLibrariesPath%NextBuilder-%ObfuscatedSuffix%\net48
XCOPY /Y /Z ".\MoviconNextBuilder\bin\Release\net48\MoviconNextBuilder.dll.config" %ServerLibrariesPath%NextBuilder-%ObfuscatedSuffix%\net48
XCOPY /Y /Z ".\MoviconNextBuilder\bin\Release\net48\AmazedSaint.Elastic.dll" %ServerLibrariesPath%NextBuilder-%ObfuscatedSuffix%\net48
XCOPY /Y /Z %LocalOptionalsObfuscatedPath%\netstandard2.0\MoviconNextBuilder.dll %ServerLibrariesPath%NextBuilder-%ObfuscatedSuffix%\netstandard2.0
XCOPY /Y /Z ".\MoviconNextBuilder\bin\Release\netstandard2.0\MoviconNextBuilder.dll.config" %ServerLibrariesPath%NextBuilder-%ObfuscatedSuffix%\netstandard2.0
XCOPY /Y /Z ".\MoviconNextBuilder\bin\Release\netstandard2.0\MoviconNextBuilder.deps.json" %ServerLibrariesPath%NextBuilder-%ObfuscatedSuffix%\netstandard2.0

if "%ObfuscatedSuffix%"=="Obfuscated" XCOPY /Y /Z %LocalOptionalsObfuscatedPath%NextEnergyLicense.exe %ServerProEneryPath%
if "%ObfuscatedSuffix%"=="Obfuscated" XCOPY /Y /Z .\NextEnergyLicense\bin\Release\NextEnergyLicense.exe.config %ServerProEneryPath%

if "%ObfuscatedSuffix%"=="Obfuscated" XCOPY /Y /Z %LocalOptionalsObfuscatedPath%NextLeanLicense.exe %ServerProLeanPath%
if "%ObfuscatedSuffix%"=="Obfuscated" XCOPY /Y /Z .\NextLeanLicence\bin\Release\NextLeanLicense.exe.config %ServerProLeanPath%

XCOPY /Y /Z "..\..\Sources\SQLite-Providers\bin\SQLiteMembershipProvider.dll" %ServerLibrariesPath%SQLLite\
XCOPY /Y /Z "..\..\Sources\SQLite-Providers\bin\SQLiteMembershipProvider.dll.config" %ServerLibrariesPath%SQLLite\
XCOPY /Y /Z "..\..\Sources\CustomThemes\VS2019Dark2\.td\Publish\*.dll" %ServerLibrariesPath%DevExpress\
XCOPY /Y /Z %ServerLibrariesPath%DevExpress\DevExpress.Xpf.Themes*.* %ServerLicenseRootPath%InstallDongle\

XCOPY /Y /Z %ServerLibrariesPath%SG-Lock\*.dll %ServerRetailVersionPath%\UFWebClient.HTML5\bin\
XCOPY /Y /Z %ServerLibrariesPath%Third-Party\*.dll %ServerRetailVersionPath%\UFWebClient.HTML5\bin\Libraries\
XCOPY /Y /Z %ServerLibrariesPath%WinWrap\*.dll %ServerRetailVersionPath%\UFWebClient.HTML5\bin\Libraries\
XCOPY /Y /Z %ServerRetailVersionPath%System.Windows.Interactivity.dll %ServerRetailVersionPath%\UFWebClient.HTML5\bin\Libraries\

XCOPY /Y /Z %ServerLibrariesPath%SG-Lock\*.dll %ServerRetailObfuscatedPath%\UFWebClient.HTML5\bin\
XCOPY /Y /Z %ServerLibrariesPath%Third-Party\*.dll %ServerRetailObfuscatedPath%\UFWebClient.HTML5\bin\Libraries\
XCOPY /Y /Z %ServerLibrariesPath%WinWrap\*.dll %ServerRetailObfuscatedPath%\UFWebClient.HTML5\bin\Libraries\
XCOPY /Y /Z %ServerRetailObfuscatedPath%System.Windows.Interactivity.dll %ServerRetailObfuscatedPath%\UFWebClient.HTML5\bin\Libraries\

MKDIR %ServerRetailVersionPath%\DriversEx\
XCOPY /Y /Z .\UFSolution\bin\Release\DriversEx\DriverCodeBaseEx.UI*.* %ServerRetailVersionPath%\DriversEx\
XCOPY /Y /Z .\UFSolution\bin\Release\DriversEx\IpDriverCodeBaseEx*.* %ServerRetailVersionPath%\DriversEx\
XCOPY /Y /Z .\UFSolution\bin\Release\DriversEx\SerialDriverCodeBaseEx*.* %ServerRetailVersionPath%\DriversEx\
FOR /F "delims=" %%a IN ('DIR /B .\UFSolution\bin\Release\Drivers\*.dll') DO XCOPY /Y /Z .\UFSolution\bin\Release\DriversEx\"%%a" %ServerRetailVersionPath%\DriversEx\

MKDIR %ServerRetailVersionPath%\DriversOld\
XCOPY /Y /Z .\UFSolution\bin\Release\DriversOld\DriverCodeBase.UI*.* %ServerRetailVersionPath%\DriversOld\
XCOPY /Y /Z .\UFSolution\bin\Release\DriversOld\IpDriverCodeBase*.* %ServerRetailVersionPath%\DriversOld\
XCOPY /Y /Z .\UFSolution\bin\Release\DriversOld\SerialDriverCodeBase*.* %ServerRetailVersionPath%\DriversOld\
FOR /F "delims=" %%a IN ('DIR /B .\UFSolution\bin\Release\Drivers\*.dll') DO XCOPY /Y /Z .\UFSolution\bin\Release\DriversOld\"%%a" %ServerRetailVersionPath%\DriversOld\

XCOPY /S /Y /D /Z .\SharedResources\*.* %OutputPathSharedResources%DarkSharedResources\
XCOPY /S /Y /D /Z .\LightSharedResources\*.* %OutputPathSharedResources%LightSharedResources\
XCOPY /S /Y /D /Z .\RuntimeDarkSharedResources\*.* %OutputPathSharedResources%RuntimeDarkSharedResources\
XCOPY /S /Y /D /Z .\RuntimeLightSharedResources\*.* %OutputPathSharedResources%RuntimeLightSharedResources\

:End

PAUSE