FROM node:20-alpine AS build
ARG PROJECT_NAME=ratemypet-web
ARG BUILD_ENV=production
WORKDIR /app/src
COPY package*.json ./
RUN npm ci
COPY . ./
RUN npm run build --configuration=$BUILD_ENV

FROM node:20-alpine
ARG PROJECT_NAME=ratemypet-web
RUN addgroup -S app
RUN adduser -S app -G app
USER app
WORKDIR /app
COPY --from=build /app/src/dist/$PROJECT_NAME/ ./
CMD ["node", "server/server.mjs"]
EXPOSE 4200
