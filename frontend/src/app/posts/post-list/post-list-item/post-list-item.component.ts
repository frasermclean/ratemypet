import { Component, inject, Input } from '@angular/core';
import { TitleCasePipe } from '@angular/common';
import { MatBadgeModule } from '@angular/material/badge';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatIconModule } from '@angular/material/icon';
import { MatTooltipModule } from '@angular/material/tooltip';
import { RouterLink } from '@angular/router';
import { Store } from '@ngxs/store';

import { GravatarComponent } from '@shared/gravatar/gravatar.component';
import { PostsActions } from '../../posts.actions';
import { allReactions, Post, Reaction } from '@models/post.models';


@Component({
  selector: 'app-post-list-item',
  standalone: true,
  imports: [
    TitleCasePipe,
    MatBadgeModule,
    MatButtonModule,
    MatCardModule,
    MatIconModule,
    MatTooltipModule,
    RouterLink,
    GravatarComponent,
  ],
  templateUrl: './post-list-item.component.html',
  styleUrl: './post-list-item.component.scss',
})
export class PostItemComponent {
  reactions = allReactions;

  private readonly store = inject(Store);
  @Input({ required: true }) post!: Post;

  getReactionCount(reaction: Reaction) {
    return this.post[`${reaction}Count`];
  }

  handleReaction(reaction: Reaction) {
    this.store.dispatch(new PostsActions.UpdatePostReaction(this.post.id, reaction));
  }
}
