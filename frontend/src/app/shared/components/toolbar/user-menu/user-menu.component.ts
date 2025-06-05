import { isPlatformBrowser } from '@angular/common';
import { ChangeDetectionStrategy, Component, inject, OnInit, PLATFORM_ID } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatMenuModule } from '@angular/material/menu';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { RouterLink } from '@angular/router';
import { AuthActions } from '@auth/auth.actions';
import { Role } from '@auth/auth.models';
import { AuthState } from '@auth/auth.state';
import { Actions, dispatch, ofActionSuccessful, select } from '@ngxs/store';
import { GravatarComponent } from '@shared/components/gravatar/gravatar.component';
import { NotificationService } from '@shared/services/notification.service';

@Component({
  changeDetection: ChangeDetectionStrategy.OnPush,
  selector: 'user-menu',
  imports: [RouterLink, MatButtonModule, MatIconModule, MatMenuModule, MatProgressSpinnerModule, GravatarComponent],
  templateUrl: './user-menu.component.html',
  styleUrl: './user-menu.component.scss'
})
export class UserMenuComponent implements OnInit {
  private readonly platformId = inject(PLATFORM_ID);
  protected readonly emailHash = select(AuthState.emailHash);
  protected readonly userRoles = select(AuthState.roles);
  protected readonly isBusy = select(AuthState.isBusy);
  protected readonly isLoggedIn = select(AuthState.isLoggedIn);
  protected readonly verifyUser = dispatch(AuthActions.VerifyUser);
  protected readonly logout = dispatch(AuthActions.Logout);
  protected readonly contributor = Role.Contributor;

  constructor(actions$: Actions, notificationService: NotificationService) {
    actions$.pipe(ofActionSuccessful(AuthActions.Logout), takeUntilDestroyed()).subscribe(() => {
      notificationService.showInformation('You have been logged out.');
    });
  }

  ngOnInit() {
    // verify user on initialization if running in the browser
    if (isPlatformBrowser(this.platformId)) {
      this.verifyUser();
    }
  }
}
