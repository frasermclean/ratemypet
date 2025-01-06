import { Component, effect, input, model } from '@angular/core';
import { MatButtonToggleModule } from '@angular/material/button-toggle';
import { MatExpansionModule } from '@angular/material/expansion';
import { MatIconModule } from '@angular/material/icon';
import { dispatch, select } from '@ngxs/store';
import { SearchPostsOrderBy } from '../../post.models';
import { PostsActions } from '../../posts.actions';
import { PostsState } from '../../posts.state';

const SPECIES_OPTIONS = [
  {
    label: 'Birds',
    value: 'Bird'
  },
  {
    label: 'Cats',
    value: 'Cat'
  },
  {
    label: 'Dogs',
    value: 'Dog'
  }
];

const ORDER_BY_OPTIONS = [
  {
    label: 'Latest',
    value: {
      orderBy: 'createdAt',
      descending: true
    }
  },
  {
    label: 'Reactions',
    value: {
      orderBy: 'reactionCount',
      descending: true
    }
  }
];

export interface SearchControlsChangeEvent {
  speciesName: string;
  orderBy: SearchPostsOrderBy;
  descending: boolean;
}

@Component({
  selector: 'app-search-controls',
  imports: [MatButtonToggleModule, MatExpansionModule, MatIconModule],
  templateUrl: './search-controls.component.html',
  styleUrl: './search-controls.component.scss'
})
export class SearchControlsComponent {
  readonly speciesOptions = SPECIES_OPTIONS;
  readonly orderByOptions = ORDER_BY_OPTIONS;
  expanded = input(false);

  speciesName = model('');
  orderBy = model<SearchPostsOrderBy>('createdAt');
  descending = model(true);

  isBusy = select(PostsState.isBusy);
  searchPosts = dispatch(PostsActions.SearchPosts);

  constructor() {
    effect(() => {
      const request = {
        speciesName: this.speciesName(),
        orderBy: this.orderBy(),
        descending: this.descending()
      };
      this.searchPosts(request);
    });
  }

  onOrderChange(value: { orderBy: SearchPostsOrderBy; descending: boolean }) {
    this.orderBy.set(value.orderBy);
    this.descending.set(value.descending);
  }
}
