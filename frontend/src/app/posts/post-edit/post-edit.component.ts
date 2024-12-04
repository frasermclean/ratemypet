import { Component, inject, signal, viewChild } from '@angular/core';
import { NonNullableFormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { MatSnackBar } from '@angular/material/snack-bar';
import { dispatch } from '@ngxs/store';

import { PostsActions } from '../posts.actions';
import { ImageUploadComponent } from '@shared/image-upload/image-upload.component';

@Component({
  selector: 'app-post-edit',
  imports: [
    ReactiveFormsModule,
    MatButtonModule,
    MatCardModule,
    MatFormFieldModule,
    MatIconModule,
    MatInputModule,
    ImageUploadComponent
  ],
  templateUrl: './post-edit.component.html',
  styleUrl: './post-edit.component.scss'
})
export class PostEditComponent {
  private readonly formBuilder = inject(NonNullableFormBuilder);
  private readonly snackbar = inject(MatSnackBar);
  addPost = dispatch(PostsActions.AddPost);
  imagePreviewUrl = signal('');
  imageState = signal<'initial' | 'valid' | 'invalid'>('initial');
  imageUpload = viewChild.required(ImageUploadComponent);

  formGroup = this.formBuilder.group({
    title: ['', [Validators.required, Validators.maxLength(50)]],
    description: ['', [Validators.required]],
    image: [null as File | null, Validators.required]
  });

  onImageFileChange(file: File | null) {
    this.formGroup.controls.image.setValue(file);
    this.formGroup.controls.image.markAsDirty();
  }

  onSubmitForm() {
    const formValue = this.formGroup.getRawValue();
    this.addPost({ ...formValue, image: formValue.image!, speciesId: 1 });
  }

  onResetForm() {
    this.formGroup.reset();
    this.imagePreviewUrl.set('');
    this.imageUpload().reset();
  }
}
