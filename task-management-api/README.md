# TaskManagement

A robust, layered .NET 8 Web API for managing employee tasks, featuring JWT authentication, role-based access control, and a clean architecture. The project is containerized with Docker and supports SQL Server for persistent storage.

---

## Table of Contents

- [Features](#features)
- [Architecture](#architecture)
- [Getting Started](#getting-started)
- [API Endpoints](#api-endpoints)
  - [Authentication](#authentication)
  - [Task Management](#task-management)
- [Data Model](#data-model)
- [Testing](#testing)
- [Configuration](#configuration)
- [Docker & Database](#docker--database)
- [License](#license)

---

## Features

- **User Authentication**: Secure login with JWT tokens.
- **Role-Based Access**: Supports Employee and Manager roles.
- **Task Management**: Employees can create, update, and delete their tasks; Managers can approve or reject tasks.
- **Status Workflow**: Tasks move through Pending, Approved, and Rejected states.
- **Pagination & Filtering**: List tasks with pagination and search.
- **OpenAPI/Swagger**: Interactive API documentation.
- **CORS**: Configured for frontend integration (default: `http://localhost:3000`).
- **Dockerized**: Easy setup for development and production.

---

## Architecture

This project follows a clean, layered architecture:

- **API Layer** (`src/TaskManagement.Api`): HTTP endpoints, controllers, middleware.
- **Application Layer** (`src/TaskManagement.Application`): Business logic, CQRS handlers, DTOs.
- **Domain Layer** (`src/TaskManagement.Domain`): Core entities and enums.
- **Infrastructure Layer** (`src/TaskManagement.Infrastructure`): Database context, repositories, services, seed data.
- **Tests** (`test/TaskManagement.Api.Tests`): Unit and integration tests.

---

## Getting Started

### Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download)
- [Docker](https://www.docker.com/get-started)
- [SQL Server](https://www.microsoft.com/en-us/sql-server/sql-server-downloads) (optional, if not using Docker)

### Quick Start (with Docker)

1. **Clone the repository:**
   ```bash
   git clone <repo-url>
   cd TaskManagement
   ```

2. **Start the application and database:**
   ```bash
   docker-compose up --build
   ```

   This will:
   - Build the API service
   - Start a SQL Server instance
   - Run database migrations and seed initial data

3. **Access the API:**
   - Swagger UI: [http://localhost:8080/swagger](http://localhost:8080/swagger)
   - API base URL: `http://localhost:8080/api`

### Manual Setup

1. **Configure the database connection** in `src/TaskManagement.Api/appsettings.json` or `appsettings.Development.json`.
2. **Run migrations and seed data** (done automatically on startup).
3. **Start the API:**
   ```bash
   dotnet run --project src/TaskManagement.Api
   ```

---

## API Endpoints

### Authentication

- **POST `/api/auth/login`**
  - Request: `{ "email": "user@example.com", "password": "string" }`
  - Response: `{ "token": "jwt-token", "user": { ... }, "roles": [ ... ] }`
  - Returns 401 Unauthorized on failure.

### Task Management

All endpoints require JWT authentication.

| Method | Endpoint                  | Roles           | Description                        |
|--------|---------------------------|-----------------|------------------------------------|
| GET    | `/api/task`               | Employee,Manager| List tasks (paginated, filterable) |
| GET    | `/api/task/{id}`          | Employee,Manager| Get task by ID                     |
| POST   | `/api/task`               | Employee        | Create a new task                  |
| PUT    | `/api/task/{id}`          | Employee        | Update an existing task            |
| PATCH  | `/api/task/{id}/status`   | Manager         | Approve/Reject a task              |
| DELETE | `/api/task/{id}`          | Employee        | Delete a task                      |

#### Example: Create Task

```json
POST /api/task
{
  "title": "Write documentation",
  "date": "2025-08-07T00:00:00Z",
  "hourWorked": 2.5
}
```

#### Example: Update Task Status

```json
PATCH /api/task/1/status
{
  "status": "Approved"
}
```

---

## Data Model

### User

| Field     | Type      | Description                |
|-----------|-----------|----------------------------|
| Id        | int       | Unique identifier          |
| UserName  | string    | User's name                |
| Email     | string    | User's email               |
| Password  | string    | Hashed password            |
| Roles     | [Role]    | User roles (Employee/Manager) |
| Tasks     | [Task]    | Tasks assigned to user     |

### Role

| Field | Type   | Description      |
|-------|--------|------------------|
| Id    | int    | Unique identifier|
| Name  | string | Role name        |
| Users | [User] | Users in role    |

### Task

| Field      | Type      | Description                |
|------------|-----------|----------------------------|
| Id         | int       | Unique identifier          |
| Title      | string    | Task title                 |
| Date       | DateTime  | Task date                  |
| HourWorked | double    | Hours worked               |
| Status     | Status    | Task status                |
| UserId     | int       | Assigned user              |
| User       | User      | User reference             |

### Status (Enum)

- `Pending`
- `Approved`
- `Rejected`

---

## Testing

Unit and integration tests are located in `test/TaskManagement.Api.Tests`.

To run tests:
```bash
dotnet test
```

---

## Configuration

- **API settings**: `src/TaskManagement.Api/appsettings.json`
- **JWT settings**: Under `Jwt` section in config files.
- **CORS**: Allows `http://localhost:3000` by default (edit in `Program.cs` if needed).

---

## Docker & Database

- **API Dockerfile**: `src/TaskManagement.Api/Dockerfile`
- **SQL Server Dockerfile**: `Dockerfile.mssql`
- **Database init script**: `init-db.sql`
- **Compose file**: `docker-compose.yml`

On startup, the API will:
- Apply EF Core migrations
- Seed initial data (users, roles, etc.)

---

## License

This project is provided for educational and demonstration purposes.
