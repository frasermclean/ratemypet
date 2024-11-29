import { APP_INITIALIZER, ApplicationConfig, provideZoneChangeDetection } from '@angular/core';
import { provideHttpClient, withFetch, withInterceptors } from '@angular/common/http';
import { DomSanitizer } from '@angular/platform-browser';
import { provideAnimationsAsync } from '@angular/platform-browser/animations/async';
import { provideRouter, withComponentInputBinding } from '@angular/router';
import { MatIconRegistry } from '@angular/material/icon';
import { withNgxsRouterPlugin } from '@ngxs/router-plugin';
import { withNgxsStoragePlugin } from '@ngxs/storage-plugin';
import { provideStore } from '@ngxs/store';

import { routes } from './app.routes';
import { AuthState } from './auth/auth.state';
import { authInterceptor } from './interceptors/auth.interceptor';
import { PostsState } from './posts/posts.state';
import { environment } from '../environments/environment';

export const appConfig: ApplicationConfig = {
  providers: [
    provideZoneChangeDetection({ eventCoalescing: true }),
    provideRouter(routes, withComponentInputBinding()),
    provideHttpClient(withFetch(), withInterceptors([authInterceptor])),
    provideAnimationsAsync(),
    provideStore(
      [AuthState, PostsState],
      { developmentMode: environment.name === 'development' },
      withNgxsRouterPlugin(),
      withNgxsStoragePlugin({
        keys: ['auth.refreshToken', 'auth.emailAddress'],
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
  // register SVG icons
  iconRegistry.addSvgIconResolver((name, namespace) => {
    const path = `icons/${namespace}/${name}.svg`;
    return sanitizer.bypassSecurityTrustResourceUrl(path);
  });
}
