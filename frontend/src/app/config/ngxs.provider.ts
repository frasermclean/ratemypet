import { RouterStateSerializer, withNgxsRouterPlugin } from '@ngxs/router-plugin';
import { withNgxsStoragePlugin } from '@ngxs/storage-plugin';
import { provideStore } from '@ngxs/store';
import { AppRouterStateSerializer } from '@shared/router-state-serializer';
import { SharedState } from '@shared/shared.state';
import { environment } from '../../environments/environment';
import { AuthState } from '../auth/auth.state';

export function provideNgxsStore() {
  return [
    provideStore(
      [AuthState, SharedState],
      { developmentMode: environment.name === 'development' },
      ...[
        withNgxsRouterPlugin(),
        withNgxsStoragePlugin({
          keys: ['auth.userId']
        }),
        ...environment.ngxsPlugins
      ]
    ),
    {
      provide: RouterStateSerializer,
      useClass: AppRouterStateSerializer
    }
  ];
}

export default provideNgxsStore;
