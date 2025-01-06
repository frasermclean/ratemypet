import { Component, OnInit } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatTooltipModule } from '@angular/material/tooltip';
import { RouterLink } from '@angular/router';
import { dispatch, select } from '@ngxs/store';
import { Role } from '../../auth/auth.models';
import { AuthState } from '../../auth/auth.state';
import { PostsActions } from '../posts.actions';
import { PostsState } from '../posts.state';
import { PostItemComponent } from './post-list-item/post-list-item.component';
import { SearchControlsComponent } from './search-controls/search-controls.component';

@Component({
  selector: 'app-post-list',
  imports: [
    RouterLink,
    MatButtonModule,
    MatIconModule,
    MatProgressSpinnerModule,
    MatTooltipModule,
    PostItemComponent,
    SearchControlsComponent
  ],
  templateUrl: './post-list.component.html',
  styleUrl: './post-list.component.scss'
})
export class PostListComponent implements OnInit {
  readonly isBusy = select(PostsState.isBusy);
  readonly matches = select(PostsState.matches);
  readonly userRoles = select(AuthState.roles);
  readonly searchPosts = dispatch(PostsActions.SearchPosts);
  readonly contributor = Role.Contributor;

  ngOnInit(): void {
    this.searchPosts();
  }
}
