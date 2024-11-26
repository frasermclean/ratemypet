export interface Post {
  id: string;
  title: string;
  caption: string;
  imageUrl: string;
  authorEmailHash: string;
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

export type Reaction = 'like' | 'funny' | 'crazy' | 'wow' | 'sad';

export const allReactions: Reaction[] = ['like', 'funny', 'crazy', 'wow', 'sad'];
