import { inject, Injectable } from '@angular/core';
import { MatSnackBar } from '@angular/material/snack-bar';

@Injectable({
  providedIn: 'root'
})
export class NotificationService {
  private readonly snackbar = inject(MatSnackBar);

  public showInformation(message: string, action?: string) {
    return this.snackbar.open(message, action ?? 'Close');
  }

  public showError(message: string, action?: string) {
    return this.snackbar.open(message, action ?? 'Close', { panelClass: 'notification-error' });
  }
}
