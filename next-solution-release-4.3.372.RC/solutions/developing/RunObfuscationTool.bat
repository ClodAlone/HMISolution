@ECHO OFF
SET ObfuscatioDotNetTool=dotNET_Reactor.Console.exe

SET BuildOffuscatedPath=.\UFSolution\bin\Obfuscated
SET BuildRetailPath=.\UFSolution\bin\Retail
SET BuildLicenseRetailPath=.\UFSolution\bin\NextLicMng
SET BuildNetCorePath=.\UFSolution\bin\NetCore
SET BuildWebHMIPath=.\UFSolution\bin\WebClientHMI
SET BuildDeployServerPath=.\UFSolution\bin\DeployServer

SET BuildOptionalsPath=.\UFSolution\bin\Optionals
REM SET BuildLicenseServicePath=.\MSZUtilsWebService\bin\Release
REM SET BuildLicenseClientPath=.\MSZUtils\bin\Release
REM SET BuildLicenseConsolePath=.\NextLicenseCnsl\bin\Release
SET BuildInstallDonglePath=.\InstallDongle\bin\Release

SET SignCheckFilePath=.\UFSolution\bin\Obfuscated\net48\MSZ.dll
SET CertFriendlyName=PROGEA SRL

:: See if sigcheck is in the path
where sigcheck.exe 2>NUL 1>NUL
if not "%ERRORLEVEL%"=="0" echo sigcheck.exe is not in your path&& goto :End

:: Make sure the file exists
if not exist "%SignCheckFilePath%" echo "%SignCheckFilePath%" does not exist&& goto :End

:: Take the output from sigcheck, parse it and put it into a variable called VerifiedLine
for /F "delims=" %%l in ('sigcheck -nobanner "%SignCheckFilePath%"^|findstr Verified') do set VerifiedLine=%%l

:: See if the line contains "Unsigned"
if not "%VerifiedLine:Unsigned=%"=="%VerifiedLine%" SET CertFriendlyName=

RMDIR /S /Q %BuildRetailPath%.obfuscated
RMDIR /S /Q %BuildRetailPath%.Connext.obfuscated
RMDIR /S /Q %BuildNetCorePath%.obfuscated
RMDIR /S /Q %BuildNetCorePath%.Connext.obfuscated
RMDIR /S /Q %BuildWebHMIPath%.obfuscated
RMDIR /S /Q %BuildDeployServerPath%.obfuscated
RMDIR /S /Q %BuildOptionalsPath%.obfuscated
RMDIR /S /Q %BuildLicenseRetailPath%

MKDIR %BuildLicenseRetailPath%

REM DEL %BuildRetailPath%\UFSolutionKeyPair.snk
REM IF EXIST %BuildRetailPath%.Connext\ DEL %BuildRetailPath%.Connext\UFSolutionKeyPair.snk

XCOPY /S /Y %BuildRetailPath%\*.* %BuildRetailPath%.obfuscated\
XCOPY /S /Y %BuildNetCorePath%\*.* %BuildNetCorePath%.obfuscated\
XCOPY /S /Y %BuildWebHMIPath%\*.* %BuildWebHMIPath%.obfuscated\
XCOPY /S /Y %BuildDeployServerPath%\*.* %BuildDeployServerPath%.obfuscated\

IF EXIST %BuildRetailPath%.Connext\ XCOPY /S /Y %BuildRetailPath%.Connext\*.* %BuildRetailPath%.Connext.obfuscated\
IF EXIST %BuildNetCorePath%.Connext\ XCOPY /S /Y %BuildNetCorePath%.Connext\*.* %BuildNetCorePath%.Connext.obfuscated\

REM XCOPY /S /Y /EXCLUDE:xCopyFilesToExclude.txt %BuildLicenseServicePath%\*.* %BuildLicenseRetailPath%\Server\
REM XCOPY /S /Y /EXCLUDE:xCopyFilesToExclude.txt %BuildLicenseClientPath%\*.* %BuildLicenseRetailPath%\Client\
REM XCOPY /S /Y /EXCLUDE:xCopyFilesToExclude.txt %BuildLicenseConsolePath%\*.* %BuildLicenseRetailPath%\Console\
XCOPY /S /Y %BuildInstallDonglePath%\*.* %BuildLicenseRetailPath%\InstallDongle\

