import { HttpErrorResponse, HttpStatusCode } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { Action, Selector, State, StateContext, StateToken } from '@ngxs/store';
import { catchError, filter, finalize, interval, of, switchMap, take, tap } from 'rxjs';
import { Post, PostReactions, Reaction, SearchPostsMatch } from './post.models';
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
    if (currentPost?.id === action.postIdOrSlug || currentPost?.slug === action.postIdOrSlug) {
      return;
    }

    context.patchState({ status: 'busy' });
    return this.postsService.getPost(action.postIdOrSlug).pipe(
      tap((post) => {
        context.patchState({ status: 'ready', currentPost: post });
      }),
      catchError((error: HttpErrorResponse) => {
        if (error.status === HttpStatusCode.NotFound) {
          context.patchState({
            status: 'error',
            currentPost: null,
            errorMessage: `Post with ID ${action.postIdOrSlug} could not be found`
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
      tap((currentPost) => {
        context.patchState({ currentPost });
      }),
      finalize(() => {
        context.patchState({ status: 'ready' });
      })
    );
  }

  @Action(PostsActions.UpdatePost)
  updatePost(context: StateContext<PostsStateModel>, action: PostsActions.UpdatePost) {
    context.patchState({ status: 'busy' });
    return this.postsService.updatePost(action.request).pipe(
      tap((currentPost) => {
        context.patchState({ currentPost });
      }),
      finalize(() => {
        context.patchState({ status: 'ready' });
      })
    );
  }

  @Action(PostsActions.DeletePost)
  deletePost(context: StateContext<PostsStateModel>, action: PostsActions.DeletePost) {
    context.patchState({ status: 'busy' });
    return this.postsService.deletePost(action.postId).pipe(
      tap(() => {
        if (context.getState().currentPost?.id === action.postId) {
          context.patchState({ currentPost: null });
        }
      }),
      finalize(() => {
        context.patchState({ status: 'ready' });
      })
    );
  }

  @Action(PostsActions.PollPostStatus)
  pollPostStatus(context: StateContext<PostsStateModel>, action: PostsActions.PollPostStatus) {
    const currentPost = context.getState().currentPost;

    // post status is already known - no need to poll
    if (currentPost?.status !== 'initial') {
      return;
    }

    return interval(1000).pipe(
      switchMap(() => this.postsService.getPostStatus(action.postIdOrSlug)),
      filter((status) => status !== 'initial'),
      tap((status) => {
        context.patchState({ currentPost: { ...currentPost, status } });
      }),
      take(1)
    );
  }

  @Action(PostsActions.AddPostReaction)
  addPostReaction(context: StateContext<PostsStateModel>, action: PostsActions.AddPostReaction) {
    return this.postsService.addPostReaction(action.postId, action.reaction).pipe(
      tap(() => {
        // update matches
        const matches = context.getState().matches.map((match) => {
          if (match.id !== action.postId) {
            return match;
          }
          const reactions = this.adjustReactions(match.reactions, { increase: action.reaction });
          return { ...match, reactions, userReaction: action.reaction };
        });
        context.patchState({ matches });

        // update current post
        const currentPost = context.getState().currentPost;
        if (currentPost?.id === action.postId) {
          const reactions = this.adjustReactions(currentPost.reactions, { increase: action.reaction });
          context.patchState({ currentPost: { ...currentPost, reactions, userReaction: action.reaction } });
        }
      })
    );
  }

  @Action(PostsActions.UpdatePostReaction)
  updatePostReaction(context: StateContext<PostsStateModel>, action: PostsActions.UpdatePostReaction) {
    return this.postsService.updatePostReaction(action.postId, action.reaction).pipe(
      tap(() => {
        // update matches
        const matches = context.getState().matches.map((match) => {
          if (match.id !== action.postId) {
            return match;
          }
          const reactions = this.adjustReactions(match.reactions, {
            increase: action.reaction,
            decrease: match.userReaction
          });
          return { ...match, reactions, userReaction: action.reaction };
        });
        context.patchState({ matches });

        // update current post
        const currentPost = context.getState().currentPost;
        if (currentPost?.id === action.postId) {
          const reactions = this.adjustReactions(currentPost.reactions, {
            increase: action.reaction,
            decrease: currentPost.userReaction
          });
          context.patchState({ currentPost: { ...currentPost, reactions, userReaction: action.reaction } });
        }
      })
    );
  }

  @Action(PostsActions.RemovePostReaction)
  removePostReaction(context: StateContext<PostsStateModel>, action: PostsActions.RemovePostReaction) {
    return this.postsService.removePostReaction(action.postId).pipe(
      tap(() => {
        // update matches
        const matches = context.getState().matches.map((match) => {
          if (match.id !== action.postId) {
            return match;
          }
          const reactions = this.adjustReactions(match.reactions, { decrease: match.userReaction });
          return { ...match, reactions, userReaction: undefined };
        });
        context.patchState({ matches });

        // update current post
        const currentPost = context.getState().currentPost;
        if (currentPost?.id === action.postId) {
          const reactions = this.adjustReactions(currentPost.reactions, { decrease: currentPost.userReaction });
          context.patchState({ currentPost: { ...currentPost, reactions, userReaction: undefined } });
        }
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

  /**
   * Adjusts the reactions count for a specific reaction type.
   * @param reactions - The current reactions state.
   * @param adjust - An object containing the reaction to increase and/or decrease.*
   * @returns The updated reactions state.
   */
  private adjustReactions(
    reactions: PostReactions,
    adjust: { increase?: Reaction; decrease?: Reaction },
    amount: number = 1
  ): PostReactions {
    const { increase, decrease } = adjust;

    // increase and decrease reactions
    if (increase && decrease) {
      return {
        ...reactions,
        [increase]: (reactions[increase] || 0) + amount,
        [decrease]: (reactions[decrease] || 0) - amount
      };
    }

    // increase reaction only
    if (increase) {
      return {
        ...reactions,
        [increase]: (reactions[increase] || 0) + amount
      };
    }

    // decrease reaction only
    if (decrease) {
      return {
        ...reactions,
        [decrease]: (reactions[decrease] || 0) - amount
      };
    }

    // no adjustments needed, return the original reactions
    return reactions;
  }
}
