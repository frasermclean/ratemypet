import { APP_INITIALIZER, ApplicationConfig, inject, PLATFORM_ID, provideZoneChangeDetection } from '@angular/core';
import { isPlatformServer } from '@angular/common';
import { provideHttpClient, withFetch } from '@angular/common/http';
import { DomSanitizer, provideClientHydration } from '@angular/platform-browser';
import { provideAnimationsAsync } from '@angular/platform-browser/animations/async';
import { provideRouter, withComponentInputBinding } from '@angular/router';
import { MatIconRegistry } from '@angular/material/icon';

import { routes } from './app.routes';
import { AuthService } from '@services/auth.service';

export const appConfig: ApplicationConfig = {
  providers: [
    provideZoneChangeDetection({ eventCoalescing: true }),
    provideRouter(routes, withComponentInputBinding()),
    provideHttpClient(withFetch()),
    provideClientHydration(),
    provideAnimationsAsync(),
    {
      provide: APP_INITIALIZER,
      useFactory: initializeApp,
      deps: [MatIconRegistry, DomSanitizer, AuthService],
    },
  ],
};

function initializeApp(iconRegistry: MatIconRegistry, sanitizer: DomSanitizer, authService: AuthService) {
  const platformId = inject(PLATFORM_ID);
  const isServer = isPlatformServer(platformId);
  const baseUrl = isServer ? 'http://localhost:4200/' : '';

  // register SVG icons
  iconRegistry.addSvgIconResolver((name, namespace) => {
    const path = `icons/${namespace}/${name}.svg`;
    return sanitizer.bypassSecurityTrustResourceUrl(baseUrl + path);
  });

  if (!isServer) {
    authService.initialize();
  }
}
