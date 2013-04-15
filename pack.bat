@echo off

git rev-parse --abbrev-ref HEAD > .currentbranch
set /p Branch=<.currentbranch

msbuild "src\NuGetContentGenerator.sln" /verbosity:m /p:Configuration=Release /p:Branch=%Branch%

del .currentbranch

PAUSE