import { Component, inject, input } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatToolbarModule } from '@angular/material/toolbar';
import { Router, RouterLink } from '@angular/router';
import { select, Store } from '@ngxs/store';
import { AuthState } from '../../auth/auth.state';
import { UserMenuComponent } from './user-menu/user-menu.component';

@Component({
  selector: 'app-toolbar',
  imports: [MatIconModule, MatButtonModule, MatProgressSpinnerModule, MatToolbarModule, RouterLink, UserMenuComponent],
  templateUrl: './toolbar.component.html',
  styleUrl: './toolbar.component.scss'
})
export class ToolbarComponent {
  title = input('Rate My Pet');
  store = inject(Store);
  router = inject(Router);
  isBusy = select(AuthState.isBusy);
  isLoggedIn = select(AuthState.isLoggedIn);
}
