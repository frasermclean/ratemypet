export interface Post {
  id: string;
  title: string;
  caption: string;
  imageUrl: string;
  authorHash: string;
  reactions: {
    like: number;
    funny: number;
    crazy: number;
    wow: number;
    sad: number;
  };
}

export const reactions: Reaction[] = ['like', 'funny', 'crazy', 'wow', 'sad'];
export type Reaction = 'like' | 'funny' | 'crazy' | 'wow' | 'sad';
