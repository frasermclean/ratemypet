import { Component, inject } from '@angular/core';
import { AsyncPipe } from '@angular/common';
import { MatMenuModule } from '@angular/material/menu';
import { MatIconModule } from '@angular/material/icon';
import { Store } from '@ngxs/store';

import { AuthState } from '../../../auth/auth.state';
import { AuthActions } from '../../../auth/auth.actions';
import { GravatarComponent } from '../../gravatar/gravatar.component';
import { MatButtonModule } from '@angular/material/button';

@Component({
  selector: 'app-user-menu',
  standalone: true,
  imports: [AsyncPipe, MatButtonModule, MatIconModule, MatMenuModule, GravatarComponent],
  templateUrl: './user-menu.component.html',
  styleUrl: './user-menu.component.scss',
})
export class UserMenuComponent {
  store = inject(Store);
  emailAddress$ = this.store.select(AuthState.emailAddress);

  logout() {
    this.store.dispatch(new AuthActions.Logout());
  }
}
