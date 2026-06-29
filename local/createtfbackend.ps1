$RESOURCE_GROUP_NAME="rg-tfstate"
$STORAGE_ACCOUNT_NAME="thuifisttfwebe"
$CONTAINER_NAME="tfstate"
$LOCATION="westeurope"

az group create `
  --name $RESOURCE_GROUP_NAME `
  --location $LOCATION

az storage account create `
  --resource-group $RESOURCE_GROUP_NAME `
  --name $STORAGE_ACCOUNT_NAME `
  --location $LOCATION `
  --sku Standard_LRS `
  --encryption-services blob

az storage container create `
  --name $CONTAINER_NAME `
  --account-name $STORAGE_ACCOUNT_NAME