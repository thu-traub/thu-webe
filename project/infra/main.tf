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

provider "azurerm" {
  features {}
}

provider "azuread" {}

data "azurerm_client_config" "current" {}

###############################################################################
# Variables
###############################################################################

variable "resource_group_name" {
  description = "Resource Group name"
  type        = string
}

variable "location" {
  description = "Azure region"
  type        = string
}

variable "webapp_name" {
  description = "Web App name (must be globally unique)"
  type        = string
}

variable "easyauth" {
  description = "Easy Auth configuration"
  type        = bool
}

locals {
  app_registration_name = "${var.webapp_name}-auth"
  easyauth_redirect_uri = "https://${var.webapp_name}.azurewebsites.net/.auth/login/aad/callback"
}

###############################################################################
# Resources
###############################################################################

data "azurerm_resource_group" "rg" {
  name     = var.resource_group_name
}

resource "azuread_application" "app_auth" {
  count = var.easyauth ? 1 : 0

  display_name = local.app_registration_name

  web {
    redirect_uris = [local.easyauth_redirect_uri]
  }
}

resource "azuread_application_password" "app_auth" {
  count = var.easyauth ? 1 : 0

  application_id = azuread_application.app_auth[0].id
  display_name   = "App Service auth secret"
  end_date       = timeadd(timestamp(), "2160h")
}

resource "azurerm_service_plan" "plan" {
  name                = "${var.webapp_name}-plan"
  location            = data.azurerm_resource_group.rg.location
  resource_group_name = data.azurerm_resource_group.rg.name

  os_type  = "Linux"
  sku_name = "F1"
}

resource "azurerm_linux_web_app" "app" {
  name                = var.webapp_name
  location            = data.azurerm_resource_group.rg.location
  resource_group_name = data.azurerm_resource_group.rg.name
  service_plan_id     = azurerm_service_plan.plan.id

  https_only = true

  site_config {
    always_on = false

    application_stack {
      dotnet_version = "10.0"
    }
  }

  dynamic "auth_settings_v2" {
    for_each = var.easyauth ? [1] : []

    content {
      auth_enabled           = true
      default_provider       = "azureactivedirectory"
      unauthenticated_action = "RedirectToLoginPage"

      login {}

      active_directory_v2 {
        client_id                  = azuread_application.app_auth[0].client_id
        tenant_auth_endpoint       = "https://login.microsoftonline.com/${data.azurerm_client_config.current.tenant_id}/v2.0"
        client_secret_setting_name = "MICROSOFT_PROVIDER_AUTHENTICATION_SECRET"
      }
    }
  }

  app_settings = merge(
    {
      "WEBSITE_RUN_FROM_PACKAGE" = "1"
    },
    var.easyauth ? {
      "MICROSOFT_PROVIDER_AUTHENTICATION_SECRET" = azuread_application_password.app_auth[0].value
    } : {}
  )
}

###############################################################################
# Outputs
###############################################################################

output "webapp_name" {
  value = azurerm_linux_web_app.app.name
}

output "webapp_url" {
  value = "https://${azurerm_linux_web_app.app.default_hostname}"
}

output "app_registration_client_id" {
  value = var.easyauth ? azuread_application.app_auth[0].client_id : null
}