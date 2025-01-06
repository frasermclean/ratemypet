import { DatePipe } from '@angular/common';
import { Component, computed, inject, input, OnDestroy, OnInit } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatDialog } from '@angular/material/dialog';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { Router } from '@angular/router';
import { Actions, dispatch, ofActionSuccessful, select } from '@ngxs/store';
import { ConfirmationComponent, ConfirmationData } from '@shared/components/confirmation/confirmation.component';
import { NotificationService } from '@shared/services/notification.service';
import { filter, Subject, takeUntil } from 'rxjs';
import { AuthState } from '../../auth/auth.state';
import { PostsActions } from '../posts.actions';
import { PostsState } from '../posts.state';
import { PostCommentsComponent } from './post-comments/post-comments.component';

@Component({
  selector: 'app-post-view',
  imports: [DatePipe, MatButtonModule, MatCardModule, MatIconModule, MatProgressSpinnerModule, PostCommentsComponent],
  templateUrl: './post-view.component.html',
  styleUrl: './post-view.component.scss'
})
export class PostViewComponent implements OnInit, OnDestroy {
  readonly postId = input.required<string>();
  readonly status = select(PostsState.status);
  readonly post = select(PostsState.currentPost);
  readonly userName = select(AuthState.userName);
  readonly errorMessage = select(PostsState.errorMessage);
  readonly getPost = dispatch(PostsActions.GetPost);
  readonly deletePost = dispatch(PostsActions.DeletePost);

  readonly imageUrl = computed(() => {
    const post = this.post();
    return post ? `${post.imageUrl}?width=1024&height=1024&format=webp` : '';
  });

  private readonly dialog = inject(MatDialog);
  private readonly destroy$ = new Subject<void>();

  constructor(actions$: Actions, notificationService: NotificationService, router: Router) {
    actions$
      .pipe(ofActionSuccessful(PostsActions.DeletePost))
      .pipe(takeUntil(this.destroy$))
      .subscribe(() => {
        notificationService.showInformation('Post deleted successfully');
        router.navigate(['/posts']);
      });
  }

  ngOnInit(): void {
    this.getPost(this.postId());
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  onDelete() {
    this.dialog
      .open<ConfirmationComponent, ConfirmationData, boolean>(ConfirmationComponent, {
        data: DELETE_DIALOG_DATA
      })
      .afterClosed()
      .pipe(
        filter((isConfirmed) => !!isConfirmed),
        takeUntil(this.destroy$)
      )
      .subscribe(() => this.deletePost(this.postId()));
  }
}

const DELETE_DIALOG_DATA: ConfirmationData = {
  title: 'Delete Post',
  message: 'Are you sure you want to delete this post?',
  confirmText: 'Yes, delete it',
  cancelText: 'No, keep it'
};
