import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { environment } from '../../environments/environment';
import { CurrentUser } from './users.models';

@Injectable({
  providedIn: 'root'
})
export class UsersService {
  private readonly httpClient = inject(HttpClient);
  private readonly baseUrl = `${environment.apiBaseUrl}/users`;

  public getCurrentUser() {
    return this.httpClient.get<CurrentUser>(`${this.baseUrl}/me`);
  }
}
