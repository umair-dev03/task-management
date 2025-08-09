#!/bin/bash

# Docker scripts for React Frontend

# Colors for output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
NC='\033[0m' # No Color

# Function to print colored output
print_status() {
    echo -e "${GREEN}[INFO]${NC} $1"
}

print_warning() {
    echo -e "${YELLOW}[WARNING]${NC} $1"
}

print_error() {
    echo -e "${RED}[ERROR]${NC} $1"
}

# Build production image
build_prod() {
    print_status "Building production Docker image..."
    docker build -t task-management-frontend:latest .
    if [ $? -eq 0 ]; then
        print_status "Production image built successfully!"
    else
        print_error "Failed to build production image"
        exit 1
    fi
}

# Build development image
build_dev() {
    print_status "Building development Docker image..."
    docker build -f Dockerfile.dev -t task-management-frontend:dev .
    if [ $? -eq 0 ]; then
        print_status "Development image built successfully!"
    else
        print_error "Failed to build development image"
        exit 1
    fi
}

# Run production container
run_prod() {
    print_status "Starting production container..."
    docker run -d -p 3000:80 --name task-management-frontend task-management-frontend:latest
    if [ $? -eq 0 ]; then
        print_status "Production container started! Access at http://localhost:3000"
    else
        print_error "Failed to start production container"
        exit 1
    fi
}

# Run development container
run_dev() {
    print_status "Starting development container..."
    docker run -d -p 3001:3000 -v $(pwd):/app -v /app/node_modules --name task-management-frontend-dev task-management-frontend:dev
    if [ $? -eq 0 ]; then
        print_status "Development container started! Access at http://localhost:3001"
    else
        print_error "Failed to start development container"
        exit 1
    fi
}

# Stop containers
stop_containers() {
    print_status "Stopping containers..."
    docker stop task-management-frontend task-management-frontend-dev 2>/dev/null || true
    docker rm task-management-frontend task-management-frontend-dev 2>/dev/null || true
    print_status "Containers stopped and removed"
}

# Clean up images
cleanup() {
    print_status "Cleaning up Docker images..."
    docker rmi task-management-frontend:latest task-management-frontend:dev 2>/dev/null || true
    print_status "Docker images cleaned up"
}

# Show logs
logs() {
    if [ "$1" = "dev" ]; then
        docker logs -f task-management-frontend-dev
    else
        docker logs -f task-management-frontend
    fi
}

# Show usage
usage() {
    echo "Usage: $0 {build-prod|build-dev|run-prod|run-dev|stop|cleanup|logs|logs-dev}"
    echo ""
    echo "Commands:"
    echo "  build-prod  - Build production Docker image"
    echo "  build-dev   - Build development Docker image"
    echo "  run-prod    - Run production container"
    echo "  run-dev     - Run development container"
    echo "  stop        - Stop and remove containers"
    echo "  cleanup     - Remove Docker images"
    echo "  logs        - Show production container logs"
    echo "  logs-dev    - Show development container logs"
    echo ""
    echo "Or use docker-compose:"
    echo "  docker-compose up -d          # Start production"
    echo "  docker-compose --profile dev up -d  # Start development"
    echo "  docker-compose down           # Stop all services"
}

# Main script logic
case "$1" in
    build-prod)
        build_prod
        ;;
    build-dev)
        build_dev
        ;;
    run-prod)
        run_prod
        ;;
    run-dev)
        run_dev
        ;;
    stop)
        stop_containers
        ;;
    cleanup)
        cleanup
        ;;
    logs)
        logs
        ;;
    logs-dev)
        logs dev
        ;;
    *)
        usage
        exit 1
        ;;
esac
