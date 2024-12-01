import { inject, Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Paging } from 'gridify-client';
import { map, Observable } from 'rxjs';

import { environment } from '../../environments/environment';
import { AddPostRequest, GetPostResponse, PostReactions, Reaction, SearchPostsMatch } from '@models/post.models';

@Injectable({
  providedIn: 'root',
})
export class PostsService {
  private readonly httpClient = inject(HttpClient);
  private readonly baseUrl = `${environment.apiBaseUrl}/posts`;

  searchPosts(): Observable<Paging<SearchPostsMatch>> {
    return this.httpClient.get<Paging<SearchPostsMatch>>(this.baseUrl);
  }

  getPost(postId: string): Observable<GetPostResponse> {
    return this.httpClient.get<GetPostResponse>(`${this.baseUrl}/${postId}`);
  }

  /**
   * Creates a new post
   * @param request Data for the new post
   * @returns Observable with the ID of the new post
   */
  addPost(request: AddPostRequest): Observable<string> {
    const formData = new FormData();
    formData.append('title', request.title);
    formData.append('description', request.description);
    formData.append('image', request.image);
    formData.append('speciesId', request.speciesId.toString());

    return this.httpClient.post(this.baseUrl, formData, { observe: 'response' }).pipe(
      map((response) => {
        const location = response.headers.get('Location');
        const postId = location?.split('/').pop();
        if (!postId) {
          throw new Error('Could not get the ID of the new post');
        }
        return postId;
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
