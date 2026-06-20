# ConstructFlow

A full-stack construction project management system built to demonstrate enterprise-grade .NET architecture — Clean Architecture, CQRS, Dapper with stored procedures and table-valued parameters, JWT authentication, and a custom-designed Blazor WebAssembly frontend.

> **Live Demo:** _Coming soon_
> **Tech Stack:** ASP.NET Core 8 · Blazor WebAssembly · SQL Server · Dapper · MediatR · FluentValidation

---

## Why this project exists

This isn't a tutorial clone. It's built to mirror the kind of system I work on professionally — a Planned Maintenance System in the shipping industry — but in a completely different domain (construction) to keep it free of any employer IP, while demonstrating the same architectural patterns: CQRS with MediatR, Dapper-based data access with stored procedures and TVPs instead of an ORM, and a layered Clean Architecture backend paired with a Blazor WASM frontend.

## Screenshots

| Login               | Dashboard           |
| ------------------- | ------------------- |
| _[screenshot here]_ | _[screenshot here]_ |

| Quote Comparison    | Purchase Request    |
| ------------------- | ------------------- |
| _[screenshot here]_ | _[screenshot here]_ |

---

## Core Features

### 🏗️ Project Management

Create and track construction projects with budget, location, timeline, and status.

### 🤝 Vendor & Quote Comparison

The centerpiece feature — submit purchase requests, collect quotes from multiple vendors, compare them side-by-side in a price-per-item grid, and award the winning bid. Built on a multi-result-set stored procedure and table-valued parameters, mirroring real-world procurement workflows.

### 📦 Inventory Tracking

Stock-in/stock-out/adjustment transactions per project, with automatic low-stock flagging and a database-level guard against overselling stock (rejects a stock-out transaction if it would drive inventory negative).

### 🔐 Authentication

JWT-based auth with BCrypt password hashing, role claims, and a custom `AuthenticationStateProvider` on the Blazor side — no third-party auth library.

---

## Architecture

```
ConstructFlow/
├── ConstructFlow.Domain/          # Entities, enums — zero dependencies
├── ConstructFlow.Contracts/       # DTOs shared between Application and API
├── ConstructFlow.Application/     # CQRS (MediatR), FluentValidation, business logic
├── ConstructFlow.Infrastructure/  # Dapper repositories, JWT, BCrypt
├── ConstructFlow.API/             # Controllers, middleware, composition root
├── ConstructFlow.Web/             # Blazor WebAssembly client
└── database/                      # SQL schema, stored procedures, TVP definitions
```

**Why no Entity Framework?** Deliberate choice. Real-world systems at scale — especially ones with complex reporting, multi-result-set queries, and tight performance requirements — frequently rely on stored procedures and table-valued parameters rather than ORM-generated SQL. This project uses Dapper throughout to demonstrate that pattern directly: hand-written, schema-prefixed stored procedures (`PRJ.usp_CreateProject`, `VND.usp_SubmitVendorQuote`, etc.), TVPs for bulk inserts, and multi-result-set queries via `QueryMultipleAsync` for the comparison screen.

### Backend layers, end to end

A request flows: **Controller → MediatR Command/Query → FluentValidation pipeline behavior → Handler → Repository interface (Application) → Dapper implementation (Infrastructure) → Stored Procedure → SQL Server.**

Every write-path stored procedure follows a consistent convention: `@ReturnStatus`/`@ErrorCode` output parameters inside a `TRY/CATCH` block with explicit transaction rollback, so failures surface as clean, typed exceptions (`BusinessRuleException`, `NotFoundException`) rather than generic 500s.

### Frontend

Blazor WebAssembly, hosted standalone (not server-hosted), talking to the API over HTTP with a JWT attached via a `DelegatingHandler`. Authentication state is driven by a custom `AuthenticationStateProvider` that reads claims directly out of the JWT — no Blazored.LocalStorage or third-party state package, just `IJSRuntime` calling browser `localStorage` directly.

All UI is hand-styled CSS (no component library, no Bootstrap component classes) — color tokens, shadows, and spacing are defined once as CSS variables and reused throughout, including a dependency-free SVG donut chart on the dashboard built with `stroke-dasharray`/`stroke-dashoffset` rather than a charting library.

---

## Tech Stack

**Backend**

- ASP.NET Core 8 Web API
- MediatR (CQRS)
- FluentValidation
- AutoMapper
- Dapper
- SQL Server (stored procedures, TVPs, transactions)
- JWT Bearer authentication
- BCrypt.Net-Next (password hashing)

**Frontend**

- Blazor WebAssembly (.NET 8)
- Custom JWT auth state provider
- Hand-written CSS design system
- Zero third-party UI/component libraries

---

## Getting Started

### Prerequisites

- .NET 8 SDK
- SQL Server (local or Express)
- Visual Studio 2022 (or VS Code)

### 1. Clone the repo

```bash
git clone https://github.com/Amal-Das/ConstructFlow.git
cd ConstructFlow
```

### 2. Set up the database

Run the scripts in `/database` against a new `ConstructFlowDb` database, in this order:

1. `schema/` — creates all schemas and tables
2. `types/` — creates TVP type definitions
3. `procedures/` — creates all stored procedures

### 3. Configure secrets

The API uses .NET User Secrets for local development. From `ConstructFlow.API`:

```bash
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Server=localhost;Database=ConstructFlowDb;Trusted_Connection=True;TrustServerCertificate=True;"
dotnet user-secrets set "JwtSettings:SecretKey" "<any-string-32-chars-or-longer>"
```

### 4. Run the API

```bash
cd ConstructFlow.API
dotnet run
```

Swagger UI will be available at `/swagger`.

### 5. Run the client

Update `ConstructFlow.Web/wwwroot/appsettings.Development.json` with your API's URL, then:

```bash
cd ConstructFlow.Web
dotnet run
```

---

## Project Status

This project is under active development as part of my preparation for a job transition. Current focus areas:

- [ ] Automated tests (xUnit)
- [ ] CI pipeline (GitHub Actions)
- [ ] Live deployment

See [open issues](../../issues) for the full roadmap.

---

## About

Built by [Amal Das](https://linkedin.com/in/amal-das-944013213) — a .NET developer specializing in enterprise web applications, SQL Server performance optimization, and Blazor.

This project intentionally avoids any code or domain logic from my employer; the construction-management domain was chosen specifically to demonstrate the same architectural patterns I use professionally without any IP overlap.
