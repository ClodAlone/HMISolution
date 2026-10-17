
@echo off
echo Starting the sequential process...

call "build-docker.bat"

call "push-docker.bat"

call "build-allinone-docker.bat"

call "push-allinone-docker.bat"
