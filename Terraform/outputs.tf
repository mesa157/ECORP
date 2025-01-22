variable "subscription_id" {
  description = "The subscription ID to use for this deployment."
  type        = string
}

variable "resource_group_name" {
  description = "The name of the resource group."
  type        = string
}

variable "location" {
  description = "The location of the resources."
  type        = string
  default     = "West Europe"
}

variable "vnet_name" {
  description = "The name of the virtual network."
  type        = string
  default     = "win10-vnet"
}

variable "aks_subnet_name" {
  description = "The name of the AKS subnet."
  type        = string
  default     = "aks-subnet"
}

variable "acr_subnet_name" {
  description = "The name of the ACR subnet."
  type        = string
  default     = "acr-subnet"
}

variable "acr_name" {
  description = "The name of the Azure Container Registry."
  type        = string
  default     = "win10acr"
}

variable "aks_name" {
  description = "The name of the AKS cluster."
  type        = string
  default     = "win10-aks"
}