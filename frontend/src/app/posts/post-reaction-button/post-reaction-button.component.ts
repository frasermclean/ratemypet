import { TitleCasePipe } from '@angular/common';
import { ChangeDetectionStrategy, Component, inject, input } from '@angular/core';
import { MatBadgeModule } from '@angular/material/badge';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatTooltipModule } from '@angular/material/tooltip';
import { dispatch, select } from '@ngxs/store';
import { NotificationService } from '@shared/services/notification.service';
import { AuthState } from '../../auth/auth.state';
import { Reaction } from '../post.models';
import { PostsActions } from '../posts.actions';

@Component({
  changeDetection: ChangeDetectionStrategy.OnPush,
  selector: 'post-reaction-button',
  imports: [TitleCasePipe, MatButtonModule, MatBadgeModule, MatIconModule, MatTooltipModule],
  templateUrl: './post-reaction-button.component.html',
  styleUrl: './post-reaction-button.component.scss'
})
export class PostReactionButtonComponent {
  postId = input.required<string>();
  reaction = input.required<Reaction>();
  count = input.required<number>();
  userReaction = input<Reaction>();
  size = input<'normal' | 'large'>('normal');
  notificationService = inject(NotificationService);
  removePostReaction = dispatch(PostsActions.RemovePostReaction);
  updatePostReaction = dispatch(PostsActions.UpdatePostReaction);
  addPostReaction = dispatch(PostsActions.AddPostReaction);
  isLoggedIn = select(AuthState.isLoggedIn);

  onClick() {
    if (!this.isLoggedIn()) {
      this.notificationService.showInformation('You must be logged in to react to a post');
      return;
    }

    if (this.userReaction() === this.reaction()) {
      this.removePostReaction(this.postId());
    } else if (this.userReaction()) {
      this.updatePostReaction(this.postId(), this.reaction());
    } else {
      this.addPostReaction(this.postId(), this.reaction());
    }
  }
}
