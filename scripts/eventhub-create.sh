#!/bin/bash
set -e

# Assign default values to variables
RESOURCE_GROUP_NAME="learn-ab9a0ab4-17ff-4daf-8f93-31ecaa196613"
LOCATION="canadacentral"
EVENT_NAMESPACE_NAME="ehubns-"${RANDOM}

# Assign default values that can be reused so that you don't have to enter these values every command
# az configure --defaults group=$RESOURCE_GROUP_NAME location=$LOCATION

# Create an Event Hub namespace
az eventhubs namespace create \
  --name $EVENT_NAMESPACE_NAME \
  --resource-group $RESOURCE_GROUP_NAME \
  --location $LOCATION \
  --sku Standard


# Fetch the connection string for the Event Hub namespace
EVENT_HUB_CONNECTION_STRING=$(az eventhubs namespace authorization-rule keys list \
  --resource-group $RESOURCE_GROUP_NAME \
  --namespace-name $EVENT_NAMESPACE_NAME \
  --name RootManageSharedAccessKey \
  --query primaryConnectionString -o tsv)

# Fetch the primary key for the Event Hub namespace
EVENT_HUB_PRIMARY_KEY=$(az eventhubs namespace authorization-rule keys list \
  --resource-group $RESOURCE_GROUP_NAME \
  --namespace-name $EVENT_NAMESPACE_NAME \
  --name RootManageSharedAccessKey \
  --query primaryKey -o tsv)


# Define the Event Hub name
EVENT_HUB_NAME="hubname-"${RANDOM}

# Create an Event Hub
az eventhubs eventhub create \
  --name $EVENT_HUB_NAME \
  --namespace-name $EVENT_HUB_NAME \
  --resource-group $RESOURCE_GROUP_NAME \
  --message-retention 1 \
  --partition-count 4 \
  --max-messages-per-second 1000

# View the details of the Event Hub
az eventhubs eventhub show \
  --name $EVENT_HUB_NAME \
  --namespace-name $EVENT_NAMESPACE_NAME \
  --resource-group $RESOURCE_GROUP_NAME