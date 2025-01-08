import { Component, inject, input, OnDestroy } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MatDialog } from '@angular/material/dialog';
import { MatIconModule } from '@angular/material/icon';
import { Navigate } from '@ngxs/router-plugin';
import { Actions, dispatch, ofActionSuccessful } from '@ngxs/store';
import { ConfirmationComponent, ConfirmationData } from '@shared/components/confirmation/confirmation.component';
import { NotificationService } from '@shared/services/notification.service';
import { filter, Subject, takeUntil } from 'rxjs';
import { PostsActions } from '../../posts.actions';

@Component({
  selector: 'app-post-delete-button',
  imports: [MatButtonModule, MatIconModule],
  template: `
    <button mat-button (click)="onDelete()">
      <mat-icon>delete</mat-icon>
      Delete
    </button>
  `
})
export class PostDeleteButtonComponent implements OnDestroy {
  postId = input.required<string>();
  private readonly dialog = inject(MatDialog);
  private readonly deletePost = dispatch(PostsActions.DeletePost);
  private readonly navigate = dispatch(Navigate);
  private readonly destroy$ = new Subject<void>();

  constructor(actions$: Actions, notificationService: NotificationService) {
    actions$.pipe(ofActionSuccessful(PostsActions.DeletePost), takeUntil(this.destroy$)).subscribe(() => {
      notificationService.showInformation('Post deleted successfully');
      this.navigate(['/posts']);
    });
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
