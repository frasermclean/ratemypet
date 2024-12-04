import { Component, input } from '@angular/core';
import { PostComment } from '../../../post.models';
import { DatePipe } from '@angular/common';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { dispatch } from '@ngxs/store';
import { PostsActions } from '../../../posts.actions';
import { MatTooltipModule } from '@angular/material/tooltip';

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

  onDelete() {
    this.deleteComment(this.postId(), this.comment().id);
  }
}
