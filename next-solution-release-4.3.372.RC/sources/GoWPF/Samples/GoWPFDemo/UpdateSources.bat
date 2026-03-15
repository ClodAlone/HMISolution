cd %1

if "%2"=="clean" goto CLEAN

mkdir source

copy *.xaml source
copy *.xaml.cs source

del/s source\*.xamltxt
rename source\*.xaml *.xamltxt
rename source\*.xaml.cs *.

del/s source\*.xamlcstxt
rename source\*.xaml *.xamlcstxt

goto :EOF

:CLEAN

del/s source\*.xamltxt
del/s source\*.xamlcstxt

