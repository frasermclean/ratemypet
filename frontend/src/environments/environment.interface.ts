import { EnvironmentProviders } from '@angular/core';

export interface Environment {
  name: 'development' | 'testing' | 'production';
  applicationInsights: {
    connectionString: string;
  };
  ngxsPlugins: EnvironmentProviders[];
}
