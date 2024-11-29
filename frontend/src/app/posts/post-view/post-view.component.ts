import { Component, inject, input, OnDestroy, OnInit } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MatDialog } from '@angular/material/dialog';
import { MatCardModule } from '@angular/material/card';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { dispatch, select } from '@ngxs/store';

import { PostsActions } from '../posts.actions';
import { PostsState } from '../posts.state';
import { ConfirmationComponent, ConfirmationData } from '@shared/confirmation/confirmation.component';
import { filter, Subject, takeUntil } from 'rxjs';

@Component({
  selector: 'app-post-view',
  standalone: true,
  imports: [MatButtonModule, MatCardModule, MatIconModule, MatProgressSpinnerModule],
  templateUrl: './post-view.component.html',
  styleUrl: './post-view.component.scss',
})
export class PostViewComponent implements OnInit, OnDestroy {
  readonly postId = input.required<string>();
  readonly status = select(PostsState.status);
  readonly post = select(PostsState.currentPost);
  readonly getPost = dispatch(PostsActions.GetPost);
  readonly deletePost = dispatch(PostsActions.DeletePost);

  private readonly dialog = inject(MatDialog);
  private readonly destroy$ = new Subject<void>();

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
        data: {
          title: 'Delete Post',
          message: 'Are you sure you want to delete this post?',
          confirmText: 'Yes, delete it',
          cancelText: 'No, keep it',
        },
      })
      .afterClosed()
      .pipe(
        filter((isConfirmed) => !!isConfirmed),
        takeUntil(this.destroy$)
      )
      .subscribe(() => this.deletePost(this.postId()));
  }
}
