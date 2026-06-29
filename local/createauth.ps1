. "$PSScriptRoot\config.ps1"

$ErrorActionPreference = "Stop"
if ($PSVersionTable.PSVersion.Major -ge 7) {
  $PSNativeCommandUseErrorActionPreference = $true
}

$hostName = az webapp show -g $ResourceGroupName -n $WebAppName --query defaultHostName -o tsv
$redirectUri = "https://$hostName/.auth/login/aad/callback"

$appId = az ad app create `
  --display-name $AppRegistrationName `
  --web-redirect-uris $redirectUri `
  --query appId `
  -o tsv

$appObjectId = az ad app show --id $appId --query id -o tsv

# Ensure the app registration is configured to issue ID tokens.
az rest `
  --method PATCH `
  --uri "https://graph.microsoft.com/v1.0/applications/$appObjectId" `
  --headers "Content-Type=application/json" `
  --body '{"web":{"implicitGrantSettings":{"enableIdTokenIssuance":true}}}' > $null

$endDate = (Get-Date).AddDays(90).ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ssZ")

$clientSecret = az ad app credential reset `
  --id $appId `
  --display-name "App Service auth secret" `
  --end-date $endDate `
  --query password `
  -o tsv

az webapp config appsettings set `
  -g $ResourceGroupName `
  -n $WebAppName `
  --settings MICROSOFT_PROVIDER_AUTHENTICATION_SECRET="$clientSecret" `
  -o none

# Configure current App Service Authentication (Auth v2) via ARM authsettingsV2
$authSettingsV2 = @{
  properties = @{
    platform = @{ enabled = $true }
    globalValidation = @{
      requireAuthentication       = $true
      unauthenticatedClientAction = "RedirectToLoginPage"
      redirectToProvider          = "azureactivedirectory"
    }
    identityProviders = @{
      azureActiveDirectory = @{
        enabled      = $true
        registration = @{
          clientId                = $appId
          clientSecretSettingName = "MICROSOFT_PROVIDER_AUTHENTICATION_SECRET"
          openIdIssuer            = "https://login.microsoftonline.com/$TenantId/v2.0"
        }
      }
    }
    login = @{
      tokenStore = @{ enabled = $true }
    }
  }
} | ConvertTo-Json -Depth 10 -Compress

$authSettingsV2Uri = "https://management.azure.com/subscriptions/$SubscriptionId/resourceGroups/$ResourceGroupName/providers/Microsoft.Web/sites/$WebAppName/config/authsettingsV2?api-version=2022-03-01"

$authSettingsV2File = Join-Path $env:TEMP ("authsettingsV2-" + [guid]::NewGuid().ToString() + ".json")
$authSettingsV2 | Out-File -FilePath $authSettingsV2File -Encoding utf8

az rest `
  --method PUT `
  --uri $authSettingsV2Uri `
  --headers "Content-Type=application/json" `
  --body "@$authSettingsV2File" > $null

Remove-Item -Path $authSettingsV2File -Force -ErrorAction SilentlyContinue

Write-Host "Authentication configured."
Write-Host "App registration: $AppRegistrationName"
Write-Host "Client ID: $appId"
Write-Host "Redirect URI: $redirectUri"
Write-Host "Secret expires: $endDate"
