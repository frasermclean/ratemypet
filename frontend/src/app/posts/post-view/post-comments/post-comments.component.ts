import { Component, input, TrackByFunction } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatTreeModule } from '@angular/material/tree';

import { Post, PostComment } from '../../post.models';

import { PostCommentComponent } from './post-comment/post-comment.component';
import { AddCommentComponent } from './add-comment/add-comment.component';

@Component({
  selector: 'app-post-comments',
  imports: [MatButtonModule, MatIconModule, MatTreeModule, PostCommentComponent, AddCommentComponent],
  templateUrl: './post-comments.component.html',
  styleUrl: './post-comments.component.scss'
})
export class PostCommentsComponent {
  post = input.required<Post>();

  getReplies = (comment: PostComment) => comment.replies ?? [];
  hasReplies = (_: number, comment: PostComment) => !!comment.replies && comment.replies.length > 0;
  trackBy: TrackByFunction<PostComment> = (_, comment) => comment.id;
}
