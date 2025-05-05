import { ChangeDetectionStrategy, Component, inject, signal } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { NonNullableFormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MAT_DIALOG_DATA, MatDialogModule, MatDialogRef } from '@angular/material/dialog';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatProgressBarModule } from '@angular/material/progress-bar';
import { Actions, dispatch, ofActionCompleted, Store } from '@ngxs/store';
import { NotificationService } from '@shared/services/notification.service';
import { PostsActions } from '../../posts.actions';

export interface PostAddCommentData {
  postId: string;
  parentCommentId?: string;
}

@Component({
  changeDetection: ChangeDetectionStrategy.OnPush,
  imports: [
    ReactiveFormsModule,
    MatButtonModule,
    MatDialogModule,
    MatFormFieldModule,
    MatInputModule,
    MatProgressBarModule
  ],
  templateUrl: './post-add-comment.component.html',
  styleUrl: './post-add-comment.component.scss'
})
export class PostAddCommentComponent {
  data: PostAddCommentData = inject(MAT_DIALOG_DATA);
  dialogRef = inject(MatDialogRef<PostAddCommentComponent>);
  formBuilder = inject(NonNullableFormBuilder);
  store = inject(Store);
  formGroup = this.formBuilder.group({
    comment: ['', Validators.required]
  });
  addPostComment = dispatch(PostsActions.AddPostComment);
  isBusy = signal(false);

  constructor(actions$: Actions, notificationService: NotificationService) {
    actions$.pipe(ofActionCompleted(PostsActions.AddPostComment), takeUntilDestroyed()).subscribe((completion) => {
      this.isBusy.set(false);
      if (completion.result.successful) {
        notificationService.showInformation('Comment added successfully.');
      } else {
        notificationService.showError('Failed to add comment.');
      }
      this.dialogRef.close();
    });
  }

  onSubmit() {
    const content = this.formGroup.controls.comment.value;
    this.store.dispatch(new PostsActions.AddPostComment(this.data.postId, content, this.data.parentCommentId));
    this.isBusy.set(true);
  }
}
