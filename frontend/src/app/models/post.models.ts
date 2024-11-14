export interface Post {
  id: string;
  title: string;
  caption: string;
  imageUrl: string;
  authorEmailHash: string;
  likeCount: number;
  funnyCount: number;
  crazyCount: number;
  wowCount: number;
  sadCount: number;
}

export interface DetailedPost extends Post {}

export type Reaction = 'like' | 'funny' | 'crazy' | 'wow' | 'sad';

export const allReactions: Reaction[] = ['like', 'funny', 'crazy', 'wow', 'sad'];
