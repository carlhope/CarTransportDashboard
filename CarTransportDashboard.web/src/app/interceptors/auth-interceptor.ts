import { HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { AuthService } from '../services/auth/auth';
import { UserStoreService } from '../services/auth/user-store-service';
import { catchError, switchMap, throwError } from 'rxjs';

export const authInterceptor: HttpInterceptorFn = (req, next) => {
  const authService = inject(AuthService);
  const userStore = inject(UserStoreService);

  const accessToken = userStore.currentUser?.accessToken;
  const csrfToken = userStore.csrfToken;

  let headers = req.headers;
  if (accessToken) {
    headers = headers.set('Authorization', `Bearer ${accessToken}`);
  }
  if (csrfToken) {
    headers = headers.set('X-CSRF-Token', csrfToken);
  }

  const authReq = req.clone({
    headers,
    withCredentials: true
  });

  return next(authReq).pipe(
    catchError(err => {
      const alreadyRetried = req.headers.has('x-retried');


      if (err.status === 401 && !alreadyRetried && !req.url.includes('/refresh')) {
        console.warn('401 detected — attempting refresh...');
        return authService.refresh().pipe(
          switchMap(user => {
            if (user) {
              userStore.setUser(user);

              let retryHeaders = req.headers.set('x-retried', 'true');
              if (user.accessToken) {
                retryHeaders = retryHeaders.set('Authorization', `Bearer ${user.accessToken}`);
              }
              if (user.csrfToken) {
                retryHeaders = retryHeaders.set('X-CSRF-Token', user.csrfToken);
              }

              const retryReq = req.clone({
                headers: retryHeaders,
                withCredentials: true
              });
              return next(retryReq);
            }

            userStore.clearUser();
            return throwError(() => err);
          }),
          catchError(refreshErr => {
            userStore.clearUser();
            return throwError(() => refreshErr);
          })
        );
      }

      return throwError(() => err);
    })
  );
};


