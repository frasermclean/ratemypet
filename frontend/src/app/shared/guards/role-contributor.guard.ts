import { inject } from '@angular/core';
import { ActivatedRouteSnapshot, CanActivateChildFn, RouterStateSnapshot } from '@angular/router';
import { Store } from '@ngxs/store';
import { Role } from '../../auth/auth.models';
import { AuthState } from '../../auth/auth.state';

export const isRoleContributor: CanActivateChildFn = (next: ActivatedRouteSnapshot, state: RouterStateSnapshot) => {
  const store = inject(Store);
  const roles = store.selectSnapshot(AuthState.roles);

  return roles.includes(Role.Contributor);
};
