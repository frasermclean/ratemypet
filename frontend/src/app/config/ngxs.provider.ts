import { RouterStateSerializer, withNgxsRouterPlugin } from '@ngxs/router-plugin';
import { provideStore } from '@ngxs/store';
import { AppRouterStateSerializer } from '@shared/router-state-serializer';
import { SharedState } from '@shared/shared.state';
import { environment } from '../../environments/environment';
import { AuthState } from '../auth/auth.state';
import { SpeciesState } from '../species/species.state';

export function provideNgxsStore() {
  return [
    provideStore(
      [AuthState, SharedState, SpeciesState],
      { developmentMode: environment.name === 'development' },
      ...[withNgxsRouterPlugin(), ...environment.ngxsPlugins]
    ),
    {
      provide: RouterStateSerializer,
      useClass: AppRouterStateSerializer
    }
  ];
}

export default provideNgxsStore;
