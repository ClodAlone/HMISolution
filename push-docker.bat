@echo off
REM ============================================================================
REM Push Docker Images to Docker Hub - HMI Solution
REM ============================================================================
REM This script pushes all Docker images to Docker Hub.
REM Make sure you are logged in: docker login
REM ============================================================================

setlocal enabledelayedexpansion

echo.
echo ============================================================================
echo Pushing Docker Images to Docker Hub - HMI Solution
echo ============================================================================
echo.

REM Set error handling
set "ERROR_OCCURRED=0"

REM Get script directory
set "SCRIPT_DIR=%~dp0"
cd /d "%SCRIPT_DIR%"

REM ============================================================================
REM Configuration - Must match build-docker.bat and GitHub documentation
REM ============================================================================
set "DOCKER_USERNAME=clodprogea"
set "VERSION=1.0.0"

REM Image names (individual components)
set "SERVER_IMAGE=%DOCKER_USERNAME%/hmi-server"
set "EDITOR_IMAGE=%DOCKER_USERNAME%/hmi-editor"
set "VIEWER_IMAGE=%DOCKER_USERNAME%/hmi-viewer"

echo [CONFIG] Docker Hub User: %DOCKER_USERNAME%
echo [CONFIG] Version Tag: %VERSION%
echo.

REM ============================================================================
echo [CHECK] Verifying Docker login...
echo ============================================================================
docker info >nul 2>&1
if errorlevel 1 (
    echo [ERROR] Docker is not running or you are not logged in
    echo.
    echo Please run: docker login
    echo.
    set "ERROR_OCCURRED=1"
    goto :error
)
echo [OK] Docker is available
echo.

REM ============================================================================
echo [STEP 1/6] Pushing Server image (%VERSION%)...
echo ============================================================================
docker push "%SERVER_IMAGE%:%VERSION%"
if errorlevel 1 (
    echo [ERROR] Failed to push %SERVER_IMAGE%:%VERSION%
    set "ERROR_OCCURRED=1"
    goto :error
)
echo [OK] Pushed %SERVER_IMAGE%:%VERSION%
echo.

REM ============================================================================
echo [STEP 2/6] Pushing Server image (latest)...
echo ============================================================================
docker push "%SERVER_IMAGE%:latest"
if errorlevel 1 (
    echo [ERROR] Failed to push %SERVER_IMAGE%:latest
    set "ERROR_OCCURRED=1"
    goto :error
)
echo [OK] Pushed %SERVER_IMAGE%:latest
echo.

REM ============================================================================
echo [STEP 3/6] Pushing Editor image (%VERSION%)...
echo ============================================================================
docker push "%EDITOR_IMAGE%:%VERSION%"
if errorlevel 1 (
    echo [ERROR] Failed to push %EDITOR_IMAGE%:%VERSION%
    set "ERROR_OCCURRED=1"
    goto :error
)
echo [OK] Pushed %EDITOR_IMAGE%:%VERSION%
echo.

REM ============================================================================
echo [STEP 4/6] Pushing Editor image (latest)...
echo ============================================================================
docker push "%EDITOR_IMAGE%:latest"
if errorlevel 1 (
    echo [ERROR] Failed to push %EDITOR_IMAGE%:latest
    set "ERROR_OCCURRED=1"
    goto :error
)
echo [OK] Pushed %EDITOR_IMAGE%:latest
echo.

REM ============================================================================
echo [STEP 5/6] Pushing RuntimeViewer image (%VERSION%)...
echo ============================================================================
docker push "%VIEWER_IMAGE%:%VERSION%"
if errorlevel 1 (
    echo [ERROR] Failed to push %VIEWER_IMAGE%:%VERSION%
    set "ERROR_OCCURRED=1"
    goto :error
)
echo [OK] Pushed %VIEWER_IMAGE%:%VERSION%
echo.

REM ============================================================================
echo [STEP 6/6] Pushing RuntimeViewer image (latest)...
echo ============================================================================
docker push "%VIEWER_IMAGE%:latest"
if errorlevel 1 (
    echo [ERROR] Failed to push %VIEWER_IMAGE%:latest
    set "ERROR_OCCURRED=1"
    goto :error
)
echo [OK] Pushed %VIEWER_IMAGE%:latest
echo.

REM ============================================================================
echo [SUCCESS] All images pushed to Docker Hub successfully!
echo ============================================================================
echo.
echo Published individual component images to clodprogea:
echo   - https://hub.docker.com/r/%DOCKER_USERNAME%/hmi-server
echo   - https://hub.docker.com/r/%DOCKER_USERNAME%/hmi-editor
echo   - https://hub.docker.com/r/%DOCKER_USERNAME%/hmi-viewer
echo.
echo These components are part of the AI Core HMI platform.
echo See: https://github.com/ClodAlone/HMISolution
echo.
echo Users can pull individual components with:
echo   docker pull %SERVER_IMAGE%:%VERSION%
echo   docker pull %EDITOR_IMAGE%:%VERSION%
echo   docker pull %VIEWER_IMAGE%:%VERSION%
echo.
echo Or use :latest tag:
echo   docker pull %SERVER_IMAGE%:latest
echo   docker pull %EDITOR_IMAGE%:latest
echo   docker pull %VIEWER_IMAGE%:latest
echo.
echo Note: The all-in-one images (aicorehmi, hmi-allinone) are built
echo       separately and combine all components with PostgreSQL and Ollama.
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
echo   1. Make sure you are logged in: docker login
echo   2. Verify images exist locally: docker images
echo   3. Check your Docker Hub repository settings
echo.
endlocal
exit /b 1
