import { HttpErrorResponse, HttpStatusCode } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { Navigate } from '@ngxs/router-plugin';
import { Action, NgxsOnInit, Selector, State, StateContext, StateToken } from '@ngxs/store';
import { NotificationService } from '@shared/services/notification.service';
import { TelemetryService } from '@shared/services/telemetry.service';
import { catchError, finalize, of, tap } from 'rxjs';
import { AuthActions } from './auth.actions';
import { AuthService } from './auth.service';

interface AuthStateModel {
  isBusy: boolean;
  error: any;
  userId: string | null;
  userName: string | null;
  emailAddress: string | null;
  emailHash: string | null;
  roles: string[];
}

const AUTH_STATE_TOKEN = new StateToken<AuthStateModel>('auth');

@State<AuthStateModel>({
  name: AUTH_STATE_TOKEN,
  defaults: {
    isBusy: false,
    error: null,
    userId: null,
    userName: null,
    emailAddress: null,
    emailHash: null,
    roles: []
  }
})
@Injectable()
export class AuthState implements NgxsOnInit {
  private readonly authService = inject(AuthService);
  private readonly telemetryService = inject(TelemetryService);
  private readonly notificationService = inject(NotificationService);

  ngxsOnInit(context: StateContext<AuthStateModel>): void {
    const userId = context.getState().userId;
    if (userId) {
      context.dispatch(new AuthActions.VerifyUser());
    }
  }

  @Action(AuthActions.Login)
  login(context: StateContext<AuthStateModel>, action: AuthActions.Login) {
    context.patchState({ isBusy: true });
    return this.authService.login(action.request).pipe(
      tap((response) => {
        context.patchState(response);
        this.notificationService.showInformation(`Welcome back, ${response.userName}!`);
        this.telemetryService.setTrackedUser(response.userId);
        context.dispatch(new Navigate(['/']));
      }),
      catchError((error: HttpErrorResponse) => {
        context.patchState({ error });
        if (error?.status === HttpStatusCode.Unauthorized) {
          this.notificationService.showError('Invalid username or password.');
          return of(null);
        }
        throw error;
      }),
      finalize(() => {
        context.patchState({ isBusy: false });
      })
    );
  }

  @Action(AuthActions.Logout)
  logout(context: StateContext<AuthStateModel>) {
    context.patchState({ isBusy: true });
    return this.authService.logout().pipe(
      tap(() => {
        context.patchState({
          userId: null,
          userName: null,
          emailAddress: null,
          emailHash: null,
          roles: []
        });
        this.telemetryService.clearTrackedUser();
        this.notificationService.showInformation('You have been logged out.');
      }),
      finalize(() => {
        context.patchState({ isBusy: false });
      })
    );
  }

  @Action(AuthActions.VerifyUser)
  verifyUser(context: StateContext<AuthStateModel>) {
    context.patchState({ isBusy: true });
    return this.authService.verifyUser().pipe(
      tap((response) => {
        context.patchState(response);
        this.telemetryService.setTrackedUser(response.userId);
      }),
      catchError((error) => {
        context.patchState({
          error: error,
          userId: null,
          userName: null,
          emailAddress: null,
          emailHash: null,
          roles: []
        });
        return of(null);
      }),
      finalize(() => {
        context.patchState({ isBusy: false });
      })
    );
  }

  @Action(AuthActions.Register)
  register(context: StateContext<AuthStateModel>, action: AuthActions.Register) {
    context.patchState({ isBusy: true });
    return this.authService.register(action.request).pipe(
      tap(() => {
        this.notificationService.showInformation(
          'Registration successful. Please check your email for a confirmation link.'
        );
        context.dispatch(new Navigate(['/']));
      }),
      catchError((error) => {
        this.notificationService.showError('An error occured while trying to register.');
        context.patchState({ error });
        throw error;
      }),
      finalize(() => {
        context.patchState({ isBusy: false });
      })
    );
  }

  @Action(AuthActions.ConfirmEmail)
  confirmEmail(context: StateContext<AuthStateModel>, action: AuthActions.ConfirmEmail) {
    context.patchState({ isBusy: true });
    return this.authService.confirmEmail(action.request).pipe(
      tap(() => {
        this.notificationService.showInformation('Email address confirmed.');
        context.dispatch(new Navigate(['/auth/login']));
      }),
      catchError((error) => {
        this.notificationService.showError('An error occured while trying to confirm your email address.');
        context.patchState({ error });
        context.dispatch(new Navigate(['/']));
        throw error;
      }),
      finalize(() => {
        context.patchState({ isBusy: false });
      })
    );
  }

  @Action(AuthActions.ForgotPassword)
  forgotPassword(context: StateContext<AuthStateModel>, action: AuthActions.ForgotPassword) {
    context.patchState({ isBusy: true });
    return this.authService.forgotPassword(action.emailAddress).pipe(
      tap(() => {
        this.notificationService.showInformation('Password reset instructions have been sent to your email address.');
      }),
      catchError((error) => {
        this.notificationService.showError('An error occurred while trying to reset your password.');
        context.patchState({ error });
        throw error;
      }),
      finalize(() => {
        context.patchState({ isBusy: false });
      })
    );
  }

  @Action(AuthActions.ResetPassword)
  resetPassword(context: StateContext<AuthStateModel>, action: AuthActions.ResetPassword) {
    context.patchState({ isBusy: true });
    return this.authService.resetPassword(action.request).pipe(
      tap(() => {
        this.notificationService.showInformation('Password reset successful.');
        context.dispatch(new Navigate(['/auth/login']));
      }),
      catchError((error) => {
        this.notificationService.showError('An error occurred while trying to reset your password.');
        context.patchState({ error });
        throw error;
      }),
      finalize(() => {
        context.patchState({ isBusy: false });
      })
    );
  }

  @Selector([AUTH_STATE_TOKEN])
  static isBusy(state: AuthStateModel) {
    return state.isBusy;
  }

  @Selector([AUTH_STATE_TOKEN])
  static isLoggedIn(state: AuthStateModel) {
    return !!state.userId;
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
