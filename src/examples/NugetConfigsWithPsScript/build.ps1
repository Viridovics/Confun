Write-Host "Restore nuget"
dotnet restore

If ($lastExitCode -ne "0") {
    Write-Error "Nuget restore is failed"
    exit
}

Write-Host "Generating configs"
dotnet run --project "src\NugetConfigsWithPsScript.ConfigGenerator\NugetConfigsWithPsScript.ConfigGenerator.fsproj"
If ($lastExitCode -ne "0") {
    Write-Error "Generating configs is failed"
    exit
}

Write-Host "Solution build"
dotnet build

If ($lastExitCode -ne "0") {
    Write-Error "Solution build is failed"
    exit
}
