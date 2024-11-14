import { Component, inject, Input } from '@angular/core';
import { TitleCasePipe } from '@angular/common';
import { MatBadgeModule } from '@angular/material/badge';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatIconModule } from '@angular/material/icon';
import { MatTooltipModule } from '@angular/material/tooltip';
import { RouterLink } from '@angular/router';

import { Post, Reaction, reactions } from '@models/post.model';

import { PostsService } from '@services/posts.service';
import { GravatarComponent } from '@shared/gravatar/gravatar.component';

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
  reactions = reactions;

  private readonly postsService = inject(PostsService);
  @Input({ required: true }) post!: Post;


  handleReaction(reaction: Reaction) {
    this.postsService.updatePostReaction(this.post.id, reaction).subscribe();
  }
}
