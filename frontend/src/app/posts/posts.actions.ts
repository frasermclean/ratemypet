import { Reaction } from "@models/post.models";

export namespace PostsActions {
  export class SearchPosts {
    static readonly type = '[Posts] Search Posts';
  }

  export class GetPost {
    static readonly type = '[Posts] Get Post';
    constructor(public postId: string) {}
  }

  export class UpdatePostReaction {
    static readonly type = '[Posts] Update Post Reaction';
    constructor(public postId: string, public reaction: Reaction) {}
  }
}
