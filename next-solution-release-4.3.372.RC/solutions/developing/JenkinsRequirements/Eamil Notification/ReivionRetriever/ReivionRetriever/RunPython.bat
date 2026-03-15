@echo off
SET username= %~1
SET PASSWORD= %~2
SET emailto=  %~3 
SET AssemblyInfoPath=%~4
python "%cd%\ReivionRetriever.py" %username% %PASSWORD% %emailto% %~4\AssemblyVersionInfo.cs