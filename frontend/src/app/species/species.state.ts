import { inject, Injectable } from '@angular/core';
import { Action, Selector, State, StateContext, StateToken } from '@ngxs/store';
import { finalize, tap } from 'rxjs';
import { SpeciesActions } from './species.actions';
import { Species } from './species.models';
import { SpeciesService } from './species.service';

interface SpeciesStateModel {
  isBusy: boolean;
  species: Species[];
}

const SPECIES_STATE_TOKEN = new StateToken<SpeciesStateModel>('species');

@State<SpeciesStateModel>({
  name: SPECIES_STATE_TOKEN,
  defaults: {
    isBusy: false,
    species: []
  }
})
@Injectable()
export class SpeciesState {
  private readonly speciesService = inject(SpeciesService);

  @Action(SpeciesActions.GetAllSpecies)
  getAllSpecies(context: StateContext<SpeciesStateModel>) {
    const species = context.getState().species;
    if (species.length) {
      return;
    }

    context.patchState({ isBusy: true });
    return this.speciesService.getAllSpecies().pipe(
      tap((species) => {
        context.patchState({ species });
      }),
      finalize(() => {
        context.patchState({ isBusy: false });
      })
    );
  }

  @Selector()
  static allSpecies(state: SpeciesStateModel) {
    return state.species;
  }
}
