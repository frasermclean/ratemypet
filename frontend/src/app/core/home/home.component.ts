import { Component } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { RouterLink } from '@angular/router';
import { select } from '@ngxs/store';
import { Role } from '../../auth/auth.models';
import { AuthState } from '../../auth/auth.state';

@Component({
  selector: 'app-home',
  imports: [RouterLink, MatButtonModule],
  templateUrl: './home.component.html',
  styleUrl: './home.component.scss'
})
export class HomeComponent {
  userName = select(AuthState.userName);
  userRoles = select(AuthState.roles);

  contributor = Role.Contributor;
}
