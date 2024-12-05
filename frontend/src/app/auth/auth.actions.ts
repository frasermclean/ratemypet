import { ConfirmEmailRequest, LoginRequest, RegisterRequest } from './auth.models';

export namespace AuthActions {
  export class Login {
    static readonly type = '[Auth] Login';
    constructor(public request: LoginRequest) {}
  }

  export class Logout {
    static readonly type = '[Auth] Logout';
  }

  export class Register {
    static readonly type = '[Auth] Register';
    constructor(public request: RegisterRequest) {}
  }

  export class ConfirmEmail {
    static readonly type = '[Auth] Confirm Email';
    constructor(public request: ConfirmEmailRequest) {}
  }

  export class RefreshAccessToken {
    static readonly type = '[Auth] Refresh Access Token';
    constructor(public refreshToken: string) {}
  }
}
