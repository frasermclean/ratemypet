import { HttpClient, HttpHeaders } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { environment } from '../../environments/environment';
import { tap, finalize } from 'rxjs';

interface LoginResponse {
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
  private readonly baseUrl = `${environment.apiBaseUrl}/auth`;

  login(email: string, password: string) {
    const options = { withCredentials: true };
    return this.httpClient.post<LoginResponse>(`${this.baseUrl}/login`, { email, password, options }).pipe(
      tap((response) => {
        let accessTokenExpiry = new Date();
        accessTokenExpiry.setSeconds(accessTokenExpiry.getSeconds() + response.expiresIn);
        localStorage.setItem('accessToken', response.accessToken);
        localStorage.setItem('accessTokenExpiry', accessTokenExpiry.toISOString());
        localStorage.setItem('refreshToken', response.refreshToken);
      })
    );
  }

  logout() {
    const headers = new HttpHeaders().set('Authorization', `Bearer ${localStorage.getItem('accessToken')}`);
    return this.httpClient.post(`${this.baseUrl}/logout`, null, { headers }).pipe(
      finalize(() => {
        localStorage.removeItem('accessToken');
        localStorage.removeItem('accessTokenExpiry');
        localStorage.removeItem('refreshToken');
      })
    );
  }
}
