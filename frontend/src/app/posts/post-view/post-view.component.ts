import { DatePipe } from '@angular/common';
import { ChangeDetectionStrategy, Component, computed, inject, input, OnInit } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatDialog } from '@angular/material/dialog';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { RouterLink } from '@angular/router';
import { RouterState } from '@ngxs/router-plugin';
import { Actions, dispatch, ofActionSuccessful, select, Store } from '@ngxs/store';
import { SharedActions } from '@shared/shared.actions';
import { AuthState } from '../../auth/auth.state';
import { PostsActions } from '../posts.actions';
import { PostsState } from '../posts.state';
import { PostAddCommentComponent, PostAddCommentData } from './post-add-comment/post-add-comment.component';
import { PostCommentsComponent } from './post-comments/post-comments.component';
import { PostDeleteButtonComponent } from './post-delete-button/post-delete-button.component';
import { PostImageComponent } from './post-image/post-image.component';
import { PostReactionsCardComponent } from './post-reactions-card/post-reactions-card.component';
import { PostTagsListComponent } from './post-tags-list/post-tags-list.component';

@Component({
  changeDetection: ChangeDetectionStrategy.OnPush,
  imports: [
    DatePipe,
    MatButtonModule,
    MatCardModule,
    MatIconModule,
    MatProgressSpinnerModule,
    PostCommentsComponent,
    PostDeleteButtonComponent,
    PostImageComponent,
    PostTagsListComponent,
    RouterLink,
    PostReactionsCardComponent
  ],
  templateUrl: './post-view.component.html',
  styleUrl: './post-view.component.scss'
})
export class PostViewComponent implements OnInit {
  readonly postIdOrSlug = input.required<string>();
  readonly dialog = inject(MatDialog);
  readonly status = select(PostsState.status);
  readonly post = select(PostsState.currentPost);
  readonly userName = select(AuthState.userName);
  readonly errorMessage = select(PostsState.errorMessage);
  readonly isAdministrator = select(AuthState.isAdministrator);
  readonly getPost = dispatch(PostsActions.GetPost);
  readonly addPostComment = dispatch(PostsActions.AddPostComment);
  readonly setPageTitle = dispatch(SharedActions.SetPageTitle);
  readonly pollPostStatus = dispatch(PostsActions.PollPostStatus);

  isAuthor = computed<boolean>(() => this.post()?.authorUserName === this.userName());

  constructor(actions$: Actions, store: Store) {
    actions$.pipe(ofActionSuccessful(PostsActions.GetPost), takeUntilDestroyed()).subscribe(() => {
      const post = this.post()!;
      const title = post.title;
      const url = store.selectSnapshot(RouterState.url)!;
      this.setPageTitle(title, url);

      if (post.status === 'initial') {
        this.pollPostStatus(this.postIdOrSlug());
      }
    });
  }

  ngOnInit(): void {
    this.getPost(this.postIdOrSlug());
  }

  showCommentDialog() {
    this.dialog.open<PostAddCommentComponent, PostAddCommentData>(PostAddCommentComponent, {
      data: { postId: this.post()!.id }
    });
  }
}
