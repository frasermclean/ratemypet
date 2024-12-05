import { DatePipe } from '@angular/common';
import { Component, input } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatTooltipModule } from '@angular/material/tooltip';
import { dispatch, select } from '@ngxs/store';
import { AuthState } from '../../../../auth/auth.state';
import { PostComment } from '../../../post.models';
import { PostsActions } from '../../../posts.actions';

@Component({
  selector: 'app-post-comment',
  imports: [DatePipe, MatButtonModule, MatIconModule, MatTooltipModule],
  templateUrl: './post-comment.component.html',
  styleUrl: './post-comment.component.scss'
})
export class PostCommentComponent {
  postId = input.required<string>();
  comment = input.required<PostComment>();
  deleteComment = dispatch(PostsActions.DeletePostComment);
  currentUser = select(AuthState.currentUser);

  onDelete() {
    this.deleteComment(this.postId(), this.comment().id);
  }
}
