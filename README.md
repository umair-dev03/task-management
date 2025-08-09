# Task Management Application

A full-stack task management system composed of:
- Backend: .NET 8 Web API with JWT auth, role-based access control, EF Core + SQL Server, and clean architecture.
- Frontend: React single-page application using KendoReact components, React Router, and Nginx for production serving.

This repository contains both projects and supports running locally or via Docker.

---

## Table of Contents

- [Repository Structure](#repository-structure)
- [Features](#features)
- [Prerequisites](#prerequisites)
- [Quick Start with Docker](#quick-start-with-docker)
  - [1) Start the API + Database](#1-start-the-api--database)
  - [2) Start the Frontend](#2-start-the-frontend)
  - [Verify](#verify)
- [Local Development (without Docker)](#local-development-without-docker)
  - [Backend (.NET API)](#backend-net-api)
  - [Frontend (React)](#frontend-react)
- [Environment Configuration](#environment-configuration)
  - [Backend](#backend)
  - [Frontend](#frontend)
- [API Summary](#api-summary)
  - [Auth](#auth)
  - [Tasks](#tasks)
- [Seeded Roles and Users](#seeded-roles-and-users)
- [Testing](#testing)
- [Deployment Notes](#deployment-notes)
- [Troubleshooting](#troubleshooting)
- [License](#license)

---

## Repository Structure

```
task-management-application/
├─ task-management-api/                 # .NET 8 Web API (Clean Architecture)
│  ├─ src/
│  │  ├─ TaskManagement.Api/            # API (controllers, middleware, config)
│  │  ├─ TaskManagement.Application/    # CQRS, DTOs, business logic
│  │  ├─ TaskManagement.Domain/         # Entities, enums
│  │  └─ TaskManagement.Infrastructure/ # EF Core DbContext, Repos, Services, Migrations, Seed
│  ├─ docker-compose.yml                # API + SQL Server
│  ├─ Dockerfile                        # API container
│  ├─ Dockerfile.mssql                  # SQL Server container
│  └─ README.md                         # API-specific docs
│
└─ task-management-frontend 2/
   └─ task-management-frontend 2/       # React app
      ├─ src/                           # Components, pages, services, utils
      ├─ public/
      ├─ .env                           # REACT_APP_API_BASE_URL
      ├─ docker-compose.yml             # Frontend (Nginx) + dev profile
      ├─ Dockerfile                     # Production build w/ Nginx
      ├─ Dockerfile.dev                 # Dev server (CRA)
      ├─ nginx.conf                     # Nginx config (SPA + proxy)
      └─ README.md                      # Frontend-specific docs
```

Note: The frontend subfolder name includes spaces. Quote paths or use tab-completion when running terminal commands.

---

## Features

- Authentication & Authorization
  - JWT-based auth, roles: Employee and Manager
- Tasks
  - Employees: Create, update, delete own tasks
  - Managers: Approve/Reject tasks
  - Pagination, filtering, and sorting support
- API
  - OpenAPI/Swagger in development
  - EF Core migrations and database seeding
- Frontend
  - React Router SPA with dashboards for Employee and Manager
  - KendoReact grid and components
  - Production Nginx config with SPA fallback and optional API proxy
- Dockerized setup for both API and frontend

---

## Prerequisites

- Docker Desktop (for Docker workflows)
- For local dev:
  - .NET 8 SDK (API)
  - Node.js 18+ and npm 8+ (Frontend)
  - SQL Server (optional if not using Docker; Docker setup provides SQL Server container)

---

## Quick Start with Docker

The API and Frontend are built and run from their respective subdirectories.

### 1) Start the API + Database

From the `task-management-api` directory:
- Windows (cmd):
  ```
  cd task-management-api
  docker compose up --build
  ```
- PowerShell:
  ```
  cd task-management-api
  docker compose up --build
  ```
- macOS/Linux:
  ```
  cd task-management-api
  docker compose up --build
  ```

This will:
- Start SQL Server (port 1433)
- Build and run the API (host port 8080)
- Apply migrations and seed data on startup

API base URL: http://localhost:8080  
Swagger (if enabled): http://localhost:8080/swagger

### 2) Start the Frontend

From the frontend directory (`task-management-frontend 2/task-management-frontend 2`):

Set the build-time environment variable for the API URL:
- Windows (cmd):
  ```
  set REACT_APP_API_BASE_URL=http://localhost:8080/api
  cd "task-management-frontend 2\task-management-frontend 2"
  docker compose up --build frontend
  ```
- PowerShell:
  ```
  $env:REACT_APP_API_BASE_URL = "http://localhost:8080/api"
  cd "task-management-frontend 2/task-management-frontend 2"
  docker compose up --build frontend
  ```
- macOS/Linux:
  ```
  export REACT_APP_API_BASE_URL=http://localhost:8080/api
  cd "task-management-frontend 2/task-management-frontend 2"
  docker compose up --build frontend
  ```

Frontend will be available at: http://localhost:3000

Alternate approach: If you prefer same-origin API calls via Nginx, set:
```
REACT_APP_API_BASE_URL=/api
```
Nginx (in the container) proxies `/api` to the backend at `http://host.docker.internal:8080`.

### Verify

- Open the frontend: http://localhost:3000
- Log in using the seeded credentials below
- Create tasks as Employee; review/approve as Manager

---

## Local Development (without Docker)

### Backend (.NET API)

1) Configure database connection in:
- `task-management-api/src/TaskManagement.Api/appsettings.Development.json`
- or set `ConnectionStrings:DefaultConnection` via environment variables.

2) Run EF migrations if needed (they are applied automatically on startup in the provided setup).

3) Start the API:
```
cd task-management-api
dotnet run --project src/TaskManagement.Api
```
Default dev URL (Kestrel) depends on `launchSettings.json`. If you run on a port other than 8080, ensure the frontend env matches.

### Frontend (React)

1) Install dependencies:
```
cd "task-management-frontend 2/task-management-frontend 2"
npm install
```

2) Set `.env`:
```
REACT_APP_API_BASE_URL=http://localhost:8080/api
```

3) Start dev server:
```
npm start
```
Visit http://localhost:3000

Dev via Docker (hot reload):
```
# Windows (cmd)
set REACT_APP_API_BASE_URL=http://localhost:8080/api
docker compose --profile dev up --build frontend-dev
# App on http://localhost:3001
```

---

## Environment Configuration

### Backend

- Connection string:
  - Docker compose sets: `Server=sqlserver;Database=TaskManagement;User=sa;Password=YourStrong!Passw0rd;TrustServerCertificate=True;`
- ASP.NET Core environment: `ASPNETCORE_ENVIRONMENT=Development` by default in docker compose
- CORS: configured to allow `http://localhost:3000` by default (adjust in `Program.cs` as needed)
- JWT settings: configured under `Jwt` sections in API `appsettings*` files

Key files:
- `task-management-api/src/TaskManagement.Api/appsettings*.json`
- `task-management-api/src/TaskManagement.Api/Program.cs`

### Frontend

- `.env`:
  ```
  REACT_APP_API_BASE_URL=http://localhost:8080/api
  ```
- Docker build arg: passed in `docker-compose.yml` for production image
- Optional same-origin API via Nginx:
  - Set `REACT_APP_API_BASE_URL=/api`
  - `nginx.conf` proxies `/api` to `http://host.docker.internal:8080`

---

## API Summary

Base URL (Docker): `http://localhost:8080/api`

### Auth

- POST `/api/auth/login`
  - Request: `{ "email": "user@example.com", "password": "string" }`
  - Response: `{ "token": "jwt-token", "user": { ... }, "roles": [ ... ] }`
  - 401 on failure

### Tasks

All require JWT.

| Method | Endpoint                | Roles             | Description                          |
|-------:|-------------------------|-------------------|--------------------------------------|
| GET    | `/api/task`             | Employee, Manager | List tasks (pagination/filter/sort)  |
| GET    | `/api/task/{id}`        | Employee, Manager | Get task by ID                       |
| POST   | `/api/task`             | Employee          | Create a new task                    |
| PUT    | `/api/task/{id}`        | Employee          | Update an existing task              |
| PATCH  | `/api/task/{id}/status` | Manager           | Approve/Reject a task                |
| DELETE | `/api/task/{id}`        | Employee          | Delete a task                        |

Examples:
- Create:
  ```
  POST /api/task
  {
    "title": "Write documentation",
    "date": "2025-08-07T00:00:00Z",
    "hourWorked": 2.5
  }
  ```
- Update status:
  ```
  PATCH /api/task/1/status
  {
    "status": "Approved"
  }
  ```

---

## Seeded Roles and Users

On first run (Docker or local with seeding enabled), the database is seeded:

Roles:
- Employee
- Manager

Users:
- Employee
  - Email: `employee1@example.com`
  - Password: `password123`
- Manager
  - Email: `manager1@example.com`
  - Password: `password123`

Source: `task-management-api/src/TaskManagement.Infrastructure/SeedData.cs`

---

## Testing

- Backend:
  ```
  cd task-management-api
  dotnet test
  ```
  Tests located under `test/TaskManagement.Api.Tests`.

- Frontend:
  ```
  cd "task-management-frontend 2/task-management-frontend 2"
  npm test
  ```

---

## Deployment Notes

- API:
  - Use the provided `Dockerfile` to build an ASP.NET image.
  - Expose the API behind a reverse proxy as needed; configure CORS accordingly.
  - Ensure JWT keys/secrets are set securely via environment variables in production.

- Frontend:
  - `npm run build` produces static assets in `build/`.
  - The provided Dockerfile serves via Nginx with SPA fallback and asset caching.
  - Ensure `REACT_APP_API_BASE_URL` matches your API endpoint at build time.

---

## Troubleshooting

- 401 Unauthorized or redirect to login:
  - Token may be missing/expired; the frontend clears localStorage and redirects to `/`.
  - Ensure `REACT_APP_API_BASE_URL` is correct and API is reachable.

- CORS in local dev:
  - Allow `http://localhost:3000` (and `http://localhost:3001` for dev container) in API CORS.

- SPA deep links 404:
  - Confirm Nginx SPA fallback is active:
    ```
    location / { try_files $uri $uri/ /index.html; }
    ```

- Docker healthcheck failing (frontend):
  - Visit `http://localhost:3000/health`.
  - Inspect logs: `docker logs <frontend-container>`

- SQL Server connection issues:
  - Wait for SQL container to become healthy.
  - Check `ConnectionStrings__DefaultConnection` and network/resolution inside compose.

- Ports:
  - API: 8080 (host)
  - SQL: 1433 (host)
  - Frontend: 3000 (host; Nginx)
  - Frontend dev container: 3001 -> 3000 mapping

---

## License

Provided for educational and demonstration purposes.
