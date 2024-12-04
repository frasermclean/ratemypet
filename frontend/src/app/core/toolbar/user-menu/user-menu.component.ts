import { Component } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatMenuModule } from '@angular/material/menu';
import { dispatch, select } from '@ngxs/store';

import { AuthState } from '../../../auth/auth.state';
import { AuthActions } from '../../../auth/auth.actions';
import { GravatarComponent } from '@shared/gravatar/gravatar.component';
import { RouterLink } from '@angular/router';

@Component({
  selector: 'app-user-menu',
  standalone: true,
  imports: [RouterLink, MatButtonModule, MatIconModule, MatMenuModule, GravatarComponent],
  templateUrl: './user-menu.component.html',
  styleUrl: './user-menu.component.scss',
})
export class UserMenuComponent {
  emailAddress = select(AuthState.emailAddress);
  logout = dispatch(AuthActions.Logout);
}
