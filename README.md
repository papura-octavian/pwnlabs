# pwnlabs.space

A CTF (Capture The Flag) platform for hands-on cybersecurity training. Each user gets an isolated lab environment, built on demand from Docker containers, with unique flags per instance.

> Bachelor's thesis project — work in progress.

## Tech stack

- **Backend:** ASP.NET Core 10 Web API (Clean Architecture)
- **Frontend:** Angular 22
- **Database:** PostgreSQL 18 with Entity Framework Core
- **Labs:** Docker (via Docker.DotNet)

## Project structure

```
pwnlabs/
├── client/                      # Angular frontend
├── server/
│   └── src/
│       ├── Pwnlabs.Domain/          # Entities and domain rules
│       ├── Pwnlabs.Application/     # Use cases, interfaces, DTOs
│       ├── Pwnlabs.Infrastructure/  # Database, Docker, external services
│       └── Pwnlabs.Api/             # HTTP layer (controllers, Program.cs)
├── docker-compose.yml           # Local PostgreSQL
└── .env.example                 # Template for local environment variables
```

## Getting started

### Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- Node.js and npm
- Angular CLI: `npm install -g @angular/cli`
- Docker Desktop
- EF Core CLI: `dotnet tool install --global dotnet-ef`

### 1. Start the database

```powershell
Copy-Item .env.example .env
```

Edit `.env` and set your own values for `POSTGRES_DB`, `POSTGRES_USER` and `POSTGRES_PASSWORD`. Then:

```powershell
docker compose up -d
```

PostgreSQL is exposed on `localhost:5433` (only to your machine).

### 2. Configure the connection string

The API reads the connection string from [user secrets](https://learn.microsoft.com/aspnet/core/security/app-secrets), so it never ends up in the repository. Use the same values you set in `.env`:

```powershell
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Host=localhost;Port=5433;Database=<POSTGRES_DB>;Username=<POSTGRES_USER>;Password=<POSTGRES_PASSWORD>" --project server/src/Pwnlabs.Api
```

### 3. Apply database migrations

```powershell
dotnet ef database update --project server/src/Pwnlabs.Infrastructure --startup-project server/src/Pwnlabs.Api
```

### 4. Run the API

```powershell
dotnet run --project server/src/Pwnlabs.Api --launch-profile https
```

Health check: `https://localhost:7173/health`

### 5. Connect to DB

```powershell
docker exec -it pwnlabs-db psql -U pwnlabs -d pwnlabs
```

### 6. Run the client

```powershell
cd client
npm install
ng serve
```

Open `http://localhost:4200`. Requests to `/api` are proxied to the backend.

## Database migrations

Add a migration after changing entities or their configuration:

```powershell
dotnet ef migrations add <Name> --project server/src/Pwnlabs.Infrastructure --startup-project server/src/Pwnlabs.Api --output-dir Persistence/Migrations
```

Apply migrations:

```powershell
dotnet ef database update --project server/src/Pwnlabs.Infrastructure --startup-project server/src/Pwnlabs.Api
```

Remove the last migration (only if it has not been applied yet):

```powershell
dotnet ef migrations remove --project server/src/Pwnlabs.Infrastructure --startup-project server/src/Pwnlabs.Api
```