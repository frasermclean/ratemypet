import { HttpClient, HttpHeaders } from '@angular/common/http';
import { inject, Injectable, signal } from '@angular/core';
import { environment } from '../../environments/environment';
import { tap, finalize } from 'rxjs';

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
  private readonly baseUrl = `${environment.apiBaseUrl}/auth`;

  private readonly _isBusy = signal(false);
  public readonly isBusy = this._isBusy.asReadonly();

  private readonly _isLoggedIn = signal(false);
  public readonly isLoggedIn = this._isLoggedIn.asReadonly();

  public login(email: string, password: string) {
    this._isBusy.set(true);
    return this.httpClient.post<AccessTokenResponse>(`${this.baseUrl}/login`, { email, password }).pipe(
      tap((response) => this.processAccessTokenResponse(response)),
      finalize(() => this._isBusy.set(false))
    );
  }

  public logout() {
    this._isBusy.set(true);
    const headers = new HttpHeaders().set('Authorization', `Bearer ${localStorage.getItem('accessToken')}`);
    return this.httpClient.post(`${this.baseUrl}/logout`, null, { headers }).pipe(
      tap(() => {
        localStorage.removeItem('accessToken');
        localStorage.removeItem('accessTokenExpiry');
        localStorage.removeItem('refreshToken');
        this._isLoggedIn.set(false);
      }),
      finalize(() => this._isBusy.set(false))
    );
  }

  private refreshToken(refreshToken: string) {
    return this.httpClient.post<AccessTokenResponse>(`${this.baseUrl}/refresh`, { refreshToken }).pipe(
      tap((response) => {
        this.processAccessTokenResponse(response);
      })
    );
  }

  private processAccessTokenResponse(response: AccessTokenResponse) {
    const accessTokenExpiry = new Date();
    accessTokenExpiry.setSeconds(accessTokenExpiry.getSeconds() + response.expiresIn);
    localStorage.setItem('accessToken', response.accessToken);
    localStorage.setItem('accessTokenExpiry', accessTokenExpiry.toISOString());
    localStorage.setItem('refreshToken', response.refreshToken);
    this._isLoggedIn.set(true);
  }
}
