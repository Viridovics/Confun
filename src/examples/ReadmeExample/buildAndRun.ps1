Write-Host "Restore nuget"
dotnet restore

If ($lastExitCode -ne "0") {
    Write-Error "Nuget restore is failed"
    exit
}
Write-Host
Write-Host "Generating configs"
dotnet run --project "./ConfigGenerator/ConfigGenerator.fsproj"
If ($lastExitCode -ne "0") {
    Write-Error "Generating configs is failed"
    exit
}


Write-Host
Write-Host "Build sln"

dotnet build
If ($lastExitCode -ne "0") {
    Write-Error "Build sln was failed"
    exit
}

Write-Host
Write-Host "Run app"

Push-Location -Path ./App/bin/Debug/netcoreapp3.1
dotnet ./App.dll
If ($lastExitCode -ne "0") {
    Write-Error "Run App was failed"
    Pop-Location
    exit
}
Pop-Location