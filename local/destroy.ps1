param(
    [string]$TenantId = "f5e575c8-bc9f-41b2-b31c-a67bfd9f16ce",
    [string]$ResourceGroupName = "rg-ifi-st-01",
    [string]$WebAppName = "webe-project-app-01",
    [string]$AppServicePlanName = "asp-webe-free",
    [string]$SubscriptionId = "7a9f01f8-5475-494b-9eec-c19c3e9465a4"
    )

$ErrorActionPreference = "Stop"

Write-Host "Deleting web app if it exists: $WebAppName"
az webapp delete --resource-group $ResourceGroupName --name $WebAppName

Write-Host "Deleting app service plan if it exists: $AppServicePlanName"
az appservice plan delete --resource-group $ResourceGroupName --name $AppServicePlanName --yes

Write-Host "Destroy complete."
