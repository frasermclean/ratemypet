import { inject, Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { BehaviorSubject, catchError, finalize, Observable } from 'rxjs';

import { environment } from '../../environments/environment';
import { Post } from '@models/post.model';

@Injectable({
  providedIn: 'root',
})
export class PostsService {
  private readonly httpClient = inject(HttpClient);
  private readonly baseUrl = `${environment.apiBaseUrl}/posts`;

  private readonly busySubject = new BehaviorSubject(false);

  get busy$() {
    return this.busySubject.asObservable();
  }

  getPosts(): Observable<Post[]> {
    this.busySubject.next(true);
    return this.httpClient.get<Post[]>(this.baseUrl).pipe(
      catchError(() => []),
      finalize(() => this.busySubject.next(false))
    );
  }

  getPost(id: string): Observable<Post | null> {
    this.busySubject.next(true);
    return this.httpClient.get<Post>(`${this.baseUrl}/${id}`).pipe(
      catchError(() => [null]),
      finalize(() => this.busySubject.next(false))
    );
  }
}
