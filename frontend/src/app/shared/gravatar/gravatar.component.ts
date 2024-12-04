import { Component, input } from '@angular/core';

@Component({
  selector: 'app-gravatar',
  standalone: true,
  imports: [],
  templateUrl: './gravatar.component.html',
  styleUrl: './gravatar.component.scss',
})
export class GravatarComponent {
  emailHash = input.required<string>();
  size = input(40);
  defaultImage = input<'404' | 'mp' | 'identicon' | 'monsterid' | 'wavatar' | 'retro' | 'robohash' | 'blank'>('retro');
  rating = input<'g' | 'pg' | 'r' | 'x'>('g');
  shape = input<'circle' | 'rounded'>('circle');

  get gravatarUrl(): string {
    return `https://gravatar.com/avatar/${this.emailHash()}?s=${this.size()}&d=${this.defaultImage()}&r=${this.rating()}`;
  }
}
