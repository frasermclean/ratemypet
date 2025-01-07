import { provideHttpClient, withFetch, withInterceptors } from '@angular/common/http';
import { ApplicationConfig, ErrorHandler, provideAppInitializer, provideZoneChangeDetection } from '@angular/core';
import { MAT_SNACK_BAR_DEFAULT_OPTIONS } from '@angular/material/snack-bar';
import { provideAnimationsAsync } from '@angular/platform-browser/animations/async';
import { provideRouter, TitleStrategy, withComponentInputBinding } from '@angular/router';
import { ApplicationinsightsAngularpluginErrorService } from '@microsoft/applicationinsights-angularplugin-js';
import { RouterStateSerializer, withNgxsRouterPlugin } from '@ngxs/router-plugin';
import { withNgxsStoragePlugin } from '@ngxs/storage-plugin';
import { provideStore } from '@ngxs/store';
import { AppRouterStateSerializer } from '@shared/router-state-serializer';
import { PageTitleService } from '@shared/services/page-title.service';
import { SharedState } from '@shared/shared.state';
import { environment } from '../environments/environment';
import initializeApp from './app.initializer';
import { routes } from './app.routes';
import { authInterceptor } from './auth/auth.interceptor';
import { AuthState } from './auth/auth.state';

export const appConfig: ApplicationConfig = {
  providers: [
    provideZoneChangeDetection({ eventCoalescing: true }),
    provideRouter(routes, withComponentInputBinding()),
    provideHttpClient(withFetch(), withInterceptors([authInterceptor])),
    provideAnimationsAsync(),
    provideAppInitializer(initializeApp),
    provideStore(
      [AuthState, SharedState],
      { developmentMode: environment.name === 'development' },
      ...[
        withNgxsRouterPlugin(),
        withNgxsStoragePlugin({
          keys: ['auth.userId']
        }),
        ...environment.ngxsPlugins
      ]
    ),
    {
      provide: RouterStateSerializer,
      useClass: AppRouterStateSerializer
    },
    {
      provide: TitleStrategy,
      useClass: PageTitleService
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
