import { ChangeDetectionStrategy, Component, computed, inject, input } from '@angular/core';
import { MatBadgeModule } from '@angular/material/badge';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatTooltipModule } from '@angular/material/tooltip';
import { RouterLink } from '@angular/router';
import { GravatarComponent } from '@shared/components/gravatar/gravatar.component';
import { ImageUrlService } from '@shared/services/image-url.service';
import { PostReactionButtonComponent } from '../../post-reaction-button/post-reaction-button.component';
import { allReactions, SearchPostsMatch } from '../../post.models';

@Component({
  changeDetection: ChangeDetectionStrategy.OnPush,
  selector: 'app-post-list-item',
  imports: [
    MatBadgeModule,
    MatButtonModule,
    MatCardModule,
    MatTooltipModule,
    RouterLink,
    GravatarComponent,
    PostReactionButtonComponent
  ],
  templateUrl: './post-list-item.component.html',
  styleUrl: './post-list-item.component.scss'
})
export class PostItemComponent {
  postMatch = input.required<SearchPostsMatch>();
  imageUrlService = inject(ImageUrlService);
  width = input(320);
  height = input(320);

  imageUrl = computed(() => {
    const imageId = this.postMatch().imageId;
    return imageId ? this.imageUrlService.getImageUrl(imageId, this.width(), this.height()) : null;
  });

  onImageError(event: Event) {
    const imageElement = event.target as HTMLImageElement;
    imageElement.src = `images/placeholders/${this.postMatch().speciesName.toLowerCase()}.png`;
    imageElement.style.padding = '1rem';
  }

  reactions = allReactions;
}
