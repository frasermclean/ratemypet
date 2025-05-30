import { AngularAppEngine, createRequestHandler } from '@angular/ssr';

const engine = new AngularAppEngine();

/**
 * This is a request handler used by the Angular CLI (dev-server and during build).
 */
export const reqHandler = createRequestHandler(async (request) => {
  const response = await engine.handle(request);

  return response ?? new Response('Page not found.', { status: 404 });
});

export default { fetch: reqHandler };
