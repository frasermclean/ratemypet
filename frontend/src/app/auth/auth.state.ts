import { inject, Injectable } from '@angular/core';
import { Navigate } from '@ngxs/router-plugin';
import { Action, Selector, State, StateContext, StateToken } from '@ngxs/store';
import { NotificationService } from '@shared/services/notification.service';
import { catchError, finalize, tap } from 'rxjs';
import { AuthActions } from './auth.actions';
import { Role } from './auth.models';
import { AuthService } from './auth.service';

interface AuthStateModel {
  isBusy: boolean;
  error: any;
  userId: string | null;
  userName: string | null;
  emailAddress: string | null;
  emailHash: string | null;
  roles: Role[];
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
export class AuthState {
  private readonly authService = inject(AuthService);
  private readonly notificationService = inject(NotificationService);

  @Action(AuthActions.Login)
  login(context: StateContext<AuthStateModel>, action: AuthActions.Login) {
    context.patchState({ isBusy: true });
    return this.authService.login(action.request).pipe(
      tap((response) => {
        context.patchState(response);
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
        const user = response.user;
        if (user) {
          context.patchState(user);
        }
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
  static userId(state: AuthStateModel) {
    return state.userId;
  }

  @Selector([AUTH_STATE_TOKEN])
  static userName(state: AuthStateModel) {
    return state.userName;
  }

  @Selector([AUTH_STATE_TOKEN])
  static emailHash(state: AuthStateModel) {
    return state.emailHash;
  }

  @Selector([AUTH_STATE_TOKEN])
  static roles(state: AuthStateModel) {
    return state.roles;
  }

  @Selector([AUTH_STATE_TOKEN])
  static isAdministrator(state: AuthStateModel) {
    return state.roles.includes(Role.Administrator);
  }
}
