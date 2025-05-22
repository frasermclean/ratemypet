import { Environment } from './environment.interface';

export const environment: Environment = {
  name: 'production',
  applicationInsights: {
    connectionString:
      'InstrumentationKey=2c98db66-4f95-4583-bef8-efcc9fb22ac3;IngestionEndpoint=https://australiaeast-1.in.applicationinsights.azure.com/;LiveEndpoint=https://australiaeast.livediagnostics.monitor.azure.com/;ApplicationId=835e56be-f22b-4ab0-b88a-f234374fcdc5'
  },
  ngxsPlugins: []
};
