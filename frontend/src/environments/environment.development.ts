import { Environment } from './environment.interface';

export const environment: Environment = {
  name: 'development',
  apiBaseUrl: 'https://localhost:5443/api',
  applicationInsights: {
    connectionString:
      'InstrumentationKey=113d2d85-aeb9-4e81-b6fa-62db294bbaa0;IngestionEndpoint=https://southeastasia-1.in.applicationinsights.azure.com/;LiveEndpoint=https://southeastasia.livediagnostics.monitor.azure.com/;ApplicationId=421044f1-5680-4fa5-8f34-72bbc3353ff3'
  }
};
