import { Injectable } from '@angular/core';
import { Action, Selector, State, StateContext, StateToken } from '@ngxs/store';
import { SharedActions } from './shared.actions';

interface SharedStateModel {
  appName: string;
  pageTitle: string;
  pageUrl: string;
}

const SHARED_STATE_TOKEN = new StateToken<SharedStateModel>('shared');

@State<SharedStateModel>({
  name: SHARED_STATE_TOKEN,
  defaults: {
    appName: 'Rate My Pet',
    pageTitle: '',
    pageUrl: ''
  }
})
@Injectable()
export class SharedState {
  @Action(SharedActions.SetPageTitle)
  setPageTitle(context: StateContext<SharedStateModel>, action: SharedActions.SetPageTitle) {
    context.patchState({ pageTitle: action.title, pageUrl: action.url });
  }

  @Selector([SHARED_STATE_TOKEN])
  static appName(state: SharedStateModel): string {
    return state.appName;
  }

  @Selector([SHARED_STATE_TOKEN])
  static pageTitle(state: SharedStateModel): string {
    return state.pageTitle;
  }

  @Selector([SHARED_STATE_TOKEN])
  static pageUrl(state: SharedStateModel): string {
    return state.pageUrl;
  }
}
