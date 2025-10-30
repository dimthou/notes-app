# Stage 1: build
FROM node:20-alpine AS build
WORKDIR /app

# Copy package files first for caching
COPY package*.json ./
RUN npm install

# Copy the rest of the source
COPY . .

# Set API URL at build time
ARG VITE_API_URL
ENV VITE_API_URL=$VITE_API_URL

# Build the app (Vite will use VITE_API_URL)
RUN npm run build

# Stage 2: serve with nginx
FROM nginx:alpine
COPY --from=build /app/dist /usr/share/nginx/html
COPY nginx.conf /etc/nginx/conf.d/default.conf
EXPOSE 80