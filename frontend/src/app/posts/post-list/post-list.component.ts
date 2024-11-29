import { Component, OnInit } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatTooltipModule } from '@angular/material/tooltip';
import { dispatch, select } from '@ngxs/store';

import { PostsState } from '../posts.state';
import { PostItemComponent } from './post-list-item/post-list-item.component';

import { PostsActions } from '../posts.actions';

@Component({
  selector: 'app-post-list',
  standalone: true,
  imports: [
    PostItemComponent,
    MatButtonModule,
    MatIconModule,
    MatProgressSpinnerModule,
    MatTooltipModule,
  ],
  templateUrl: './post-list.component.html',
  styleUrl: './post-list.component.scss',
})
export class PostListComponent implements OnInit {

  readonly status = select(PostsState.status);
  readonly posts = select(PostsState.posts);
  readonly searchPosts = dispatch(PostsActions.SearchPosts);
  readonly openPostEditDialog = dispatch(PostsActions.OpenPostEditDialog);

  ngOnInit(): void {
    this.searchPosts();
  }
}
