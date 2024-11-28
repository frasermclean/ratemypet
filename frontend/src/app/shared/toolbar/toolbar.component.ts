import { Component, inject, Input } from '@angular/core';
import { AsyncPipe } from '@angular/common';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatToolbarModule } from '@angular/material/toolbar';
import { Router, RouterLink } from '@angular/router';
import { Store } from '@ngxs/store';
import { AuthState } from '../../auth/auth.state';
import { AuthActions } from '../../auth/auth.actions';
import { UserMenuComponent } from "./user-menu/user-menu.component";

@Component({
  selector: 'app-toolbar',
  standalone: true,
  imports: [
    AsyncPipe,
    MatIconModule,
    MatButtonModule,
    MatProgressSpinnerModule,
    MatToolbarModule,
    RouterLink,
    UserMenuComponent
],
  templateUrl: './toolbar.component.html',
  styleUrl: './toolbar.component.scss',
})
export class ToolbarComponent {
  store = inject(Store);
  router = inject(Router);
  @Input() title: string = 'Rate My Pet';
  status$ = this.store.select(AuthState.status);
}
