import { Component, inject } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MAT_DIALOG_DATA, MatDialogModule } from '@angular/material/dialog';

export interface ConfirmationData {
  title: string;
  message: string;
  confirmText: string;
  cancelText: string;
}

@Component({
  selector: 'app-confirmation',
  imports: [MatDialogModule, MatButtonModule],
  templateUrl: './confirmation.component.html'
})
export class ConfirmationComponent {
  data: ConfirmationData = inject(MAT_DIALOG_DATA);
}
