import { Routes } from '@angular/router';
import { PostViewComponent } from './post-view/post-view.component';
import { PostListComponent } from './post-list/post-list.component';
import { provideStore } from '@ngxs/store';
import { PostsState } from './posts.state';

const routes: Routes = [
  {
    path: '',
    pathMatch: 'prefix',
    children: [
      { path: ':postId', component: PostViewComponent },
      { path: '', component: PostListComponent, title: 'Posts' },
    ],
    providers: [provideStore([PostsState])]
  },
];

export default routes;
