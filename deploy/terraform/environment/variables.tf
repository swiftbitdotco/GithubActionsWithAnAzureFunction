variable "project" {
  type        = string
  description = "Project name"
  default = "ghazfunction"
}

variable "environment" {
  type        = string
  description = "Environment (dev / stage / prod)"
}

variable "location" {
  type        = string
  description = "Azure region to deploy module to"
}

variable "tags" {
  type = map
  description = "Tags for everything in the Resource Group"
  default = {
    BusinessUnit  = "GitHubActions"
    OpsCommitment = "Baseline only"
  }
}
