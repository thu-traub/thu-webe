. "$PSScriptRoot\config.ps1"

$ErrorActionPreference = "Stop"
if ($PSVersionTable.PSVersion.Major -ge 7) {
  $PSNativeCommandUseErrorActionPreference = $true
}

$SecretSettingName = "MICROSOFT_PROVIDER_AUTHENTICATION_SECRET"

# Disable App Service Authentication (Auth v2)
az webapp auth update `
  -g $ResourceGroupName `
  -n $WebAppName `
  --enabled false `
  -o none

# Remove the secret app setting from the Web App
az webapp config appsettings delete `
  -g $ResourceGroupName `
  -n $WebAppName `
  --setting-names $SecretSettingName `
  -o none

# Delete the created app registration
$appId = az ad app list --display-name $AppRegistrationName --query "[0].appId" -o tsv
if (-not [string]::IsNullOrWhiteSpace($appId)) {
  az ad app delete --id $appId
} else {
  Write-Host "No app registration found for: $AppRegistrationName"
}

Write-Host "Undo complete."
Write-Host "Disabled auth for: $WebAppName"
Write-Host "Removed app setting: $SecretSettingName"
Write-Host "Deleted app registration: $AppRegistrationName"