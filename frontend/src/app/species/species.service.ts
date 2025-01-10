import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { environment } from '../../environments/environment';
import { Species } from './species.models';

@Injectable({ providedIn: 'root' })
export class SpeciesService {
  private readonly httpClient = inject(HttpClient);
  private readonly baseUrl = `${environment.apiBaseUrl}/api/species`;

  getAllSpecies() {
    return this.httpClient.get<Species[]>(this.baseUrl);
  }
}
