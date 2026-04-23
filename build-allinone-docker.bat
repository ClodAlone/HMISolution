@echo off
REM ============================================================================
REM Build All-in-One Docker Image - AI Core HMI
REM ============================================================================
REM This script builds the complete all-in-one Docker image with:
REM   - OPC-UA Server + REST API
REM   - Blazor Web Editor
REM   - Runtime Viewer
REM   - PostgreSQL (TimescaleDB)
REM   - Ollama (Local LLM)
REM   - ffmpeg (camera support)
REM   - YOLOv8n ONNX model
REM ============================================================================

setlocal enabledelayedexpansion

echo.
echo ============================================================================
echo Building All-in-One Docker Image - AI Core HMI
echo ============================================================================
echo.

REM Set error handling
set "ERROR_OCCURRED=0"

REM Get script directory (and remove trailing backslash for Docker context)
set "SCRIPT_DIR=%~dp0"
if "%SCRIPT_DIR:~-1%"=="\" set "SCRIPT_DIR=%SCRIPT_DIR:~0,-1%"
cd /d "%SCRIPT_DIR%"

REM ============================================================================
REM Configuration
REM ============================================================================
set "DOCKER_USERNAME=clodprogea"
set "VERSION=1.0.0"
set "ALLINONE_IMAGE=%DOCKER_USERNAME%/aicorehmi"
set "DOCKERFILE=%SCRIPT_DIR%\Dockerfile.allinone"

echo [CONFIG] Docker Hub User: %DOCKER_USERNAME%
echo [CONFIG] Version Tag: %VERSION%
echo [CONFIG] Image Name: %ALLINONE_IMAGE%
echo [CONFIG] Dockerfile: %DOCKERFILE%
echo.

REM Check if Dockerfile exists
if not exist "%DOCKERFILE%" (
    echo [ERROR] Dockerfile.allinone not found at: %DOCKERFILE%
    echo [INFO] Looking for it in current directory...
    if exist "Dockerfile.allinone" (
        set "DOCKERFILE=Dockerfile.allinone"
        echo [OK] Found Dockerfile.allinone in current directory
    ) else (
        echo [ERROR] Cannot find Dockerfile.allinone
        set "ERROR_OCCURRED=1"
        goto :error
    )
)

REM ============================================================================
echo [STEP 1/2] Building all-in-one image...
echo ============================================================================
echo [INFO] This builds a complete 12+ GB image containing:
echo [INFO]   - .NET 10 Runtime
echo [INFO]   - Server (OPC-UA + 13 drivers)
echo [INFO]   - Editor (Blazor Web UI)
echo [INFO]   - Viewer (Runtime monitoring)
echo [INFO]   - PostgreSQL 16 + TimescaleDB
echo [INFO]   - Ollama (Local LLM)
echo [INFO]   - ffmpeg (Camera support)
echo [INFO]   - YOLOv8n ONNX model
echo.
echo [WARNING] This will take 10-15 minutes and download several GB...
echo.

docker build ^
    --file "%DOCKERFILE%" ^
    --tag "%ALLINONE_IMAGE%:%VERSION%" ^
    --tag "%ALLINONE_IMAGE%:latest" ^
    --progress=plain ^
    "%SCRIPT_DIR%"

if errorlevel 1 (
    echo [ERROR] All-in-one image build failed
    set "ERROR_OCCURRED=1"
    goto :error
)
echo [OK] All-in-one image built successfully
echo.

REM ============================================================================
echo [SUCCESS] All-in-one image build completed!
echo ============================================================================
echo.
echo Built image:
echo   - %ALLINONE_IMAGE%:%VERSION%
echo   - %ALLINONE_IMAGE%:latest
echo.
echo Image size: ~12-13 GB
echo.
echo To test locally:
echo   docker run -d --name aicorehmi \
echo     -p 14840:14840 -p 14841:14841 \
echo     -p 8080:8080 -p 8081:8081 -p 8088:8088 \
echo     -p 5432:5432 -p 11434:11434 \
echo     -v hmi-data:/data \
echo     %ALLINONE_IMAGE%:latest
echo.
echo Access:
echo   - Web Editor: http://localhost:8080
echo   - Runtime Viewer: http://localhost:8088
echo   - OPC-UA: opc.tcp://localhost:14840
echo   - REST API: http://localhost:14841
echo   - PostgreSQL: localhost:5432
echo   - Ollama: http://localhost:11434
echo.
echo Next steps:
echo   1. Test the image locally
echo   2. Push to Docker Hub: push-allinone-docker.bat
echo.

endlocal
exit /b 0

:error
echo.
echo ============================================================================
echo [FAILED] All-in-one image build failed
echo ============================================================================
echo.
endlocal
exit /b 1
