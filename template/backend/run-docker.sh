#!/bin/bash

# Script to run Docker with specific configuration

echo "🐳 Running Docker with specific configuration..."

# Check if .env file exists
if [ ! -f ".env" ]; then
    echo "❌ .env file not found!"
    echo "📝 Please run setup-env.sh first to create the .env file"
    exit 1
fi

# Stop any existing containers
echo "1. Stopping existing containers..."
docker-compose -f docker-compose.yml  --env-file .env.develop -p abi-gth-omnia down

# Build and start containers
echo "2. Building and starting containers..."
docker-compose -f docker-compose.yml  --env-file .env.develop -p abi-gth-omnia up -d --build

# Wait for startup
echo "3. Waiting for containers to start..."
sleep 15

# Check status
echo "4. Container status:"
docker-compose -f docker-compose.yml --env-file .env -p abi-gth-omnia ps

# Test health endpoint
echo "5. Testing health endpoint..."
sleep 5
if curl -f -s "http://localhost:8080/health" > /dev/null; then
    echo "✅ Health check OK!"
    echo "🔗 Swagger UI: http://localhost:8080/swagger"
    echo "🎉 Application is running successfully!"
else
    echo "❌ Health check failed"
    echo "📋 Checking logs..."
    docker-compose -f docker-compose.yml --env-file .env -p abi-gth-omnia logs ambev.developerevaluation.webapi
fi

echo ""
echo "📋 Useful commands:"
echo "   View logs: docker-compose -f docker-compose.yml --env-file .env -p abi-gth-omnia logs -f"
echo "   Stop: docker-compose -f docker-compose.yml --env-file .env -p abi-gth-omnia down"
echo "   Restart: docker-compose -f docker-compose.yml --env-file .env -p abi-gth-omnia restart"
