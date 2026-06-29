param(
  [string] $ResourceGroupName = "rg-ifi-st-01",
  [string] $WebAppName = "webe-project-app-01",
  [string] $AppRegistrationName = "$WebAppName-auth",
  [string] $SecretSettingName = "MICROSOFT_PROVIDER_AUTHENTICATION_SECRET"
)

# Disable App Service Authentication v1/classic
az webapp auth-classic update `
  -g $ResourceGroupName `
  -n $WebAppName `
  --enabled false

# Remove the secret app setting from the Web App
az webapp config appsettings delete `
  -g $ResourceGroupName `
  -n $WebAppName `
  --setting-names $SecretSettingName

# Delete the created app registration
$appId = az ad app list --display-name $AppRegistrationName --query "[0].appId" -o tsv
az ad app delete --id $appId

Write-Host "Undo complete."
Write-Host "Disabled classic auth for: $WebAppName"
Write-Host "Removed app setting: $SecretSettingName"
Write-Host "Deleted app registration: $AppRegistrationName"