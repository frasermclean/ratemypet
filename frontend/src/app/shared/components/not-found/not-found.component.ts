import { ChangeDetectionStrategy, Component, inject, OnInit, RESPONSE_INIT } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { RouterLink } from '@angular/router';

@Component({
  changeDetection: ChangeDetectionStrategy.OnPush,
  imports: [RouterLink, MatButtonModule, MatIconModule],
  templateUrl: './not-found.component.html',
  styleUrl: './not-found.component.scss'
})
export class NotFoundComponent implements OnInit {
  private readonly responseInit = inject(RESPONSE_INIT);

  ngOnInit(): void {
    if (this.responseInit) {
      this.responseInit.status = 404;
    }
  }
}
