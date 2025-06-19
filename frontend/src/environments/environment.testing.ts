import { Environment } from './environment.interface';

export const environment: Environment = {
  name: 'testing',
  applicationInsights: {
    connectionString:
      'InstrumentationKey=3ed381c8-76f7-4d3c-bbc9-d0309b388c55;IngestionEndpoint=https://australiaeast-1.in.applicationinsights.azure.com/;LiveEndpoint=https://australiaeast.livediagnostics.monitor.azure.com/;ApplicationId=aa4291f8-feda-4999-a7ba-dd8e7b505d50'
  },
  ngxsPlugins: []
};
