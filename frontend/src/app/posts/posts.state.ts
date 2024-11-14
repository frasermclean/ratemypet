import { inject, Injectable } from '@angular/core';
import { Action, createSelector, Selector, State, StateContext } from '@ngxs/store';
import { catchError, tap } from 'rxjs';

import { Post } from '@models/post.model';
import { PostsService } from '@services/posts.service';
import { PostsActions } from './posts.actions';

interface PostsStateModel {
  status: 'initial' | 'busy' | 'error' | 'ready';
  error: any;
  posts: Post[];
}

@State<PostsStateModel>({
  name: 'posts',
  defaults: {
    status: 'initial',
    error: null,
    posts: [],
  },
})
@Injectable()
export class PostsState {
  private readonly postsService = inject(PostsService);

  @Action(PostsActions.SearchPosts)
  searchPosts(context: StateContext<PostsStateModel>) {
    context.patchState({ status: 'busy' });
    return this.postsService.searchPosts().pipe(
      tap((posts) => {
        context.patchState({ status: 'ready', posts });
      }),
      catchError((error) => {
        context.patchState({ status: 'error', error });
        return error;
      })
    );
  }

  @Action(PostsActions.GetPost)
  getPost(context: StateContext<PostsStateModel>, action: PostsActions.GetPost) {
    const state = context.getState();
    if (state.posts.some((post) => post.id === action.postId)) {
      return;
    }

    context.patchState({ status: 'busy' });
    return this.postsService.getPost(action.postId).pipe(
      tap((post) => {
        context.patchState({ status: 'ready', posts: [post] });
      }),
      catchError((error) => {
        context.patchState({ status: 'error', error });
        return error;
      })
    );
  }

  @Action(PostsActions.UpdatePostReaction)
  updatePostReaction(context: StateContext<PostsStateModel>, action: PostsActions.UpdatePostReaction) {
    return this.postsService.updatePostReaction(action.postId, action.reaction).pipe(
      tap((post) => {
        context.patchState({ status: 'ready' });
      }),
      catchError((error) => {
        context.patchState({ status: 'error', error });
        return error;
      })
    );
  }

  @Selector()
  static status(state: PostsStateModel) {
    return state.status;
  }

  @Selector()
  static posts(state: PostsStateModel) {
    return state.posts;
  }

  static getPostById(postId: string) {
    return createSelector([PostsState], (state: PostsStateModel) => {
      return state.posts.find((post) => post.id === postId);
    });
  }
}
