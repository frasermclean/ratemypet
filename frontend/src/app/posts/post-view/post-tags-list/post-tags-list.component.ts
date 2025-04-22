import { Component, input } from '@angular/core';
import { RouterLink } from '@angular/router';

@Component({
  selector: 'post-tags-list',
  imports: [RouterLink],
  templateUrl: './post-tags-list.component.html',
  styleUrl: './post-tags-list.component.scss'
})
export class PostTagsListComponent {
  tags = input.required<string[]>();
}
