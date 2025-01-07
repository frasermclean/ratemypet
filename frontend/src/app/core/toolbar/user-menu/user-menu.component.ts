import { Component } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatMenuModule } from '@angular/material/menu';
import { RouterLink } from '@angular/router';
import { Actions, dispatch, ofActionSuccessful, select } from '@ngxs/store';
import { GravatarComponent } from '@shared/components/gravatar/gravatar.component';
import { NotificationService } from '@shared/services/notification.service';
import { AuthActions } from '../../../auth/auth.actions';
import { Role } from '../../../auth/auth.models';
import { AuthState } from '../../../auth/auth.state';

@Component({
  selector: 'app-user-menu',
  imports: [RouterLink, MatButtonModule, MatIconModule, MatMenuModule, GravatarComponent],
  templateUrl: './user-menu.component.html',
  styleUrl: './user-menu.component.scss'
})
export class UserMenuComponent {
  emailHash = select(AuthState.emailHash);
  userRoles = select(AuthState.roles);
  logout = dispatch(AuthActions.Logout);
  contributor = Role.Contributor;

  constructor(actions$: Actions, notificationService: NotificationService) {
    actions$.pipe(ofActionSuccessful(AuthActions.Logout)).subscribe(() => {
      notificationService.showInformation('You have been logged out.');
    });
  }
}
