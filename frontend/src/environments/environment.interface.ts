import { EnvironmentProviders } from '@angular/core';

export interface Environment {
  name: 'development' | 'production';
  apiBaseUrl: string;
  applicationInsights: {
    connectionString: string;
  };
  ngxsPlugins: EnvironmentProviders[];
}
