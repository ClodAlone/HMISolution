REM #############################################################################
REM This sample command line creates a Syncfusion.Shared.resources.dll from 
REM a set or .resources files in the de-DEResources sub-dir.
REM This sub-dir and the files are not provided by default and needs to be created
REM before running this batch file.
REM
REM This batch file will also utilize the sf.publicsnk file to create the delay signed assembly.
REM #############################################################################


al /target:lib /culture:de-DE /out:Syncfusion.Windows.Forms.Localization.resources.dll /v:1.1.0.0 /delay+ /keyf:sf.publicsnk /embed:de-DEResources/Syncfusion.Windows.Forms.ColorPicker.Resources.colordlg.data
pause
