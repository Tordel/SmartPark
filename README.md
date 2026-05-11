# SmartPark 🅿️

A smart parking space management system that uses **Computer Vision** to automatically detect and update parking space occupancy in real time. Built with a clean architecture .NET backend, a Blazor WebAssembly frontend, and a Python AI service.

---

## Features

- **Live occupancy detection** — AI-powered image analysis determines whether a parking space is free or occupied
- **Interactive parking map** — visual grid of all spaces with real-time status colours
- **Reservations** — authenticated users can create, view, and cancel reservations
- **Admin panel** — full CRUD for parking spaces, manual status overrides, and AI detection testing
- **Role-based access** — Admin and User roles with cookie-based authentication
- **Status history & occupancy rates** — per-space historical data and 24h occupancy analytics

---

## Tech Stack

| Layer | Technology |
|---|---|
| Frontend | Blazor WebAssembly (.NET 9) |
| Backend API | ASP.NET Core Web API (.NET 9) |
| AI Service | Python (FastAPI) |
| Database | PostgreSQL |
| ORM | Entity Framework Core + Npgsql |
| Architecture | Clean Architecture + CQRS (MediatR) |
| Auth | Cookie-based authentication + BCrypt |

---

## Project Structure

```
SmartPark/
├── SmartPark.API/              # ASP.NET Core Web API
│   └── Controllers/            # ParkingSpace, Reservations, Auth, AiOccupancy
├── SmartPark.Application/      # CQRS — Commands, Queries, Handlers, DTOs
│   ├── ParkingSpaces/
│   └── Reservations/
├── SmartPark.Domain/           # Domain entities, value objects, enums, exceptions
│   ├── Entities/               # ParkingSpace, Reservation, User
│   ├── ValueObjects/           # DateTimeRange
│   └── Enums/                  # ParkingSpaceStatus, SpaceType, ReservationStatus
├── SmartPark.Infrastructure/   # EF Core DbContext, repositories, migrations
├── SmartPark.Frontend/         # Blazor WebAssembly app
│   ├── Pages/                  # Home, Spaces, Map, SpaceDetails, Reservations, Admin, Login
│   ├── Layout/                 # MainLayout, NavMenu, AuthLayout
│   ├── Services/               # ParkingSpaceService, ReservationService, AuthService, AiDetectionService
│   └── Models/                 # DTOs mirroring API responses
└── ai-service/                 # Python FastAPI AI service
```

---

## Getting Started

### Prerequisites

- [.NET 9 SDK](https://dotnet.microsoft.com/download)
- [PostgreSQL](https://www.postgresql.org/download/) (default port `5433`)
- [Python 3.10+](https://www.python.org/) with FastAPI
- [Node.js](https://nodejs.org/) (optional, for tooling)

### 1. Clone the repository

```bash
git clone https://github.com/yourusername/smartpark.git
cd smartpark
```

### 2. Configure the database

Update the connection string in `SmartPark.API/appsettings.json`:

```json
"ConnectionStrings": {
  "SmartParkDb": "Host=localhost;Port=5433;Database=smartpark_dev;Username=postgres;Password=yourpassword"
}
```

### 3. Run database migrations

```bash
dotnet ef database update --project SmartPark.Infrastructure --startup-project SmartPark.API
```

This will create all tables and seed the default admin account.

### 4. Configure the AI service URL

In `SmartPark.API/appsettings.json`:

```json
"AiIntegration": {
  "PythonBaseUrl": "http://localhost:8000",
  "AnalyzeEndpoint": "/analyze-local",
  "ApiKey": "your-api-key"
}
```

### 5. Start the backend

```bash
cd SmartPark.API
dotnet run
```

The API will be available at `https://localhost:5067`. Swagger UI at `https://localhost:5067/swagger`.

### 6. Start the frontend

```bash
cd SmartPark.Frontend
dotnet run
```

The app will be available at `http://localhost:5096`.

### 7. Start the Python AI service

```bash
cd ai-service
pip install -r requirements.txt
uvicorn main:app --reload --port 8000
```

---

## Default Accounts

| Username | Password | Role |
|---|---|---|
| `admin` | `admin` | Admin |

New accounts can be registered via the Sign In page — they are assigned the `User` role by default.

---

## API Overview

### Authentication
| Method | Endpoint | Description |
|---|---|---|
| POST | `/api/auth/login` | Sign in |
| POST | `/api/auth/logout` | Sign out |
| POST | `/api/auth/register` | Create account |
| GET | `/api/auth/me` | Current user info |

### Parking Spaces
| Method | Endpoint | Description |
|---|---|---|
| GET | `/api/parkingspace` | List all (paginated) |
| GET | `/api/parkingspace/{id}` | Get by ID |
| GET | `/api/parkingspace/available` | Get free spaces |
| GET | `/api/parkingspace/occupancy` | Occupancy rate |
| GET | `/api/parkingspace/{id}/history` | Status history |
| POST | `/api/parkingspace` | Create space (Admin) |
| PUT | `/api/parkingspace/{id}/status` | Update status (Admin) |
| DELETE | `/api/parkingspace/{id}` | Delete space (Admin) |

### Reservations
| Method | Endpoint | Description |
|---|---|---|
| GET | `/api/reservations/user/{userId}` | User's reservations |
| GET | `/api/reservations/by-parking-space/{id}` | Space reservations |
| POST | `/api/reservations` | Create reservation |
| PUT | `/api/reservations/{id}/cancel` | Cancel reservation |

### AI Detection
| Method | Endpoint | Description |
|---|---|---|
| POST | `/api/ai/occupancy/analyze` | Analyze image (Admin) |
| POST | `/api/ai/occupancy` | Push AI update (API key) |
| POST | `/api/ai/occupancy/batch` | Batch update (API key) |

---

## Parking Space Statuses

| Status | Colour | Description |
|---|---|---|
| Free | 🟢 Green | Space is available |
| Occupied | 🔴 Red | Space is taken |
| Reserved | 🟣 Purple | Space has an active reservation |

---

## Space Types

- **Standard** — regular parking space
- **EV** — electric vehicle charging space
- **Disabled** — accessibility space
- **Motorcycle** — motorcycle-only space

---

## Environment Variables / Configuration

| Key | Description | Default |
|---|---|---|
| `ConnectionStrings:SmartParkDb` | PostgreSQL connection string | — |
| `AiIntegration:PythonBaseUrl` | Python AI service base URL | `http://localhost:8000` |
| `AiIntegration:AnalyzeEndpoint` | AI analyze endpoint path | `/analyze-local` |
| `AiIntegration:ApiKey` | API key for AI push updates | — |
| `AllowedOrigins` | CORS allowed frontend origins | `http://localhost:5096` |

---

## License

MIT
