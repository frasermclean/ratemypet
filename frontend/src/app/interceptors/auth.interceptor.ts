import { HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { Store } from '@ngxs/store';
import { AuthState } from '../auth/auth.state';
import { environment } from '../../environments/environment';

export const authInterceptor: HttpInterceptorFn = (request, next) => {
  // ensure the request is for the API
  if (!request.url.startsWith(environment.apiBaseUrl)) {
    return next(request);
  }

  const store = inject(Store);

  // get the access token from the store
  const accessToken = store.selectSnapshot(AuthState.accessToken);
  if (!accessToken) {
    return next(request);
  }

  // clone the request and add the Authorization header
  const newRequest = request.clone({
    headers: request.headers.append('Authorization', `Bearer ${accessToken}`),
  });

  return next(newRequest);
};
