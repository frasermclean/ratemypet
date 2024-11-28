import { Component, inject, input, OnInit } from '@angular/core';
import { AsyncPipe } from '@angular/common';
import { MatCardModule } from '@angular/material/card';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { Store } from '@ngxs/store';
import { Observable } from 'rxjs';

import { PostsActions } from '../posts.actions';
import { PostsState } from '../posts.state';
import { DetailedPost } from '@models/post.models';

@Component({
  selector: 'app-post-view',
  standalone: true,
  imports: [AsyncPipe, MatCardModule, MatProgressSpinnerModule],
  templateUrl: './post-view.component.html',
  styleUrl: './post-view.component.scss',
})
export class PostViewComponent implements OnInit {
  private readonly store = inject(Store);
  postId = input.required<string>();
  status$ = this.store.select(PostsState.status);
  post$ = this.store.select(PostsState.currentPost);

  ngOnInit(): void {
    this.store.dispatch(new PostsActions.GetPost(this.postId()));
  }
}
