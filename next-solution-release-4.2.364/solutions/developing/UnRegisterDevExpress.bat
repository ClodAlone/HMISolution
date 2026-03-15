@ECHO OFF
IF NOT "%1"=="" GOTO argsok:

IF EXIST "%PROGRAMFILES(X86)%" (GOTO 64BIT) ELSE (GOTO 32BIT)

:64BIT
echo 64-bit...

SET GacUtil="C:\Program Files (x86)\Microsoft SDKs\Windows\v10.0A\bin\NETFX 4.8 Tools\gacutil.exe"
SET AssembliesPath="C:\Program Files (x86)\DevExpress 21.2\Components\Bin\Framework\"

GOTO Execute

:32BIT
echo 32-bit...

SET GACUTIL="C:\Program Files\Microsoft SDKs\Windows\v10.0A\bin\NETFX 4.8 Tools\gacutil.exe"
SET AssembliesPath="C:\Program Files\DevExpress 21.2\Components\Bin\Framework\"

GOTO Execute

:argsok
SET GacUtil=%1
SET AssembliesPath=%2

:Execute

FOR /R %AssembliesPath% %%a IN (*.dll) DO %GacUtil% /uf "%%~na"

PAUSE