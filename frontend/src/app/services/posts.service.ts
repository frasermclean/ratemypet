import { inject, Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

import { environment } from '../../environments/environment';
import { DetailedPost, Post, PostReactions, Reaction } from '@models/post.models';

@Injectable({
  providedIn: 'root',
})
export class PostsService {
  private readonly httpClient = inject(HttpClient);
  private readonly baseUrl = `${environment.apiBaseUrl}/posts`;

  searchPosts(): Observable<Post[]> {
    return this.httpClient.get<Post[]>(this.baseUrl);
  }

  getPost(postId: string) {
    return this.httpClient.get<DetailedPost>(`${this.baseUrl}/${postId}`);
  }

  updatePostReaction(postId: string, reaction: Reaction) {
    return this.httpClient.put<PostReactions>(`${this.baseUrl}/${postId}/reactions`, { reaction });
  }

  removePostReaction(postId: string) {
    return this.httpClient.delete<PostReactions>(`${this.baseUrl}/${postId}/reactions`);
  }
}
