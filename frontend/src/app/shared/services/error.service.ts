import { inject, Injectable } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { IErrorService } from '@microsoft/applicationinsights-angularplugin-js';
import { ErrorComponent } from '@shared/components/error/error.component';

@Injectable({
  providedIn: 'root'
})
export class ErrorService implements IErrorService {
  dialog = inject(MatDialog);

  handleError(error: any): void {
    console.error(error);
    this.dialog.open(ErrorComponent, { data: { error } });
  }
}
