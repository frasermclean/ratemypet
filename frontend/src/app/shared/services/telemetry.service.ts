import { inject, Injectable } from '@angular/core';
import { ActivatedRouteSnapshot, ResolveEnd, Router } from '@angular/router';
import { AngularPlugin } from '@microsoft/applicationinsights-angularplugin-js';
import { ApplicationInsights } from '@microsoft/applicationinsights-web';
import { filter } from 'rxjs';
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

    // enable page view tracking with active component name
    this.router.events.pipe(filter((event): event is ResolveEnd => event instanceof ResolveEnd)).subscribe((event) => {
      const activatedComponent = this.getActivatedComponent(event.state.root);
      if (activatedComponent) {
        this.appInsights.trackPageView({
          name: activatedComponent.name,
          uri: event.urlAfterRedirects
        });
      }
    });
  }

  private getActivatedComponent(snapshot: ActivatedRouteSnapshot): any {
    if (snapshot.firstChild) {
      return this.getActivatedComponent(snapshot.firstChild);
    }
    return snapshot.component;
  }
}
