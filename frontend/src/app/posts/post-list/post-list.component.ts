import { Component, OnInit } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatTooltipModule } from '@angular/material/tooltip';
import { dispatch, select } from '@ngxs/store';

import { PostsState } from '../posts.state';
import { PostItemComponent } from './post-list-item/post-list-item.component';

import { PostsActions } from '../posts.actions';
import { RouterLink } from '@angular/router';

@Component({
  selector: 'app-post-list',
  imports: [RouterLink, PostItemComponent, MatButtonModule, MatIconModule, MatProgressSpinnerModule, MatTooltipModule],
  templateUrl: './post-list.component.html',
  styleUrl: './post-list.component.scss'
})
export class PostListComponent implements OnInit {
  readonly status = select(PostsState.status);
  readonly matches = select(PostsState.matches);
  readonly searchPosts = dispatch(PostsActions.SearchPosts);

  ngOnInit(): void {
    this.searchPosts();
  }
}
