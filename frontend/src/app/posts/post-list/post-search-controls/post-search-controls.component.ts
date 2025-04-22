import { Component, effect, input, model, OnInit } from '@angular/core';
import { MatButtonToggleModule } from '@angular/material/button-toggle';
import { MatExpansionModule } from '@angular/material/expansion';
import { MatIconModule } from '@angular/material/icon';
import { dispatch, select } from '@ngxs/store';
import { SpeciesActions } from '../../../species/species.actions';
import { SpeciesState } from '../../../species/species.state';
import { SearchPostsOrderBy } from '../../post.models';
import { PostsActions } from '../../posts.actions';
import { PostsState } from '../../posts.state';

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
  selector: 'post-search-controls',
  imports: [MatButtonToggleModule, MatExpansionModule, MatIconModule],
  templateUrl: './post-search-controls.component.html',
  styleUrl: './post-search-controls.component.scss'
})
export class PostSearchControlsComponent implements OnInit {
  readonly orderByOptions = ORDER_BY_OPTIONS;
  expanded = input(false);
  speciesName = model('');
  orderBy = model<SearchPostsOrderBy>('createdAt');
  descending = model(true);
  allSpecies = select(SpeciesState.allSpecies);
  getAllSpecies = dispatch(SpeciesActions.GetAllSpecies);

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

  ngOnInit(): void {
    this.getAllSpecies();
  }

  onOrderChange(value: { orderBy: SearchPostsOrderBy; descending: boolean }) {
    this.orderBy.set(value.orderBy);
    this.descending.set(value.descending);
  }
}
