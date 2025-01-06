import { Component, input } from '@angular/core';
import { MatButtonToggleModule } from '@angular/material/button-toggle';
import { MatExpansionModule } from '@angular/material/expansion';
import { MatIconModule } from '@angular/material/icon';

@Component({
  selector: 'app-search-controls',
  imports: [MatButtonToggleModule, MatExpansionModule, MatIconModule],
  templateUrl: './search-controls.component.html',
  styleUrl: './search-controls.component.scss'
})
export class SearchControlsComponent {
  expanded = input(false);
}
