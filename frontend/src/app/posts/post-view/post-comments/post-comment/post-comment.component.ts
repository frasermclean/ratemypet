import { DatePipe } from '@angular/common';
import { Component, input } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatTooltipModule } from '@angular/material/tooltip';
import { Actions, dispatch, ofActionSuccessful, select } from '@ngxs/store';
import { NotificationService } from '@shared/services/notification.service';
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
  userName = select(AuthState.userName);
  deleteComment = dispatch(PostsActions.DeletePostComment);

  constructor(actions$: Actions, notificationService: NotificationService) {
    actions$
      .pipe(ofActionSuccessful(PostsActions.DeletePostComment))
      .pipe(takeUntilDestroyed())
      .subscribe(() => {
        notificationService.showInformation('Comment has been removed.');
      });
  }

  onDelete() {
    this.deleteComment(this.postId(), this.comment().id);
  }
}
