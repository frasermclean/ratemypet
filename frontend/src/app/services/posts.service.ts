import { inject, Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { map, Observable } from 'rxjs';

import { environment } from '../../environments/environment';
import { AddPostRequest, DetailedPost, Post, PostReactions, Reaction } from '@models/post.models';

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

  addPost(request: AddPostRequest) {
    const formData = new FormData();
    formData.append('title', request.title);
    formData.append('description', request.description);
    formData.append('image', request.image);
    formData.append('speciesId', request.speciesId.toString());

    return this.httpClient.post(this.baseUrl, formData, { observe: 'response' }).pipe(
      map((response) => {
        const location = response.headers.get('Location');
        return location?.split('/').pop();
      })
    );
  }

  deletePost(postId: string) {
    return this.httpClient.delete(`${this.baseUrl}/${postId}`);
  }

  updatePostReaction(postId: string, reaction: Reaction) {
    return this.httpClient.put<PostReactions>(`${this.baseUrl}/${postId}/reactions`, { reaction });
  }

  removePostReaction(postId: string) {
    return this.httpClient.delete<PostReactions>(`${this.baseUrl}/${postId}/reactions`);
  }
}
