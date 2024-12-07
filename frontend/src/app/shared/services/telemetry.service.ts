import { inject, Injectable } from '@angular/core';
import { Router } from '@angular/router';
import { AngularPlugin } from '@microsoft/applicationinsights-angularplugin-js';
import { ApplicationInsights } from '@microsoft/applicationinsights-web';
import { environment } from '../../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class TelemetryService {
  private readonly router = inject(Router);
  private readonly angularPlugin = new AngularPlugin();
  private readonly appInsights = new ApplicationInsights({
    config: {
      connectionString: environment.applicationInsights.connectionString,
      extensions: [this.angularPlugin],
      extensionConfig: {
        [this.angularPlugin.identifier]: { router: this.router }
      },
      enableCorsCorrelation: true,
      enableRequestHeaderTracking: true,
      enableResponseHeaderTracking: true
    }
  });

  public initialize(): void {
    this.appInsights.loadAppInsights();
    this.appInsights.addTelemetryInitializer((item) => {
      item.tags = item.tags ?? [];
      item.tags['ai.cloud.role'] = 'frontend';
    });
  }
}
