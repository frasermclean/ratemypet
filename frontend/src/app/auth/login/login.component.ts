import { HttpErrorResponse, HttpStatusCode } from '@angular/common/http';
import { Component, inject } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { NonNullableFormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { MatProgressBarModule } from '@angular/material/progress-bar';
import { RouterLink } from '@angular/router';
import { Navigate } from '@ngxs/router-plugin';
import { Actions, dispatch, ofActionErrored, ofActionSuccessful, select } from '@ngxs/store';
import { NotificationService } from '@shared/services/notification.service';
import { AuthActions } from '../auth.actions';
import { AuthState } from '../auth.state';

@Component({
  selector: 'app-login',
  imports: [
    ReactiveFormsModule,
    RouterLink,
    MatButtonModule,
    MatCheckboxModule,
    MatCardModule,
    MatFormFieldModule,
    MatIconModule,
    MatInputModule,
    MatProgressBarModule
  ],
  templateUrl: './login.component.html',
  styleUrl: './login.component.scss'
})
export class LoginComponent {
  private readonly formBuilder = inject(NonNullableFormBuilder);
  formGroup = this.formBuilder.group({
    emailOrUserName: ['', Validators.required],
    password: ['', Validators.required],
    rememberMe: [false]
  });

  login = dispatch(AuthActions.Login);
  navigate = dispatch(Navigate);
  isBusy = select(AuthState.isBusy);
  userName = select(AuthState.userName);

  constructor(actions$: Actions, notificationService: NotificationService) {
    // handle successful login
    actions$.pipe(ofActionSuccessful(AuthActions.Login), takeUntilDestroyed()).subscribe(() => {
      this.navigate(['/']);
      notificationService.showInformation(`Welcome back, ${this.userName()}!`);
    });

    // handle login error
    actions$.pipe(ofActionErrored(AuthActions.Login), takeUntilDestroyed()).subscribe((completion) => {
      const error = completion.result.error as HttpErrorResponse;
      if (error.status === HttpStatusCode.Unauthorized) {
        notificationService.showError('Invalid username or password.');
      }
    });
  }

  onSubmit() {
    const formValue = this.formGroup.getRawValue();
    this.login(formValue);
  }
}
