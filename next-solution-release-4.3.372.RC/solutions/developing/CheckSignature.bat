@echo off
If "%~1"=="" (
echo The base path parameter is missing, provide a valid path wwhere to check for exclude file
exit 1
) 
If "%~2"=="" (
echo The working branch name is missing,please provide a branch anme. 
exit 1
) 

setlocal enabledelayedexpansion
SET rootPath=%~1\%~2
SET workingPath=%~1\FilesChanges\%~2\
SET ExclusionListFile=%workingPath%FileToExcludeFromSigCheck.txt
SET UnsingedFilesList= %workingPath%UnsignedFiles.txt
If Exist %UnsingedFilesList% del %UnsingedFilesList%

If not Exist %ExclusionListFile% (
	echo Warning, no exlusion list was found
	(echo Warning, no exlusion list was found)>>%UnsingedFilesList%
	Exit 0
)
 for %%e in (dll,exe) do (
	for /F %%d in ('dir /B /A-D %rootPath%\*.%%e /S') do (
		set /A bSkipFile=0
		SET fileName=%%d
		for /F "delims=" %%s in (%ExclusionListFile%) do If not "!fileName:%%s=!"=="!fileName!" SET bSkipFile=1
		If !bSkipFile! EQU 0 (
			for /F "delims=" %%l in ('sigcheck -nobanner "%%d"^|findstr Verified') do (
				
				set VerifiedLine=%%l
					if not "!VerifiedLine:Unsigned=!"=="!VerifiedLine!" (
						(echo %%d)>>%UnsingedFilesList%
					)
			)
		)
	)
 )
