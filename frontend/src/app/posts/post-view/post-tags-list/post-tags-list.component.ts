import { Component, input } from '@angular/core';

@Component({
  selector: 'post-tags-list',
  imports: [],
  templateUrl: './post-tags-list.component.html',
  styleUrl: './post-tags-list.component.scss'
})
export class PostTagsListComponent {
  tags = input.required<string[]>();
}
