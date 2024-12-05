export interface AccessTokenResponse {
  tokenType: string;
  accessToken: string;
  expiresIn: number;
  refreshToken: string;
}

export interface LoginRequest {
  emailOrUserName: string;
  password: string;
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
}
