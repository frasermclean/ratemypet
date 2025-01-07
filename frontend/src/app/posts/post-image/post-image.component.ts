import { Component, input } from '@angular/core';
import { Post } from '../post.models';

@Component({
  selector: 'app-post-image',
  imports: [],
  templateUrl: './post-image.component.html',
  styleUrl: './post-image.component.scss'
})
export class PostImageComponent {
  post = input.required<Post>();

  getImageUrl(mediaWidth: number = 1024) {
    const post = this.post();
    return post ? `${post.imageUrl}?width=${mediaWidth}&height=${mediaWidth}&format=webp` : '';
  }
}
