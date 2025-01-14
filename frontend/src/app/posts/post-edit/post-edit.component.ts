import { Component, inject, OnInit, viewChild } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { NonNullableFormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { Router } from '@angular/router';
import { Actions, dispatch, ofActionSuccessful, select } from '@ngxs/store';
import { ImageUploadComponent } from '@shared/components/image-upload/image-upload.component';
import { NotificationService } from '@shared/services/notification.service';
import { SpeciesActions } from '../../species/species.actions';
import { SpeciesState } from '../../species/species.state';
import { PostsActions } from '../posts.actions';
import { PostsState } from '../posts.state';

@Component({
  selector: 'app-post-edit',
  imports: [
    ReactiveFormsModule,
    MatButtonModule,
    MatCardModule,
    MatFormFieldModule,
    MatIconModule,
    MatInputModule,
    MatSelectModule,
    ImageUploadComponent
  ],
  templateUrl: './post-edit.component.html',
  styleUrl: './post-edit.component.scss'
})
export class PostEditComponent implements OnInit {
  private readonly formBuilder = inject(NonNullableFormBuilder);
  addPost = dispatch(PostsActions.AddPost);
  getAllSpecies = dispatch(SpeciesActions.GetAllSpecies);
  currentPost = select(PostsState.currentPost);
  allSpecies = select(SpeciesState.allSpecies);

  imageUpload = viewChild.required(ImageUploadComponent);

  formGroup = this.formBuilder.group({
    title: ['', [Validators.required, Validators.maxLength(50)]],
    description: ['', [Validators.required]],
    speciesId: [0, Validators.required],
    image: [null as File | null, Validators.required]
  });

  constructor(actions$: Actions, notificationService: NotificationService, router: Router) {
    actions$.pipe(ofActionSuccessful(PostsActions.AddPost), takeUntilDestroyed()).subscribe(() => {
      notificationService.showInformation('Post created successfully');
      router.navigate(['/posts', this.currentPost()?.id]);
    });
  }

  ngOnInit(): void {
    this.getAllSpecies();
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
