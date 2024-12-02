import { inject, Injectable } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { MatSnackBar } from '@angular/material/snack-bar';
import { Navigate } from '@ngxs/router-plugin';
import { Action, Selector, State, StateContext, StateToken } from '@ngxs/store';
import { catchError, of, tap } from 'rxjs';

import { PostsActions } from './posts.actions';
import { PostsService } from './posts.service';

import { PostEditComponent } from './post-edit/post-edit.component';
import { AddPostRequest, GetPostResponse, SearchPostsMatch } from './post.models';

interface PostsStateModel {
  status: 'initial' | 'busy' | 'error' | 'ready';
  error: any;
  matches: SearchPostsMatch[];
  totalMatches: number;
  currentPost: GetPostResponse | null;
}

const POSTS_STATE_TOKEN = new StateToken<PostsStateModel>('posts');

@State<PostsStateModel>({
  name: POSTS_STATE_TOKEN,
  defaults: {
    status: 'initial',
    error: null,
    matches: [],
    totalMatches: 0,
    currentPost: null,
  },
})
@Injectable()
export class PostsState {
  private readonly postsService = inject(PostsService);
  private readonly dialog = inject(MatDialog);
  private readonly snackbar = inject(MatSnackBar);

  @Action(PostsActions.SearchPosts)
  searchPosts(context: StateContext<PostsStateModel>) {
    context.patchState({ status: 'busy' });
    return this.postsService.searchPosts().pipe(
      tap((paging) => {
        context.patchState({ status: 'ready', matches: paging.data, totalMatches: paging.count });
      }),
      catchError((error) => {
        context.patchState({ status: 'error', error, matches: [], totalMatches: 0 });
        return of([]);
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
        return of([]);
      })
    );
  }

  @Action(PostsActions.OpenPostEditDialog)
  openPostEditDialog(context: StateContext<PostsStateModel>) {
    return this.dialog
      .open<PostEditComponent, any, AddPostRequest>(PostEditComponent)
      .afterClosed()
      .pipe(
        tap((request) => {
          if (!request) return;
          context.dispatch(new PostsActions.AddPost(request));
        })
      );
  }

  @Action(PostsActions.AddPost)
  addPost(context: StateContext<PostsStateModel>, action: PostsActions.AddPost) {
    context.patchState({ status: 'busy' });
    return this.postsService.addPost(action.request).pipe(
      tap((postId) => {
        context.patchState({ status: 'ready' });
        context.dispatch(new Navigate(['/posts', postId]));
        this.snackbar.open('Post created successfully', 'Close', { duration: 3000 });
      }),
      catchError((error) => {
        context.patchState({ status: 'error', error });
        throw error;
      })
    );
  }

  @Action(PostsActions.DeletePost)
  deletePost(context: StateContext<PostsStateModel>, action: PostsActions.DeletePost) {
    context.patchState({ status: 'busy' });
    return this.postsService.deletePost(action.postId).pipe(
      tap(() => {
        context.patchState({ status: 'ready' });
        context.dispatch(new Navigate(['/posts']));
        this.snackbar.open('Post deleted successfully', 'Close', { duration: 3000 });
      }),
      catchError((error) => {
        context.patchState({ status: 'error', error });
        throw error;
      })
    );
  }

  @Action(PostsActions.UpdatePostReaction)
  updatePostReaction(context: StateContext<PostsStateModel>, action: PostsActions.UpdatePostReaction) {
    return this.postsService.updatePostReaction(action.postId, action.reaction).pipe(
      tap((reactions) => {
        const state = context.getState();
        const matches = state.matches.map((match) =>
          match.id === action.postId ? { ...match, reactions, userReaction: action.reaction } : match
        );
        context.patchState({ status: 'ready', matches });
      }),
      catchError(() => {
        this.snackbar.open('Could not update post reaction', 'Close', { duration: 3000 });
        return of([]);
      })
    );
  }

  @Action(PostsActions.RemovePostReaction)
  removePostReaction(context: StateContext<PostsStateModel>, action: PostsActions.RemovePostReaction) {
    return this.postsService.removePostReaction(action.postId).pipe(
      tap((reactions) => {
        const state = context.getState();
        const matches = state.matches.map((match) =>
          match.id === action.postId ? { ...match, reactions, userReaction: undefined } : match
        );
        context.patchState({ status: 'ready', matches });
      }),
      catchError(() => {
        this.snackbar.open('Could not remove post reaction', 'Close', { duration: 3000 });
        return of([]);
      })
    );
  }

  @Selector([POSTS_STATE_TOKEN])
  static status(state: PostsStateModel) {
    return state.status;
  }

  @Selector([POSTS_STATE_TOKEN])
  static matches(state: PostsStateModel) {
    return state.matches;
  }

  @Selector([POSTS_STATE_TOKEN])
  static currentPost(state: PostsStateModel) {
    return state.currentPost;
  }
}
