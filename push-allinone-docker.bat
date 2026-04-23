@echo off
REM ============================================================================
REM Push All-in-One Docker Image to Docker Hub - AI Core HMI
REM ============================================================================
REM This script pushes the complete all-in-one aicorehmi image to Docker Hub.
REM ============================================================================

setlocal enabledelayedexpansion

echo.
echo ============================================================================
echo Pushing All-in-One Docker Image to Docker Hub - AI Core HMI
echo ============================================================================
echo.

REM Set error handling
set "ERROR_OCCURRED=0"

REM Get script directory
set "SCRIPT_DIR=%~dp0"
cd /d "%SCRIPT_DIR%"

REM ============================================================================
REM Configuration - Must match build-allinone-docker.bat
REM ============================================================================
set "DOCKER_USERNAME=clodprogea"
set "VERSION=1.0.0"
set "ALLINONE_IMAGE=%DOCKER_USERNAME%/aicorehmi"

echo [CONFIG] Docker Hub User: %DOCKER_USERNAME%
echo [CONFIG] Version Tag: %VERSION%
echo [CONFIG] Image Name: %ALLINONE_IMAGE%
echo.

REM ============================================================================
echo [CHECK] Verifying Docker login...
echo ============================================================================
docker info >nul 2>&1
if errorlevel 1 (
    echo [ERROR] Docker is not running or you are not logged in
    echo.
    echo Please run: docker login -u %DOCKER_USERNAME%
    echo.
    set "ERROR_OCCURRED=1"
    goto :error
)
echo [OK] Docker is available
echo.

REM ============================================================================
echo [CHECK] Verifying image exists locally...
echo ============================================================================
docker images %ALLINONE_IMAGE%:latest -q >nul 2>&1
if errorlevel 1 (
    echo [ERROR] Image %ALLINONE_IMAGE%:latest not found locally
    echo.
    echo Please build the image first:
    echo   build-allinone-docker.bat
    echo.
    set "ERROR_OCCURRED=1"
    goto :error
)
echo [OK] Image found locally
echo.

REM ============================================================================
echo [STEP 1/2] Pushing %ALLINONE_IMAGE%:%VERSION%...
echo ============================================================================
echo [INFO] Pushing version-tagged image (~12-13 GB)
echo [INFO] This may take 15-30 minutes depending on your internet speed...
echo.

docker push "%ALLINONE_IMAGE%:%VERSION%"
if errorlevel 1 (
    echo [ERROR] Failed to push %ALLINONE_IMAGE%:%VERSION%
    set "ERROR_OCCURRED=1"
    goto :error
)
echo [OK] Pushed %ALLINONE_IMAGE%:%VERSION%
echo.

REM ============================================================================
echo [STEP 2/2] Pushing %ALLINONE_IMAGE%:latest...
echo ============================================================================
echo [INFO] Pushing latest tag
echo.

docker push "%ALLINONE_IMAGE%:latest"
if errorlevel 1 (
    echo [ERROR] Failed to push %ALLINONE_IMAGE%:latest
    set "ERROR_OCCURRED=1"
    goto :error
)
echo [OK] Pushed %ALLINONE_IMAGE%:latest
echo.

REM ============================================================================
echo [SUCCESS] All-in-one image pushed to Docker Hub successfully!
echo ============================================================================
echo.
echo Published image: %ALLINONE_IMAGE%
echo   - Version: %VERSION%
echo   - Latest: latest
echo.
echo Docker Hub URL:
echo   https://hub.docker.com/r/%DOCKER_USERNAME%/aicorehmi
echo.
echo Users can now pull and run with:
echo   docker pull %ALLINONE_IMAGE%:latest
echo.
echo   docker run -d --name aicorehmi \
echo     -p 14840:14840 -p 14841:14841 \
echo     -p 8080:8080 -p 8081:8081 -p 8088:8088 \
echo     -p 5432:5432 -p 11434:11434 \
echo     -v hmi-data:/data \
echo     %ALLINONE_IMAGE%:latest
echo.
echo The complete AI Core HMI platform is now publicly available!
echo.

endlocal
exit /b 0

:error
echo.
echo ============================================================================
echo [FAILED] Docker push failed
echo ============================================================================
echo.
echo Troubleshooting:
echo   1. Make sure you are logged in: docker login -u %DOCKER_USERNAME%
echo   2. Verify image exists: docker images %ALLINONE_IMAGE%
echo   3. Check Docker Hub repository permissions
echo   4. Ensure stable internet connection (12+ GB upload)
echo.
endlocal
exit /b 1
