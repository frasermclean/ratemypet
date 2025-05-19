import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { Species } from './species.models';

@Injectable({ providedIn: 'root' })
export class SpeciesService {
  private readonly httpClient = inject(HttpClient);
  private readonly baseUrl = '/api/species';

  getAllSpecies() {
    return this.httpClient.get<Species[]>(this.baseUrl);
  }
}