%ObfuscatioDotNetTool% -project "NetPlatformNext.nrproj" -cert_friendly_name "%CertFriendlyName%"

IF %errorlevel% neq 0 GOTO Error

IF EXIST %BuildRetailPath%.Connext.obfuscated\ %ObfuscatioDotNetTool% -project "NetConnext.nrproj" -cert_friendly_name "%CertFriendlyName%"

IF %errorlevel% neq 0 GOTO Error

%ObfuscatioDotNetTool% -project "NetProEnergy.nrproj" -cert_friendly_name "%CertFriendlyName%"

IF %errorlevel% neq 0 GOTO Error

%ObfuscatioDotNetTool% -project "NetProLean.nrproj" -cert_friendly_name "%CertFriendlyName%"

IF %errorlevel% neq 0 GOTO Error

%ObfuscatioDotNetTool% -project "NetNextBuilder.nrproj" -cert_friendly_name "%CertFriendlyName%"

IF %errorlevel% neq 0 GOTO Error

%ObfuscatioDotNetTool% -project "DotNetNextBuilder.nrproj" -cert_friendly_name "%CertFriendlyName%"

IF %errorlevel% neq 0 GOTO Error

REM %ObfuscatioDotNetTool% -project "NetLicenseServer.nrproj" -cert_friendly_name "%CertFriendlyName%"

REM IF %errorlevel% neq 0 GOTO Error

REM %ObfuscatioDotNetTool% -project "NetLicenseClient.nrproj" -cert_friendly_name "%CertFriendlyName%"

REM IF %errorlevel% neq 0 GOTO Error

%ObfuscatioDotNetTool% -project "DotNetServerHMI.nrproj" -cert_friendly_name "%CertFriendlyName%"

IF %errorlevel% neq 0 GOTO Error

IF EXIST %BuildNetCorePath%.Connext.obfuscated\ %ObfuscatioDotNetTool% -project "DotNetConnext.nrproj" -cert_friendly_name "%CertFriendlyName%"

IF %errorlevel% neq 0 GOTO Error

%ObfuscatioDotNetTool% -project "DotNetWebClientHMI.nrproj" -cert_friendly_name "%CertFriendlyName%"

IF %errorlevel% neq 0 GOTO Error

%ObfuscatioDotNetTool% -project "DotNetDeployServer.nrproj" -cert_friendly_name "%CertFriendlyName%"

IF %errorlevel% neq 0 GOTO Error

%ObfuscatioDotNetTool% -project "NetLicense.nrproj" -cert_friendly_name "%CertFriendlyName%"

IF %errorlevel% neq 0 GOTO Error

XCOPY /J /Y /U %BuildOffuscatedPath%\net48\*.* %BuildRetailPath%\
XCOPY /J /Y /U %BuildOffuscatedPath%\net48\*.* %BuildRetailPath%.obfuscated\
XCOPY /J /Y /U %BuildOffuscatedPath%\net48\*.* %BuildRetailPath%\UFWebClient.HTML5\bin\
XCOPY /J /Y /U %BuildOffuscatedPath%\net48\*.* %BuildRetailPath%.obfuscated\UFWebClient.HTML5\bin\

IF EXIST %BuildOffuscatedPath%.Connext\ XCOPY /J /Y /U %BuildOffuscatedPath%.Connext\net48\*.* %BuildRetailPath%.Connext\
IF EXIST %BuildOffuscatedPath%.Connext\ XCOPY /J /Y /U %BuildOffuscatedPath%.Connext\net48\*.* %BuildRetailPath%.Connext.obfuscated\

