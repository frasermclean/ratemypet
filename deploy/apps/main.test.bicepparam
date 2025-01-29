using 'main.bicep'

param appEnv = 'test'

param allowedExternalIpAddresses = [
  {
    name: 'hive'
    ipAddress: readEnvironmentVariable('HIVE_IP_ADDRESS', '')
  }
]

param cloudinaryApiKey = '415326786129473'
