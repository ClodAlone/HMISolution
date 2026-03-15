SET OUTDIR=Bin\nuget
mkdir %OUTDIR%
..\..\3rdParty\nuget\nuget pack -sym WriteableBitmapEx.csproj -outputdirectory %OUTDIR%