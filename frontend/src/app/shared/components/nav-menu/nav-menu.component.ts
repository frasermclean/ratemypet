import { Component, output } from '@angular/core';
import { MatIconModule } from '@angular/material/icon';
import { MatListModule } from '@angular/material/list';
import { RouterLink, RouterLinkActive } from '@angular/router';

const LINKS = [
  { label: 'Home', path: '/', icon: 'home' },
  { label: 'Posts', path: '/posts', icon: 'photo_library' }
];

@Component({
  selector: 'app-nav-menu',
  imports: [RouterLink, RouterLinkActive, MatIconModule, MatListModule],
  templateUrl: './nav-menu.component.html',
  styleUrl: './nav-menu.component.scss'
})
export class NavMenuComponent {
  itemClicked = output<void>();
  readonly links = LINKS;
}
