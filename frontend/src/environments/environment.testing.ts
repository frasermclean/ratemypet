import { Environment } from './environment.interface';

export const environment: Environment = {
  name: 'testing',
  applicationInsights: {
    connectionString:
      'InstrumentationKey=7aa846bc-30f1-4c66-8c03-88aea16da39d;IngestionEndpoint=https://australiaeast-1.in.applicationinsights.azure.com/;LiveEndpoint=https://australiaeast.livediagnostics.monitor.azure.com/;ApplicationId=0ce4e4ca-b8c3-4398-bccf-53abe311eacf'
  },
  ngxsPlugins: []
};
