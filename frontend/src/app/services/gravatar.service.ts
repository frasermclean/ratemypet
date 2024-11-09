import { Injectable } from '@angular/core';

type DefaultImage =
  | '404'
  | 'mp'
  | 'identicon'
  | 'monsterid'
  | 'wavatar'
  | 'retro'
  | 'robohash'
  | 'blank';

type Rating = 'g' | 'pg' | 'r' | 'x';

@Injectable({
  providedIn: 'root',
})
export class GravatarService {
  getGravatarUrl(
    hash: string,
    size: number = 40,
    defaultImage: DefaultImage = 'retro',
    rating: Rating = 'g'
  ): string {
    return `https://gravatar.com/avatar/${hash}?s=${size}&d=${defaultImage}&r=${rating}`;
  }
}
