import {
  ApplicationConfig,
  ErrorHandler,
  inject,
  provideAppInitializer,
  provideZoneChangeDetection
} from '@angular/core';
import { provideHttpClient, withFetch, withInterceptors } from '@angular/common/http';
import { DomSanitizer } from '@angular/platform-browser';
import { provideAnimationsAsync } from '@angular/platform-browser/animations/async';
import { provideRouter, TitleStrategy, withComponentInputBinding } from '@angular/router';
import { MatIconRegistry } from '@angular/material/icon';
import { MAT_SNACK_BAR_DEFAULT_OPTIONS } from '@angular/material/snack-bar';
import { withNgxsLoggerPlugin } from '@ngxs/logger-plugin';
import { withNgxsRouterPlugin } from '@ngxs/router-plugin';
import { withNgxsStoragePlugin } from '@ngxs/storage-plugin';
import { provideStore } from '@ngxs/store';

import { routes } from './app.routes';
import { AuthState } from './auth/auth.state';
import { authInterceptor } from './auth/auth.interceptor';
import { environment } from '../environments/environment';
import { GlobalErrorHandler } from './errors/global-error-handler';
import { AppTitleStrategy } from './core/app-title-strategy.service';

export const appConfig: ApplicationConfig = {
  providers: [
    provideZoneChangeDetection({ eventCoalescing: true }),
    provideRouter(routes, withComponentInputBinding()),
    provideHttpClient(withFetch(), withInterceptors([authInterceptor])),
    provideAnimationsAsync(),
    provideAppInitializer(initializeApp),
    provideStore(
      [AuthState],
      { developmentMode: environment.name === 'development' },
      withNgxsRouterPlugin(),
      withNgxsStoragePlugin({
        keys: ['auth.refreshToken', 'auth.emailAddress']
      }),
      withNgxsLoggerPlugin({
        disabled: environment.name !== 'development'
      })
    ),
    {
      provide: TitleStrategy,
      useClass: AppTitleStrategy
    },
    {
      provide: ErrorHandler,
      useClass: GlobalErrorHandler
    },
    {
      provide: MAT_SNACK_BAR_DEFAULT_OPTIONS,
      useValue: { duration: 5000 }
    }
  ]
};

function initializeApp() {
  const iconRegistry = inject(MatIconRegistry);
  const sanitizer = inject(DomSanitizer);

  iconRegistry.addSvgIconResolver((name, namespace) => {
    const path = `icons/${namespace}/${name}.svg`;
    return sanitizer.bypassSecurityTrustResourceUrl(path);
  });
}
