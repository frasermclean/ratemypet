import { Component } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatMenuModule } from '@angular/material/menu';
import { dispatch, select } from '@ngxs/store';

import { RouterLink } from '@angular/router';
import { GravatarComponent } from '@shared/components/gravatar/gravatar.component';
import { AuthActions } from '../../../auth/auth.actions';
import { AuthState } from '../../../auth/auth.state';

@Component({
  selector: 'app-user-menu',
  imports: [RouterLink, MatButtonModule, MatIconModule, MatMenuModule, GravatarComponent],
  templateUrl: './user-menu.component.html',
  styleUrl: './user-menu.component.scss'
})
export class UserMenuComponent {
  emailHash = select(AuthState.emailHash);
  logout = dispatch(AuthActions.Logout);
}
