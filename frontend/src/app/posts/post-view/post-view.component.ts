import { Component, input, OnInit } from '@angular/core';
import { MatCardModule } from '@angular/material/card';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { dispatch, select } from '@ngxs/store';

import { PostsActions } from '../posts.actions';
import { PostsState } from '../posts.state';

@Component({
  selector: 'app-post-view',
  standalone: true,
  imports: [MatCardModule, MatProgressSpinnerModule],
  templateUrl: './post-view.component.html',
  styleUrl: './post-view.component.scss',
})
export class PostViewComponent implements OnInit {
  readonly postId = input.required<string>();
  readonly status = select(PostsState.status);
  readonly post = select(PostsState.currentPost);
  private readonly getPost = dispatch(PostsActions.GetPost);

  ngOnInit(): void {
    this.getPost(this.postId());
  }
}
