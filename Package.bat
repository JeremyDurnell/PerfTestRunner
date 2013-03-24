@echo off
REM Build and clean .sln
msbuild PerfTestRunner.sln /p:Configuration=Release /t:Clean,Build /verbosity:quiet

REM Create package folder
rd Package /s /q

REM Populate content folder 
mkdir Package\content\Samples\PerfTestRunner
copy PerfTestRunner.Demo\SampleTests.cs Package\content\Samples\PerfTestRunner\SampleTests.cs
copy RunPerfTests.bat Package\content\RunPerfTests.bat

REM Popuate lib folder
mkdir Package\lib\40
copy PerfTestRunner.Common\bin\Release\*.* Package\lib\40

REM Populate tools folder
mkdir Package\tools
copy PerfTestRunner\bin\Release\*.* Package\tools

REM Build NuGet package
copy Package.nuspec Package\Package.nuspec
pushd Package
..\tools\NuGet\NuGet pack Package.nuspec
popd