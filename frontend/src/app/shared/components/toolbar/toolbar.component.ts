import { Component, inject, input, output } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatToolbarModule } from '@angular/material/toolbar';
import { Router, RouterLink } from '@angular/router';
import { AuthState } from '@auth/auth.state';
import { select, Store } from '@ngxs/store';
import { UserMenuComponent } from './user-menu/user-menu.component';

@Component({
  selector: 'app-toolbar',
  imports: [MatIconModule, MatButtonModule, MatProgressSpinnerModule, MatToolbarModule, RouterLink, UserMenuComponent],
  templateUrl: './toolbar.component.html',
  styleUrl: './toolbar.component.scss'
})
export class ToolbarComponent {
  title = input.required<string>();
  menuClicked = output<void>();
  store = inject(Store);
  router = inject(Router);
  isBusy = select(AuthState.isBusy);
  isLoggedIn = select(AuthState.isLoggedIn);
}
