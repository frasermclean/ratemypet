import { EnvironmentProviders } from '@angular/core';

export interface Environment {
  name: 'development' | 'testing' | 'production';
  apiBaseUrl: string;
  applicationInsights: {
    connectionString: string;
  };
  ngxsPlugins: EnvironmentProviders[];
}
