import { inject, Injectable } from '@angular/core';
import { Action, Selector, State, StateContext } from '@ngxs/store';
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

@State<PostsStateModel>({
  name: 'posts',
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
      tap((summaries) => {
        context.patchState({ status: 'ready', posts: summaries });
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
      tap((response) => {
        context.patchState({ status: 'ready' });
      }),
      catchError((error) => {
        console.error('Error updating post reaction', error);
        context.patchState({ status: 'error', error });
        return [error];
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

  @Selector()
  static currentPost(state: PostsStateModel) {
    return state.currentPost;
  }
}
