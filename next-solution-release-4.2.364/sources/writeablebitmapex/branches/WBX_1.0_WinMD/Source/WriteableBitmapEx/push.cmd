SET Id=WriteableBitmapEx
SET VERSION=0.9.8.6
..\..\3rdParty\nuget\nuget delete %ID% %VERSION%
..\..\3rdParty\nuget\nuget push Bin\nuget\%ID%.%VERSION%.nupkg