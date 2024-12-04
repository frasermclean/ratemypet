import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';

import { ToolbarComponent } from './core/toolbar/toolbar.component';
import { FooterComponent } from './core/footer/footer.component';

@Component({
  selector: 'app-root',
  imports: [RouterOutlet, ToolbarComponent, FooterComponent],
  templateUrl: './app.component.html',
  styleUrl: './app.component.scss'
})
export class AppComponent {
  title = 'Rate My Pet';
}
