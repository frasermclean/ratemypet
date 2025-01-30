using 'main.bicep'

param appEnv = 'prod'

param allowedExternalIpAddresses = [
  {
    name: 'hive'
    ipAddress: readEnvironmentVariable('HIVE_IP_ADDRESS', '')
  }
]

param cloudinaryApiKey = '248796789595321'
