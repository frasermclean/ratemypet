export interface Post {
  id: string;
  title: string;
  description?: string;
  imageUrl: string;
  authorEmailHash: string;
  userReaction?: Reaction;
  reactions: PostReactions;
}

export interface PostReactions {
  like: number;
  funny: number;
  crazy: number;
  wow: number;
  sad: number;
}

export interface DetailedPost extends Post {}

export interface AddPostRequest {
  title: string;
  description: string;
  image: File;
  speciesId: number;
}

export enum Reaction {
  Like = 'like',
  Funny = 'funny',
  Crazy = 'crazy',
  Wow = 'wow',
  Sad = 'sad',
}

export const allReactions: Reaction[] = [Reaction.Like, Reaction.Funny, Reaction.Crazy, Reaction.Wow, Reaction.Sad];
