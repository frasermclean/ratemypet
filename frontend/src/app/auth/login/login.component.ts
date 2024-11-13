import { Component, inject, OnDestroy } from '@angular/core';
import { NonNullableFormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatIconModule } from '@angular/material/icon';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { catchError, tap } from 'rxjs';

import { AuthService } from '@services/auth.service';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [
    ReactiveFormsModule,
    RouterLink,
    MatButtonModule,
    MatCardModule,
    MatFormFieldModule,
    MatIconModule,
    MatInputModule,
    MatSnackBarModule,
  ],
  templateUrl: './login.component.html',
  styleUrl: './login.component.scss',
})
export class LoginComponent {
  private readonly formBuilder = inject(NonNullableFormBuilder);
  formGroup = this.formBuilder.group({
    email: ['', Validators.email],
    password: ['', Validators.required],
  });

  private router: Router = inject(Router);
  private readonly authService = inject(AuthService);
  private readonly snackBar = inject(MatSnackBar);

  onSubmit() {
    if (this.formGroup.invalid) {
      return;
    }
    const formValue = this.formGroup.getRawValue();
    this.authService.login(formValue.email, formValue.password).pipe(
      tap(() => {
        this.snackBar.open('Welcome back!', 'Close', { duration: 1500 });
        this.router.navigate(['/']);
      }),
      catchError((error) => {
        this.snackBar.open('Invalid credentials. Please check and try again.', 'Close', { duration: 5000 });
        return error;
      })
    ).subscribe();
  }
}
