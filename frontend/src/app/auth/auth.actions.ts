import { RegisterRequest } from '@services/auth.service';

export namespace AuthActions {
  export class Login {
    static readonly type = '[Auth] Login';
    constructor(public email: string, public password: string) {}
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
    constructor(public userId: string, public token: string) {}
  }

  export class RefreshAccessToken {
    static readonly type = '[Auth] Refresh Access Token';
    constructor(public refreshToken: string) {}
  }
}
