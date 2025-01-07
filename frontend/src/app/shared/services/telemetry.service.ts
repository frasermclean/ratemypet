import { inject, Injectable } from '@angular/core';
import { ActivatedRouteSnapshot, ResolveEnd, Router } from '@angular/router';
import { AngularPlugin } from '@microsoft/applicationinsights-angularplugin-js';
import { ApplicationInsights } from '@microsoft/applicationinsights-web';
import { Actions, ofActionSuccessful, select } from '@ngxs/store';
import { filter } from 'rxjs';
import { environment } from '../../../environments/environment';
import { AuthActions } from '../../auth/auth.actions';
import { AuthState } from '../../auth/auth.state';

@Injectable({
  providedIn: 'root'
})
export class TelemetryService {
  private readonly actions$ = inject(Actions);
  private readonly router = inject(Router);
  private readonly userId = select(AuthState.userId);
  private readonly angularPlugin = new AngularPlugin();
  private readonly appInsights = new ApplicationInsights({
    config: {
      connectionString: environment.applicationInsights.connectionString,
      extensions: [this.angularPlugin],
      extensionConfig: {
        [this.angularPlugin.identifier]: {}
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

    this.registerRouterEventHandlers();
    this.registerUserContextHandlers();
  }

  private registerRouterEventHandlers(): void {
    // enable page view tracking with active component name
    this.router.events.pipe(filter((event): event is ResolveEnd => event instanceof ResolveEnd)).subscribe((event) => {
      const activatedComponent = getActivatedComponent(event.state.root);
      if (activatedComponent) {
        this.appInsights.trackPageView({
          name: activatedComponent.name,
          uri: event.urlAfterRedirects
        });
      }
    });

    function getActivatedComponent(snapshot: ActivatedRouteSnapshot): any {
      if (snapshot.firstChild) {
        return getActivatedComponent(snapshot.firstChild);
      }
      return snapshot.component;
    }
  }

  private registerUserContextHandlers(): void {
    // track authenticated user id on login and verify user
    this.actions$.pipe(ofActionSuccessful(AuthActions.Login, AuthActions.VerifyUser)).subscribe(() => {
      const userId = this.userId()!; // should not be null as the user is authenticated
      this.appInsights.setAuthenticatedUserContext(userId, undefined, true);
    });

    // clear user id on logout
    this.actions$
      .pipe(ofActionSuccessful(AuthActions.Logout))
      .subscribe(() => this.appInsights.clearAuthenticatedUserContext());
  }
}
