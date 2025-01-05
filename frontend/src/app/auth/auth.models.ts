export interface LoginRequest {
  emailOrUserName: string;
  password: string;
  rememberMe?: boolean;
}

export interface LoginResponse {
  userId: string;
  userName: string;
  emailAddress: string;
  emailHash: string;
  roles: Role[];
}

export interface RegisterRequest {
  userName: string;
  emailAddress: string;
  password: string;
}

export interface ConfirmEmailRequest {
  userId: string;
  token: string;
}

export interface ResetPasswordRequest {
  emailAddress: string;
  resetCode: string;
  newPassword: string;
}

export enum Role {
  Contributor = 'Contributor',
  Administrator = 'Administrator'
}
