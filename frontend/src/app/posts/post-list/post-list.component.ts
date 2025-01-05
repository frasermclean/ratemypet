import { Component, OnInit } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatTooltipModule } from '@angular/material/tooltip';
import { dispatch, select } from '@ngxs/store';

import { PostsState } from '../posts.state';
import { PostItemComponent } from './post-list-item/post-list-item.component';

import { RouterLink } from '@angular/router';
import { Role } from '../../auth/auth.models';
import { AuthState } from '../../auth/auth.state';
import { PostsActions } from '../posts.actions';

@Component({
  selector: 'app-post-list',
  imports: [RouterLink, PostItemComponent, MatButtonModule, MatIconModule, MatProgressSpinnerModule, MatTooltipModule],
  templateUrl: './post-list.component.html',
  styleUrl: './post-list.component.scss'
})
export class PostListComponent implements OnInit {
  readonly status = select(PostsState.status);
  readonly matches = select(PostsState.matches);
  readonly userRoles = select(AuthState.roles);
  readonly searchPosts = dispatch(PostsActions.SearchPosts);
  readonly contributor = Role.Contributor;

  ngOnInit(): void {
    this.searchPosts();
  }
}
