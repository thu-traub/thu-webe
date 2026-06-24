param(
    [string]$TenantId = "f5e575c8-bc9f-41b2-b31c-a67bfd9f16ce",
    [string]$ResourceGroupName = "rg-ifi-st-01",
    [string]$WebAppName = "webe-project-app-01",
    [string]$Location = "westeurope",
    [string]$AppServicePlanName = "asp-webe-free",
    [string]$SubscriptionId = "7a9f01f8-5475-494b-9eec-c19c3e9465a4",
    [string]$ProjectFile = ".\project.csproj"
)

$ErrorActionPreference = "Stop"

$scriptRoot = Split-Path -Parent $MyInvocation.MyCommand.Path
$publishPath = Join-Path $scriptRoot "publish"
$zipPath = Join-Path $scriptRoot "site.zip"
$keyFilePath = Join-Path $scriptRoot "key.txt"
$usersFilePath = Join-Path $scriptRoot "users.txt"

# if (-not (Test-Path $keyFilePath)) {
#     throw "Missing required file: $keyFilePath"
# }

# if (-not (Test-Path $usersFilePath)) {
#     throw "Missing required file: $usersFilePath"
# }

# $keyEnvValue = (Get-Content -Path $keyFilePath -Raw).Trim()
# $usersEnvValue = (Get-Content -Path $usersFilePath -Raw).Trim()

# Write-Host "Creating App Service plan (Free tier F1): $AppServicePlanName"
# az appservice plan create `
#     --name $AppServicePlanName `
#     --resource-group $ResourceGroupName `
#     --sku F1

# Write-Host "Creating web app: $WebAppName"
# az webapp create `
#     --resource-group $ResourceGroupName `
#     --plan $AppServicePlanName `
#     --name $WebAppName

# Write-Host "Setting web app environment variables: key, users"
# az webapp config appsettings set `
#     --resource-group $ResourceGroupName `
#     --name $WebAppName `
#     --settings "key=$keyEnvValue" "users=$usersEnvValue"

Write-Host "Publishing .NET app..."
dotnet publish $ProjectFile -c Release -o $publishPath

if (Test-Path $zipPath) {
    Remove-Item $zipPath -Force
}

copy .\users.txt $publishPath
copy .\key.txt $publishPath

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
