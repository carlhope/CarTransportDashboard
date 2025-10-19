// src/app/app.initializer.ts
import { inject, Injector, runInInjectionContext } from '@angular/core';
import { AuthService } from './services/auth/auth';
import { UserStoreService } from './services/auth/user-store-service';
import {catchError, Observable, of, tap} from 'rxjs';
import {UserModel} from './models/user';

export function initializeSession(injector: Injector): () => Observable<UserModel | null> {
  return () =>
    runInInjectionContext(injector, () => {
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
    });
}
