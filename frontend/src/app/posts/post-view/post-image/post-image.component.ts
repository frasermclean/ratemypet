import { Component, input } from '@angular/core';

@Component({
  selector: 'app-post-image',
  imports: [],
  templateUrl: './post-image.component.html',
  styleUrl: './post-image.component.scss'
})
export class PostImageComponent {
  baseUrl = input.required<string>();
  title = input<string>();
  defaultSize = input<number>(1024);

  getImageUrl(mediaWidth: number = this.defaultSize()) {
    return `${this.baseUrl()}?width=${mediaWidth}&height=${mediaWidth}&format=webp`;
  }
}
