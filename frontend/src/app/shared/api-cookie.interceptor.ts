import { HttpEvent, HttpHandlerFn, HttpRequest, HttpResponse } from '@angular/common/http';
import { inject, REQUEST } from '@angular/core';
import { VerifyUserResponse } from '@auth/auth.models';
import { Observable, of } from 'rxjs';

export function apiCookieInterceptor(
  request: HttpRequest<unknown>,
  next: HttpHandlerFn
): Observable<HttpEvent<unknown>> {
  const serverRequest = inject(REQUEST);

  // skip the interceptor for non-API requests or if running on the client side
  if (!serverRequest || !request.url.startsWith('/api/')) {
    return next(request);
  }

  // get the cookie from the server request
  const cookie = serverRequest.headers.get('cookie');

  if (!cookie) {
    // return synthetic response for verify-user endpoint if no cookie is present
    if (request.url.endsWith('/auth/verify-user')) {
      return of(ANONYMOUS_RESPONSE);
    }
    return next(request);
  }

  // clone the request and set the cookie header
  const clonedRequest = request.clone({
    setHeaders: { cookie }
  });

  return next(clonedRequest);
}

const ANONYMOUS_RESPONSE = new HttpResponse<VerifyUserResponse>({
  status: 200,
  body: {
    isAuthenticated: false,
    user: null
  }
});
