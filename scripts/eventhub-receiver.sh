STORAGE_NAME="storagename"${RANDOM}

# Create a storage account
az storage account create \
  --name $STORAGE_NAME \
  --resource-group $RESOURCE_GROUP_NAME \
  --location $LOCATION \
  --sku Standard_LRS \
  --encryption-services blob

# Obtain the access key for the storage account
STORAGE_KEY=$(az storage account keys list \
  --account-name $STORAGE_NAME \
  --resource-group $RESOURCE_GROUP_NAME \
  --query '[0].value' -o tsv)

# Obtain the connection string for the storage account
STORAGE_CONNECTION_STRING=$(az storage account show-connection-string \
  --name $STORAGE_NAME \
  --resource-group $RESOURCE_GROUP_NAME \
  --query connectionString -o tsv)

  # Createa a container in the storage account
az storage container create \
  --name messages \
  --connection-string $STORAGE_CONNECTION_STRING