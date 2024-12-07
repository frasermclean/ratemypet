import { TitleCasePipe } from '@angular/common';
import { Component, inject, input } from '@angular/core';
import { MatBadgeModule } from '@angular/material/badge';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatIconModule } from '@angular/material/icon';
import { MatTooltipModule } from '@angular/material/tooltip';
import { RouterLink } from '@angular/router';
import { dispatch, select } from '@ngxs/store';
import { GravatarComponent } from '@shared/gravatar/gravatar.component';
import { NotificationService } from '@shared/services/notification.service';
import { AuthState } from '../../../auth/auth.state';
import { allReactions, Reaction, SearchPostsMatch } from '../../post.models';
import { PostsActions } from '../../posts.actions';

@Component({
  selector: 'app-post-list-item',
  imports: [
    TitleCasePipe,
    MatBadgeModule,
    MatButtonModule,
    MatCardModule,
    MatIconModule,
    MatTooltipModule,
    RouterLink,
    GravatarComponent
  ],
  templateUrl: './post-list-item.component.html',
  styleUrl: './post-list-item.component.scss'
})
export class PostItemComponent {
  private readonly notificationService = inject(NotificationService);
  postMatch = input.required<SearchPostsMatch>();
  reactions = allReactions;
  removePostReaction = dispatch(PostsActions.RemovePostReaction);
  updatePostReaction = dispatch(PostsActions.UpdatePostReaction);
  isLoggedIn = select(AuthState.isLoggedIn);

  handleReaction(reaction: Reaction) {
    if (!this.isLoggedIn()) {
      this.notificationService.showInformation('You must be logged in to react to a post');
      return;
    }
    const post = this.postMatch();
    if (post.userReaction === reaction) {
      this.removePostReaction(post.id);
    } else {
      this.updatePostReaction(post.id, reaction);
    }
  }
}
