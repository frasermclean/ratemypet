import { Routes } from '@angular/router';
import { provideStates } from '@ngxs/store';
import { isAuthenticated } from '@shared/guards/authenticated.guard';
import { PostEditComponent } from './post-edit/post-edit.component';
import { PostListComponent } from './post-list/post-list.component';
import { PostViewComponent } from './post-view/post-view.component';
import { PostsService } from './posts.service';
import { PostsState } from './posts.state';

const routes: Routes = [
  {
    path: '',
    pathMatch: 'prefix',
    children: [
      { path: 'add', component: PostEditComponent, canActivate: [isAuthenticated] },
      { path: ':postId', component: PostViewComponent },
      { path: '', component: PostListComponent, title: 'Posts' }
    ],
    providers: [provideStates([PostsState]), PostsService]
  }
];

export default routes;
