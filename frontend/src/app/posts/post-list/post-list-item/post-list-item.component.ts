import { Component, input } from '@angular/core';
import { TitleCasePipe } from '@angular/common';
import { MatBadgeModule } from '@angular/material/badge';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatIconModule } from '@angular/material/icon';
import { MatTooltipModule } from '@angular/material/tooltip';
import { RouterLink } from '@angular/router';
import { dispatch } from '@ngxs/store';

import { GravatarComponent } from '@shared/gravatar/gravatar.component';
import { PostsActions } from '../../posts.actions';
import { allReactions, Reaction, SearchPostsMatch } from '../../post.models';

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
  postMatch = input.required<SearchPostsMatch>();
  reactions = allReactions;
  removePostReaction = dispatch(PostsActions.RemovePostReaction);
  updatePostReaction = dispatch(PostsActions.UpdatePostReaction);

  handleReaction(reaction: Reaction) {
    const post = this.postMatch();
    if (post.userReaction === reaction) {
      this.removePostReaction(post.id);
    } else {
      this.updatePostReaction(post.id, reaction);
    }
  }
}
