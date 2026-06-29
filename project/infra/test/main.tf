terraform {
  required_version = ">= 1.6.0"

  backend "azurerm" {
    resource_group_name  = "rg-tfstate"
    storage_account_name = "thuifisttfwebe" # Replace with your storage account
    container_name       = "tfstate"
    key                  = "webapp-dev.tfstate"
    use_azuread_auth     = true
  }

  required_providers {
    azurerm = {
      source  = "hashicorp/azurerm"
      version = "~> 4.0"
    }
    azuread = {
      source  = "hashicorp/azuread"
      version = "~> 3.0"
    }
  }
}

module "modules" {
  source = "../modules"
  resource_group_name = "rg-WEBE"
  location            = "West Europe"
  webapp_name         = "thu-st-webe-demo-01"
  easyauth            = true
}