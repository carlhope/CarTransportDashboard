import {CanActivateFn, Router} from '@angular/router';
import {inject} from '@angular/core';
import {UserStoreService} from '../../services/auth/user-store-service';

export const authGuard: CanActivateFn = (route, state) => {
  const userStore = inject(UserStoreService);
  const router = inject(Router);

  if (userStore.isLoggedIn) {
    return true;
  } else {
    router.navigate(['/login']);
    return false;
  };
};
