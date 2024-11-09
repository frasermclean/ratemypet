import { Component, inject, Input } from '@angular/core';
import { AsyncPipe } from '@angular/common';
import { MatCardModule } from '@angular/material/card';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { Observable } from 'rxjs';

import { PostsService } from '@services/posts.service';
import { Post } from '@models/post.model';

@Component({
  selector: 'app-post-view',
  standalone: true,
  imports: [AsyncPipe, MatCardModule, MatProgressSpinnerModule],
  templateUrl: './post-view.component.html',
  styleUrl: './post-view.component.scss',
})
export class PostViewComponent {
  private readonly postsService = inject(PostsService);
  post$!: Observable<Post | null>;
  loading$ = this.postsService.busy$;

  @Input()
  set postId(value: string) {
    this.post$ = this.postsService.getPost(value);
  }
}
