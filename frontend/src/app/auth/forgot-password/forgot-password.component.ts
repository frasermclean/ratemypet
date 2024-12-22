import { Component, inject, signal } from '@angular/core';
import { NonNullableFormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { MatProgressBarModule } from '@angular/material/progress-bar';
import { RouterLink } from '@angular/router';
import { dispatch, select } from '@ngxs/store';
import { AuthActions } from '../auth.actions';
import { AuthState } from '../auth.state';

@Component({
  selector: 'app-forgot-password',
  imports: [
    ReactiveFormsModule,
    RouterLink,
    MatButtonModule,
    MatCardModule,
    MatFormFieldModule,
    MatIconModule,
    MatInputModule,
    MatProgressBarModule
  ],
  templateUrl: './forgot-password.component.html',
  styleUrl: './forgot-password.component.scss'
})
export class ForgotPasswordComponent {
  private readonly formBuilder = inject(NonNullableFormBuilder);
  formGroup = this.formBuilder.group({
    emailAddress: ['', [Validators.required, Validators.email]]
  });
  isBusy = select(AuthState.isBusy);
  forgotPassword = dispatch(AuthActions.ForgotPassword);
  hasSubmitted = signal(false);

  onSubmit() {
    const emailAddress = this.formGroup.getRawValue().emailAddress;
    this.formGroup.controls.emailAddress.disable();
    this.forgotPassword(emailAddress).subscribe(() => {
      this.hasSubmitted.set(true);
    });
  }
}
