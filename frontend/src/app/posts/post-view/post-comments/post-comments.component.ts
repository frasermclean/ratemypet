import { ChangeDetectionStrategy, Component, input, TrackByFunction } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatTreeModule } from '@angular/material/tree';
import { select } from '@ngxs/store';
import { AuthState } from '../../../auth/auth.state';
import { PostComment } from '../../post.models';
import { PostCommentComponent } from './post-comment/post-comment.component';

@Component({
  changeDetection: ChangeDetectionStrategy.OnPush,
  selector: 'app-post-comments',
  imports: [MatButtonModule, MatIconModule, MatTreeModule, PostCommentComponent],
  templateUrl: './post-comments.component.html',
  styleUrl: './post-comments.component.scss'
})
export class PostCommentsComponent {
  postId = input.required<string>();
  comments = input.required<PostComment[]>();
  isLoggedIn = select(AuthState.isLoggedIn);

  getReplies = (comment: PostComment) => comment.replies ?? [];
  hasReplies = (_: number, comment: PostComment) => !!comment.replies && comment.replies.length > 0;
  trackBy: TrackByFunction<PostComment> = (_, comment) => comment.id;
}
