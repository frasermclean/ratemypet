export interface AccessTokenResponse {
  tokenType: string;
  accessToken: string;
  expiresIn: number;
  refreshToken: string;
}

export interface RegisterRequest {
  userName: string;
  emailAddress: string;
  password: string;
}
