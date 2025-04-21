import { Component, computed, input } from '@angular/core';
import { MatCardModule } from '@angular/material/card';
import { AuthState } from '@auth/auth.state';
import { select } from '@ngxs/store';
import { PostReactionButtonComponent } from '../../post-reaction-button/post-reaction-button.component';
import { allReactions, PostReactions, Reaction } from '../../post.models';

@Component({
  selector: 'post-reactions-card',
  imports: [MatCardModule, PostReactionButtonComponent],
  templateUrl: './post-reactions-card.component.html',
  styleUrl: './post-reactions-card.component.scss'
})
export class PostReactionsCardComponent {
  public readonly postId = input.required<string>();
  public readonly reactions = input.required<PostReactions>();
  public readonly userReaction = input<Reaction>();
  protected readonly isLoggedIn = select(AuthState.isLoggedIn);
  protected readonly allReactions = allReactions;

  readonly reactionCount = computed<number>(() =>
    Object.values(this.reactions()).reduce((acc, value) => acc + value, 0)
  );
}
