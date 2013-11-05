To update package:

1. Increment version of package in Package.nuspec and RunPerfTests.bat
2. Run Package.bat
3. If necessary, set nuget api key (get the key from nuget.org) :
    nuget.exe setApiKey aa00aaa0-00aa-0a00-a000-000aa00a00aa
4. Push to nuget.org:
    cd Package
    ..\tools\nuget\nuget.exe push PerfTestRunner.0.0.3.nupkg