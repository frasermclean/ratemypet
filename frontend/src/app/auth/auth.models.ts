export interface LoginRequest {
  emailOrUserName: string;
  password: string;
  rememberMe?: boolean;
}

export interface LoginResponse {
  id: string;
  userName: string;
  emailAddress: string;
  emailHash: string;
  roles: string[];
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

export interface CurrentUserResponse {
  id: string;
  userName: string;
  emailAddress: string;
  emailHash: string;
  roles: string[];
}

export interface ResetPasswordRequest {
  emailAddress: string;
  resetCode: string;
  newPassword: string;
}
