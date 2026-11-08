# pwnlabs.space

A CTF (Capture The Flag) platform for hands-on cybersecurity training. Each user gets an isolated lab environment, built on demand from Docker containers, with unique flags per instance.

> Bachelor's thesis project — work in progress.

## Tech stack

- **Backend:** ASP.NET Core 10 Web API (Clean Architecture)
- **Frontend:** Angular 22
- **Labs:** Docker (via Docker.DotNet)

## Project structure

```
pwnlabs/
├── client/                      # Angular frontend
└── server/
    └── src/
        ├── Pwnlabs.Domain/          # Entities and domain rules
        ├── Pwnlabs.Application/     # Use cases, interfaces, DTOs
        ├── Pwnlabs.Infrastructure/  # Database, Docker, external services
        └── Pwnlabs.Api/             # HTTP layer (controllers, Program.cs)
```

## Getting started

### Prerequisites

- .NET 10 SDK
- Node.js and npm
- Angular CLI (`npm install -g @angular/cli`)

### Run the API

```powershell
dotnet run --project server/src/Pwnlabs.Api --launch-profile https
```

Health check: `https://localhost:7173/health`

### Run the client

```powershell
cd client
npm install
ng serve
```

Open `http://localhost:4200`. Requests to `/api` are proxied to the backend.