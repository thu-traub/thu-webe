param(
  [string] $TenantId = "f5e575c8-bc9f-41b2-b31c-a67bfd9f16ce",
  [string] $SubscriptionId = "7a9f01f8-5475-494b-9eec-c19c3e9465a4",
  [string] $ResourceGroupName = "rg-ifi-st-01",
  [string] $WebAppName = "webe-project-app-01",
  [string] $AppRegistrationName = "$WebAppName-auth"
)

$ErrorActionPreference = "Stop"

$hostName = az webapp show -g $ResourceGroupName -n $WebAppName --query defaultHostName -o tsv
$redirectUri = "https://$hostName/.auth/login/aad/callback"

$appId = az ad app create `
  --display-name $AppRegistrationName `
  --web-redirect-uris $redirectUri `
  --query appId `
  -o tsv

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
  --settings MICROSOFT_PROVIDER_AUTHENTICATION_SECRET="$clientSecret"

az webapp auth-classic update `
  -g $ResourceGroupName `
  -n $WebAppName `
  --enabled true `
  --action LoginWithAzureActiveDirectory `
  --aad-client-id $appId `
  --aad-client-secret "$clientSecret" `
  --aad-token-issuer-url "https://login.microsoftonline.com/$TenantId/v2.0"

Write-Host "Authentication configured."
Write-Host "App registration: $AppRegistrationName"
Write-Host "Client ID: $appId"
Write-Host "Redirect URI: $redirectUri"
Write-Host "Secret expires: $endDate"