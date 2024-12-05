import { Component, effect, inject, output, signal } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatSnackBar } from '@angular/material/snack-bar';

interface ImageInfo {
  url: string;
  state: 'valid' | 'invalid' | '';
  fileName?: string;
  sizeInKb?: number;
}

@Component({
  selector: 'app-image-upload',
  imports: [MatButtonModule, MatIconModule],
  templateUrl: './image-upload.component.html',
  styleUrl: './image-upload.component.scss'
})
export class ImageUploadComponent {
  private readonly snackbar = inject(MatSnackBar);
  private readonly file = signal<File | null>(null);
  readonly imageInfo = signal<ImageInfo>({ url: '', state: '' });
  readonly fileChange = output<File | null>();

  constructor() {
    effect(() => {
      const file = this.file();

      if (!file) {
        this.imageInfo.set({ url: '', state: '', fileName: undefined, sizeInKb: undefined });
        this.fileChange.emit(null);
        return;
      }

      const fileName = file.name;
      const sizeInKb = Math.round(file.size / 1024);

      if (!file.type.startsWith('image/')) {
        this.imageInfo.set({ url: '', state: 'invalid', fileName, sizeInKb });
        this.snackbar.open('Only image files are allowed', 'Close');
        this.fileChange.emit(null);
        return;
      }

      const reader = new FileReader();
      reader.onload = (event) => {
        const url = event.target?.result as string;
        this.imageInfo.set({ url, state: 'valid', fileName, sizeInKb });
        this.fileChange.emit(file);
      };

      reader.readAsDataURL(file);
    });
  }

  reset() {
    this.file.set(null);
  }

  onFileChange(event: Event) {
    const input = event.target as HTMLInputElement;
    const file = input.files?.[0] ?? null;
    this.file.set(file);
  }

  onFileDrop(event: DragEvent) {
    event.preventDefault();
    const file = event.dataTransfer?.files[0] ?? null;
    this.file.set(file);
  }

  onDragOver(event: DragEvent) {
    event.preventDefault();
  }
}
