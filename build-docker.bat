@echo off
REM ============================================================================
REM Build Docker Images - HMI Solution
REM ============================================================================
REM This script builds Docker images for all platform components.
REM Images are tagged with version and 'latest'.
REM ============================================================================

setlocal enabledelayedexpansion

echo.
echo ============================================================================
echo Building Docker Images - HMI Solution
echo ============================================================================
echo.

REM Set error handling
set "ERROR_OCCURRED=0"

REM Get script directory (and remove trailing backslash for Docker context)
set "SCRIPT_DIR=%~dp0"
if "%SCRIPT_DIR:~-1%"=="\" set "SCRIPT_DIR=%SCRIPT_DIR:~0,-1%"
cd /d "%SCRIPT_DIR%"

REM ============================================================================
REM Configuration - Docker Hub repository: clodprogea (matches GitHub docs)
REM ============================================================================
set "DOCKER_USERNAME=clodprogea"
set "VERSION=1.0.0"

REM Image names (individual components)
set "SERVER_IMAGE=%DOCKER_USERNAME%/hmi-server"
set "EDITOR_IMAGE=%DOCKER_USERNAME%/hmi-editor"
set "VIEWER_IMAGE=%DOCKER_USERNAME%/hmi-viewer"

REM All-in-one images
set "ALLINONE_IMAGE=%DOCKER_USERNAME%/hmi-allinone"
set "AICORE_IMAGE=%DOCKER_USERNAME%/aicorehmi"

echo [CONFIG] Docker Hub User: %DOCKER_USERNAME%
echo [CONFIG] Version Tag: %VERSION%
echo [CONFIG] Building individual component images
echo.

REM ============================================================================
echo [STEP 1/3] Building Server Docker Image...
echo ============================================================================
echo [INFO] Building: %SERVER_IMAGE%:%VERSION%
echo [INFO] Context: %SCRIPT_DIR%
echo [INFO] Dockerfile: Server\Dockerfile
echo [INFO] This is the OPC-UA Server component (part of aicorehmi)
echo.

docker build ^
    --file "%SCRIPT_DIR%\Server\Dockerfile" ^
    --tag "%SERVER_IMAGE%:%VERSION%" ^
    --tag "%SERVER_IMAGE%:latest" ^
    --build-arg BUILD_CONFIGURATION=Release ^
    "%SCRIPT_DIR%"

if errorlevel 1 (
    echo [ERROR] Server image build failed
    set "ERROR_OCCURRED=1"
    goto :error
)
echo [OK] Server image built successfully
echo.

REM ============================================================================
echo [STEP 2/3] Building Editor Docker Image...
echo ============================================================================
echo [INFO] Building: %EDITOR_IMAGE%:%VERSION%
echo [INFO] Context: %SCRIPT_DIR%
echo [INFO] Dockerfile: ServerEditorWeb\Dockerfile
echo [INFO] This is the Blazor Web Editor component (part of aicorehmi)
echo.

docker build ^
    --file "%SCRIPT_DIR%\ServerEditorWeb\Dockerfile" ^
    --tag "%EDITOR_IMAGE%:%VERSION%" ^
    --tag "%EDITOR_IMAGE%:latest" ^
    --build-arg BUILD_CONFIGURATION=Release ^
    "%SCRIPT_DIR%"

if errorlevel 1 (
    echo [ERROR] Editor image build failed
    set "ERROR_OCCURRED=1"
    goto :error
)
echo [OK] Editor image built successfully
echo.

REM ============================================================================
echo [STEP 3/3] Building RuntimeViewer Docker Image...
echo ============================================================================
echo [INFO] Building: %VIEWER_IMAGE%:%VERSION%
echo [INFO] Context: %SCRIPT_DIR%
echo [INFO] Dockerfile: RuntimeViewer\Dockerfile
echo [INFO] This is the Runtime Viewer component (part of aicorehmi)
echo.

docker build ^
    --file "%SCRIPT_DIR%\RuntimeViewer\Dockerfile" ^
    --tag "%VIEWER_IMAGE%:%VERSION%" ^
    --tag "%VIEWER_IMAGE%:latest" ^
    --build-arg BUILD_CONFIGURATION=Release ^
    "%SCRIPT_DIR%"

if errorlevel 1 (
    echo [ERROR] RuntimeViewer image build failed
    set "ERROR_OCCURRED=1"
    goto :error
)
echo [OK] RuntimeViewer image built successfully
echo.

REM ============================================================================
echo [SUCCESS] All Docker images built successfully!
echo ============================================================================
echo.
echo Built individual component images:
echo   - %SERVER_IMAGE%:%VERSION%
echo   - %SERVER_IMAGE%:latest
echo.
echo   - %EDITOR_IMAGE%:%VERSION%
echo   - %EDITOR_IMAGE%:latest
echo.
echo   - %VIEWER_IMAGE%:%VERSION%
echo   - %VIEWER_IMAGE%:latest
echo.
echo These are the individual components that make up the aicorehmi platform.
echo.
echo Note: The all-in-one images (aicorehmi, hmi-allinone) require a separate
echo       Dockerfile that combines all components with PostgreSQL and Ollama.
echo.
echo Next steps:
echo   1. Test images locally:
echo      docker run -p 14840:14840 -p 14841:14841 %SERVER_IMAGE%:latest
echo      docker run -p 8080:8080 %EDITOR_IMAGE%:latest
echo      docker run -p 8081:8081 %VIEWER_IMAGE%:latest
echo.
echo   2. Push to Docker Hub (clodprogea):
echo      run: push-docker.bat
echo.

endlocal
exit /b 0

:error
echo.
echo ============================================================================
echo [FAILED] Docker image build failed
echo ============================================================================
echo.
endlocal
exit /b 1
