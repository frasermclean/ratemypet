import { HttpClient, HttpParams } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { GridifyQueryBuilder, ConditionalOperator as op, Paging } from 'gridify-client';
import { Observable } from 'rxjs';
import {
  AddPostRequest,
  Post,
  PostComment,
  PostReactions,
  PostStatus,
  Reaction,
  SearchPostsMatch,
  SearchPostsRequest,
  UpdatePostRequest
} from './post.models';

const DEFAULT_PAGE = 1;
const DEFAULT_PAGE_SIZE = 20;
const DEFAULT_ORDER_BY = 'createdAt';
const DEFAULT_DESCENDING = true;

@Injectable()
export class PostsService {
  private readonly httpClient = inject(HttpClient);
  private readonly baseUrl = '/api/posts';

  searchPosts(request: Partial<SearchPostsRequest>): Observable<Paging<SearchPostsMatch>> {
    const queryBuilder = new GridifyQueryBuilder()
      .setPage(request.page ?? DEFAULT_PAGE)
      .setPageSize(request.pageSize ?? DEFAULT_PAGE_SIZE)
      .addOrderBy(request.orderBy ?? DEFAULT_ORDER_BY, request.descending ?? DEFAULT_DESCENDING);

    if (request.speciesName) {
      queryBuilder.addCondition('speciesName', op.Equal, request.speciesName);
    }

    if (request.tag) {
      queryBuilder.addCondition('tags', op.Contains, request.tag);
    }

    const query = queryBuilder.build();
    const params = new HttpParams({ fromObject: { ...query } });

    return this.httpClient.get<Paging<SearchPostsMatch>>(this.baseUrl, { params });
  }

  getPost(postIdOrSlug: string): Observable<Post> {
    return this.httpClient.get<Post>(`${this.baseUrl}/${postIdOrSlug}`);
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
    formData.append('tags', JSON.stringify(request.tags));

    return this.httpClient.post<Post>(this.baseUrl, formData);
  }

  updatePost(request: UpdatePostRequest): Observable<Post> {
    return this.httpClient.put<Post>(`${this.baseUrl}/${request.id}`, request);
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

  getPostStatus(postIdOrSlug: string): Observable<PostStatus> {
    return this.httpClient.get<PostStatus>(`${this.baseUrl}/${postIdOrSlug}/status`);
  }
}
