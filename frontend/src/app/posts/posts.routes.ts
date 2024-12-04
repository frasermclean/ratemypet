import { Routes } from '@angular/router';
import { PostViewComponent } from './post-view/post-view.component';
import { PostListComponent } from './post-list/post-list.component';
import { provideStates } from '@ngxs/store';
import { PostsState } from './posts.state';
import { PostEditComponent } from './post-edit/post-edit.component';

const routes: Routes = [
  {
    path: '',
    pathMatch: 'prefix',
    children: [
      { path: 'add', component: PostEditComponent },
      { path: ':postId', component: PostViewComponent },
      { path: '', component: PostListComponent, title: 'Posts' },
    ],
    providers: [provideStates([PostsState])],
  },
];

export default routes;
