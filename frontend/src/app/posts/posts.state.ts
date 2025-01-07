import { HttpErrorResponse, HttpStatusCode } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { Action, Selector, State, StateContext, StateToken } from '@ngxs/store';
import { catchError, finalize, of, tap } from 'rxjs';
import { Post, SearchPostsMatch } from './post.models';
import { PostsActions } from './posts.actions';
import { PostsService } from './posts.service';

interface PostsStateModel {
  status: 'ready' | 'busy' | 'error';
  errorMessage: string | null;
  matches: SearchPostsMatch[];
  totalMatches: number;
  currentPost: Post | null;
}

const POSTS_STATE_TOKEN = new StateToken<PostsStateModel>('posts');

@State<PostsStateModel>({
  name: POSTS_STATE_TOKEN,
  defaults: {
    status: 'ready',
    errorMessage: null,
    matches: [],
    totalMatches: 0,
    currentPost: null
  }
})
@Injectable()
export class PostsState {
  private readonly postsService = inject(PostsService);

  @Action(PostsActions.SearchPosts)
  searchPosts(context: StateContext<PostsStateModel>, action: PostsActions.SearchPosts) {
    context.patchState({ status: 'busy' });
    return this.postsService.searchPosts(action.request).pipe(
      tap((paging) => {
        context.patchState({ matches: paging.data, totalMatches: paging.count });
      }),
      finalize(() => {
        context.patchState({ status: 'ready' });
      })
    );
  }

  @Action(PostsActions.GetPost)
  getPost(context: StateContext<PostsStateModel>, action: PostsActions.GetPost) {
    // prevent fetching the same post multiple times
    const currentPost = context.getState().currentPost;
    if (currentPost?.id === action.postId) {
      return;
    }

    context.patchState({ status: 'busy' });
    return this.postsService.getPost(action.postId).pipe(
      tap((post) => {
        context.patchState({ status: 'ready', currentPost: post });
      }),
      catchError((error: HttpErrorResponse) => {
        if (error.status === HttpStatusCode.NotFound) {
          context.patchState({
            status: 'error',
            currentPost: null,
            errorMessage: `Post with ID ${action.postId} could not be found`
          });
          return of(null);
        }

        throw error;
      })
    );
  }

  @Action(PostsActions.AddPost)
  addPost(context: StateContext<PostsStateModel>, action: PostsActions.AddPost) {
    context.patchState({ status: 'busy' });
    return this.postsService.addPost(action.request).pipe(
      tap((post) => {
        context.patchState({ status: 'ready', currentPost: post });
      })
    );
  }

  @Action(PostsActions.DeletePost)
  deletePost(context: StateContext<PostsStateModel>, action: PostsActions.DeletePost) {
    context.patchState({ status: 'busy' });
    return this.postsService.deletePost(action.postId).pipe(
      tap(() => {
        context.patchState({ status: 'ready' });
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
      })
    );
  }

  @Action(PostsActions.AddPostComment)
  addPostComment(context: StateContext<PostsStateModel>, action: PostsActions.AddPostComment) {
    return this.postsService.addPostComment(action.postId, action.content, action.parentId).pipe(
      tap((comment) => {
        const currentPost = context.getState().currentPost;
        if (currentPost) {
          context.patchState({
            currentPost: {
              ...currentPost,
              comments: [...currentPost.comments, comment]
            }
          });
        }
      })
    );
  }

  @Action(PostsActions.DeletePostComment)
  deletePostComment(context: StateContext<PostsStateModel>, action: PostsActions.DeletePostComment) {
    return this.postsService.deletePostComment(action.postId, action.commentId).pipe(
      tap(() => {
        const currentPost = context.getState().currentPost;
        if (currentPost) {
          const comments = currentPost.comments.filter((comment) => comment.id !== action.commentId);
          context.patchState({ currentPost: { ...currentPost, comments } });
        }
      })
    );
  }

  @Selector([POSTS_STATE_TOKEN])
  static isBusy(state: PostsStateModel) {
    return state.status === 'busy';
  }

  @Selector([POSTS_STATE_TOKEN])
  static status(state: PostsStateModel) {
    return state.status;
  }

  @Selector([POSTS_STATE_TOKEN])
  static errorMessage(state: PostsStateModel) {
    return state.errorMessage;
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
