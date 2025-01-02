import { Component, inject, viewChild } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { NonNullableFormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { Router } from '@angular/router';
import { Actions, dispatch, ofActionSuccessful } from '@ngxs/store';
import { ImageUploadComponent } from '@shared/components/image-upload/image-upload.component';
import { NotificationService } from '@shared/services/notification.service';
import { PostsActions } from '../posts.actions';

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
  addPost = dispatch(PostsActions.AddPost);
  imageUpload = viewChild.required(ImageUploadComponent);

  formGroup = this.formBuilder.group({
    title: ['', [Validators.required, Validators.maxLength(50)]],
    description: ['', [Validators.required]],
    speciesId: [1, Validators.required],
    image: [null as File | null, Validators.required]
  });

  constructor(actions$: Actions, notificationService: NotificationService, router: Router) {
    actions$
      .pipe(ofActionSuccessful(PostsActions.AddPost))
      .pipe(takeUntilDestroyed())
      .subscribe((postId) => {
        notificationService.showInformation('Post created successfully');
        console.log(postId);
        router.navigate(['/posts', postId]);
      });
  }

  onImageFileChange(file: File | null) {
    this.formGroup.controls.image.setValue(file);
    this.formGroup.controls.image.markAsDirty();
  }

  onSubmitForm() {
    const formValue = this.formGroup.getRawValue();
    this.addPost({ ...formValue, image: formValue.image! });
  }

  onResetForm() {
    this.formGroup.reset();
    this.imageUpload().reset();
  }
}
