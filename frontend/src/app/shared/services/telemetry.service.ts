import { inject, Injectable } from '@angular/core';
import { AngularPlugin } from '@microsoft/applicationinsights-angularplugin-js';
import { ApplicationInsights } from '@microsoft/applicationinsights-web';
import { RouterNavigated } from '@ngxs/router-plugin';
import { Actions, ofActionSuccessful, Store } from '@ngxs/store';
import { SharedActions } from '@shared/shared.actions';
import { SharedState } from '@shared/shared.state';
import { environment } from '../../../environments/environment';
import { AuthActions } from '../../auth/auth.actions';
import { AuthState } from '../../auth/auth.state';

@Injectable({
  providedIn: 'root'
})
export class TelemetryService {
  private readonly actions$ = inject(Actions);
  private readonly store = inject(Store);

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

    this.registerPageTrackingEventHandlers();
    this.registerUserContextHandlers();
  }

  private registerPageTrackingEventHandlers(): void {
    // start tracking page view on page title change
    this.actions$.pipe(ofActionSuccessful(SharedActions.SetPageTitle)).subscribe(() => {
      const name = this.store.selectSnapshot(SharedState.pageTitle);
      this.appInsights.startTrackPage(name);
    });

    // stop tracking page view on router navigated event
    this.actions$.pipe(ofActionSuccessful(RouterNavigated)).subscribe(() => {
      const name = this.store.selectSnapshot(SharedState.pageTitle);
      const url = this.store.selectSnapshot(SharedState.pageUrl);
      this.appInsights.stopTrackPage(name, url);
    });
  }

  private registerUserContextHandlers(): void {
    // track authenticated user id on login and verify user
    this.actions$.pipe(ofActionSuccessful(AuthActions.Login, AuthActions.VerifyUser)).subscribe(() => {
      const userId = this.store.selectSnapshot(AuthState.userId)!; // should not be null as the user is authenticated
      this.appInsights.setAuthenticatedUserContext(userId, undefined, true);
    });

    // clear user id on logout
    this.actions$
      .pipe(ofActionSuccessful(AuthActions.Logout))
      .subscribe(() => this.appInsights.clearAuthenticatedUserContext());
  }
}
