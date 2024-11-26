import { inject, Injectable } from '@angular/core';
import { Action, NgxsOnInit, Selector, State, StateContext } from '@ngxs/store';

import { AuthActions } from './auth.actions';
import { AuthService } from '@services/auth.service';
import { catchError, tap } from 'rxjs';

interface AuthStateModel {
  status: 'initial' | 'busy' | 'error' | 'loggedIn' | 'loggedOut';
  error: any;
  emailAddress: string | null;
  accessToken: string | null;
  accessTokenExpiry: Date | null;
  refreshToken: string | null;
}

@State<AuthStateModel>({
  name: 'auth',
  defaults: {
    status: 'initial',
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
        context.patchState({ status: 'error', error });
        return error;
      })
    );
  }

  @Action(AuthActions.Logout)
  logout(context: StateContext<AuthStateModel>) {
    context.patchState({ status: 'busy' });
    return this.authService.logout().pipe(
      tap(() => {
        context.patchState({
          status: 'loggedOut',
          emailAddress: null,
          accessToken: null,
          accessTokenExpiry: null,
          refreshToken: null,
        });
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
        context.patchState({ status: 'error', error });
        return error;
      })
    );
  }

  @Selector()
  static status(state: AuthStateModel) {
    return state.status;
  }

  @Selector()
  static emailAddress(state: AuthStateModel) {
    return state.emailAddress;
  }

  @Selector()
  static accessToken(state: AuthStateModel) {
    return state.accessToken;
  }
}
