# OmniBus Client - React/Vite (Multi-stage optimized)
FROM node:18-alpine AS build
WORKDIR /app

# Build arguments
ARG VITE_API_URL=http://localhost:5000/api
ENV VITE_API_URL=$VITE_API_URL

# Copy package files first for better caching
COPY src/OmniBus.Client/package*.json ./
RUN npm ci --production=false

# Copy source and build
COPY src/OmniBus.Client .
RUN npm run build

FROM nginx:alpine AS final
WORKDIR /usr/share/nginx/html

# Remove default nginx static files
RUN rm -rf ./*

# Copy built files from build stage
COPY --from=build /app/dist .

# Copy nginx configuration
COPY --from=build /app/nginx.conf /etc/nginx/conf.d/default.conf || echo "server { listen 5173; location / { root /usr/share/nginx/html; index index.html; try_files \$uri \$uri/ /index.html; } location /api { proxy_pass http://api:5000; } }" > /etc/nginx/conf.d/default.conf

# Add non-root user
RUN addgroup -g 101 -S nginx && adduser -S -D -H -u 101 -h /var/cache/nginx -s /sbin/nologin -G nginx -g nginx nginx || true

EXPOSE 5173

# Health check
HEALTHCHECK --interval=30s --timeout=3s --start-period=5s --retries=3 \
    CMD wget --no-verbose --tries=1 --spider http://localhost:5173 || exit 1

CMD ["nginx", "-g", "daemon off;"]
