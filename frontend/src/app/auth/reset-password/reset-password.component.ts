import { Component, inject, OnInit } from '@angular/core';
import { NonNullableFormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { MatProgressBarModule } from '@angular/material/progress-bar';
import { ActivatedRoute } from '@angular/router';
import { dispatch, select } from '@ngxs/store';
import { AuthActions } from '../auth.actions';
import { AuthState } from '../auth.state';

@Component({
  selector: 'app-reset-password',
  imports: [
    ReactiveFormsModule,
    MatButtonModule,
    MatCardModule,
    MatFormFieldModule,
    MatIconModule,
    MatInputModule,
    MatProgressBarModule
  ],
  templateUrl: './reset-password.component.html',
  styleUrl: './reset-password.component.scss'
})
export class ResetPasswordComponent implements OnInit {
  private route = inject(ActivatedRoute);
  private readonly formBuilder = inject(NonNullableFormBuilder);
  formGroup = this.formBuilder.group({
    emailAddress: ['', [Validators.required, Validators.email]],
    resetCode: ['', [Validators.required]],
    newPassword: ['', [Validators.required, Validators.minLength(8)]]
  });

  status = select(AuthState.status);
  resetPassword = dispatch(AuthActions.ResetPassword);

  ngOnInit(): void {
    const emailAddress = this.route.snapshot.queryParamMap.get('emailAddress');
    if (emailAddress) {
      this.formGroup.controls.emailAddress.setValue(emailAddress);
      this.formGroup.controls.emailAddress.disable();
    }

    const resetCode = this.route.snapshot.queryParamMap.get('resetCode');
    if (resetCode) {
      this.formGroup.controls.resetCode.setValue(resetCode);
      this.formGroup.controls.resetCode.disable();
    }
  }

  onSubmit() {
    this.resetPassword(this.formGroup.getRawValue());
  }
}
