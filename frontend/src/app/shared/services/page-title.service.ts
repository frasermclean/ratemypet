import { inject, Injectable } from '@angular/core';
import { Title } from '@angular/platform-browser';
import { RouterStateSnapshot, TitleStrategy } from '@angular/router';
import { Actions, dispatch, ofActionSuccessful, select } from '@ngxs/store';
import { SharedActions } from '@shared/shared.actions';
import { SharedState } from '@shared/shared.state';

@Injectable()
export class PageTitleService extends TitleStrategy {
  private readonly titleService = inject(Title);
  private readonly appName = select(SharedState.appName);
  private readonly pageTitle = select(SharedState.pageTitle);
  private readonly setPageTitle = dispatch(SharedActions.SetPageTitle);

  constructor(actions$: Actions) {
    super();

    actions$.pipe(ofActionSuccessful(SharedActions.SetPageTitle)).subscribe(() => {
      const title = `${this.pageTitle()} | ${this.appName()}`;
      this.titleService.setTitle(title);
    });
  }

  override updateTitle(snapshot: RouterStateSnapshot): void {
    const title = this.buildTitle(snapshot);
    if (title) {
      this.setPageTitle(title, snapshot.url);
    }
  }
}
