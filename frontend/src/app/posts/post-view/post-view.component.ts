import { DatePipe } from '@angular/common';
import { Component, input, OnInit } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { Navigate } from '@ngxs/router-plugin';
import { Actions, dispatch, ofActionSuccessful, select, Store } from '@ngxs/store';
import { SharedActions } from '@shared/shared.actions';
import { AuthState } from '../../auth/auth.state';
import { PostImageComponent } from '../post-image/post-image.component';
import { PostsActions } from '../posts.actions';
import { PostsState } from '../posts.state';
import { PostCommentsComponent } from './post-comments/post-comments.component';
import { PostDeleteButtonComponent } from './post-delete-button/post-delete-button.component';

@Component({
  selector: 'app-post-view',
  imports: [
    DatePipe,
    MatButtonModule,
    MatCardModule,
    MatIconModule,
    MatProgressSpinnerModule,
    PostCommentsComponent,
    PostDeleteButtonComponent,
    PostImageComponent
  ],
  templateUrl: './post-view.component.html',
  styleUrl: './post-view.component.scss'
})
export class PostViewComponent implements OnInit {
  readonly postId = input.required<string>();
  readonly status = select(PostsState.status);
  readonly post = select(PostsState.currentPost);
  readonly userName = select(AuthState.userName);
  readonly errorMessage = select(PostsState.errorMessage);
  readonly getPost = dispatch(PostsActions.GetPost);
  readonly setPageTitle = dispatch(SharedActions.SetPageTitle);
  readonly navigate = dispatch(Navigate);

  constructor(actions$: Actions, store: Store) {
    actions$.pipe(ofActionSuccessful(PostsActions.GetPost), takeUntilDestroyed()).subscribe(() => {
      const title = this.post()!.title;
      const url = store.selectSnapshot((state) => state.router.state.url) as string;
      this.setPageTitle(title, url);
    });
  }

  ngOnInit(): void {
    this.getPost(this.postId());
  }
}
