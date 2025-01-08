import { Component, computed, input } from '@angular/core';
import { MatBadgeModule } from '@angular/material/badge';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatTooltipModule } from '@angular/material/tooltip';
import { RouterLink } from '@angular/router';
import { GravatarComponent } from '@shared/components/gravatar/gravatar.component';
import { PostReactionComponent } from '../../post-reaction/post-reaction.component';
import { allReactions, SearchPostsMatch } from '../../post.models';

@Component({
  selector: 'app-post-list-item',
  imports: [
    MatBadgeModule,
    MatButtonModule,
    MatCardModule,
    MatTooltipModule,
    RouterLink,
    GravatarComponent,
    PostReactionComponent
  ],
  templateUrl: './post-list-item.component.html',
  styleUrl: './post-list-item.component.scss'
})
export class PostItemComponent {
  postMatch = input.required<SearchPostsMatch>();
  width = input(320);
  height = input(320);
  imageUrl = computed(() => `${this.postMatch().imageUrl}?width=${this.width()}&height=${this.height()}&format=webp`);
  reactions = allReactions;
}
