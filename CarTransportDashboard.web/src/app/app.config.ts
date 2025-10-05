import {ApplicationConfig, inject, provideBrowserGlobalErrorListeners, provideZoneChangeDetection} from '@angular/core';
import { provideRouter } from '@angular/router';
import { authInterceptor } from './interceptors/auth-interceptor';
import { provideAppInitializer } from '@angular/core';

import { routes } from './app.routes';
import {provideHttpClient, withInterceptors} from '@angular/common/http';
import {AuthService} from './services/auth/auth';
import {UserStoreService} from './services/auth/user-store-service';
import {catchError, of, tap} from 'rxjs';
import {SessionReadyService} from './services/auth/session-ready';

export const appConfig: ApplicationConfig = {
  providers: [
    provideBrowserGlobalErrorListeners(),
    provideZoneChangeDetection({ eventCoalescing: true }),
    provideRouter(routes),
    provideHttpClient(withInterceptors([authInterceptor])),
    provideAppInitializer(() => {
      const auth = inject(AuthService);
      const userStore = inject(UserStoreService);
      const sessionReady = inject(SessionReadyService);

      return auth.refresh().pipe(
        tap(user => {
          if (user) userStore.setUser(user);
          sessionReady.markReady();
        }),
        catchError(() => {
          userStore.clearUser();
          sessionReady.markReady();
          return of(null);
        })
      );
    })


  ]
};
