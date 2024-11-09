import { Component, inject } from '@angular/core';
import { AsyncPipe } from '@angular/common';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';

import { PostsService } from '@services/posts.service';
import { PostItemComponent } from './post-list-item/post-list-item.component';

@Component({
  selector: 'app-post-list',
  standalone: true,
  imports: [PostItemComponent, AsyncPipe, MatProgressSpinnerModule],
  templateUrl: './post-list.component.html',
  styleUrl: './post-list.component.scss',
})
export class PostListComponent {
  private readonly postsService = inject(PostsService);
  posts$ = this.postsService.getPosts();
  loading$ = this.postsService.busy$;
}
