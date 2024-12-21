import { ConfirmEmailRequest, LoginRequest, RegisterRequest, ResetPasswordRequest } from './auth.models';

export namespace AuthActions {
  export class Login {
    static readonly type = '[Auth] Login';
    constructor(public request: LoginRequest) {}
  }

  export class Logout {
    static readonly type = '[Auth] Logout';
  }

  export class VerifyUser {
    static readonly type = '[Auth] Verify User';
  }

  export class Register {
    static readonly type = '[Auth] Register';
    constructor(public request: RegisterRequest) {}
  }

  export class ConfirmEmail {
    static readonly type = '[Auth] Confirm Email';
    constructor(public request: ConfirmEmailRequest) {}
  }

  export class ForgotPassword {
    static readonly type = '[Auth] Forgot Password';
    constructor(public emailAddress: string) {}
  }

  export class ResetPassword {
    static readonly type = '[Auth] Reset Password';
    constructor(public request: ResetPasswordRequest) {}
  }
}
