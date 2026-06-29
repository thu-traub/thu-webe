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

$projdir = (Get-Location)
$publishPath = Join-Path $projdir "publish"
$zipPath = Join-Path $projdir "site.zip"
$keyFilePath = Join-Path $projdir "key.txt"
$usersFilePath = Join-Path $projdir "users.txt"

if (-not (Test-Path $keyFilePath)) {
    throw "Missing required file: $keyFilePath"
}

if (-not (Test-Path $usersFilePath)) {
    throw "Missing required file: $usersFilePath"
}

$keyEnvValue = (Get-Content -Path $keyFilePath -Raw).Trim()
$usersEnvValue = (Get-Content -Path $usersFilePath -Raw).Trim()

Write-Host "Creating App Service plan (Free tier F1): $AppServicePlanName"
az appservice plan create `
    --name $AppServicePlanName `
    --resource-group $ResourceGroupName `
    --sku F1

Write-Host "Creating web app: $WebAppName"
az webapp create `
    --resource-group $ResourceGroupName `
    --plan $AppServicePlanName `
    --name $WebAppName

Write-Host "Setting web app environment variables: key, users"
az webapp config appsettings set `
    --resource-group $ResourceGroupName `
    --name $WebAppName `
    --settings "key=$keyEnvValue" "users=$usersEnvValue"

