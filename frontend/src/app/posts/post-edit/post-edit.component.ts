import { Component, inject } from '@angular/core';
import { NonNullableFormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';

import { dispatch } from '@ngxs/store';
import { PostsActions } from '../posts.actions';

@Component({
  selector: 'app-post-edit',
  imports: [ReactiveFormsModule, MatButtonModule, MatCardModule, MatFormFieldModule, MatIconModule, MatInputModule],
  templateUrl: './post-edit.component.html',
  styleUrl: './post-edit.component.scss'
})
export class PostEditComponent {
  private readonly formBuilder = inject(NonNullableFormBuilder);
  addPost = dispatch(PostsActions.AddPost);

  formGroup = this.formBuilder.group({
    title: ['', [Validators.required, Validators.maxLength(50)]],
    description: ['', [Validators.required]],
    image: [null as File | null, Validators.required]
  });

  onFileChange(event: Event) {
    const input = event.target as HTMLInputElement;
    const file = input.files?.[0];
    this.formGroup.patchValue({ image: file });
  }

  onSubmit() {
    const formValue = this.formGroup.getRawValue();
    this.addPost({ ...formValue, image: formValue.image!, speciesId: 1 });
  }
}
