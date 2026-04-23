@echo off
REM ============================================================================
REM Complete Build and Deploy Pipeline with All-in-One - HMI Solution
REM ============================================================================
REM This script runs the complete CI/CD pipeline including all-in-one image:
REM   1. Build release platform
REM   2. Build individual Docker images
REM   3. Build all-in-one Docker image
REM   4. Push all images to Docker Hub
REM ============================================================================

setlocal enabledelayedexpansion

echo.
echo ============================================================================
echo HMI Solution - Complete Build and Deploy Pipeline (with All-in-One)
echo ============================================================================
echo.

set "SCRIPT_DIR=%~dp0"
cd /d "%SCRIPT_DIR%"

REM ============================================================================
echo [PIPELINE 1/4] Building Release Platform...
echo ============================================================================
call "%SCRIPT_DIR%build-release.bat"
if errorlevel 1 (
    echo [ERROR] Release build failed
    goto :error
)
echo.

REM ============================================================================
echo [PIPELINE 2/4] Building Individual Component Docker Images...
echo ============================================================================
call "%SCRIPT_DIR%build-docker.bat"
if errorlevel 1 (
    echo [ERROR] Docker image build failed
    goto :error
)
echo.

REM ============================================================================
echo [PIPELINE 3/4] Building All-in-One Docker Image...
echo ============================================================================
echo [INFO] This step builds the complete 12+ GB aicorehmi image
echo [WARNING] This will take 10-15 minutes...
echo.
call "%SCRIPT_DIR%build-allinone-docker.bat"
if errorlevel 1 (
    echo [ERROR] All-in-one image build failed
    goto :error
)
echo.

REM ============================================================================
echo [PIPELINE 4/4] Pushing to Docker Hub...
echo ============================================================================
echo [INFO] Pushing individual components...
call "%SCRIPT_DIR%push-docker.bat"
if errorlevel 1 (
    echo [ERROR] Individual components push failed
    goto :error
)
echo.

echo [INFO] Pushing all-in-one image...
call "%SCRIPT_DIR%push-allinone-docker.bat"
if errorlevel 1 (
    echo [ERROR] All-in-one image push failed
    goto :error
)
echo.

REM ============================================================================
echo [SUCCESS] Complete pipeline finished successfully!
echo ============================================================================
echo.
echo Your HMI Solution is now:
echo   ✓ Built in Release mode
echo   ✓ Packaged as individual Docker images
echo   ✓ Packaged as all-in-one Docker image
echo   ✓ Published to Docker Hub (clodprogea)
echo.
echo Published images:
echo   Individual Components:
echo     - clodprogea/hmi-server:1.0.0, latest (815 MB)
echo     - clodprogea/hmi-editor:1.0.0, latest (473 MB)
echo     - clodprogea/hmi-viewer:1.0.0, latest (407 MB)
echo.
echo   All-in-One:
echo     - clodprogea/aicorehmi:1.0.0, latest (~12 GB)
echo.
echo Ready for production deployment!
echo.

endlocal
exit /b 0

:error
echo.
echo ============================================================================
echo [FAILED] Pipeline failed - see errors above
echo ============================================================================
echo.
echo You can run individual steps manually:
echo   1. build-release.bat           - Build .NET Release artifacts
echo   2. build-docker.bat            - Build individual Docker images
echo   3. build-allinone-docker.bat   - Build all-in-one Docker image
echo   4. push-docker.bat             - Push individual images to Docker Hub
echo   5. push-allinone-docker.bat    - Push all-in-one image to Docker Hub
echo.
endlocal
exit /b 1
