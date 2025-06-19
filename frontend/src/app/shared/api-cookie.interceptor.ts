import { isPlatformServer } from '@angular/common';
import { HttpEvent, HttpHandlerFn, HttpRequest } from '@angular/common/http';
import { inject, PLATFORM_ID, REQUEST } from '@angular/core';
import { Observable } from 'rxjs';

export function apiCookieInterceptor(
  request: HttpRequest<unknown>,
  next: HttpHandlerFn
): Observable<HttpEvent<unknown>> {
  const platformId = inject(PLATFORM_ID);

  // skip the interceptor for non-API requests or if running on the client side
  if (!request.url.startsWith('/api/') || !isPlatformServer(platformId)) {
    return next(request);
  }

  // get the cookie from the server request
  const serverRequest = inject(REQUEST);
  const cookie = serverRequest?.headers.get('cookie') || '';
  if (!cookie) {
    return next(request);
  }

  // clone the request and set the cookie header
  const clonedRequest = request.clone({
    setHeaders: { cookie }
  });

  return next(clonedRequest);
}
