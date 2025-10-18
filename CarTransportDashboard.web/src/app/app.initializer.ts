// src/app/app.initializer.ts
import { inject } from '@angular/core';
import { AuthService } from './services/auth/auth';
import { UserStoreService } from './services/auth/user-store-service';
import { catchError, of, tap } from 'rxjs';

export function initializeSession() {
  const auth = inject(AuthService);
  const userStore = inject(UserStoreService);

  return auth.refresh().pipe(
    tap(user => {
      if (user) userStore.setUser(user);
    }),
    catchError(() => {
      userStore.clearUser();
      return of(null);
    })
  );
}

