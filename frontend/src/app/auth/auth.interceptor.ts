import { HttpInterceptorFn } from '@angular/common/http';
import { environment } from '../../environments/environment';

export const authInterceptor: HttpInterceptorFn = (request, next) => {
  // ensure the request is for the API
  if (!request.url.startsWith(environment.apiBaseUrl)) {
    return next(request);
  }

  // add the `withCredentials` option to the request to include cookies
  const newRequest = request.clone({
    withCredentials: true
  });

  return next(newRequest);
};
