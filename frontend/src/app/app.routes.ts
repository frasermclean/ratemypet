import { Routes } from '@angular/router';

import { HomeComponent } from './core/home/home.component';
import { NotFoundComponent } from './core/not-found/not-found.component';

export const routes: Routes = [
  { path: 'auth', loadChildren: () => import('./auth/auth.routes') },
  { path: 'posts', loadChildren: () => import('./posts/posts.routes') },
  { path: '', component: HomeComponent, title: 'Home' },
  { path: '**', component: NotFoundComponent, title: 'Not Found' },
];
