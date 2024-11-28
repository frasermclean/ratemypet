import { Component, input } from '@angular/core';
import { sha256 } from 'js-sha256';

@Component({
  selector: 'app-gravatar',
  standalone: true,
  imports: [],
  templateUrl: './gravatar.component.html',
  styleUrl: './gravatar.component.scss',
})
export class GravatarComponent {
  emailOrHash = input.required<string>();
  size = input(40);
  defaultImage = input<'404' | 'mp' | 'identicon' | 'monsterid' | 'wavatar' | 'retro' | 'robohash' | 'blank'>('retro');
  rating = input<'g' | 'pg' | 'r' | 'x'>('g');
  shape = input<'circle' | 'rounded'>('circle');

  get gravatarUrl(): string {
    const hash = this.emailOrHash().includes('@') ? sha256(this.emailOrHash()) : this.emailOrHash();
    const size = this.size();
    const defaultImage = this.defaultImage();
    const rating = this.rating();

    return `https://gravatar.com/avatar/${hash}?s=${size}&d=${defaultImage}&r=${rating}`;
  }
}
