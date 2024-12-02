import { Routes } from '@angular/router';

import { LoginComponent } from './auth/login/login.component';
import { ConfirmEmailComponent } from './auth/confirm-email/confirm-email.component';
import { RegisterComponent } from './auth/register/register.component';
import { HomeComponent } from './core/home/home.component';
import { NotFoundComponent } from './core/not-found/not-found.component';

export const routes: Routes = [
  { path: 'auth/login', component: LoginComponent, title: 'Login' },
  { path: 'auth/register', component: RegisterComponent, title: 'Register' },
  { path: 'auth/confirm-email', component: ConfirmEmailComponent },
  { path: 'posts', loadChildren: () => import('./posts/posts.routes') },
  { path: '', component: HomeComponent, title: 'Home' },
  { path: '**', component: NotFoundComponent, title: 'Not Found' },
];
