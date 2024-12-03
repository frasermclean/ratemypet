import { Component, input } from '@angular/core';
import { PostComment } from '../../../post.models';
import { DatePipe } from '@angular/common';

@Component({
  selector: 'app-post-comment',
  standalone: true,
  imports: [DatePipe],
  templateUrl: './post-comment.component.html',
  styleUrl: './post-comment.component.scss',
})
export class PostCommentComponent {
  comment = input.required<PostComment>();
}
