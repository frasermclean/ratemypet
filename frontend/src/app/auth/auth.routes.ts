import { Routes } from '@angular/router';
import { isAnonymous } from '@shared/guards/anonymous.guard';
import { ConfirmEmailComponent } from './confirm-email/confirm-email.component';
import { ForgotPasswordComponent } from './forgot-password/forgot-password.component';
import { LoginComponent } from './login/login.component';
import { RegisterComponent } from './register/register.component';
import { ResetPasswordComponent } from './reset-password/reset-password.component';

const routes: Routes = [
  {
    path: '',
    pathMatch: 'prefix',
    children: [
      { path: 'login', component: LoginComponent, title: 'Login', canActivate: [isAnonymous] },
      { path: 'register', component: RegisterComponent, title: 'Register', canActivate: [isAnonymous] },
      { path: 'confirm-email', component: ConfirmEmailComponent },
      {
        path: 'forgot-password',
        component: ForgotPasswordComponent,
        title: 'Forgot Password',
        canActivate: [isAnonymous]
      },
      { path: 'reset-password', component: ResetPasswordComponent, title: 'Reset Password', canActivate: [isAnonymous] }
    ]
  }
];

export default routes;
