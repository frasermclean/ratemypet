import { TitleCasePipe } from '@angular/common';
import { Component, inject, input } from '@angular/core';
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
  selector: 'app-post-reaction',
  imports: [TitleCasePipe, MatButtonModule, MatBadgeModule, MatIconModule, MatTooltipModule],
  templateUrl: './post-reaction.component.html',
  styleUrl: './post-reaction.component.scss'
})
export class PostReactionComponent {
  postId = input.required<string>();
  reaction = input.required<Reaction>();
  count = input.required<number>();
  userReaction = input<Reaction>();
  notificationService = inject(NotificationService);
  removePostReaction = dispatch(PostsActions.RemovePostReaction);
  updatePostReaction = dispatch(PostsActions.UpdatePostReaction);
  isLoggedIn = select(AuthState.isLoggedIn);

  onClick() {
    if (!this.isLoggedIn()) {
      this.notificationService.showInformation('You must be logged in to react to a post');
      return;
    }

    if (this.userReaction() === this.reaction()) {
      this.removePostReaction(this.postId());
    } else {
      this.updatePostReaction(this.postId(), this.reaction());
    }
  }
}
