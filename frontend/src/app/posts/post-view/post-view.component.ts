import { Component, inject, Input, OnInit } from '@angular/core';
import { AsyncPipe } from '@angular/common';
import { MatCardModule } from '@angular/material/card';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { Store } from '@ngxs/store';
import { Observable } from 'rxjs';

import { Post } from '@models/post.model';
import { PostsActions } from '../posts.actions';
import { PostsState } from '../posts.state';

@Component({
  selector: 'app-post-view',
  standalone: true,
  imports: [AsyncPipe, MatCardModule, MatProgressSpinnerModule],
  templateUrl: './post-view.component.html',
  styleUrl: './post-view.component.scss',
})
export class PostViewComponent implements OnInit {
  private readonly store = inject(Store);
  status$ = this.store.select(PostsState.status)
  post$!: Observable<Post | undefined>;

  @Input({ required: true }) postId!: string;

  ngOnInit(): void {
    this.store.dispatch(new PostsActions.GetPost(this.postId));
    this.post$ = this.store.select(PostsState.getPostById(this.postId));
  }
}
