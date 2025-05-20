/**
 * Cloudflare Pages function that acts as a proxy for API requests.
 */
export const onRequest: PagesFunction<Env> = async (context) => {
  const requestUrl = new URL(context.request.url);

  // create a new URL object for the API backend based on the request URL
  const apiUrl = new URL(context.env.API_BASE_URL);
  apiUrl.pathname = requestUrl.pathname;
  apiUrl.search = requestUrl.search;

  // proxy the request to the API backend.
  const request = new Request(apiUrl, context.request);

  return fetch(request);
};
