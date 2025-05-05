import { ChangeDetectionStrategy, Component } from '@angular/core';
import { MatSidenavModule } from '@angular/material/sidenav';
import { RouterOutlet } from '@angular/router';
import { select } from '@ngxs/store';
import { NavMenuComponent } from '@shared/components/nav-menu/nav-menu.component';
import { ToolbarComponent } from '@shared/components/toolbar/toolbar.component';
import { SharedState } from '@shared/shared.state';

@Component({
  changeDetection: ChangeDetectionStrategy.OnPush,
  selector: 'app-root',
  imports: [RouterOutlet, MatSidenavModule, ToolbarComponent, NavMenuComponent],
  templateUrl: './app.component.html',
  styleUrl: './app.component.scss'
})
export class AppComponent {
  title = select(SharedState.appName);
}
