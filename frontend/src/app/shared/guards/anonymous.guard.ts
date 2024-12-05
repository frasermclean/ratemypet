import { inject } from '@angular/core';
import { ActivatedRouteSnapshot, CanActivateChildFn, RouterStateSnapshot } from '@angular/router';
import { Store } from '@ngxs/store';
import { AuthState } from '../../auth/auth.state';

export const isAnonymous: CanActivateChildFn = (next: ActivatedRouteSnapshot, state: RouterStateSnapshot) => {
  const store = inject(Store);
  const isLoggedIn = store.selectSnapshot(AuthState.isLoggedIn);

  return !isLoggedIn;
};
