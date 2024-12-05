import { Routes } from '@angular/router';
import { LoginComponent } from './login/login.component';
import { RegisterComponent } from './register/register.component';
import { ConfirmEmailComponent } from './confirm-email/confirm-email.component';
import { isAnonymous } from '@shared/guards/anonymous.guard';

const routes: Routes = [
  {
    path: '',
    pathMatch: 'prefix',
    children: [
      { path: 'login', component: LoginComponent, title: 'Login', canActivate: [isAnonymous] },
      { path: 'register', component: RegisterComponent, title: 'Register', canActivate: [isAnonymous] },
      { path: 'confirm-email', component: ConfirmEmailComponent }
    ]
  }
];

export default routes;
