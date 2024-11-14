import { inject, Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { finalize, Observable } from 'rxjs';

import { environment } from '../../environments/environment';
import { Post, Reaction } from '@models/post.model';

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
    return this.httpClient.get<Post>(`${this.baseUrl}/${postId}`);
  }

  updatePostReaction(postId: string, reaction: Reaction) {
    return this.httpClient.put<Post>(`${this.baseUrl}/${postId}/reactions`, { reaction });
  }
}
