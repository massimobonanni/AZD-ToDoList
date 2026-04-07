// RBAC role assignments for the Function App's system-assigned managed identity
// on the shared Storage Account.
//
// Required roles:
//   Storage Blob Data Owner      — Functions host uses blobs for locks/leases
//   Storage Queue Data Contributor — Functions host uses queues for triggers
//   Storage Table Data Contributor — App uses Table Storage for task data

param principalId string
param storageAccountName string

var storageBlobDataOwner      = subscriptionResourceId('Microsoft.Authorization/roleDefinitions', 'b7e6dc6d-f1e8-4753-8033-0f276bb0955b')
var storageQueueDataContributor = subscriptionResourceId('Microsoft.Authorization/roleDefinitions', '974c5e8b-45b9-4653-ba55-5f855dd0fb88')
var storageTableDataContributor = subscriptionResourceId('Microsoft.Authorization/roleDefinitions', '0a9a7e1f-b9d0-4cc4-a60d-0319b160aaa3')

resource storageAccount 'Microsoft.Storage/storageAccounts@2023-01-01' existing = {
  name: storageAccountName
}

resource blobOwner 'Microsoft.Authorization/roleAssignments@2022-04-01' = {
  name: guid(storageAccount.id, principalId, storageBlobDataOwner)
  scope: storageAccount
  properties: {
    roleDefinitionId: storageBlobDataOwner
    principalId: principalId
    principalType: 'ServicePrincipal'
  }
}

resource queueContributor 'Microsoft.Authorization/roleAssignments@2022-04-01' = {
  name: guid(storageAccount.id, principalId, storageQueueDataContributor)
  scope: storageAccount
  properties: {
    roleDefinitionId: storageQueueDataContributor
    principalId: principalId
    principalType: 'ServicePrincipal'
  }
}

resource tableContributor 'Microsoft.Authorization/roleAssignments@2022-04-01' = {
  name: guid(storageAccount.id, principalId, storageTableDataContributor)
  scope: storageAccount
  properties: {
    roleDefinitionId: storageTableDataContributor
    principalId: principalId
    principalType: 'ServicePrincipal'
  }
}