XCOPY /D /J /Y /U /EXCLUDE:xCopyFilesToExclude.txt %BuildRetailPath%.obfuscated\*.dll %BuildRetailPath%.obfuscated\UFWebClient.HTML5\bin\
XCOPY /D /J /Y /U /EXCLUDE:xCopyFilesToExclude.txt %BuildRetailPath%.obfuscated\Drivers\*.dll %BuildRetailPath%.obfuscated\UFWebClient.HTML5\bin\
XCOPY /D /J /Y /U /EXCLUDE:xCopyFilesToExclude.txt %BuildRetailPath%.obfuscated\CommonPlugins\*.dll %BuildRetailPath%.obfuscated\UFWebClient.HTML5\bin\
XCOPY /D /J /Y /U /EXCLUDE:xCopyFilesToExclude.txt %BuildRetailPath%.obfuscated\RuntimePlugins\*.dll %BuildRetailPath%.obfuscated\UFWebClient.HTML5\bin\
XCOPY /D /J /Y /U /EXCLUDE:xCopyFilesToExclude.txt %BuildRetailPath%.obfuscated\DocumentManagers\*.dll %BuildRetailPath%.obfuscated\UFWebClient.HTML5\bin\
XCOPY /D /J /Y /U /EXCLUDE:xCopyFilesToExclude.txt %BuildRetailPath%.obfuscated\Managers\*.dll %BuildRetailPath%.obfuscated\UFWebClient.HTML5\bin\
XCOPY /D /J /Y /U /EXCLUDE:xCopyFilesToExclude.txt %BuildRetailPath%.obfuscated\Toolbox\*.dll %BuildRetailPath%.obfuscated\UFWebClient.HTML5\bin\Toolbox\
XCOPY /D /J /Y /U /EXCLUDE:xCopyFilesToExclude.txt %BuildRetailPath%.obfuscated\DataSinks\*.dll %BuildRetailPath%.obfuscated\UFWebClient.HTML5\bin\DataSinks\
XCOPY /D /J /Y /U /EXCLUDE:xCopyFilesToExclude.txt %BuildRetailPath%.obfuscated\ThemeResources\Dark\*.dll %BuildRetailPath%.obfuscated\UFWebClient.HTML5\bin\ThemeResources\Dark\
XCOPY /D /J /Y /U /EXCLUDE:xCopyFilesToExclude.txt %BuildRetailPath%.obfuscated\ThemeResources\Light\*.dll %BuildRetailPath%.obfuscated\UFWebClient.HTML5\bin\ThemeResources\Light\

XCOPY /D /J /Y /U /EXCLUDE:xCopyFilesToExclude.txt %BuildRetailPath%.obfuscated\*.dll %BuildRetailPath%.obfuscated\CommonPlugins\
XCOPY /D /J /Y /U /EXCLUDE:xCopyFilesToExclude.txt %BuildRetailPath%.obfuscated\*.dll %BuildRetailPath%.obfuscated\DataSinks\
XCOPY /D /J /Y /U /EXCLUDE:xCopyFilesToExclude.txt %BuildRetailPath%.obfuscated\*.dll %BuildRetailPath%.obfuscated\DesignPlugins\
XCOPY /D /J /Y /U /EXCLUDE:xCopyFilesToExclude.txt %BuildRetailPath%.obfuscated\*.dll %BuildRetailPath%.obfuscated\DocumentManagers\
XCOPY /D /J /Y /U /EXCLUDE:xCopyFilesToExclude.txt %BuildRetailPath%.obfuscated\*.dll %BuildRetailPath%.obfuscated\Drivers\
XCOPY /D /J /Y /U /EXCLUDE:xCopyFilesToExclude.txt %BuildRetailPath%.obfuscated\*.dll %BuildRetailPath%.obfuscated\LOD\
XCOPY /D /J /Y /U /EXCLUDE:xCopyFilesToExclude.txt %BuildRetailPath%.obfuscated\*.dll %BuildRetailPath%.obfuscated\LogicService\
XCOPY /D /J /Y /U /EXCLUDE:xCopyFilesToExclude.txt %BuildRetailPath%.obfuscated\*.dll %BuildRetailPath%.obfuscated\Managers\
XCOPY /D /J /Y /U /EXCLUDE:xCopyFilesToExclude.txt %BuildRetailPath%.obfuscated\*.dll %BuildRetailPath%.obfuscated\RecipeService\
XCOPY /D /J /Y /U /EXCLUDE:xCopyFilesToExclude.txt %BuildRetailPath%.obfuscated\*.dll %BuildRetailPath%.obfuscated\Splashes\
XCOPY /D /J /Y /U /EXCLUDE:xCopyFilesToExclude.txt %BuildRetailPath%.obfuscated\*.dll %BuildRetailPath%.obfuscated\ThemeResources\
XCOPY /D /J /Y /U /EXCLUDE:xCopyFilesToExclude.txt %BuildRetailPath%.obfuscated\*.dll %BuildRetailPath%.obfuscated\Toolbox\
XCOPY /D /J /Y /U /EXCLUDE:xCopyFilesToExclude.txt %BuildRetailPath%.obfuscated\*.dll %BuildRetailPath%.obfuscated\Tools\
XCOPY /D /J /Y /U /EXCLUDE:xCopyFilesToExclude.txt %BuildRetailPath%.obfuscated\*.dll %BuildRetailPath%.obfuscated\WebService\
XCOPY /D /J /Y /U /EXCLUDE:xCopyFilesToExclude.txt %BuildRetailPath%.obfuscated\*.dll %BuildRetailPath%.obfuscated\WizardPlugins\
REM XCOPY /D /J /Y /U /EXCLUDE:xCopyFilesToExclude.txt %BuildRetailPath%.obfuscated\*.dll %BuildLicenseRetailPath%\Server\
REM XCOPY /D /J /Y /U /EXCLUDE:xCopyFilesToExclude.txt %BuildRetailPath%.obfuscated\*.dll %BuildLicenseRetailPath%\Client\
REM XCOPY /D /J /Y /U /EXCLUDE:xCopyFilesToExclude.txt %BuildRetailPath%.obfuscated\*.dll %BuildLicenseRetailPath%\Console\
XCOPY /D /J /Y /U %BuildRetailPath%.obfuscated\*.dll %BuildLicenseRetailPath%\InstallDongle\

