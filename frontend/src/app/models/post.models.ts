export interface Post {
  id: string;
  title: string;
  caption: string;
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

export enum Reaction {
  Like = 'like',
  Funny = 'funny',
  Crazy = 'crazy',
  Wow = 'wow',
  Sad = 'sad',
}

export const allReactions: Reaction[] = [Reaction.Like, Reaction.Funny, Reaction.Crazy, Reaction.Wow, Reaction.Sad];
