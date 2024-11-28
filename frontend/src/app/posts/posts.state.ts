import { inject, Injectable } from '@angular/core';
import { Action, Selector, State, StateContext, StateToken } from '@ngxs/store';
import { catchError, tap } from 'rxjs';

import { PostsService } from '@services/posts.service';
import { PostsActions } from './posts.actions';
import { DetailedPost, Post } from '@models/post.models';

interface PostsStateModel {
  status: 'initial' | 'busy' | 'error' | 'ready';
  error: any;
  posts: Post[];
  currentPost: DetailedPost | null;
}

const POSTS_STATE_TOKEN = new StateToken<PostsStateModel>('posts');

@State<PostsStateModel>({
  name: POSTS_STATE_TOKEN,
  defaults: {
    status: 'initial',
    error: null,
    posts: [],
    currentPost: null,
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
    context.patchState({ status: 'busy' });
    return this.postsService.getPost(action.postId).pipe(
      tap((post) => {
        context.patchState({ status: 'ready', currentPost: post });
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
      tap((reactions) => {
        const state = context.getState();
        const posts = state.posts.map((post) =>
          post.id === action.postId ? { ...post, reactions, userReaction: action.reaction } : post
        );
        context.patchState({ status: 'ready', posts });
      }),
      catchError((error) => {
        console.error('Error updating post reaction', error);
        context.patchState({ status: 'error', error });
        return error;
      })
    );
  }

  @Action(PostsActions.RemovePostReaction)
  removePostReaction(context: StateContext<PostsStateModel>, action: PostsActions.RemovePostReaction) {
    return this.postsService.removePostReaction(action.postId).pipe(
      tap((reactions) => {
        const state = context.getState();
        const posts = state.posts.map((post) =>
          post.id === action.postId ? { ...post, reactions, userReaction: undefined } : post
        );
        context.patchState({ status: 'ready', posts });
      }),
      catchError((error) => {
        console.error('Error removing post reaction', error);
        context.patchState({ status: 'error', error });
        return error;
      })
    );
  }

  @Selector([POSTS_STATE_TOKEN])
  static status(state: PostsStateModel) {
    return state.status;
  }

  @Selector([POSTS_STATE_TOKEN])
  static posts(state: PostsStateModel) {
    return state.posts;
  }

  @Selector([POSTS_STATE_TOKEN])
  static currentPost(state: PostsStateModel) {
    return state.currentPost;
  }
}
