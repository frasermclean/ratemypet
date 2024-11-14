import { Routes } from '@angular/router';

import { LoginComponent } from './auth/login/login.component';
import { PostListComponent } from './posts/post-list/post-list.component';
import { PostViewComponent } from './posts/post-view/post-view.component';

export const routes: Routes = [
  { path: 'auth/login', component: LoginComponent },
  { path: 'posts/:postId', component: PostViewComponent },
  { path: 'posts', component: PostListComponent },
  { path: '', redirectTo: '/posts', pathMatch: 'full' },
];
