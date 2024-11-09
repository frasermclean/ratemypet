import {
  APP_INITIALIZER,
  ApplicationConfig,
  inject,
  PLATFORM_ID,
  provideZoneChangeDetection,
} from '@angular/core';
import { isPlatformServer } from '@angular/common';
import { provideHttpClient, withFetch } from '@angular/common/http';
import {
  DomSanitizer,
  provideClientHydration,
} from '@angular/platform-browser';
import { provideAnimationsAsync } from '@angular/platform-browser/animations/async';
import { provideRouter, withComponentInputBinding } from '@angular/router';
import { MatIconRegistry } from '@angular/material/icon';

import { routes } from './app.routes';

export const appConfig: ApplicationConfig = {
  providers: [
    provideZoneChangeDetection({ eventCoalescing: true }),
    provideRouter(routes, withComponentInputBinding()),
    provideHttpClient(withFetch()),
    provideClientHydration(),
    provideAnimationsAsync(),
    {
      provide: APP_INITIALIZER,
      useFactory: initializeIcons,
      deps: [MatIconRegistry, DomSanitizer, PLATFORM_ID],
    },
  ],
};

function initializeIcons(
  iconRegistry: MatIconRegistry,
  sanitizer: DomSanitizer
) {
  const platformId = inject(PLATFORM_ID);
  iconRegistry.addSvgIconResolver((name, namespace) => {
    const baseUrl = isPlatformServer(platformId)
      ? 'http://localhost:4200/'
      : '';
    const path = `icons/${namespace}/${name}.svg`;

    return sanitizer.bypassSecurityTrustResourceUrl(baseUrl + path);
  });
}
