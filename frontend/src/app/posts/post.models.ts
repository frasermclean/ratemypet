export interface SearchPostsMatch {
  id: string;
  title: string;
  description?: string;
  imageUrl: string;
  authorUserName: string;
  authorEmailHash: string;
  speciesName: string;
  createdAtUtc: string;
  updatedAtUtc?: string;
  userReaction?: Reaction;
  reactions: PostReactions;
  commentCount: number;
}

export interface GetPostResponse {
  id: string;
  title: string;
  description?: string;
  imageUrl: string;
  authorUserName: string;
  authorEmailHash: string;
  speciesName: string;
  createdAtUtc: string;
  updatedAtUtc?: string;
  userReaction?: Reaction;
  reactions: PostReactions;
  comments: PostComment[];
  commentCount: number;
}

export interface PostReactions {
  like: number;
  funny: number;
  crazy: number;
  wow: number;
  sad: number;
}

export interface PostComment {
  id: string;
  content: string;
  authorUserName: string;
  createdAtUtc: string;
  updatedAtUtc?: string;
  replies?: PostComment[];
}

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
