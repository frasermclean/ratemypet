import { Component, inject } from '@angular/core';
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
  selector: 'app-login',
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
  templateUrl: './login.component.html',
  styleUrl: './login.component.scss'
})
export class LoginComponent {
  private readonly formBuilder = inject(NonNullableFormBuilder);
  formGroup = this.formBuilder.group({
    emailOrUserName: ['', Validators.required],
    password: ['', Validators.required]
  });

  login = dispatch(AuthActions.Login);
  status = select(AuthState.status);

  onSubmit() {
    const formValue = this.formGroup.getRawValue();
    this.login(formValue);
  }
}
