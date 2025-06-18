import { AddPostRequest, Reaction, SearchPostsRequest, UpdatePostRequest } from './post.models';

export namespace PostsActions {
  export class SearchPosts {
    static readonly type = '[Posts] Search Posts';
    constructor(public request: Partial<SearchPostsRequest> = {}) {}
  }

  export class GetPost {
    static readonly type = '[Posts] Get Post';
    constructor(public postIdOrSlug: string) {}
  }

  export class AddPost {
    static readonly type = '[Posts] Add Post';
    constructor(public request: AddPostRequest) {}
  }

  export class UpdatePost {
    static readonly type = '[Posts] Update Post';
    constructor(public request: UpdatePostRequest) {}
  }

  export class DeletePost {
    static readonly type = '[Posts] Delete Post';
    constructor(public postId: string) {}
  }

  export class PollPostStatus {
    static readonly type = '[Posts] Poll Post Status';
    constructor(public postIdOrSlug: string) {}
  }

  export class GetPostReactions {
    static readonly type = '[Posts] Get Post Reactions';
    constructor(public postId: string) {}
  }

  export class AddPostReaction {
    static readonly type = '[Posts] Add Post Reaction';
    constructor(public postId: string, public reaction: Reaction) {}
  }

  export class UpdatePostReaction {
    static readonly type = '[Posts] Update Post Reaction';
    constructor(public postId: string, public reaction: Reaction) {}
  }

  export class RemovePostReaction {
    static readonly type = '[Posts] Remove Post Reaction';
    constructor(public postId: string) {}
  }

  export class AddPostComment {
    static readonly type = '[Posts] Add Post Comment';
    constructor(public postId: string, public content: string, public parentId?: string) {}
  }

  export class DeletePostComment {
    static readonly type = '[Posts] Delete Post Comment';
    constructor(public postId: string, public commentId: string) {}
  }
}
