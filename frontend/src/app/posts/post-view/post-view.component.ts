import { DatePipe } from '@angular/common';
import { Component, computed, inject, input, OnInit } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatDialog } from '@angular/material/dialog';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { RouterState } from '@ngxs/router-plugin';
import { Actions, dispatch, ofActionSuccessful, select, Store } from '@ngxs/store';
import { SharedActions } from '@shared/shared.actions';
import { AuthState } from '../../auth/auth.state';
import { PostReactionComponent } from '../post-reaction/post-reaction.component';
import { allReactions } from '../post.models';
import { PostsActions } from '../posts.actions';
import { PostsState } from '../posts.state';
import { PostAddCommentComponent, PostAddCommentData } from './post-add-comment/post-add-comment.component';
import { PostCommentsComponent } from './post-comments/post-comments.component';
import { PostDeleteButtonComponent } from './post-delete-button/post-delete-button.component';
import { PostImageComponent } from './post-image/post-image.component';

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
    PostImageComponent,
    PostReactionComponent
  ],
  templateUrl: './post-view.component.html',
  styleUrl: './post-view.component.scss'
})
export class PostViewComponent implements OnInit {
  readonly postId = input.required<string>();
  readonly dialog = inject(MatDialog);
  readonly status = select(PostsState.status);
  readonly post = select(PostsState.currentPost);
  readonly userName = select(AuthState.userName);
  readonly errorMessage = select(PostsState.errorMessage);
  readonly getPost = dispatch(PostsActions.GetPost);
  readonly addPostComment = dispatch(PostsActions.AddPostComment);
  readonly setPageTitle = dispatch(SharedActions.SetPageTitle);
  readonly allReactions = allReactions;

  reactionCount = computed<number>(() => {
    const reactions = this.post()?.reactions;
    if (!reactions) {
      return 0;
    }

    return Object.values(reactions).reduce((acc, value) => acc + value, 0);
  });

  constructor(actions$: Actions, store: Store) {
    actions$.pipe(ofActionSuccessful(PostsActions.GetPost), takeUntilDestroyed()).subscribe(() => {
      const title = this.post()!.title;
      const url = store.selectSnapshot(RouterState.url)!;
      this.setPageTitle(title, url);
    });
  }

  ngOnInit(): void {
    this.getPost(this.postId());
  }

  showCommentDialog() {
    this.dialog.open<PostAddCommentComponent, PostAddCommentData>(PostAddCommentComponent, {
      data: { postId: this.postId() }
    });
  }
}
