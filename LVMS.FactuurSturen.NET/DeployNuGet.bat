@echo off
del *.nupkg
@echo on
".nuget/nuget.exe" pack FactuurSturenNet.Signed.nuspec -symbols

".nuget/nuget.exe" push LVMS.FactuurSturenNet.Signed.*.nupkg

pause