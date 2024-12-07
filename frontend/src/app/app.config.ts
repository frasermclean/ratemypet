import { provideHttpClient, withFetch, withInterceptors } from '@angular/common/http';
import {
  ApplicationConfig,
  ErrorHandler,
  inject,
  provideAppInitializer,
  provideZoneChangeDetection
} from '@angular/core';
import { MatIconRegistry } from '@angular/material/icon';
import { MAT_SNACK_BAR_DEFAULT_OPTIONS } from '@angular/material/snack-bar';
import { DomSanitizer } from '@angular/platform-browser';
import { provideAnimationsAsync } from '@angular/platform-browser/animations/async';
import { provideRouter, TitleStrategy, withComponentInputBinding } from '@angular/router';
import { ApplicationinsightsAngularpluginErrorService } from '@microsoft/applicationinsights-angularplugin-js';
import { withNgxsLoggerPlugin } from '@ngxs/logger-plugin';
import { withNgxsRouterPlugin } from '@ngxs/router-plugin';
import { withNgxsStoragePlugin } from '@ngxs/storage-plugin';
import { provideStore } from '@ngxs/store';
import { TelemetryService } from '@shared/services/telemetry.service';
import { environment } from '../environments/environment';
import { routes } from './app.routes';
import { authInterceptor } from './auth/auth.interceptor';
import { AuthState } from './auth/auth.state';
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
      useClass: ApplicationinsightsAngularpluginErrorService
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
  const telemetryService = inject(TelemetryService);

  iconRegistry.addSvgIconResolver((name, namespace) => {
    const path = `icons/${namespace}/${name}.svg`;
    return sanitizer.bypassSecurityTrustResourceUrl(path);
  });

  telemetryService.initialize();
}
