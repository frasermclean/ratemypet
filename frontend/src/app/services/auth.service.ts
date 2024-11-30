import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { environment } from '../../environments/environment';

interface AccessTokenResponse {
  tokenType: string;
  accessToken: string;
  expiresIn: number;
  refreshToken: string;
}

@Injectable({
  providedIn: 'root',
})
export class AuthService {
  private readonly httpClient = inject(HttpClient);
  private readonly baseUrl = `${environment.apiBaseUrl}/api/auth`;

  public login(email: string, password: string) {
    return this.httpClient.post<AccessTokenResponse>(`${this.baseUrl}/login`, { email, password });
  }

  public logout() {
    return this.httpClient.post(`${this.baseUrl}/logout`, null);
  }

  public refreshAccessToken(refreshToken: string) {
    return this.httpClient.post<AccessTokenResponse>(`${this.baseUrl}/refresh-token`, { refreshToken });
  }
}
