import { ChangeDetectionStrategy, Component, inject, input } from '@angular/core';
import { ImageUrlService } from '@shared/services/image-url.service';

@Component({
  changeDetection: ChangeDetectionStrategy.OnPush,
  selector: 'app-post-image',
  imports: [],
  templateUrl: './post-image.component.html',
  styleUrl: './post-image.component.scss'
})
export class PostImageComponent {
  imageId = input.required<string>();
  title = input<string>();
  defaultSize = input<number>(1024);
  imageUrlService = inject(ImageUrlService);

  getImageUrl(mediaWidth: number = this.defaultSize()) {
    return this.imageUrlService.getImageUrl(this.imageId(), mediaWidth, mediaWidth);
  }
}
