import { Component, inject, OnInit } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MatDialog, MatDialogModule } from '@angular/material/dialog';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatTooltipModule } from '@angular/material/tooltip';
import { dispatch, select } from '@ngxs/store';

import { PostsState } from '../posts.state';
import { PostItemComponent } from './post-list-item/post-list-item.component';
import { PostEditComponent } from '../post-edit/post-edit.component';
import { PostsActions } from '../posts.actions';

@Component({
  selector: 'app-post-list',
  standalone: true,
  imports: [
    PostItemComponent,
    MatButtonModule,
    MatDialogModule,
    MatIconModule,
    MatProgressSpinnerModule,
    MatTooltipModule,
  ],
  templateUrl: './post-list.component.html',
  styleUrl: './post-list.component.scss',
})
export class PostListComponent implements OnInit {
  private readonly dialog = inject(MatDialog);
  readonly status = select(PostsState.status);
  readonly posts = select(PostsState.posts);
  readonly searchPosts = dispatch(PostsActions.SearchPosts);

  ngOnInit(): void {
    this.searchPosts();
  }

  addPost() {
    this.dialog.open(PostEditComponent).afterClosed().subscribe((data) => {
      console.log('Dialog output:', data);
    });
  }
}
