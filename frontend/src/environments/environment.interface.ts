export interface Environment {
  name: 'development' | 'production';
  apiBaseUrl: string;
  applicationInsights: {
    connectionString: string;
  };
}
