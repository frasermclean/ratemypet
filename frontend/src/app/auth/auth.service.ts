import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { environment } from '../../environments/environment';
import {
  AccessTokenResponse,
  ConfirmEmailRequest,
  CurrentUserResponse,
  LoginRequest,
  RegisterRequest,
  ResetPasswordRequest
} from './auth.models';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private readonly httpClient = inject(HttpClient);
  private readonly baseUrl = `${environment.apiBaseUrl}/auth`;

  public login(request: LoginRequest) {
    return this.httpClient.post<AccessTokenResponse>(`${this.baseUrl}/login`, request);
  }

  public logout() {
    return this.httpClient.post(`${this.baseUrl}/logout`, null);
  }

  public register(request: RegisterRequest) {
    return this.httpClient.post(`${this.baseUrl}/register`, request);
  }

  public confirmEmail(request: ConfirmEmailRequest) {
    return this.httpClient.post(`${this.baseUrl}/confirm-email`, request);
  }

  public refreshAccessToken(refreshToken: string) {
    return this.httpClient.post<AccessTokenResponse>(`${this.baseUrl}/refresh-token`, { refreshToken });
  }

  public getCurrentUser() {
    return this.httpClient.get<CurrentUserResponse>(`${this.baseUrl}/current-user`);
  }

  public forgotPassword(emailAddress: string) {
    return this.httpClient.post(`${this.baseUrl}/forgot-password`, { emailAddress });
  }

  public resetPassword(request: ResetPasswordRequest) {
    return this.httpClient.post(`${this.baseUrl}/reset-password`, request);
  }
}
