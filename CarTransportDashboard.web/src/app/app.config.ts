import {
  APP_INITIALIZER,
  ApplicationConfig,
  inject, Injector,
  provideBrowserGlobalErrorListeners,
  provideZoneChangeDetection
} from '@angular/core';
import { provideRouter } from '@angular/router';
import { authInterceptor } from './interceptors/auth-interceptor';
import { provideAppInitializer } from '@angular/core';

import { routes } from './app.routes';
import {provideHttpClient, withInterceptors} from '@angular/common/http';
import {AuthService} from './services/auth/auth';
import {UserStoreService} from './services/auth/user-store-service';
import {catchError, of, tap} from 'rxjs';
import {initializeSession} from './app.initializer';

export const appConfig: ApplicationConfig = {
  providers: [
    provideBrowserGlobalErrorListeners(),
    provideZoneChangeDetection({ eventCoalescing: true }),
    provideRouter(routes),
    provideHttpClient(withInterceptors([authInterceptor])),
    provideAppInitializer(() => {
      const injector = inject(Injector);
      return initializeSession(injector)();
    })


  ]
};
