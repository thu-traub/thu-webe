# setup-github-azure-oidc.ps1

param(
    [string] $GitHubOrg = "thu-traub",
    [string] $GitHubRepo = "thu-webe-test",
    [string] $Branch = "test",
    [string] $TfStateResourceGroup = "rg-tfstate",
    [string] $TfStateStorageAccount = "tfstate$(Get-Random -Minimum 10000 -Maximum 99999)",
    [string] $TfStateContainer = "tfstate",
    [string] $AppResourceGroup = "rg-my-dotnet-webapp"
)

# Login first if needed:
# az login

$ErrorActionPreference = "Stop"

Write-Host "Getting Azure subscription..."
$SubscriptionId = az account show --query id -o tsv
$TenantId = az account show --query tenantId -o tsv

Write-Host "Creating Entra app registration..."
$ClientId = az ad app create `
    --display-name $AppName `
    --query appId `
    -o tsv

Write-Host "Creating service principal..."
az ad sp create --id $ClientId | Out-Null

Start-Sleep -Seconds 10

$SpObjectId = az ad sp show `
    --id $ClientId `
    --query id `
    -o tsv

Write-Host "Creating GitHub federated credential..."
$FederatedCredential = @{
    name        = "github-$GitHubRepo-$Branch"
    issuer      = "https://token.actions.githubusercontent.com"
    subject     = "repo:$GitHubOrg/$GitHubRepo`:ref:refs/heads/$Branch"
    audiences   = @("api://AzureADTokenExchange")
    description = "GitHub Actions OIDC for $GitHubOrg/$GitHubRepo on $Branch"
}

$FederatedCredentialJson = $FederatedCredential | ConvertTo-Json -Depth 10

$TempFile = New-TemporaryFile
$FederatedCredentialJson | Set-Content -Path $TempFile -Encoding utf8

az ad app federated-credential create `
    --id $ClientId `
    --parameters "@$TempFile" | Out-Null

Remove-Item $TempFile

Write-Host "Assigning Contributor role to app resource group..."
az role assignment create `
    --assignee-object-id $SpObjectId `
    --assignee-principal-type ServicePrincipal `
    --role "Contributor" `
    --scope "/subscriptions/$SubscriptionId/resourceGroups/$AppResourceGroup" | Out-Null

Write-Host "Assigning Storage Blob Data Contributor role to Terraform state storage..."
az role assignment create `
    --assignee-object-id $SpObjectId `
    --assignee-principal-type ServicePrincipal `
    --role "Storage Blob Data Contributor" `
    --scope "/subscriptions/$SubscriptionId/resourceGroups/$TfStateResourceGroup/providers/Microsoft.Storage/storageAccounts/$TfStateStorageAccount" | Out-Null

Write-Host ""
Write-Host "Done."
Write-Host ""
Write-Host "Add these GitHub secrets:"
Write-Host "AZURE_CLIENT_ID=$ClientId"
Write-Host "AZURE_TENANT_ID=$TenantId"
Write-Host "AZURE_SUBSCRIPTION_ID=$SubscriptionId"
Write-Host ""
Write-Host "Terraform backend values:"
Write-Host "resource_group_name  = $TfStateResourceGroup"
Write-Host "storage_account_name = $TfStateStorageAccount"
Write-Host "container_name       = $TfStateContainer"
Write-Host "key                  = webapp-dev.tfstate"
