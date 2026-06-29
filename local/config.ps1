$cenv = "test"

$ProjectFile = "$PSScriptRoot\..\project\project.csproj"

if ($cenv -eq "test") {
    $TenantId = "0f5e575c8-bc9f-41b2-b31c-a67bfd9f16ce"
    $ResourceGroupName = "rg-webe-st-01"
    $WebAppName = "webe-project-app-01"
    $Location = "westeurope"
    $AppServicePlanName = "asp-webe-free"
    $SubscriptionId = "4ac87745-438e-4b22-9df3-b28d731a2772"
} else {
    $TenantId = "f5e575c8-bc9f-41b2-b31c-a67bfd9f16ce"
    $ResourceGroupName = "rg-ifi-st-01"
    $WebAppName = "webe-project-app-01"
    $Location = "westeurope"
    $AppServicePlanName = "asp-webe-free"
    $SubscriptionId = "7a9f01f8-5475-494b-9eec-c19c3e9465a4"
}