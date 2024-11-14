import { APP_INITIALIZER, ApplicationConfig, inject, PLATFORM_ID, provideZoneChangeDetection } from '@angular/core';
import { isPlatformServer } from '@angular/common';
import { provideHttpClient, withFetch, withInterceptors, withInterceptorsFromDi } from '@angular/common/http';
import { DomSanitizer, provideClientHydration } from '@angular/platform-browser';
import { provideAnimationsAsync } from '@angular/platform-browser/animations/async';
import { provideRouter, withComponentInputBinding } from '@angular/router';
import { MatIconRegistry } from '@angular/material/icon';
import { withNgxsStoragePlugin } from '@ngxs/storage-plugin';
import { provideStore, withNgxsDevelopmentOptions } from '@ngxs/store';

import { routes } from './app.routes';
import { AuthState } from './auth/auth.state';
import { authInterceptor } from './interceptors/auth.interceptor';

export const appConfig: ApplicationConfig = {
  providers: [
    provideZoneChangeDetection({ eventCoalescing: true }),
    provideRouter(routes, withComponentInputBinding()),
    provideHttpClient(withFetch(), withInterceptors([authInterceptor])),
    provideClientHydration(),
    provideAnimationsAsync(),
    provideStore(
      [AuthState],
      withNgxsStoragePlugin({
        keys: ['auth.refreshToken'],
      })
    ),
    {
      provide: APP_INITIALIZER,
      useFactory: initializeApp,
      deps: [MatIconRegistry, DomSanitizer],
    },
  ],
};

function initializeApp(iconRegistry: MatIconRegistry, sanitizer: DomSanitizer) {
  const platformId = inject(PLATFORM_ID);
  const isServer = isPlatformServer(platformId);
  const baseUrl = isServer ? 'http://localhost:4200/' : '';

  // register SVG icons
  iconRegistry.addSvgIconResolver((name, namespace) => {
    const path = `icons/${namespace}/${name}.svg`;
    return sanitizer.bypassSecurityTrustResourceUrl(baseUrl + path);
  });
}
