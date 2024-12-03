import { Component, inject } from '@angular/core';
import { NonNullableFormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { dispatch, select } from '@ngxs/store';

import { PostsState } from '../../../posts.state';
import { PostsActions } from '../../../posts.actions';

@Component({
  selector: 'app-add-comment',
  standalone: true,
  imports: [ReactiveFormsModule, MatButtonModule, MatFormFieldModule, MatInputModule],
  templateUrl: './add-comment.component.html',
  styleUrl: './add-comment.component.scss',
})
export class AddCommentComponent {
  post = select(PostsState.currentPost);
  formBuilder = inject(NonNullableFormBuilder);
  formGroup = this.formBuilder.group({
    content: ['', Validators.required],
  });

  addPostComment = dispatch(PostsActions.AddPostComment);

  onSubmit() {
    const postId = this.post()?.id;
    if (!postId) {
      return;
    }

    const content = this.formGroup.getRawValue().content;
    this.addPostComment(postId, content);
    this.formGroup.reset();
  }
}
