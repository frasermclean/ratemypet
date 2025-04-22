import { Component } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatTooltipModule } from '@angular/material/tooltip';
import { RouterLink } from '@angular/router';
import { select } from '@ngxs/store';
import { Role } from '../../auth/auth.models';
import { AuthState } from '../../auth/auth.state';
import { PostsState } from '../posts.state';
import { PostItemComponent } from './post-list-item/post-list-item.component';
import { PostSearchControlsComponent } from './post-search-controls/post-search-controls.component';

@Component({
  imports: [
    RouterLink,
    MatButtonModule,
    MatIconModule,
    MatProgressSpinnerModule,
    MatTooltipModule,
    PostItemComponent,
    PostSearchControlsComponent
  ],
  templateUrl: './post-list.component.html',
  styleUrl: './post-list.component.scss'
})
export class PostListComponent {
  readonly isBusy = select(PostsState.isBusy);
  readonly matches = select(PostsState.matches);
  readonly userRoles = select(AuthState.roles);
  readonly contributor = Role.Contributor;
}
