import { HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { AuthService } from '../services/auth/auth';
import { UserStoreService } from '../services/auth/user-store-service';
import {catchError, delay, switchMap, throwError} from 'rxjs';
import { filter, take } from 'rxjs/operators';

function isTokenExpired(token: string): boolean {
  try {
    const payload = JSON.parse(atob(token.split('.')[1]));
    return Date.now() >= payload.exp * 1000;
  } catch {
    return true;
  }
}

export const authInterceptor: HttpInterceptorFn = (req, next) => {
  const authService = inject(AuthService);
  const userStore = inject(UserStoreService);
  const accessToken = userStore.currentUser?.accessToken;


  const authReq = accessToken
    ? req.clone({
      headers: req.headers.set('Authorization', `Bearer ${accessToken}`),
      withCredentials: true
    })
    : req.clone({ withCredentials: true });

  //console.log('Intercepting request:', req.url);
  //console.log('Access Token:', accessToken);

  return next(authReq)
    .pipe(
    catchError(err => {
      if (err.status === 401 && accessToken && !isTokenExpired(accessToken)) {
        console.warn('401 detected despite valid token—attempting refresh...');
        return authService.refresh().pipe(
          switchMap(user => {
            const retryReq = req.clone({
              headers: req.headers.set('Authorization', `Bearer ${user.accessToken}`),
              withCredentials: true
            });
            return next(retryReq);
          })
        );
      }

      return throwError(() => err);
    })
  );
};


