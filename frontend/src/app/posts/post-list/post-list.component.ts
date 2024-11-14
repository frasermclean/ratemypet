import { Component, inject, OnInit } from '@angular/core';
import { AsyncPipe } from '@angular/common';
import { MatButtonModule } from '@angular/material/button';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { Store } from '@ngxs/store';

import { PostsState } from '../posts.state';
import { PostItemComponent } from './post-list-item/post-list-item.component';
import { PostsActions } from '../posts.actions';

@Component({
  selector: 'app-post-list',
  standalone: true,
  imports: [PostItemComponent, AsyncPipe, MatButtonModule, MatProgressSpinnerModule],
  templateUrl: './post-list.component.html',
  styleUrl: './post-list.component.scss',
})
export class PostListComponent implements OnInit{
  private readonly store = inject(Store);
  status$ = this.store.select(PostsState.status);
  posts$ = this.store.select(PostsState.posts);

  ngOnInit(): void {
    this.searchPosts();
  }

  searchPosts() {
    this.store.dispatch(new PostsActions.SearchPosts());
  }
}
