import { HttpErrorResponse } from '@angular/common/http';
import { Component, inject } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { AbstractControl, NonNullableFormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { MatProgressBarModule } from '@angular/material/progress-bar';
import { RouterLink } from '@angular/router';
import { Navigate } from '@ngxs/router-plugin';
import { ActionCompletion, Actions, dispatch, ofActionErrored, ofActionSuccessful, select } from '@ngxs/store';
import { NotificationService } from '@shared/services/notification.service';
import { AuthActions } from '../auth.actions';
import { AuthState } from '../auth.state';

function mustMatch(controlName: string, matchingControlName: string) {
  return (group: AbstractControl) => {
    const control = group.get(controlName);
    const matchingControl = group.get(matchingControlName);

    if (!control || !matchingControl) {
      return null;
    }

    // return if another validator has already found an error on the matchingControl
    if (matchingControl.hasError('mustMatch')) {
      return null;
    }

    // set error on matchingControl if validation fails
    if (control.value !== matchingControl.value) {
      matchingControl.setErrors({ mustMatch: true });
    } else {
      matchingControl.setErrors(null);
    }
    return null;
  };
}

@Component({
  selector: 'app-register',
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
  templateUrl: './register.component.html',
  styleUrl: './register.component.scss'
})
export class RegisterComponent {
  private readonly formBuilder = inject(NonNullableFormBuilder);
  private readonly notificationService = inject(NotificationService);
  formGroup = this.formBuilder.group(
    {
      userName: ['', [Validators.required, Validators.minLength(3), Validators.minLength(3), Validators.maxLength(30)]],
      emailAddress: ['', [Validators.required, Validators.email]],
      password: ['', [Validators.required, Validators.minLength(8)]],
      confirmPassword: ['', Validators.required]
    },
    {
      validators: mustMatch('password', 'confirmPassword')
    }
  );

  isBusy = select(AuthState.isBusy);
  register = dispatch(AuthActions.Register);
  navigate = dispatch(Navigate);

  constructor(actions$: Actions) {
    actions$.pipe(ofActionSuccessful(AuthActions.Register), takeUntilDestroyed()).subscribe(() => {
      this.handleRegisterSuccess();
    });

    actions$.pipe(ofActionErrored(AuthActions.Register), takeUntilDestroyed()).subscribe((completion) => {
      this.handleRegisterError(completion);
    });
  }

  private handleRegisterSuccess() {
    const message = 'Registration successful. Please check your email for a confirmation link.';
    this.notificationService.showInformation(message);
    this.navigate(['/']);
  }

  private handleRegisterError(completion: ActionCompletion<AuthActions.Register, Error>) {
    const response = completion.result.error as HttpErrorResponse;
    const errors = response.error.errors;

    if (errors.duplicateUserName) {
      this.formGroup.controls.userName.setErrors({ duplicateUserName: true });
    }

    if (errors.duplicateEmail) {
      this.formGroup.controls.emailAddress.setErrors({ duplicateEmail: true });
    }

    this.notificationService.showError('An error occured while trying to register.');
  }
}
