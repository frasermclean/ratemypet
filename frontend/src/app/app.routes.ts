import { Routes } from '@angular/router';
import { HomeComponent } from '@shared/components/home/home.component';
import { NotFoundComponent } from '@shared/components/not-found/not-found.component';

export const routes: Routes = [
  { path: 'auth', loadChildren: () => import('./auth/auth.routes') },
  { path: 'posts', loadChildren: () => import('./posts/posts.routes') },
  { path: '', component: HomeComponent, title: 'Home' },
  { path: '**', component: NotFoundComponent, title: 'Not Found' }
];
