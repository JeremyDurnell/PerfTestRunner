@echo off
@echo Build and clean .sln
msbuild PerfTestRunner.sln /p:Configuration=Release /t:Clean,Build /verbosity:quiet

@echo Delete old package folder
rd Package /s /q

@echo Populate content folder 
mkdir Package\content\Samples\PerfTestRunner
copy PerfTestRunner.Demo\SampleTests.cs Package\content\Samples\PerfTestRunner\SampleTests.cs
copy RunPerfTests.bat Package\content\RunPerfTests.bat

@echo Popuate lib folder
mkdir Package\lib\40
copy PerfTestRunner.Common\bin\Release\*.* Package\lib\40

@echo Populate tools folder
mkdir Package\tools
copy PerfTestRunner\bin\Release\*.* Package\tools

@echo Build NuGet package
copy Package.nuspec Package\Package.nuspec
pushd Package
..\tools\NuGet\NuGet pack Package.nuspec
popd