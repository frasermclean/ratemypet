import { Component, inject, input, output, signal } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatSnackBar } from '@angular/material/snack-bar';

@Component({
  selector: 'app-image-upload',
  imports: [MatButtonModule, MatIconModule],
  templateUrl: './image-upload.component.html',
  styleUrl: './image-upload.component.scss'
})
export class ImageUploadComponent {
  imagePreview = signal({ url: '', state: '' });
  snackbar = inject(MatSnackBar);
  fileChange = output<File | null>();

  reset() {
    this.imagePreview.set({ url: '', state: '' });
  }

  onFileChange(event: Event) {
    const input = event.target as HTMLInputElement;
    const file = input.files?.[0];
    this.processFile(file);
  }

  onFileDrop(event: DragEvent) {
    event.preventDefault();
    const file = event.dataTransfer?.files[0];
    this.processFile(file);
  }

  onDragOver(event: DragEvent) {
    event.preventDefault();
  }

  private processFile(file: File | undefined) {
    if (!file || !file.type.startsWith('image/')) {
      this.imagePreview.set({ url: '', state: 'invalid' });
      this.snackbar.open('Only image files are allowed', 'Close');
      this.fileChange.emit(null);
      return;
    }

    const reader = new FileReader();
    reader.onload = (event) => {
      const url = event.target?.result as string;
      this.imagePreview.set({ url, state: 'valid' });
      this.fileChange.emit(file);
    };

    reader.readAsDataURL(file);
  }
}
