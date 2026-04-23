@echo off
REM ============================================================================
REM Complete Build and Deploy Pipeline - HMI Solution
REM ============================================================================
REM This script runs the complete CI/CD pipeline:
REM   1. Build release platform
REM   2. Build Docker images
REM   3. Push to Docker Hub
REM ============================================================================

setlocal enabledelayedexpansion

echo.
echo ============================================================================
echo HMI Solution - Complete Build and Deploy Pipeline
echo ============================================================================
echo.

set "SCRIPT_DIR=%~dp0"
cd /d "%SCRIPT_DIR%"

REM ============================================================================
echo [PIPELINE 1/3] Building Release Platform...
echo ============================================================================
call "%SCRIPT_DIR%build-release.bat"
if errorlevel 1 (
    echo [ERROR] Release build failed
    goto :error
)
echo.

REM ============================================================================
echo [PIPELINE 2/3] Building Docker Images...
echo ============================================================================
call "%SCRIPT_DIR%build-docker.bat"
if errorlevel 1 (
    echo [ERROR] Docker image build failed
    goto :error
)
echo.

REM ============================================================================
echo [PIPELINE 3/3] Pushing to Docker Hub...
echo ============================================================================
call "%SCRIPT_DIR%push-docker.bat"
if errorlevel 1 (
    echo [ERROR] Docker push failed
    goto :error
)
echo.

REM ============================================================================
echo [SUCCESS] Complete pipeline finished successfully!
echo ============================================================================
echo.
echo Your HMI Solution is now:
echo   ✓ Built in Release mode
echo   ✓ Packaged as Docker images
echo   ✓ Published to Docker Hub
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
echo   1. build-release.bat  - Build .NET Release artifacts
echo   2. build-docker.bat   - Build Docker images
echo   3. push-docker.bat    - Push to Docker Hub
echo.
endlocal
exit /b 1
