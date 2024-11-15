export namespace AuthActions {
  export class Login {
    static readonly type = '[Auth] Login';
    constructor(public email: string, public password: string) {}
  }

  export class Logout {
    static readonly type = '[Auth] Logout';
  }

  export class RefreshAccessToken {
    static readonly type = '[Auth] Refresh Access Token';
    constructor(public refreshToken: string) {}
  }
}
