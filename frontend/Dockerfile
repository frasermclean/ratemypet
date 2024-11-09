FROM node:20-alpine AS build
WORKDIR /app/src
COPY package*.json ./
RUN npm ci
COPY . ./
RUN npm run build

FROM node:20-alpine
RUN addgroup -S app
RUN adduser -S app -G app
USER app
WORKDIR /app
COPY --from=build /app/src/dist/ratemypet-web/ ./
CMD ["node", "server/server.mjs"]
EXPOSE 4200
