import { ErrorHandler, inject } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { ErrorComponent } from './error.component';

export class GlobalErrorHandler implements ErrorHandler {
  dialog = inject(MatDialog);

  handleError(error: any): void {
    console.error(error);
    this.dialog.open(ErrorComponent, { data: { error } });
  }
}
