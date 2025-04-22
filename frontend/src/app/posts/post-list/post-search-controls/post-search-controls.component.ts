import { Component, effect, inject, input, OnInit, signal } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { MatButtonToggleModule } from '@angular/material/button-toggle';
import { MatExpansionModule } from '@angular/material/expansion';
import { MatIconModule } from '@angular/material/icon';
import { ActivatedRoute, Router } from '@angular/router';
import { RouterState } from '@ngxs/router-plugin';
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
  private readonly router = inject(Router);
  private readonly activatedRoute = inject(ActivatedRoute);
  expanded = input(false);
  speciesName = signal('');
  orderBy = signal<SearchPostsOrderBy>('createdAt');
  descending = signal(true);
  allSpecies = select(SpeciesState.allSpecies);
  routerState = select(RouterState.state);
  getAllSpecies = dispatch(SpeciesActions.GetAllSpecies);

  isBusy = select(PostsState.isBusy);
  searchPosts = dispatch(PostsActions.SearchPosts);

  constructor() {
    effect(() => {
      this.searchPosts({
        speciesName: this.speciesName(),
        orderBy: this.orderBy(),
        descending: this.descending()
      });
    });

    // map query params to signals
    this.activatedRoute.queryParams.pipe(takeUntilDestroyed()).subscribe((queryParams) => {
      if (queryParams['speciesName']) {
        this.speciesName.set(queryParams['speciesName']);
      }

      if (queryParams['orderBy']) {
        this.orderBy.set(queryParams['orderBy']);
      }

      if (queryParams['descending'] !== undefined) {
        this.descending.set(queryParams['descending'] === 'true');
      }
    });
  }

  ngOnInit(): void {
    this.getAllSpecies();
  }

  onSpeciesChange(value: string) {
    this.speciesName.set(value);
    this.updateQueryParams();
  }

  onOrderChange(value: { orderBy: SearchPostsOrderBy; descending: boolean }) {
    this.orderBy.set(value.orderBy);
    this.descending.set(value.descending);
    this.updateQueryParams();
  }

  private updateQueryParams() {
    const queryParams = {
      speciesName: this.speciesName() || undefined,
      orderBy: this.orderBy() || undefined,
      descending: this.descending() || undefined
    };

    this.router.navigate([], { queryParams });
  }
}
