@echo off
REM ============================================================================
REM Build Release Platform - HMI Solution
REM ============================================================================
REM This script builds all platform components in Release configuration
REM and prepares them for deployment or Docker packaging.
REM ============================================================================

setlocal enabledelayedexpansion

echo.
echo ============================================================================
echo Building HMI Solution - Release Platform
echo ============================================================================
echo.

REM Set error handling
set "ERROR_OCCURRED=0"

REM Get script directory (this is C:\Users\cfior\source\repos\)
set "SCRIPT_DIR=%~dp0"
cd /d "%SCRIPT_DIR%"

REM Solution file (slnx format in the repos root)
set "SOLUTION_FILE=%SCRIPT_DIR%HMISolution.slnx"

if not exist "%SOLUTION_FILE%" (
    echo [ERROR] Solution file not found: %SOLUTION_FILE%
    exit /b 1
)

echo [INFO] Solution: %SOLUTION_FILE%
echo.

REM ============================================================================
echo [STEP 1/5] Cleaning previous builds...
echo ============================================================================
dotnet clean "%SOLUTION_FILE%" --configuration Release --verbosity minimal
if errorlevel 1 (
    echo [ERROR] Clean failed
    set "ERROR_OCCURRED=1"
    goto :error
)
echo [OK] Clean completed
echo.

REM ============================================================================
echo [STEP 2/5] Restoring NuGet packages...
echo ============================================================================
dotnet restore "%SOLUTION_FILE%" --verbosity minimal
if errorlevel 1 (
    echo [ERROR] Restore failed
    set "ERROR_OCCURRED=1"
    goto :error
)
echo [OK] Restore completed
echo.

REM ============================================================================
echo [STEP 3/5] Building solution in Release mode...
echo ============================================================================
dotnet build "%SOLUTION_FILE%" --configuration Release --no-restore --verbosity minimal
if errorlevel 1 (
    echo [ERROR] Build failed
    set "ERROR_OCCURRED=1"
    goto :error
)
echo [OK] Build completed
echo.

REM ============================================================================
echo [STEP 4/5] Running tests...
echo ============================================================================
echo [INFO] Tests are skipped due to known .NET 10 + xunit.v3 compatibility issue
echo [INFO] Issue: testhost.dll not found (testhost version 18.3.0-release-26177-108)
echo [INFO] This will be resolved when xunit.v3 or .NET 10 RTM is released
echo [SKIPPED] Test execution skipped - build continues
REM dotnet test "%SOLUTION_FILE%" --configuration Release --no-build --verbosity minimal --logger:"console;verbosity=minimal"
REM if errorlevel 1 (
REM     echo [WARNING] Some tests failed - continuing anyway
REM     echo [INFO] Review test results above
REM ) else (
REM     echo [OK] All tests passed
REM )
echo.

REM ============================================================================
echo [STEP 5/5] Publishing server and editor...
echo ============================================================================

REM Publish Server
set "SERVER_OUTPUT=%SCRIPT_DIR%publish\Server"
echo [INFO] Publishing Server to: %SERVER_OUTPUT%
dotnet publish "%SCRIPT_DIR%Server\Server.csproj" ^
    --configuration Release ^
    --output "%SERVER_OUTPUT%" ^
    --no-build ^
    --verbosity minimal
if errorlevel 1 (
    echo [ERROR] Server publish failed
    set "ERROR_OCCURRED=1"
    goto :error
)
echo [OK] Server published

REM Publish ServerEditorWeb
set "EDITOR_OUTPUT=%SCRIPT_DIR%publish\ServerEditorWeb"
echo [INFO] Publishing Editor to: %EDITOR_OUTPUT%
dotnet publish "%SCRIPT_DIR%ServerEditorWeb\ServerEditorWeb.csproj" ^
    --configuration Release ^
    --output "%EDITOR_OUTPUT%" ^
    --no-build ^
    --verbosity minimal
if errorlevel 1 (
    echo [ERROR] Editor publish failed
    set "ERROR_OCCURRED=1"
    goto :error
)
echo [OK] Editor published

REM Publish RuntimeViewer
set "VIEWER_OUTPUT=%SCRIPT_DIR%publish\RuntimeViewer"
echo [INFO] Publishing RuntimeViewer to: %VIEWER_OUTPUT%
dotnet publish "%SCRIPT_DIR%RuntimeViewer\RuntimeViewer.csproj" ^
    --configuration Release ^
    --output "%VIEWER_OUTPUT%" ^
    --no-build ^
    --verbosity minimal
if errorlevel 1 (
    echo [ERROR] RuntimeViewer publish failed
    set "ERROR_OCCURRED=1"
    goto :error
)
echo [OK] RuntimeViewer published

echo.

REM ============================================================================
echo [SUCCESS] Build completed successfully!
echo ============================================================================
echo.
echo Published outputs:
echo   - Server:         %SERVER_OUTPUT%
echo   - Editor:         %EDITOR_OUTPUT%
echo   - RuntimeViewer:  %VIEWER_OUTPUT%
echo.
echo You can now:
echo   1. Run: build-docker.bat     (to build Docker images)
echo   2. Run: push-docker.bat      (to push to Docker Hub)
echo   3. Test locally from publish folders
echo.

endlocal
exit /b 0

:error
echo.
echo ============================================================================
echo [FAILED] Build failed with errors
echo ============================================================================
echo.
endlocal
exit /b 1
