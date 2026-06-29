# param(
#     [string]$TenantId = "f5e575c8-bc9f-41b2-b31c-a67bfd9f16ce",
#     [string]$ResourceGroupName = "rg-ifi-st-01",
#     [string]$WebAppName = "webe-project-app-01",
#     [string]$Location = "westeurope",
#     [string]$AppServicePlanName = "asp-webe-free",
#     [string]$SubscriptionId = "7a9f01f8-5475-494b-9eec-c19c3e9465a4",
#     [string]$ProjectFile = ".\project.csproj"
# )

. "$PSScriptRoot\config.ps1"

$ErrorActionPreference = "Stop"

$publishPath = Join-Path $PubPath "publish"
$zipPath = Join-Path $PubPath "site.zip"

Write-Host "Publishing .NET app..."
dotnet publish $ProjectFile -c Release -o $publishPath

if (Test-Path $zipPath) {
    Remove-Item $zipPath -Force
}

Copy-Item "$ProjectDir\users.txt" $publishPath
Copy-Item "$ProjectDir\key.txt" $publishPath

Write-Host "Creating deployment zip: $zipPath"
Compress-Archive -Path (Join-Path $publishPath "*") -DestinationPath $zipPath -Force

Write-Host "Deploying zip package to web app..."
az webapp deploy `
    --resource-group $ResourceGroupName `
    --name $WebAppName `
    --src-path $zipPath `
    --type zip

$webUrl = "https://$WebAppName.azurewebsites.net"
Write-Host "Deployment complete."
Write-Host "Web App URL: $webUrl"
