import { Component, Input } from '@angular/core';

type DefaultImage = '404' | 'mp' | 'identicon' | 'monsterid' | 'wavatar' | 'retro' | 'robohash' | 'blank';

type Rating = 'g' | 'pg' | 'r' | 'x';

@Component({
  selector: 'app-gravatar',
  standalone: true,
  imports: [],
  templateUrl: './gravatar.component.html',
  styleUrl: './gravatar.component.scss',
})
export class GravatarComponent {
  @Input({ required: true }) emailHash!: string;
  @Input() size: number = 40;
  @Input() defaultImage: DefaultImage = 'retro';
  @Input() rating: Rating = 'g';

  get gravatarUrl(): string {
    return `https://gravatar.com/avatar/${this.emailHash}?s=${this.size}&d=${this.defaultImage}&r=${this.rating}`;
  }
}
