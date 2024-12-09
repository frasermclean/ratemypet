import { inject, Injectable } from '@angular/core';
import { Navigate } from '@ngxs/router-plugin';
import { Action, NgxsOnInit, Selector, State, StateContext, StateToken } from '@ngxs/store';
import { NotificationService } from '@shared/services/notification.service';
import { TelemetryService } from '@shared/services/telemetry.service';
import { catchError, of, switchMap, tap } from 'rxjs';
import { AuthActions } from './auth.actions';
import { CurrentUserResponse } from './auth.models';
import { AuthService } from './auth.service';

interface AuthStateModel {
  status: 'loggedOut' | 'busy' | 'loggedIn';
  error: any;
  accessToken: string | null;
  accessTokenExpiry: Date | null;
  refreshToken: string | null;
  currentUser: CurrentUserResponse | null;
}

const AUTH_STATE_TOKEN = new StateToken<AuthStateModel>('auth');

@State<AuthStateModel>({
  name: AUTH_STATE_TOKEN,
  defaults: {
    status: 'loggedOut',
    error: null,
    accessToken: null,
    accessTokenExpiry: null,
    refreshToken: null,
    currentUser: null
  }
})
@Injectable()
export class AuthState implements NgxsOnInit {
  private readonly authService = inject(AuthService);
  private readonly telemetryService = inject(TelemetryService);
  private readonly notificationService = inject(NotificationService);

  ngxsOnInit(context: StateContext<AuthStateModel>): void {
    const refreshToken = context.getState().refreshToken;
    if (refreshToken) {
      context.dispatch(new AuthActions.RefreshAccessToken(refreshToken));
    } else {
      context.patchState({ status: 'loggedOut' });
    }
  }

  @Action(AuthActions.Login)
  login(context: StateContext<AuthStateModel>, action: AuthActions.Login) {
    context.patchState({ status: 'busy' });
    return this.authService.login(action.request).pipe(
      switchMap((response) => {
        const accessTokenExpiry = new Date();
        accessTokenExpiry.setSeconds(accessTokenExpiry.getSeconds() + response.expiresIn);
        context.patchState({
          accessToken: response.accessToken,
          accessTokenExpiry,
          refreshToken: response.refreshToken
        });
        return this.authService.getCurrentUser();
      }),
      tap((currentUser) => {
        context.patchState({ status: 'loggedIn', currentUser });
        this.notificationService.showInformation(`Welcome back, ${currentUser.userName}!`);
        this.telemetryService.setTrackedUser(currentUser.id);
        context.dispatch(new Navigate(['/']));
      }),
      catchError((error) => {
        context.patchState({ status: 'loggedOut', error });
        if (error?.status === 401) {
          this.notificationService.showError('Invalid username or password.');
          return of(null);
        }
        throw error;
      })
    );
  }

  @Action(AuthActions.Logout)
  logout(context: StateContext<AuthStateModel>) {
    context.patchState({ status: 'busy' });
    return this.authService.logout().pipe(
      tap(() => {
        this.notificationService.showInformation('You have been logged out.');
        this.telemetryService.clearTrackedUser();
        context.patchState({
          status: 'loggedOut',
          accessToken: null,
          accessTokenExpiry: null,
          refreshToken: null,
          currentUser: null
        });
      })
    );
  }

  @Action(AuthActions.Register)
  register(context: StateContext<AuthStateModel>, action: AuthActions.Register) {
    context.patchState({ status: 'busy' });
    return this.authService.register(action.request).pipe(
      tap(() => {
        this.notificationService.showInformation(
          'Registration successful. Please check your email for a confirmation link.'
        );
        context.patchState({ status: 'loggedOut' });
        context.dispatch(new Navigate(['/auth/login']));
      }),
      catchError((error) => {
        this.notificationService.showError('An error occured while trying to register.');
        context.patchState({ status: 'loggedOut', error });
        throw error;
      })
    );
  }

  @Action(AuthActions.ConfirmEmail)
  confirmEmail(context: StateContext<AuthStateModel>, action: AuthActions.ConfirmEmail) {
    context.patchState({ status: 'busy' });
    return this.authService.confirmEmail(action.request).pipe(
      tap(() => {
        this.notificationService.showInformation('Email address confirmed.');
        context.patchState({ status: 'loggedOut' });
        context.dispatch(new Navigate(['/auth/login']));
      }),
      catchError((error) => {
        this.notificationService.showError('An error occured while trying to confirm your email address.');
        context.patchState({ status: 'loggedOut', error });
        context.dispatch(new Navigate(['/']));
        throw error;
      })
    );
  }

  @Action(AuthActions.RefreshAccessToken)
  refreshAccessToken(context: StateContext<AuthStateModel>, action: AuthActions.RefreshAccessToken) {
    context.patchState({ status: 'busy' });
    return this.authService.refreshAccessToken(action.refreshToken).pipe(
      switchMap((response) => {
        const accessTokenExpiry = new Date();
        accessTokenExpiry.setSeconds(accessTokenExpiry.getSeconds() + response.expiresIn);
        context.patchState({
          accessToken: response.accessToken,
          accessTokenExpiry,
          refreshToken: response.refreshToken
        });
        return this.authService.getCurrentUser();
      }),
      tap((currentUser) => {
        context.patchState({
          status: 'loggedIn',
          currentUser
        });
        this.telemetryService.setTrackedUser(currentUser.id);
      }),
      catchError((error) => {
        context.patchState({
          status: 'loggedOut',
          error,
          accessToken: null,
          accessTokenExpiry: null,
          refreshToken: null
        });
        this.telemetryService.clearTrackedUser();
        this.notificationService.showError('An error occurred, and you have been logged out.');
        throw error;
      })
    );
  }

  @Action(AuthActions.ForgotPassword)
  forgotPassword(context: StateContext<AuthStateModel>, action: AuthActions.ForgotPassword) {
    context.patchState({ status: 'busy' });
    return this.authService.forgotPassword(action.emailAddress).pipe(
      tap(() => {
        this.notificationService.showInformation('Password reset instructions have been sent to your email address.');
        context.patchState({ status: 'loggedOut' });
      }),
      catchError((error) => {
        this.notificationService.showError('An error occurred while trying to reset your password.');
        context.patchState({ status: 'loggedOut', error });
        throw error;
      })
    );
  }

  @Selector([AUTH_STATE_TOKEN])
  static status(state: AuthStateModel) {
    return state.status;
  }

  @Selector([AUTH_STATE_TOKEN])
  static isLoggedIn(state: AuthStateModel) {
    return state.status === 'loggedIn';
  }

  @Selector([AUTH_STATE_TOKEN])
  static accessToken(state: AuthStateModel) {
    return state.accessToken;
  }

  @Selector([AUTH_STATE_TOKEN])
  static currentUser(state: AuthStateModel) {
    return state.currentUser;
  }
}
