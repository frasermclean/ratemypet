import { inject, Injectable } from '@angular/core';
import { Action, NgxsOnInit, Selector, State, StateContext } from '@ngxs/store';

import { AuthActions } from './auth.actions';
import { AuthService } from '@services/auth.service';
import { catchError, tap } from 'rxjs';

interface AuthStateModel {
  isBusy: boolean;
  error: any;
  accessToken: string | null;
  accessTokenExpiry: Date | null;
  refreshToken: string | null;
}

@State<AuthStateModel>({
  name: 'auth',
  defaults: {
    isBusy: false,
    error: null,
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
    }
  }

  @Action(AuthActions.Login)
  login(context: StateContext<AuthStateModel>, action: AuthActions.Login) {
    context.patchState({ isBusy: true });
    return this.authService.login(action.email, action.password).pipe(
      tap((response) => {
        const accessTokenExpiry = new Date();
        accessTokenExpiry.setSeconds(accessTokenExpiry.getSeconds() + response.expiresIn);
        context.patchState({
          isBusy: false,
          accessToken: response.accessToken,
          accessTokenExpiry,
          refreshToken: response.refreshToken,
        });
      }),
      catchError((error) => {
        context.patchState({ isBusy: false, error });
        return error;
      })
    );
  }

  @Action(AuthActions.Logout)
  logout(context: StateContext<AuthStateModel>) {
    context.patchState({ isBusy: true });
    return this.authService.logout().pipe(
      tap(() => {
        context.patchState({
          isBusy: false,
          accessToken: null,
          accessTokenExpiry: null,
          refreshToken: null,
        });
      })
    );
  }

  @Action(AuthActions.RefreshAccessToken)
  refreshAccessToken(context: StateContext<AuthStateModel>, action: AuthActions.RefreshAccessToken) {
    context.patchState({ isBusy: true });
    return this.authService.refreshAccessToken(action.refreshToken).pipe(
      tap((response) => {
        const accessTokenExpiry = new Date();
        accessTokenExpiry.setSeconds(accessTokenExpiry.getSeconds() + response.expiresIn);
        context.patchState({
          isBusy: false,
          accessToken: response.accessToken,
          accessTokenExpiry,
          refreshToken: response.refreshToken,
        });
      }),
      catchError((error) => {
        context.patchState({ isBusy: false, error });
        return error;
      })
    );
  }

  @Selector()
  static isBusy(state: AuthStateModel): boolean {
    return state.isBusy;
  }

  @Selector()
  static isLoggedIn(state: AuthStateModel): boolean {
    if (!state.accessToken || !state.accessTokenExpiry) {
      return false;
    }

    return state.accessTokenExpiry > new Date();
  }

  @Selector()
  static accessToken(state: AuthStateModel): string | null {
    return state.accessToken;
  }
}
