// src/app/app.initializer.ts
import { inject } from '@angular/core';
import { AuthService } from './services/auth/auth';
import { UserStoreService } from './services/auth/user-store-service';
import { SessionReadyService } from './services/auth/session-ready';
import { catchError, of, tap } from 'rxjs';

export function initializeSession() {
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
}

