import { inject, Injectable } from '@angular/core';
import { Action, NgxsOnInit, Selector, State, StateContext, StateToken } from '@ngxs/store';

import { AuthActions } from './auth.actions';
import { AuthService } from '@services/auth.service';
import { catchError, of, tap } from 'rxjs';
import { Navigate } from '@ngxs/router-plugin';
import { MatSnackBar } from '@angular/material/snack-bar';

interface AuthStateModel {
  status: 'loggedOut' | 'busy' | 'loggedIn';
  error: any;
  emailAddress: string | null;
  accessToken: string | null;
  accessTokenExpiry: Date | null;
  refreshToken: string | null;
}

const AUTH_STATE_TOKEN = new StateToken<AuthStateModel>('auth');

@State<AuthStateModel>({
  name: AUTH_STATE_TOKEN,
  defaults: {
    status: 'loggedOut',
    error: null,
    emailAddress: null,
    accessToken: null,
    accessTokenExpiry: null,
    refreshToken: null,
  },
})
@Injectable()
export class AuthState implements NgxsOnInit {
  private readonly authService = inject(AuthService);
  private readonly snackBar = inject(MatSnackBar);

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
    context.patchState({ status: 'busy', emailAddress: action.email });
    return this.authService.login(action.email, action.password).pipe(
      tap((response) => {
        const accessTokenExpiry = new Date();
        accessTokenExpiry.setSeconds(accessTokenExpiry.getSeconds() + response.expiresIn);
        context.patchState({
          status: 'loggedIn',
          accessToken: response.accessToken,
          accessTokenExpiry,
          refreshToken: response.refreshToken,
        });
      }),
      catchError((error) => {
        context.patchState({ status: 'loggedOut', error });
        throw error;
      })
    );
  }

  @Action(AuthActions.Logout)
  logout(context: StateContext<AuthStateModel>) {
    context.patchState({ status: 'busy' });
    return this.authService.logout().pipe(
      tap(() => {
        this.snackBar.open('You have been logged out.', 'Close');
        context.patchState({
          status: 'loggedOut',
          emailAddress: null,
          accessToken: null,
          accessTokenExpiry: null,
          refreshToken: null,
        });
        context.dispatch(new Navigate(['/auth/login']));
      })
    );
  }

  @Action(AuthActions.Register)
  register(context: StateContext<AuthStateModel>, action: AuthActions.Register) {
    context.patchState({ status: 'busy' });
    return this.authService.register(action.request).pipe(
      tap(() => {
        this.snackBar.open('Registration successful. Please check your email for a confirmation link.', 'Close');
        context.patchState({ status: 'loggedOut' });
        context.dispatch(new Navigate(['/auth/login']));
      }),
      catchError((error) => {
        this.snackBar.open('An error occured while trying to register.', 'Close');
        context.patchState({ status: 'loggedOut', error });
        throw error;
      })
    );
  }

  @Action(AuthActions.ConfirmEmail)
  confirmEmail(context: StateContext<AuthStateModel>, action: AuthActions.ConfirmEmail) {
    context.patchState({ status: 'busy' });
    return this.authService.confirmEmail(action.userId, action.token).pipe(
      tap(() => {
        this.snackBar.open('Email address confirmed.', 'Close');
        context.patchState({ status: 'loggedOut' });
        context.dispatch(new Navigate(['/auth/login']));
      }),
      catchError((error) => {
        this.snackBar.open('An error occured while trying to confirm your email address.', 'Close');
        context.patchState({ status: 'loggedOut', error });
        context.dispatch(new Navigate(['/']));
        return of([]);
      })
    );
  }

  @Action(AuthActions.RefreshAccessToken)
  refreshAccessToken(context: StateContext<AuthStateModel>, action: AuthActions.RefreshAccessToken) {
    context.patchState({ status: 'busy' });
    return this.authService.refreshAccessToken(action.refreshToken).pipe(
      tap((response) => {
        const accessTokenExpiry = new Date();
        accessTokenExpiry.setSeconds(accessTokenExpiry.getSeconds() + response.expiresIn);
        context.patchState({
          status: 'loggedIn',
          accessToken: response.accessToken,
          accessTokenExpiry,
          refreshToken: response.refreshToken,
        });
      }),
      catchError((error) => {
        context.patchState({ error });
        throw error;
      })
    );
  }

  @Selector([AUTH_STATE_TOKEN])
  static status(state: AuthStateModel) {
    return state.status;
  }

  @Selector([AUTH_STATE_TOKEN])
  static emailAddress(state: AuthStateModel) {
    return state.emailAddress;
  }

  @Selector([AUTH_STATE_TOKEN])
  static accessToken(state: AuthStateModel) {
    return state.accessToken;
  }
}
