import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { environment } from '../../environments/environment';

interface AccessTokenResponse {
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

@Injectable({
  providedIn: 'root',
})
export class AuthService {
  private readonly httpClient = inject(HttpClient);
  private readonly baseUrl = `${environment.apiBaseUrl}/auth`;

  public login(emailOrPassword: string, password: string) {
    return this.httpClient.post<AccessTokenResponse>(`${this.baseUrl}/login`, { emailOrPassword, password });
  }

  public logout() {
    return this.httpClient.post(`${this.baseUrl}/logout`, null);
  }

  public register(request: RegisterRequest) {
    return this.httpClient.post(`${this.baseUrl}/register`, request);
  }

  public confirmEmail(userId: string, token: string) {
    return this.httpClient.post(`${this.baseUrl}/confirm-email`, { userId, token });
  }

  public refreshAccessToken(refreshToken: string) {
    return this.httpClient.post<AccessTokenResponse>(`${this.baseUrl}/refresh-token`, { refreshToken });
  }
}
