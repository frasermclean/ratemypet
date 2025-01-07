import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';

import { select } from '@ngxs/store';
import { SharedState } from '@shared/shared.state';
import { FooterComponent } from './core/footer/footer.component';
import { ToolbarComponent } from './core/toolbar/toolbar.component';

@Component({
  selector: 'app-root',
  imports: [RouterOutlet, ToolbarComponent, FooterComponent],
  templateUrl: './app.component.html',
  styleUrl: './app.component.scss'
})
export class AppComponent {
  title = select(SharedState.appName);
}
