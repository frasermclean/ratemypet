import { Component } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MatDialogModule } from '@angular/material/dialog';

@Component({
  imports: [MatButtonModule, MatDialogModule],
  templateUrl: './error.component.html'
})
export class ErrorComponent {}
