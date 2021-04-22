##############################
# Common
##############################
variable "project" {
  type        = string
  description = "Project name"
  default     = "ghazfunction"
}

variable "environment" {
  type        = string
  description = "Environment (dev / stage / prod)"
}

variable "location" {
  type        = string
  description = "Azure region to deploy module to"
  default     = "UK South"
}

variable "tags" {
  type        = map
  description = "Tags for everything in the Resource Group"
  default = {
    BusinessUnit  = "GitHubActions"
    OpsCommitment = "Baseline only"
  }
}

##############################
# Storage Account
##############################
variable "storage_account_tier" {
  type        = string
  description = "Storage Account Tier"
  default     = "Standard"
}
variable "storage_account_replication_type" {
  type        = string
  description = "Storage Account Replication Type"
  default     = "LRS"
}

##############################
# App Service 
##############################
variable "app_service_plan_sku_tier" {
  type        = string
  description = "App Service Plan Tier"
  default     = "Dynamic"
}
variable "app_service_plan_sku_size" {
  type        = string
  description = "App Service Plan Size"
  default     = "Y1"
}

