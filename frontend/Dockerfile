# Build stage
FROM node:22-slim AS build
ARG CONFIGURATION=production
WORKDIR /app/src
COPY package.json pnpm-lock.yaml ./
RUN corepack enable && corepack prepare pnpm@latest-10 --activate
RUN pnpm install --frozen-lockfile
COPY . ./
RUN npm run build -- --configuration $CONFIGURATION

# Runtime stage
FROM node:22-slim
ARG PORT=8080
WORKDIR /usr/app
COPY --from=build /app/src/dist ./
CMD ["node", "server/server.mjs"]
ENV PORT=$PORT
ENV API_BASE_URL=http://localhost:5080/api
EXPOSE $PORT
LABEL org.opencontainers.image.source=https://github.com/frasermclean/ratemypet
LABEL org.opencontainers.image.authors="Fraser McLean"
LABEL org.opencontainers.image.description="Rate My Pet - Frontend Angular SSR application"
