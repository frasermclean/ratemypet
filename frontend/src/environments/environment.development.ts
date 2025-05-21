import { withNgxsLoggerPlugin } from '@ngxs/logger-plugin';
import { withNgxsDevelopmentOptions } from '@ngxs/store';
import { Environment } from './environment.interface';

export const environment: Environment = {
  name: 'development',
  applicationInsights: {
    connectionString:
      'InstrumentationKey=16c59f48-bd40-4cbf-9100-5cfaf090a83b;IngestionEndpoint=https://australiaeast-1.in.applicationinsights.azure.com/;LiveEndpoint=https://australiaeast.livediagnostics.monitor.azure.com/;ApplicationId=1f0a8655-ba17-4e40-8f15-9355627cfc15'
  },
  ngxsPlugins: [
    withNgxsDevelopmentOptions({
      warnOnUnhandledActions: true
    }),
    withNgxsLoggerPlugin()
  ]
};
