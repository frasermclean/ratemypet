import { Component, inject, Input } from '@angular/core';
import { TitleCasePipe } from '@angular/common';
import { MatBadgeModule } from '@angular/material/badge';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatIconModule } from '@angular/material/icon';
import { MatTooltipModule } from '@angular/material/tooltip';
import { RouterLink } from '@angular/router';

import { Post, reactions } from '@models/post.model';
import { GravatarService } from '@services/gravatar.service';

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
  ],
  templateUrl: './post-list-item.component.html',
  styleUrl: './post-list-item.component.scss',
})
export class PostItemComponent {
  reactions = reactions;
  private readonly gravatarService = inject(GravatarService);
  @Input({ required: true }) post!: Post;

  get authorAvatarUrl() {
    return this.gravatarService.getGravatarUrl(this.post.authorHash);
  }

  handleReaction(reaction: string) {
    console.log(`You reacted with ${reaction}`);
  }
}
