import { Injectable } from '@angular/core';
import { Title } from '@angular/platform-browser';
import { RouterStateSnapshot, TitleStrategy } from '@angular/router';

@Injectable({ providedIn: 'root' })
export class AppTitleStrategy extends TitleStrategy {
  private readonly baseTitle = 'Rate My Pet';

  constructor(private readonly titleService: Title) {
    super();
  }

  override updateTitle(snapshot: RouterStateSnapshot): void {
    const title = this.buildTitle(snapshot);
    if (title) {
      this.titleService.setTitle(`${this.baseTitle} | ${title}`);
    } else {
      this.titleService.setTitle(this.baseTitle);
    }
  }
}
