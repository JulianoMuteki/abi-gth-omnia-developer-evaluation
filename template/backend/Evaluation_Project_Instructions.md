# Evaluation Project Setup Instructions

This document provides instructions for setting up and running the development environment on Linux systems.

## Prerequisites

Before running the scripts, ensure you have the following installed:

- **Docker**: [Install Docker](https://docs.docker.com/engine/install/)
- **Docker Compose**: [Install Docker Compose](https://docs.docker.com/compose/install/)
- **Bash shell**: Usually pre-installed on most Linux distributions

## File Permissions

Make sure the shell scripts have executable permissions:

```bash
chmod +x setup-env.sh
chmod +x run-docker.sh
```

## Step-by-Step Setup

### 1. Environment Configuration

Run the environment setup script to create the necessary configuration files:

```bash
./setup-env.sh
```

This script will:
- Detect your system's IP address
- Create a `.env` file with database and JWT configurations
- Set up connection strings for the application

**Note**: If a `.env` file already exists, the script will ask for confirmation before overwriting it.

### 2. Running the Application

After setting up the environment, run the Docker application:

```bash
./run-docker.sh
```

This script will:
- Stop any existing containers
- Build and start the Docker containers
- Wait for the application to initialize
- Perform a health check
- Display the application status

## Manual Commands

If you prefer to run commands manually instead of using the scripts:

### Environment Setup
```bash
# Get your system IP
WSL_IP=$(hostname -I | awk '{print $1}')

# Create .env file manually
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
```

### Docker Commands
```bash
# Stop existing containers
docker-compose -f docker-compose.yml --env-file .env -p abi-gth-omnia down

# Build and start containers
docker-compose -f docker-compose.yml --env-file .env -p abi-gth-omnia up -d --build

# Check container status
docker-compose -f docker-compose.yml --env-file .env -p abi-gth-omnia ps

# View logs
docker-compose -f docker-compose.yml --env-file .env -p abi-gth-omnia logs -f

# Stop containers
docker-compose -f docker-compose.yml --env-file .env -p abi-gth-omnia down
```

## Accessing the Application

Once the application is running successfully, you can access:

- **Swagger UI**: http://localhost:8080/swagger
- **Health Check**: http://localhost:8080/health
- **API Endpoints**: http://localhost:8080/api

## Troubleshooting

### Common Issues

1. **Permission Denied**: Make sure the scripts have executable permissions
   ```bash
   chmod +x *.sh
   ```

2. **Docker Not Running**: Ensure Docker service is started
   ```bash
   sudo systemctl start docker
   sudo systemctl enable docker
   ```

3. **Port Already in Use**: Check if port 8080 is available
   ```bash
   sudo netstat -tulpn | grep :8080
   ```

4. **Container Build Failures**: Check Docker logs
   ```bash
   docker-compose -f docker-compose.yml --env-file .env -p abi-gth-omnia logs
   ```

### Health Check

If the health check fails, check the application logs:

```bash
docker-compose -f docker-compose.yml --env-file .env -p abi-gth-omnia logs ambev.developerevaluation.webapi
```

## Environment Variables

The `.env` file contains the following configuration:

| Variable | Description | Default Value |
|----------|-------------|---------------|
| `DB_HOST` | Database host IP | Auto-detected |
| `DB_PORT` | Database port | 32778 |
| `DB_NAME` | Database name | developer_evaluation |
| `DB_USER` | Database username | developer |
| `DB_PASSWORD` | Database password | ev@luAt10n |
| `JWT_SECRET_KEY` | JWT token secret | Auto-generated |
| `RUN_SEEDING` | Enable database seeding | true |

## Useful Commands

```bash
# View running containers
docker ps

# View container logs
docker logs <container_name>

# Access container shell
docker exec -it <container_name> /bin/bash

# Clean up Docker resources
docker system prune -a

# Restart containers
docker-compose -f docker-compose.yml --env-file .env -p abi-gth-omnia restart
```

## Support

If you encounter any issues not covered in this guide, please check:

1. Docker and Docker Compose versions are up to date
2. System has sufficient resources (memory, disk space)
3. Firewall settings allow Docker traffic
4. Network connectivity is working properly
