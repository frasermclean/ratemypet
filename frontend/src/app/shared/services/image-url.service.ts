import { Injectable } from '@angular/core';
import { Cloudinary } from '@cloudinary/url-gen';
import { fill } from '@cloudinary/url-gen/actions/resize';

@Injectable({
  providedIn: 'root'
})
export class ImageUrlService {
  private readonly cloudinary = new Cloudinary({
    cloud: {
      cloudName: 'ratemypet'
    }
  });

  public getImageUrl(publicId: string, width: number = 1024, height: number = 1024): string {
    const image = this.cloudinary.image(publicId);

    // perform transformations
    image.resize(fill(width, height)).format('auto');

    return image.toURL();
  }
}
