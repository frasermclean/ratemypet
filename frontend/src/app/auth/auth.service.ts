import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { environment } from '../../environments/environment';
import { ConfirmEmailRequest, LoginRequest, LoginResponse, RegisterRequest, ResetPasswordRequest } from './auth.models';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private readonly httpClient = inject(HttpClient);
  private readonly baseUrl = `${environment.apiBaseUrl}/api/auth`;

  public login(request: LoginRequest) {
    return this.httpClient.post<LoginResponse>(`${this.baseUrl}/login`, request);
  }

  public logout() {
    return this.httpClient.post(`${this.baseUrl}/logout`, null);
  }

  public verifyUser() {
    return this.httpClient.get<LoginResponse>(`${this.baseUrl}/verify-user`);
  }

  public register(request: RegisterRequest) {
    return this.httpClient.post(`${this.baseUrl}/register`, request);
  }

  public confirmEmail(request: ConfirmEmailRequest) {
    return this.httpClient.post(`${this.baseUrl}/confirm-email`, request);
  }

  public forgotPassword(emailAddress: string) {
    return this.httpClient.post(`${this.baseUrl}/forgot-password`, { emailAddress });
  }

  public resetPassword(request: ResetPasswordRequest) {
    return this.httpClient.post(`${this.baseUrl}/reset-password`, request);
  }
}