XCOPY /J /Y /U %BuildOffuscatedPath%\netstandard2.0\*.dll %BuildNetCorePath%\
XCOPY /J /Y /U %BuildOffuscatedPath%\netstandard2.0\*.dll %BuildNetCorePath%.obfuscated\
XCOPY /J /Y /U %BuildOffuscatedPath%\netstandard2.0\*.dll %BuildWebHMIPath%\
XCOPY /J /Y /U %BuildOffuscatedPath%\netstandard2.0\*.dll %BuildWebHMIPath%.obfuscated\
XCOPY /J /Y /U %BuildOffuscatedPath%\netstandard2.0\*.dll %BuildDeployServerPath%\
XCOPY /J /Y /U %BuildOffuscatedPath%\netstandard2.0\*.dll %BuildDeployServerPath%.obfuscated\

IF EXIST %BuildOffuscatedPath%.Connext\ XCOPY /J /Y /U %BuildOffuscatedPath%.Connext\netstandard2.0\*.* %BuildNetCorePath%.Connext\
IF EXIST %BuildOffuscatedPath%.Connext\ XCOPY /J /Y /U %BuildOffuscatedPath%.Connext\netstandard2.0\*.* %BuildNetCorePath%.Connext.obfuscated\

XCOPY /D /J /Y /U /EXCLUDE:xCopyFilesToExclude.txt %BuildNetCorePath%.obfuscated\*.dll %BuildNetCorePath%.obfuscated\Drivers\
XCOPY /D /J /Y /U /EXCLUDE:xCopyFilesToExclude.txt %BuildNetCorePath%.obfuscated\*.dll %BuildNetCorePath%.obfuscated\LogicExtensions\
XCOPY /D /J /Y /U /EXCLUDE:xCopyFilesToExclude.txt %BuildNetCorePath%.obfuscated\*.dll %BuildWebHMIPath%.obfuscated\
XCOPY /D /J /Y /U /EXCLUDE:xCopyFilesToExclude.txt %BuildNetCorePath%.obfuscated\*.dll %BuildDeployServerPath%.obfuscated\

GOTo Exit

:Error

RMDIR /S /Q %BuildRetailPath%.obfuscated
RMDIR /S /Q %BuildRetailPath%.Connext.obfuscated
RMDIR /S /Q %BuildNetCorePath%.obfuscated
RMDIR /S /Q %BuildNetCorePath%.Connext.obfuscated
RMDIR /S /Q %BuildWebHMIPath%.obfuscated
RMDIR /S /Q %BuildDeployServerPath%.obfuscated
RMDIR /S /Q %BuildOptionalsPath%.obfuscated

ECHO Obfuscation was failed!

:Exit

PAUSE
