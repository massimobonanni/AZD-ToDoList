param location string
param tags object

@description('Storage account name — max 24 chars, lowercase alphanumeric only.')
@maxLength(24)
param name string

resource storageAccount 'Microsoft.Storage/storageAccounts@2023-01-01' = {
  name: name
  location: location
  tags: tags
  sku: { name: 'Standard_LRS' }
  kind: 'StorageV2'
  properties: {
    allowBlobPublicAccess: false
    allowSharedKeyAccess: false           // enforce managed-identity-only access
    minimumTlsVersion: 'TLS1_2'
    supportsHttpsTrafficOnly: true
    defaultToOAuthAuthentication: true
  }
}

output name string = storageAccount.name
output id string = storageAccount.id
output tableServiceUri string = storageAccount.properties.primaryEndpoints.table
output blobServiceUri string = storageAccount.properties.primaryEndpoints.blob
output queueServiceUri string = storageAccount.properties.primaryEndpoints.queue
