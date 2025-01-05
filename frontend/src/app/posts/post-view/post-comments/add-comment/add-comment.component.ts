import { Component, inject, input } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { NonNullableFormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { Actions, dispatch, ofActionSuccessful } from '@ngxs/store';
import { NotificationService } from '@shared/services/notification.service';
import { Post } from '../../../post.models';
import { PostsActions } from '../../../posts.actions';

@Component({
  selector: 'app-add-comment',
  imports: [ReactiveFormsModule, MatButtonModule, MatFormFieldModule, MatInputModule],
  templateUrl: './add-comment.component.html',
  styleUrl: './add-comment.component.scss'
})
export class AddCommentComponent {
  post = input.required<Post>();
  formBuilder = inject(NonNullableFormBuilder);
  formGroup = this.formBuilder.group({
    content: ['', Validators.required]
  });

  constructor(actions$: Actions, notificationService: NotificationService) {
    actions$
      .pipe(ofActionSuccessful(PostsActions.AddPostComment))
      .pipe(takeUntilDestroyed())
      .subscribe((x) => {
        notificationService.showInformation('Comment added successfully.');
      });
  }

  addPostComment = dispatch(PostsActions.AddPostComment);

  onSubmit() {
    const postId = this.post().id;
    const content = this.formGroup.getRawValue().content;
    this.addPostComment(postId, content);
    this.formGroup.reset();
  }
}
