import { provideHttpClient, withFetch, withInterceptors } from '@angular/common/http';
import {
  ApplicationConfig,
  ErrorHandler,
  provideAppInitializer,
  provideExperimentalZonelessChangeDetection
} from '@angular/core';
import { provideClientHydration, withEventReplay } from '@angular/platform-browser';
import { provideAnimationsAsync } from '@angular/platform-browser/animations/async';
import { provideRouter, TitleStrategy, withComponentInputBinding } from '@angular/router';
import { ApplicationinsightsAngularpluginErrorService } from '@microsoft/applicationinsights-angularplugin-js';
import { apiCookieInterceptor } from '@shared/api-cookie.interceptor';
import { PageTitleService } from '@shared/services/page-title.service';
import initializeApp from './app.initializer';
import { routes } from './app.routes';
import provideNgxsStore from './config/ngxs.provider';
import provideUiDefaults from './config/ui-defaults.provider';

export const appConfig: ApplicationConfig = {
  providers: [
    provideExperimentalZonelessChangeDetection(),
    provideRouter(routes, withComponentInputBinding()),
    provideHttpClient(withFetch(), withInterceptors([apiCookieInterceptor])),
    provideClientHydration(withEventReplay()),
    provideAnimationsAsync(),
    provideAppInitializer(initializeApp),
    provideUiDefaults(),
    provideNgxsStore(),
    {
      provide: TitleStrategy,
      useClass: PageTitleService
    },
    {
      provide: ErrorHandler,
      useClass: ApplicationinsightsAngularpluginErrorService
    }
  ]
};
