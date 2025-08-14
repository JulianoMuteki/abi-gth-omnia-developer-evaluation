#!/bin/bash

# Script to configure the development environment

echo "🚀 Configuring development environment..."

# Check if .env file already exists
if [ -f ".env" ]; then
    echo "⚠️  .env file already exists. Do you want to overwrite it? (y/n)"
    read -r response
    if [[ "$response" != "y" && "$response" != "Y" ]]; then
        echo "❌ Configuration cancelled."
        exit 1
    fi
fi

# Get WSL IP
WSL_IP=$(hostname -I | awk '{print $1}')

echo "📝 Creating .env file..."

cat > .env << EOF
# Database Configuration
DB_HOST=${WSL_IP}
DB_PORT=32778
DB_NAME=developer_evaluation
DB_USER=developer
DB_PASSWORD=ev@luAt10n

# Connection String
CONNECTION_STRING=Host=\${DB_HOST};Port=\${DB_PORT};Database=\${DB_NAME};Username=\${DB_USER};Password=\${DB_PASSWORD}

# JWT Configuration
JWT_SECRET_KEY=YourSuperSecretKeyForJwtTokenGenerationThatShouldBeAtLeast32BytesLong

RUN_SEEDING=true
EOF

echo "✅ .env file created successfully!"
echo "📍 WSL IP detected: ${WSL_IP}"
echo ""
echo "🔧 To run the application:"
echo "   docker-compose -f docker-compose.yml --env-file .env -p abi-gth-omnia up -d --build"
echo ""
echo "📖 For more information, see ENVIRONMENT_SETUP.md"
