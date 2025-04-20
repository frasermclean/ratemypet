import { Component, inject, input, OnDestroy } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MatDialog } from '@angular/material/dialog';
import { MatIconModule } from '@angular/material/icon';
import { Navigate } from '@ngxs/router-plugin';
import { dispatch } from '@ngxs/store';
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
  public readonly postId = input.required<string>();
  private readonly dialog = inject(MatDialog);
  private readonly notificationService = inject(NotificationService);
  private readonly deletePost = dispatch(PostsActions.DeletePost);
  private readonly navigate = dispatch(Navigate);
  private readonly destroy$ = new Subject<void>();

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
      .subscribe(() => {
        this.notificationService.showInformation('Request to delete post sent.');
        this.deletePost(this.postId());
        this.navigate(['/posts']);
      });
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }
}

const DELETE_DIALOG_DATA: ConfirmationData = {
  title: 'Delete Post',
  message: 'Are you sure you want to delete this post?',
  confirmText: 'Yes, delete it',
  cancelText: 'No, keep it'
};
