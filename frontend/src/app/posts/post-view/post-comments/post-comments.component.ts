import { Component, input, TrackByFunction } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MatTreeModule } from '@angular/material/tree';

import { PostComment } from '../../post.models';
import { MatIconModule } from '@angular/material/icon';
import { PostCommentComponent } from './post-comment/post-comment.component';
import { AddCommentComponent } from './add-comment/add-comment.component';

@Component({
  selector: 'app-post-comments',
  standalone: true,
  imports: [MatButtonModule, MatIconModule, MatTreeModule, PostCommentComponent, AddCommentComponent],
  templateUrl: './post-comments.component.html',
  styleUrl: './post-comments.component.scss',
})
export class PostCommentsComponent {
  comments = input.required<PostComment[]>();
  total = input.required<number>();

  getReplies = (comment: PostComment) => comment.replies ?? [];
  hasReplies = (_: number, comment: PostComment) => !!comment.replies && comment.replies.length > 0;
  trackBy: TrackByFunction<PostComment> = (_, comment) => comment.id;
}
