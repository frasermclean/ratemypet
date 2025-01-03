import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { Paging } from 'gridify-client';
import { Observable } from 'rxjs';

import { environment } from '../../environments/environment';
import { AddPostRequest, Post, PostComment, PostReactions, Reaction, SearchPostsMatch } from './post.models';

@Injectable()
export class PostsService {
  private readonly httpClient = inject(HttpClient);
  private readonly baseUrl = `${environment.apiBaseUrl}/api/posts`;

  searchPosts(): Observable<Paging<SearchPostsMatch>> {
    return this.httpClient.get<Paging<SearchPostsMatch>>(this.baseUrl);
  }

  getPost(postId: string): Observable<Post> {
    return this.httpClient.get<Post>(`${this.baseUrl}/${postId}`);
  }

  /**
   * Creates a new post
   * @param request Data for the new post
   * @returns Observable with the ID of the new post
   */
  addPost(request: AddPostRequest): Observable<Post> {
    const formData = new FormData();
    formData.append('title', request.title);
    formData.append('description', request.description);
    formData.append('image', request.image);
    formData.append('speciesId', request.speciesId.toString());

    return this.httpClient.post<Post>(this.baseUrl, formData);
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

  addPostComment(postId: string, content: string, parentId?: string) {
    return this.httpClient.post<PostComment>(`${this.baseUrl}/${postId}/comments`, { content, parentId });
  }

  deletePostComment(postId: string, commentId: string) {
    return this.httpClient.delete(`${this.baseUrl}/${postId}/comments/${commentId}`);
  }

  getImageUrl(post: Post | SearchPostsMatch, width: number = 320, height: number = 320) {
    return `${environment.apiBaseUrl}${post.imagePath}?width=${width}&height=${height}&format=webp`;
  }
}
