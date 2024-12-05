import { Component, inject } from '@angular/core';
import { NonNullableFormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { MatProgressBarModule } from '@angular/material/progress-bar';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { Router, RouterLink } from '@angular/router';
import { select, Store } from '@ngxs/store';
import { catchError, of, tap } from 'rxjs';
import { AuthActions } from '../auth.actions';
import { AuthState } from '../auth.state';

@Component({
  selector: 'app-login',
  imports: [
    ReactiveFormsModule,
    RouterLink,
    MatButtonModule,
    MatCardModule,
    MatFormFieldModule,
    MatIconModule,
    MatInputModule,
    MatSnackBarModule,
    MatProgressBarModule
  ],
  templateUrl: './login.component.html',
  styleUrl: './login.component.scss'
})
export class LoginComponent {
  private readonly formBuilder = inject(NonNullableFormBuilder);
  formGroup = this.formBuilder.group({
    emailOrUserName: ['', Validators.required],
    password: ['', Validators.required]
  });

  private readonly router = inject(Router);
  private readonly store = inject(Store);
  private readonly snackBar = inject(MatSnackBar);

  status = select(AuthState.status);

  onSubmit() {
    if (this.formGroup.invalid) {
      return;
    }
    const formValue = this.formGroup.getRawValue();
    this.store
      .dispatch(new AuthActions.Login(formValue))
      .pipe(
        tap(() => {
          this.snackBar.open('Welcome back!', 'Thanks');
          this.router.navigate(['/']);
        }),
        catchError(() => {
          this.snackBar.open('Invalid credentials. Please check and try again.', 'Close');
          return of([]);
        })
      )
      .subscribe();
  }
}
