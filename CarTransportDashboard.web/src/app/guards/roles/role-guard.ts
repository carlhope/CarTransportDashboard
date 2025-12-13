import { CanActivateFn, Router } from '@angular/router';
import { inject } from '@angular/core';
import { UserStoreService } from '../../services/auth/user-store-service';


export const roleGuard: CanActivateFn = (route, state) => {
  const userStore = inject(UserStoreService);
  const router = inject(Router);

  const expectedRoles = route.data['roles'] as string[];

  if (!userStore.isLoggedIn) {
    router.navigate(['/login']);
    return false;
  }

  if (expectedRoles.some(role => userStore.hasRole(role))) {
    return true;
  } else {
    router.navigate(['/access-denied']);
    return false;
  }


};
