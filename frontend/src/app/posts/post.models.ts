export interface Post {
  id: string;
  slug: string | null;
  title: string;
  description?: string;
  imageUrl: string | null;
  authorUserName: string;
  authorEmailHash: string;
  speciesId: number;
  createdAt: string;
  updatedAt?: string;
  userReaction?: Reaction;
  reactions: PostReactions;
  comments: PostComment[];
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

export interface UpdatePostRequest {
  id: string;
  title: string;
  description: string;
  speciesId: number;
}

export interface SearchPostsRequest {
  page: number;
  pageSize: number;
  speciesName: string;
  orderBy: SearchPostsOrderBy;
  descending: boolean;
}

export type SearchPostsOrderBy = 'createdAt' | 'reactionCount';

export interface SearchPostsMatch {
  id: string;
  slug: string | null;
  title: string;
  description?: string;
  imageUrl: string | null;
  authorUserName: string;
  authorEmailHash: string;
  speciesName: string;
  createdAt: string;
  updatedAt?: string;
  userReaction?: Reaction;
  reactions: PostReactions;
  reactionCount: number;
  commentCount: number;
}

export enum Reaction {
  Like = 'like',
  Funny = 'funny',
  Crazy = 'crazy',
  Wow = 'wow'
}

export const allReactions: Reaction[] = [Reaction.Like, Reaction.Funny, Reaction.Crazy, Reaction.Wow];
