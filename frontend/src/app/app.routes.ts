import { Routes } from '@angular/router';
import { PostListComponent } from './components/post-list/post-list.component';
import { PostViewComponent } from './components/post-view/post-view.component';
import { LoginComponent } from './components/login/login.component';

export const routes: Routes = [
  { path: 'posts/:postId', component: PostViewComponent },
  { path: 'posts', component: PostListComponent },
  { path: 'login', component: LoginComponent },
  { path: '', redirectTo: '/posts', pathMatch: 'full' }
];
