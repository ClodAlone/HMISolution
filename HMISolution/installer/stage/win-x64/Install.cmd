@echo off
powershell.exe -NoProfile -ExecutionPolicy Bypass -File "%~dp0Install-HMISolution.ps1" %*
pause
