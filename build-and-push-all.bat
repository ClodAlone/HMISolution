
@echo off
setlocal
set "SCRIPT_DIR=%~dp0"
if "%SCRIPT_DIR:~-1%"=="\" set "SCRIPT_DIR=%SCRIPT_DIR:~0,-1%"
set "DEMO_IMAGE=clodprogea/hmi-styledinstruments"
set "VERSION="

if not exist "%SCRIPT_DIR%\HMISolution\Version.props" (
    echo [ERROR] HMISolution\Version.props not found.
    exit /b 1
)
for /f "tokens=3 delims=<>" %%V in ('findstr /i "<HMIVersion>" "%SCRIPT_DIR%\HMISolution\Version.props"') do set "VERSION=%%V"
if not defined VERSION (
    echo [ERROR] HMIVersion is missing from Version.props.
    exit /b 1
)

echo Starting the sequential process...

call "%SCRIPT_DIR%\build-docker.bat"
if errorlevel 1 goto :error

call "%SCRIPT_DIR%\push-docker.bat"
if errorlevel 1 goto :error

call "%SCRIPT_DIR%\build-allinone-docker.bat"
if errorlevel 1 goto :error

call "%SCRIPT_DIR%\push-allinone-docker.bat"
if errorlevel 1 goto :error

echo [INFO] Building Styled Instruments demo image...
docker build ^
    --file "%SCRIPT_DIR%\Dockerfile.styledinstruments" ^
    --tag "%DEMO_IMAGE%:%VERSION%" ^
    --tag "%DEMO_IMAGE%:latest" ^
    "%SCRIPT_DIR%"
if errorlevel 1 goto :error

echo [INFO] Pushing Styled Instruments demo image...
docker push "%DEMO_IMAGE%:%VERSION%"
if errorlevel 1 goto :error
docker push "%DEMO_IMAGE%:latest"
if errorlevel 1 goto :error

echo [SUCCESS] All images built and pushed, including %DEMO_IMAGE%:%VERSION% and :latest.
endlocal
exit /b 0

:error
echo [ERROR] Build/push sequence failed. No further steps will run.
endlocal
exit /b 1
