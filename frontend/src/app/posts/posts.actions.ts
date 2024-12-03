import { AddPostRequest, Reaction } from './post.models';

export namespace PostsActions {
  export class SearchPosts {
    static readonly type = '[Posts] Search Posts';
  }

  export class GetPost {
    static readonly type = '[Posts] Get Post';
    constructor(public postId: string) {}
  }

  export class OpenPostEditDialog {
    static readonly type = '[Posts] Open Post Edit Dialog';
  }

  export class AddPost {
    static readonly type = '[Posts] Add Post';
    constructor(public request: AddPostRequest) {}
  }

  export class DeletePost {
    static readonly type = '[Posts] Delete Post';
    constructor(public postId: string) {}
  }

  export class GetPostReactions {
    static readonly type = '[Posts] Get Post Reactions';
    constructor(public postId: string) {}
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
}
