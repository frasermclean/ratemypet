import { Component, inject, Input } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatToolbarModule } from '@angular/material/toolbar';
import { Router, RouterLink } from '@angular/router';
import { AuthService } from '@services/auth.service';

@Component({
  selector: 'app-toolbar',
  standalone: true,
  imports: [MatIconModule, MatButtonModule, MatProgressSpinnerModule, MatToolbarModule, RouterLink],
  templateUrl: './toolbar.component.html',
  styleUrl: './toolbar.component.scss',
})
export class ToolbarComponent {
  authService = inject(AuthService);
  router = inject(Router);
  @Input() title: string = 'Rate My Pet';
  isBusy = this.authService.isBusy;
  isLoggedIn = this.authService.isLoggedIn;

  logout() {
    this.authService.logout().subscribe();
  }
}
