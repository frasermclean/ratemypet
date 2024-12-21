import { HttpErrorResponse, HttpStatusCode } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { Navigate } from '@ngxs/router-plugin';
import { Action, NgxsOnInit, Selector, State, StateContext, StateToken } from '@ngxs/store';
import { NotificationService } from '@shared/services/notification.service';
import { TelemetryService } from '@shared/services/telemetry.service';
import { catchError, of, tap } from 'rxjs';
import { AuthActions } from './auth.actions';
import { AuthService } from './auth.service';

interface AuthStateModel {
  status: 'loggedOut' | 'busy' | 'loggedIn';
  error: any;
  id?: string;
  userName?: string;
  emailAddress?: string;
  emailHash?: string;
  roles?: string[];
}

const AUTH_STATE_TOKEN = new StateToken<AuthStateModel>('auth');

@State<AuthStateModel>({
  name: AUTH_STATE_TOKEN,
  defaults: {
    status: 'loggedOut',
    error: null
  }
})
@Injectable()
export class AuthState implements NgxsOnInit {
  private readonly authService = inject(AuthService);
  private readonly telemetryService = inject(TelemetryService);
  private readonly notificationService = inject(NotificationService);

  ngxsOnInit(context: StateContext<AuthStateModel>): void {
    const status = context.getState().status;
    if (status === 'loggedIn') {
      context.dispatch(new AuthActions.VerifyUser());
    }
  }

  @Action(AuthActions.Login)
  login(context: StateContext<AuthStateModel>, action: AuthActions.Login) {
    context.patchState({ status: 'busy' });
    return this.authService.login(action.request).pipe(
      tap((response) => {
        context.patchState({ status: 'loggedIn', ...response });
        this.notificationService.showInformation(`Welcome back, ${response.userName}!`);
        this.telemetryService.setTrackedUser(response.id);
        context.dispatch(new Navigate(['/']));
      }),
      catchError((error: HttpErrorResponse) => {
        context.patchState({ status: 'loggedOut', error });
        if (error?.status === HttpStatusCode.Unauthorized) {
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
        context.patchState({ status: 'loggedOut' });
      })
    );
  }

  @Action(AuthActions.VerifyUser)
  verifyUser(context: StateContext<AuthStateModel>) {
    context.patchState({ status: 'busy' });
    return this.authService.verifyUser().pipe(
      tap((response) => {
        context.patchState({ status: 'loggedIn', ...response });
        this.telemetryService.setTrackedUser(response.id);
      }),
      catchError((error) => {
        context.patchState({ status: 'loggedOut', error });
        throw error;
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

  @Action(AuthActions.ResetPassword)
  resetPassword(context: StateContext<AuthStateModel>, action: AuthActions.ResetPassword) {
    context.patchState({ status: 'busy' });
    return this.authService.resetPassword(action.request).pipe(
      tap(() => {
        this.notificationService.showInformation('Password reset successful.');
        context.patchState({ status: 'loggedOut' });
        context.dispatch(new Navigate(['/auth/login']));
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
  static userName(state: AuthStateModel) {
    return state.userName;
  }

  @Selector([AUTH_STATE_TOKEN])
  static emailHash(state: AuthStateModel) {
    return state.emailHash;
  }
}
