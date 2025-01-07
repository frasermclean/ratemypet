import { DatePipe } from '@angular/common';
import { Component, inject, input, OnDestroy, OnInit } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatDialog } from '@angular/material/dialog';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { Navigate } from '@ngxs/router-plugin';
import { Actions, dispatch, ofActionSuccessful, select, Store } from '@ngxs/store';
import { SharedActions } from '@shared/shared.actions';
import { Subject, takeUntil } from 'rxjs';
import { AuthState } from '../../auth/auth.state';
import { PostImageComponent } from '../post-image/post-image.component';
import { PostsActions } from '../posts.actions';
import { PostsState } from '../posts.state';
import { PostAddCommentComponent, PostAddCommentData } from './post-add-comment/post-add-comment.component';
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
export class PostViewComponent implements OnInit, OnDestroy {
  readonly postId = input.required<string>();
  readonly dialog = inject(MatDialog);
  readonly status = select(PostsState.status);
  readonly post = select(PostsState.currentPost);
  readonly userName = select(AuthState.userName);
  readonly errorMessage = select(PostsState.errorMessage);
  readonly getPost = dispatch(PostsActions.GetPost);
  readonly addPostComment = dispatch(PostsActions.AddPostComment);
  readonly setPageTitle = dispatch(SharedActions.SetPageTitle);
  readonly navigate = dispatch(Navigate);
  readonly destroy$ = new Subject<void>();

  constructor(actions$: Actions, store: Store) {
    actions$.pipe(ofActionSuccessful(PostsActions.GetPost), takeUntil(this.destroy$)).subscribe(() => {
      const title = this.post()!.title;
      const url = store.selectSnapshot((state) => state.router.state.url) as string;
      this.setPageTitle(title, url);
    });
  }

  ngOnInit(): void {
    this.getPost(this.postId());
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  onAddComment() {
    this.dialog.open<PostAddCommentComponent, PostAddCommentData>(PostAddCommentComponent, {
      data: { postId: this.postId() }
    });
  }
}
