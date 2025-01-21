using 'main.bicep'

param workload = 'ratemypet'
param location = 'Australia East'
param appEnvironments = ['prod', 'test']
param gitHubRepository = 'frasermclean/ratemypet'

param adminGroupMembers = [
  'd120ebdd-dad5-4b31-9bb0-2b9cea918b09'
]
