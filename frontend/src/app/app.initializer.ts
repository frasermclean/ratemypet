import { inject } from '@angular/core';
import { MatIconRegistry } from '@angular/material/icon';
import { DomSanitizer } from '@angular/platform-browser';
import { TelemetryService } from '@shared/services/telemetry.service';

/**
 * This function is run on application startup to perform any necessary initialization.
 */
export default function initializeApp(): void {
  const iconRegistry = inject(MatIconRegistry);
  const sanitizer = inject(DomSanitizer);
  const telemetryService = inject(TelemetryService);

  iconRegistry.addSvgIconResolver((name, namespace) => {
    const path = `icons/${namespace}/${name}.svg`;
    return sanitizer.bypassSecurityTrustResourceUrl(path);
  });

  telemetryService.initialize();
}
