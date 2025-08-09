# Task Management Frontend (React)

A single-page React application for managing employee tasks with role-based views for Employees and Managers. The app integrates with the Task Management API, supports JWT-based authentication, and uses KendoReact components for a rich UI. It can run locally with Create React App or be containerized behind Nginx for production.

---

## Table of Contents

- [Features](#features)
- [Tech Stack](#tech-stack)
- [Project Structure](#project-structure)
- [Environment Variables](#environment-variables)
- [Getting Started (Local Development)](#getting-started-local-development)
- [Available Scripts](#available-scripts)
- [Routing](#routing)
- [Authentication Flow](#authentication-flow)
- [Services (API Integration)](#services-api-integration)
  - [Auth Service](#auth-service)
  - [Task Service](#task-service)
- [UI Components](#ui-components)
- [Docker (Production and Development)](#docker-production-and-development)
  - [Production Container (Nginx)](#production-container-nginx)
  - [Development Container](#development-container)
  - [Nginx Configuration Highlights](#nginx-configuration-highlights)
- [Deployment](#deployment)
- [Troubleshooting](#troubleshooting)
- [Notes on KendoReact Licensing](#notes-on-kendoreact-licensing)
- [License](#license)

---

## Features

- Login with email and password; persists JWT token client-side.
- Role-based dashboards:
  - Employee: Create, list, update, and delete personal tasks.
  - Manager: Review and update task status (Approved/Rejected).
- Data grid with pagination, sorting, and filtering using KendoReact Grid.
- Global notifications using React Toastify.
- SPA routing with React Router.
- Production-ready Nginx image with SPA fallback, asset caching, and healthcheck.

---

## Tech Stack

- React 19, React DOM 19
- React Router DOM 7
- KendoReact (Grid, Buttons, Dropdowns, Intl, Theme Default)
- React Toastify
- Create React App (react-scripts 5)
- Testing Library (Jest DOM, React, User Event)
- Nginx (for production container)

---

## Project Structure

```
task-management-frontend 2/task-management-frontend 2/
├─ public/
├─ src/
│  ├─ App.js
│  ├─ index.js
│  ├─ components/
│  │  ├─ loginForm/
│  │  │  ├─ index.js
│  │  │  └─ style.css
│  │  ├─ employeeDashboard/
│  │  │  ├─ index.js
│  │  │  └─ style.css
│  │  └─ managerDashboard/
│  │     ├─ index.js
│  │     └─ style.css
│  ├─ pages/
│  │  ├─ LoginPage.js
│  │  ├─ EmployeePage.js
│  │  └─ ManagerPage.js
│  ├─ services/
│  │  ├─ authService.js
│  │  └─ taskService.js
│  └─ utils/
│     └─ validation.js
├─ .env
├─ Dockerfile
├─ Dockerfile.dev
├─ docker-compose.yml
└─ nginx.conf
```

---

## Environment Variables

Create a `.env` file at the project root (same directory as `package.json`):

```
REACT_APP_API_BASE_URL=http://localhost:8080/api
```

Notes:
- This is consumed at build time by Create React App. Changing it requires rebuilding the app.
- In development (`npm start`), CRA reads `.env` on startup.
- In Docker production, it is passed as a build-arg via `docker-compose.yml` and baked into the static build.
- The provided `nginx.conf` also proxies `/api/` to `http://host.docker.internal:8080` if you prefer using a relative base URL like `/api`. In that case set `REACT_APP_API_BASE_URL=/api` and rely on Nginx proxying.

---

## Getting Started (Local Development)

Prerequisites:
- Node.js 18+
- npm 8+

Steps:
1. Install dependencies:
   ```
   npm install
   ```
2. Set API base URL in `.env` (see above).
3. Start the development server:
   ```
   npm start
   ```
4. Open http://localhost:3000

The dev server live-reloads on code changes.

---

## Available Scripts

- `npm start` - Runs the app in development mode at http://localhost:3000
- `npm run build` - Builds the app for production into the `build/` folder
- `npm test` - Runs tests in watch mode (CRA + Testing Library)
- `npm run eject` - Ejects CRA configuration (one-way)

---

## Routing

Defined in `src/App.js` using React Router:

- `/` - Login page
- `/employee-dashboard` - Employee dashboard
- `/manager-dashboard` - Manager dashboard

`nginx.conf` includes SPA fallback to `index.html` so deep links work in production builds.

---

## Authentication Flow

- The login form calls `authService.login(email, password)` which POSTs to `${REACT_APP_API_BASE_URL}/Auth/login`.
- On success, the API returns an object like:
  ```
  { isSuccess: true, value: { user: { ... }, token: "JWT..." } }
  ```
- The app stores this object in `localStorage` under the `user` key.
- Token validity is checked client-side by decoding the JWT `exp` claim (see `authService.js` and `taskService.js`).
- If the token is missing/expired, the user is redirected to `/` (login).

To log out, `authService.logout()` clears `localStorage` and redirects to `/`.

---

## Services (API Integration)

All requests use `REACT_APP_API_BASE_URL` from `.env`.

### Auth Service

`src/services/authService.js`

- `login(email, password)`:
  - POST `${API_BASE_URL}/Auth/login`
  - Body: `{ email, password }`
  - Returns the parsed JSON response on success.
  - Throws an error with a descriptive message on failure.

- `isAuthenticated()`:
  - Reads the `user` object from `localStorage` and validates the JWT `exp`.
  - Returns `true` if a non-expired token exists; `false` otherwise.

- `logout()`:
  - Clears the `user` item and redirects to `/`.

### Task Service

`src/services/taskService.js`

All methods automatically attach the `Authorization: Bearer <token>` header when a valid token exists. If 401 is returned, the user session is cleared and redirected to login.

- `createTask(taskData)`
  - POST `${API_BASE_URL}/Task`
  - Body: `{ title: string, date: ISO string, hourWorked: number }`
  - Returns created task payload.

- `fetchTasksPaginated({ page = 1, pageSize = 10, searchEmployeeName = '', status = '' })`
  - GET `${API_BASE_URL}/Task?<query>`
  - Query params:
    - `page`: number
    - `pageSize`: number
    - `title`: string filter (mapped from `searchEmployeeName`)
    - `status`: string (Pending|Approved|Rejected)
    - `sortBy`: set to `date`
    - `sortOrder`: `desc`
  - Returns a paged result (`items`, `totalCount`, etc.) compatible with grid pagination.

- `updateTask(taskId, taskData)`
  - PUT `${API_BASE_URL}/Task/${taskId}`
  - Body: `{ title, date, hourWorked, status }`
  - Returns updated task payload.

- `deleteTask(taskId)`
  - DELETE `${API_BASE_URL}/Task/${taskId}`
  - Returns `true` on success (204 No Content).

---

## UI Components

- `components/loginForm`
  - Handles email/password login and basic validation.
- `components/employeeDashboard`
  - Displays the employee’s tasks in a KendoReact Grid.
  - Supports pagination, filtering, creation, editing, and deletion.
- `components/managerDashboard`
  - Displays tasks requiring managerial action.
  - Supports status updates (Approve/Reject) via a PATCH flow implemented as PUT with status in `taskData` (see backend docs for exact endpoint semantics).
- Styling
  - Kendo Default Theme is imported globally in `src/index.js`:
    ```
    import '@progress/kendo-theme-default/dist/all.css';
    ```

---

## Docker (Production and Development)

A `docker-compose.yml` is provided for both production-style and development runs.

### Production Container (Nginx)

Build and run:
```
# Powershell/cmd set var for the build (example):
set REACT_APP_API_BASE_URL=http://localhost:8080/api

# Build and start
docker compose up --build frontend
```

- Exposes `http://localhost:3000` (mapped to Nginx port 80 in the container).
- Build-time argument `REACT_APP_API_BASE_URL` is injected and baked into the static files.
- Healthcheck at `/health`.

Environment and args excerpt:
```yaml
services:
  frontend:
    build:
      context: .
      dockerfile: Dockerfile
      args:
        REACT_APP_API_BASE_URL: ${REACT_APP_API_BASE_URL}
    ports:
      - "3000:80"
    environment:
      - NODE_ENV=production
```

### Development Container

Live development with hot reload via the dev service:

```
# Ensure API base URL is set for dev
set REACT_APP_API_BASE_URL=http://localhost:8080/api

# Start dev profile
docker compose --profile dev up --build frontend-dev
```

- Exposes `http://localhost:3001` (container CRA dev server on 3000 mapped to 3001).
- Mounts project files into the container for live reload.

Excerpt:
```yaml
frontend-dev:
  build:
    context: .
    dockerfile: Dockerfile.dev
  ports:
    - "3001:3000"
  volumes:
    - .:/app
    - /app/node_modules
  environment:
    - NODE_ENV=development
    - CHOKIDAR_USEPOLLING=true
    - REACT_APP_API_BASE_URL=${REACT_APP_API_BASE_URL}
  profiles:
    - dev
```

### Nginx Configuration Highlights

`nginx.conf` (used by the production image):
- SPA fallback:
  ```
  location / {
    try_files $uri $uri/ /index.html;
  }
  ```
- Optional API proxy:
  ```
  location /api/ {
    proxy_pass http://host.docker.internal:8080;
  }
  ```
  If you set `REACT_APP_API_BASE_URL=/api`, the browser will call the same origin and Nginx will forward API requests to your backend at port 8080.
- Static asset caching and common security headers
- Health endpoint at `/health`

---

## Deployment

Option 1: Docker (recommended)
- Use the provided `Dockerfile` and `nginx.conf`.
- Set `REACT_APP_API_BASE_URL` during the build.
- Serve at your chosen port (compose maps 3000:80).

Option 2: Static hosting
- Run `npm run build`.
- Upload the `build/` folder to any static host (S3 + CloudFront, Netlify, Vercel, etc.).
- Ensure your host supports SPA fallback to `index.html`.
- If using a different domain for the API, configure CORS on the backend appropriately.

---

## Troubleshooting

- 401 Unauthorized / immediate logout
  - Token may be expired or missing. The app clears session and redirects to `/`.
  - Check `localStorage` content under `user`. Ensure the API returns a valid `value.token`.

- CORS errors in development
  - Ensure the backend allows `http://localhost:3000` (dev) and `http://localhost:3001` (dev container) in its CORS settings.
  - Alternatively, set `REACT_APP_API_BASE_URL=/api` and use the Nginx proxy (production container).

- Blank page after deployment
  - Verify SPA fallback is configured (Nginx `try_files` to `/index.html`).
  - Ensure `REACT_APP_API_BASE_URL` matches your backend in production builds.

- Healthcheck failing (Docker)
  - Visit `http://localhost:3000/health` to verify response.
  - Check container logs: `docker logs <frontend-container>`

---

## Notes on KendoReact Licensing

KendoReact is a commercial UI library. This project includes:
```js
// src/index.js
window.kendoLicenseActivation = false;
window.kendoLicenseKey = 'demo';
```
For production use, obtain and activate a proper KendoReact license per Telerik documentation. Remove demo settings and configure license as required.

---

## License

This frontend is provided for educational and demonstration purposes.
