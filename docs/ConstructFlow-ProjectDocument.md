# ConstructFlow — Complete Project Reference Document
### Interview Preparation & Technical Deep Dive

---

## Table of Contents

1. [Project Overview](#1-project-overview)
2. [Why This Project Exists](#2-why-this-project-exists)
3. [Architecture Overview](#3-architecture-overview)
4. [Technology Stack — Full Detail](#4-technology-stack)
5. [Backend — Layer by Layer](#5-backend-layer-by-layer)
6. [Database Design](#6-database-design)
7. [Frontend — Blazor WASM](#7-frontend-blazor-wasm)
8. [Authentication & Security](#8-authentication--security)
9. [Key Design Decisions & Why](#9-key-design-decisions--why)
10. [Deployment Architecture](#10-deployment-architecture)
11. [What Was Built — Module by Module](#11-what-was-built)
12. [Problems Solved & Lessons Learned](#12-problems-solved--lessons-learned)
13. [How to Present This in an Interview](#13-how-to-present-this-in-an-interview)
14. [Common Interview Questions & Answers](#14-common-interview-questions--answers)

---

## 1. Project Overview

**ConstructFlow** is a full-stack enterprise web application for construction project management. It handles:

- **Project tracking** — create and monitor construction projects with budgets, timelines, and status
- **Vendor management** — maintain a supplier/contractor database
- **Procurement workflow** — raise purchase requests, collect vendor quotes, compare them side-by-side, and award to the best vendor
- **Inventory management** — track material stock per project with stock-in/out/adjustment transactions and automatic low-stock alerts

**Live URLs:**
- Frontend: `https://amal-das.github.io/ConstructFlow/`
- API: `https://constructflowapi.runasp.net`
- GitHub: `https://github.com/Amal-Das/ConstructFlow`

**Demo credentials:**
- Email: `amal@constructflow.com`
- Password: `Demo@1234`

---

## 2. Why This Project Exists

This is a **portfolio project built to demonstrate enterprise .NET patterns** in a publicly showable domain, without any intellectual property overlap with my employer.

Professionally, I work on a **Planned Maintenance System (PMS)** for the maritime/shipping industry. The architectural patterns are identical — CQRS with MediatR, Dapper with stored procedures and TVPs, Blazor WASM frontend, multi-tenant SQL Server database — but the construction management domain was deliberately chosen so the code is 100% original and safe to share publicly.

This project proves I can apply the same patterns I use at work independently, in a clean-room implementation.

---

## 3. Architecture Overview

### Solution Structure

```
ConstructFlow/
├── ConstructFlow.Domain/          # Core entities and enums — zero dependencies
├── ConstructFlow.Contracts/       # DTOs, request/response models — no logic
├── ConstructFlow.Application/     # Business logic, CQRS, validators
├── ConstructFlow.Infrastructure/  # Data access, JWT, BCrypt
├── ConstructFlow.API/             # HTTP layer — controllers, middleware
├── ConstructFlow.Web/             # Blazor WASM client
├── ConstructFlow.Database/        # SSDT database project
└── ConstructFlow.UnitTests/       # Test project (future)
```

### Dependency Direction (Clean Architecture)

```
Domain ← Application ← Infrastructure ← API
                ↑
           Contracts
```

- **Domain** knows nothing about anyone
- **Contracts** knows nothing about anyone (pure DTOs)
- **Application** knows Domain and Contracts only — defines interfaces (ports), never implements them
- **Infrastructure** knows Application — provides concrete implementations of Application's interfaces
- **API** knows Application and Infrastructure — wires everything together (composition root)
- **Web** is completely separate — talks to API over HTTP only

This means if we wanted to swap Dapper for Entity Framework, or swap SQL Server for PostgreSQL, the Application layer would be completely untouched. Only Infrastructure changes.

### Request Flow (end to end)

```
Browser → Blazor WASM → HTTPS → API Controller
       → MediatR Send
       → ValidationBehavior (FluentValidation pipeline)
       → Command/Query Handler
       → Repository Interface (Application layer)
       → Dapper Repository Implementation (Infrastructure)
       → Stored Procedure
       → SQL Server
       → Result flows back up the same chain
       → JSON response
       → Blazor component re-renders
```

---

## 4. Technology Stack

### Backend

| Technology | Version | Purpose |
|---|---|---|
| ASP.NET Core | 8.0 | Web API framework |
| MediatR | 11.x (last open-source) | CQRS implementation |
| FluentValidation | Latest | Request validation with pipeline behavior |
| AutoMapper | 12.0.1 (last free) | Entity→DTO mapping |
| Dapper | Latest | Micro-ORM for SQL execution |
| Microsoft.Data.SqlClient | Latest | SQL Server connection |
| BCrypt.Net-Next | Latest | Password hashing |
| System.IdentityModel.Tokens.Jwt | Latest | JWT generation/validation |
| Swashbuckle.AspNetCore | 6.6.2 (pinned) | Swagger/OpenAPI |

### Frontend

| Technology | Version | Purpose |
|---|---|---|
| Blazor WebAssembly | .NET 8.0 | SPA frontend framework |
| Microsoft.AspNetCore.Components.Authorization | .NET 8.0 | Auth state management |
| Custom CSS (no UI library) | — | All styling hand-written |
| IJSRuntime | Built-in | Browser localStorage access |

### Database

| Technology | Purpose |
|---|---|
| SQL Server | Primary database |
| Stored Procedures | All data access (no inline SQL in app) |
| Table-Valued Parameters (TVPs) | Bulk insert of child records |
| SSDT Database Project | Version-controlled schema |

### DevOps / Infrastructure

| Technology | Purpose |
|---|---|
| GitHub Actions | CI/CD pipeline |
| GitHub Pages | Blazor WASM hosting (free) |
| MonsterASP.NET | API hosting (free, no credit card) |
| MonsterASP.NET SQL | Database hosting |
| Let's Encrypt (via MonsterASP.NET) | HTTPS certificate |

---

## 5. Backend — Layer by Layer

### 5.1 Domain Layer (`ConstructFlow.Domain`)

**Purpose:** Core business entities and enums. No dependencies on any other project or NuGet package.

**Entities:**
- `Project` — name, code, location, status, dates, budget
- `PurchaseRequest` — linked to project, has status workflow, contains items
- `PurchaseRequestItem` — individual line items on a request
- `Vendor` — supplier with contact info
- `VendorQuote` — a vendor's response to a purchase request
- `VendorQuoteItem` — per-item pricing in a quote
- `InventoryItem` — a material tracked per project
- `InventoryTransaction` — stock in/out/adjustment against an inventory item
- `User` — auth entity with hashed password and role

**Base Entity:**
All entities inherit from `BaseEntity` which provides:
```csharp
public int Id { get; set; }
public DateTime CreatedAt { get; set; }
public DateTime? UpdatedAt { get; set; }
public bool IsDeleted { get; set; }  // soft delete pattern
```

**Enums:**
- `ProjectStatus`: Planning, InProgress, OnHold, Completed, Cancelled
- `PurchaseRequestStatus`: Draft, Submitted, QuotesReceived, Approved, Rejected, Awarded
- `InventoryTransactionType`: StockIn, StockOut, Adjustment

### 5.2 Contracts Layer (`ConstructFlow.Contracts`)

**Purpose:** Shared DTOs between Application and API. Pure data classes, no logic, no dependencies.

This is the contract between your API and any consumer (Blazor client, Postman, future mobile app). If you change a DTO here, both the API and the client need updating — that's intentional, it makes breaking changes explicit.

Key contracts:
- `LoginRequest` / `LoginResponse` — auth
- `ProjectDto`, `CreateProjectRequest`, `UpdateProjectRequest`
- `PurchaseRequestDto`, `PurchaseRequestListDto` (lighter version for list views)
- `QuoteComparisonDto` — complex nested structure for the comparison grid
- `InventoryItemDto` — includes computed `IsLowStock` flag
- `AwardQuoteRequest`

### 5.3 Application Layer (`ConstructFlow.Application`)

**Purpose:** All business logic lives here. No infrastructure concerns. No Dapper, no SQL, no HTTP.

#### CQRS Pattern

Every operation is either a **Command** (changes state) or a **Query** (reads state):

```
Commands (write):
├── Auth/Commands/Register/RegisterCommand + Handler + Validator
├── Auth/Commands/Login/LoginCommand + Handler + Validator
├── Projects/Commands/CreateProject/...
├── Projects/Commands/UpdateProject/...
├── Vendors/Commands/CreateVendor/...
├── Vendors/Commands/SubmitVendorQuote/...
├── Vendors/Commands/AwardQuote/...
├── PurchaseRequests/Commands/CreatePurchaseRequest/...
├── Inventory/Commands/CreateInventoryItem/...
└── Inventory/Commands/RecordStockTransaction/...

Queries (read):
├── Projects/Queries/GetAllProjects/...
├── Projects/Queries/GetProjectById/...
├── Vendors/Queries/GetAllVendors/...
├── Vendors/Queries/GetQuoteComparison/...
├── PurchaseRequests/Queries/GetAllPurchaseRequests/...
├── PurchaseRequests/Queries/GetPurchaseRequestById/...
└── Inventory/Queries/GetInventoryByProject/...
```

Each command/query has exactly three files: the **Command/Query** class (the request), the **Handler** (the logic), and optionally a **Validator** (the rules).

#### Validation Pipeline (MediatR Behavior)

```csharp
public class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
```

This sits in the MediatR pipeline and automatically runs all validators before any handler executes. If validation fails, it throws a `ValidationException` with all errors — the handler never even runs. You define validators once, and they fire automatically on every matching request. No `if (ModelState.IsValid)` in controllers, no manual validation calls in handlers.

#### Repository Interfaces (Ports)

Application defines interfaces for all data access:
```csharp
IProjectRepository
IPurchaseRequestRepository
IVendorRepository
IVendorQuoteRepository
IInventoryRepository
IUserRepository
IJwtTokenGenerator
IPasswordHasher
```

Application only knows about these interfaces — never the concrete implementations. This is the Dependency Inversion Principle in practice.

#### Custom Exceptions

```csharp
NotFoundException       → HTTP 404
ValidationException     → HTTP 400 with field errors
BusinessRuleException   → HTTP 400 with error code (e.g. INSUFFICIENT_STOCK)
UnauthorizedAccessException → HTTP 401
```

### 5.4 Infrastructure Layer (`ConstructFlow.Infrastructure`)

**Purpose:** Implements all the interfaces defined in Application. This is where Dapper, SQL Server, JWT, and BCrypt actually live.

#### DapperContext

```csharp
public class DapperContext
{
    public IDbConnection CreateConnection() => new SqlConnection(_connectionString);
}
```

Simple factory — creates a new `SqlConnection` per call. Dapper works best with fresh connections per operation (SQL Server's connection pool handles the actual pooling underneath).

#### Repository Pattern — How it works

Every repository follows the same pattern:
1. Create `DynamicParameters` — map C# types to SQL parameter types explicitly
2. Add OUTPUT parameters where needed (for `@NewId`, `@ReturnStatus`, `@ErrorCode`)
3. Execute stored procedure with `commandType: CommandType.StoredProcedure`
4. Check `@ReturnStatus` — throw `BusinessRuleException` if not "SUCCESS"
5. Return result

Example (simplified):
```csharp
public async Task<int> CreateAsync(Project project)
{
    var parameters = new DynamicParameters();
    parameters.Add("Name", project.Name, DbType.String);
    parameters.Add("Status", (int)project.Status, DbType.Int32);
    parameters.Add("NewId", dbType: DbType.Int32, direction: ParameterDirection.Output);

    using var connection = _context.CreateConnection();
    await connection.ExecuteAsync("PRJ.usp_CreateProject", parameters, 
        commandType: CommandType.StoredProcedure);

    return parameters.Get<int>("NewId");
}
```

#### TVP Pattern (Table-Valued Parameters)

For operations that insert a parent + multiple child records in one SP call:

```csharp
var itemsTable = new DataTable();
itemsTable.Columns.Add("ItemName", typeof(string));
itemsTable.Columns.Add("Quantity", typeof(decimal));

foreach (var item in items)
    itemsTable.Rows.Add(item.ItemName, item.Quantity);

parameters.Add("Items", itemsTable.AsTableValuedParameter("PR.tvp_PurchaseRequestItem"));
```

This passes the entire list of items as a single parameter to the SP, which then does a set-based INSERT rather than a loop from C#. More efficient, and keeps the transaction entirely inside SQL Server.

#### Multi-Result-Set Queries

The Quote Comparison screen needs three result sets in one round trip:

```csharp
using var multi = await connection.QueryMultipleAsync(
    "VND.usp_GetQuoteComparisonData", parameters,
    commandType: CommandType.StoredProcedure);

var prItems = (await multi.ReadAsync<PrItemFlat>()).ToList();       // Result set 1
var vendorSummaries = (await multi.ReadAsync<VendorSummaryFlat>()).ToList();  // Result set 2
var priceCells = (await multi.ReadAsync<PriceCellFlat>()).ToList(); // Result set 3
```

Then the C# code stitches these three lists into a single `QuoteComparisonDto` object with nested collections — avoiding multiple round trips to the database.

#### JWT Token Generator

```csharp
var claims = new[]
{
    new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
    new Claim(JwtRegisteredClaimNames.Email, user.Email),
    new Claim(ClaimTypes.Name, user.FullName),
    new Claim(ClaimTypes.Role, user.Role),
    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
};
```

Key: `Jti` (JWT ID) is a unique identifier per token — important for token invalidation if you ever build a refresh token / logout flow.

#### BCrypt Password Hashing

```csharp
public string Hash(string password) =>
    BCrypt.Net.BCrypt.HashPassword(password, workFactor: 12);

public bool Verify(string password, string hash) =>
    BCrypt.Net.BCrypt.Verify(password, hash);
```

Work factor 12 means BCrypt performs 2^12 = 4096 iterations of hashing. Higher = slower to brute-force, but also slightly slower on legitimate logins. 12 is the industry-standard recommendation as of 2024-2026.

### 5.5 API Layer (`ConstructFlow.API`)

**Purpose:** HTTP concerns only. Controllers, middleware, DI composition root.

#### Controllers

Every controller follows the same pattern:
```csharp
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ProjectsController : ControllerBase
{
    private readonly IMediator _mediator;
    // ...

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateProjectCommand command)
    {
        var id = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetById), new { id }, new { id });
    }
}
```

Controllers are intentionally thin — no business logic, no validation, just receiving the HTTP request and handing it to MediatR. 

#### Global Exception Middleware

```csharp
var (statusCode, message, errors) = exception switch
{
    NotFoundException notFoundEx => (404, notFoundEx.Message, null),
    ValidationException validationEx => (400, "Validation failed.", validationEx.Errors),
    BusinessRuleException businessEx => (400, businessEx.Message, new { errorCode = businessEx.ErrorCode }),
    UnauthorizedAccessException => (401, "Invalid email or password.", null),
    _ => (500, "An unexpected error occurred.", null)
};
```

This single piece of middleware catches every unhandled exception from any layer and converts it into a consistent JSON error response. No try-catch in controllers, no inconsistent error formats.

#### Program.cs — Dependency Injection

```csharp
builder.Services.AddApplication();      // MediatR, FluentValidation, AutoMapper
builder.Services.AddInfrastructure(config);  // Dapper repos, JWT, BCrypt
```

Both Application and Infrastructure expose a single extension method (`AddApplication()`, `AddInfrastructure()`) that registers all their own dependencies. The API project just calls these two methods — it doesn't need to know about individual repositories or validators. This is the composition root pattern.

---

## 6. Database Design

### Schema Organization

Every module has its own SQL schema, matching the domain boundaries:

| Schema | Purpose |
|---|---|
| `AUTH` | Users table, auth stored procedures |
| `PRJ` | Projects table |
| `PR` | Purchase requests and items |
| `VND` | Vendors, vendor quotes, quote items |
| `INV` | Inventory items and transactions |

This organization means:
- You can look at `VND.*` objects and immediately know they relate to vendors
- Permissions can be granted at the schema level
- It mirrors your application's module structure — consistency throughout

### Soft Delete Pattern

Every table has `IsDeleted BIT NOT NULL DEFAULT 0`. Nothing is ever physically deleted — records are just marked as deleted. Every query filters `WHERE IsDeleted = 0`.

This is standard practice in enterprise systems because:
- You maintain an audit trail of everything
- Deleted records can be restored if needed
- Foreign key constraints remain valid

### Stored Procedure Convention

Every write-path SP follows this exact pattern:
```sql
CREATE PROCEDURE PRJ.usp_CreateProject
    -- input parameters...
    @NewId INT OUTPUT,
    @ReturnStatus NVARCHAR(50) OUTPUT,
    @ErrorCode NVARCHAR(50) OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        BEGIN TRANSACTION;
        
        -- actual work here
        
        COMMIT TRANSACTION;
        SET @ReturnStatus = 'SUCCESS';
        SET @ErrorCode = NULL;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0
            ROLLBACK TRANSACTION;
        SET @ReturnStatus = 'FAILURE';
        SET @ErrorCode = ERROR_MESSAGE();
    END CATCH
END
```

`@ReturnStatus` / `@ErrorCode` output pattern is consistent across all write SPs — the C# layer always checks `@ReturnStatus == 'SUCCESS'` and throws `BusinessRuleException` if not. This means business rule failures (insufficient stock, quote not found) are communicated through a proper exception chain rather than silent failure.

### TVP Types

```sql
CREATE TYPE PR.tvp_PurchaseRequestItem AS TABLE
(
    ItemName NVARCHAR(200),
    Unit NVARCHAR(50),
    Quantity DECIMAL(18,2),
    Specification NVARCHAR(500) NULL
);

CREATE TYPE VND.tvp_VendorQuoteItem AS TABLE
(
    PurchaseRequestItemId INT,
    UnitPrice DECIMAL(18,2)
);
```

These are SQL Server user-defined table types — used as parameters to pass entire lists into a single SP call.

### Key Stored Procedures

| SP | What it does |
|---|---|
| `PRJ.usp_CreateProject` | INSERT project, return new ID |
| `PR.usp_CreatePurchaseRequest` | INSERT PR header + all items via TVP in one transaction |
| `VND.usp_SubmitVendorQuote` | INSERT quote + all item prices via TVP, compute total, update PR status |
| `VND.usp_GetQuoteComparisonData` | 3 result sets: PR items, vendor summaries, per-item pricing grid |
| `VND.usp_AwardQuote` | Reset all quotes IsAwarded=0, set winner IsAwarded=1, update PR status, with @@ROWCOUNT guard |
| `INV.usp_RecordStockTransaction` | StockOut guard (reject if quantity < stock), CASE expression for update type |
| `AUTH.usp_CreateUser` | INSERT user with pre-hashed password |
| `AUTH.usp_GetUserByEmail` | Fetch user for login verification |

### SSDT Database Project

`ConstructFlow.Database` is a Visual Studio SQL Server Database Project. It was created by importing the local database, which captured all tables, SPs, and TVP types as individual `.sql` files organized by `Schema\Object Type` folder structure.

**Why this matters:**
- Schema is version-controlled in git alongside application code
- Publish to any target (MonsterASP.NET, Azure SQL, local) with one click
- SSDT generates a diff between "current state" and "desired state" — safe migrations
- When you move to Azure SQL later: just change the publish target connection string

---

## 7. Frontend — Blazor WASM

### Architecture

Blazor WebAssembly runs .NET code **in the browser** via WebAssembly. There's no server-side rendering — the entire .NET runtime downloads to the client on first load, and then the app runs entirely client-side, making API calls over HTTP.

This means:
- Fast after initial load (no round trips for navigation)
- Works offline for static content
- The same C# code and types you write on the backend can be shared/reused
- **No Node.js, no JavaScript framework** — pure C#

### Code-Behind Pattern

Every page is split into two files:
- `ComponentName.razor` — markup only (HTML + Razor syntax)
- `ComponentName.razor.cs` — logic only (C# partial class)

```csharp
// Login.razor.cs
public partial class Login : ComponentBase
{
    [Inject] private HttpClient Http { get; set; } = default!;
    [Inject] private NavigationManager Navigation { get; set; } = default!;
    // ...
}
```

`[Inject]` in the code-behind file works the same as `@inject` in the markup file — Blazor merges them because it's a `partial class`. This is the cleaner approach for any component with more than a few lines of logic.

### Custom Authentication State Provider

The most interesting Blazor piece. Instead of using a third-party auth library, we built a custom `JwtAuthenticationStateProvider`:

```csharp
public class JwtAuthenticationStateProvider : AuthenticationStateProvider
{
    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        var token = await _localStorage.GetItemAsync("authToken");
        
        if (!string.IsNullOrWhiteSpace(token) && !IsTokenExpired(token))
        {
            var identity = new ClaimsIdentity(ParseClaimsFromJwt(token), "jwt");
            return new AuthenticationState(new ClaimsPrincipal(identity));
        }
        
        return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
    }
}
```

This reads the JWT from `localStorage`, parses its claims (user name, role) directly in the browser, and makes them available to the entire app via Blazor's `CascadingAuthenticationState`. No separate API call needed to check who's logged in — the information is already in the token.

### localStorage Without a Library

Instead of a third-party package (Blazored.LocalStorage was archived/unmaintained), we use `IJSRuntime` directly:

```csharp
public async Task SetItemAsync(string key, string value) =>
    await _jsRuntime.InvokeVoidAsync("localStorage.setItem", key, value);

public async Task<string?> GetItemAsync(string key) =>
    await _jsRuntime.InvokeAsync<string?>("localStorage.getItem", key);
```

This calls the browser's native `localStorage` API through Blazor's JS interop system. No external dependency, and it shows you understand the interop layer rather than just consuming a wrapper.

### Auth Header DelegatingHandler

```csharp
public class AuthHeaderHandler : DelegatingHandler
{
    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var token = await _authStateProvider.GetTokenAsync();
        if (!string.IsNullOrWhiteSpace(token))
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        
        return await base.SendAsync(request, cancellationToken);
    }
}
```

This is a **message handler** in the `HttpClient` pipeline. Every API call automatically gets the Bearer token attached without any manual code in individual components. Registered once in `Program.cs`, works everywhere.

### Unauthorized Response Handler

```csharp
public class UnauthorizedResponseHandler : DelegatingHandler
{
    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var response = await base.SendAsync(request, cancellationToken);
        
        if (response.StatusCode == HttpStatusCode.Unauthorized)
        {
            await _authStateProvider.MarkUserAsLoggedOut();
            _navigation.NavigateTo("login", forceLoad: true);
        }
        
        return response;
    }
}
```

Complements the auth header handler — while the first one attaches tokens going out, this one handles expired tokens coming back as 401s. One-click logout from anywhere in the app, automatically triggered by any API call that returns 401.

### CSS Design System (No Framework)

Instead of Bootstrap component classes or a UI library, all styling uses a CSS custom properties (variables) system:

```css
:root {
    --color-primary: #2563eb;
    --color-bg: #f8fafc;
    --color-surface: #ffffff;
    --color-text: #0f172a;
    --color-text-muted: #64748b;
    --color-border: #e2e8f0;
    --radius: 10px;
    --shadow: 0 1px 3px rgba(0,0,0,0.08);
}
```

These tokens are defined once and used throughout every component. Consistent spacing, colors, and effects without repetition. Adding a new page means reusing existing classes (`.table-card`, `.stat-card`, `.badge`, `.form-card`) — the design is already there.

### SVG Donut Chart (No Library)

The dashboard's project status chart is built entirely with SVG, no charting library:

```csharp
// SVG circle with stroke-dasharray creates a pie slice
private const double CircleCircumference = 2 * Math.PI * 45;

private string GetDashArray(double percent) =>
    $"{percent * CircleCircumference} {CircleCircumference}";
```

`stroke-dasharray` controls how much of the circle's outline is drawn (the colored arc) vs transparent (the gap). `stroke-dashoffset` rotates where each arc starts. Stacking multiple circles with calculated offsets creates the multi-color donut. Understanding SVG at this level is uncommon and worth mentioning.

---

## 8. Authentication & Security

### Flow

```
1. User submits email + password to POST /api/Auth/login
2. UserRepository fetches user by email from AUTH.Users
3. BCrypt.Verify(plainPassword, storedHash) — if false, throw UnauthorizedAccessException
4. JwtTokenGenerator.GenerateToken(user) — creates signed JWT with claims
5. JWT returned to client
6. Client stores JWT in localStorage
7. JwtAuthenticationStateProvider reads JWT, parses claims
8. Every subsequent API call includes: Authorization: Bearer {token}
9. API's JwtBearer middleware validates the token on every request
10. If token expired → 401 → UnauthorizedResponseHandler → auto-logout
```

### Security Decisions

**Why BCrypt over MD5/SHA:** BCrypt is deliberately slow (configurable via work factor) and includes a built-in salt. MD5/SHA are fast, which makes them vulnerable to brute-force and rainbow table attacks. BCrypt's slowness is a feature, not a bug.

**Why we don't return "user not found" vs "wrong password" as separate errors:** The login handler returns the same "Invalid email or password" message whether the email doesn't exist or the password is wrong. This prevents **email enumeration attacks** — an attacker can't use your login endpoint to check which emails are registered.

**JWT Claims:** We embed `Sub` (user ID), `Email`, `Name`, and `Role` in the token. The Blazor client reads these directly from the token without an extra API call — the user's identity is fully contained in the token, which is the point of JWT.

**JWT Secret Key:** The production key is stored in `appsettings.Production.json` on the server, not committed to git. The local development key is in .NET User Secrets (stored outside the project folder, never committed). The key must be at least 32 characters (256 bits) for HMAC-SHA256.

---

## 9. Key Design Decisions & Why

### 9.1 Why Dapper over Entity Framework?

This is the most common question you'll face. The honest answer:

**Real reason:** My professional experience is primarily with stored procedures and Dapper. This project intentionally mirrors production patterns from my day job, where complex reporting queries, multi-result-set SPs, and TVPs are the norm.

**Technical justification:**
- Stored procedures give DBAs control over execution plans and indexing
- TVPs allow set-based operations instead of looping from application code
- Multi-result-set queries reduce round trips for complex screens like the comparison view
- SPs can be updated without redeploying the application (for hotfixes)
- Full visibility into exactly what SQL is executing — no hidden N+1 queries

**The tradeoff acknowledged:** EF Core is faster to develop with for simple CRUD and handles migrations elegantly. For a new greenfield project without complex reporting requirements, EF Core might be the right choice. The right tool depends on the context.

### 9.2 Why MediatR version 11.x (not latest)?

MediatR went commercial after version 12.x. For an open-source portfolio project, using a commercial library is an unnecessary risk and complication. Pinning to the last fully-open-source version is a deliberate, reasoned choice — not ignorance of newer versions.

Same logic applies to AutoMapper 12.0.1.

### 9.3 Why a separate Contracts project?

The Contracts project (`ConstructFlow.Contracts`) holds all DTOs and request/response models. This means:
- The API knows what to deserialize
- The Application knows what to return
- The Blazor client could reference this project directly if they were in the same solution (or via a NuGet package if separate)
- Breaking changes to DTOs are explicit — you have to change Contracts, which forces you to update all consumers

Without Contracts, DTOs would live in Application or API — making them either polluted with HTTP concerns (in API) or polluted with infrastructure concerns (in Application).

### 9.4 Why a custom AuthenticationStateProvider?

Blazored.LocalStorage (the commonly recommended package) is now archived and unmaintained. Rather than depend on dead code, we implemented `IJSRuntime` calls to browser `localStorage` directly — a 15-line wrapper that we fully control and understand.

This also avoids pulling in a package just to call two browser APIs. It's a good example of evaluating whether a dependency is actually worth its weight.

### 9.5 Why GitHub Pages + MonsterASP.NET instead of a single host?

Two reasons:
- **Architecture:** Blazor WASM is just static files after build. It doesn't need a .NET runtime — it literally runs in the browser. GitHub Pages is purpose-built for static files, it's fast, has great uptime, and is free forever. Using a .NET host to serve static files is wasteful.
- **Constraints:** The RuPay debit card isn't accepted by Azure for verification. MonsterASP.NET is a legitimate hosting provider that requires no card for a free plan. This is a practical constraint, not a permanent limitation — Azure is the target once a Visa/Mastercard becomes available.

### 9.6 Why @ReturnStatus/@ErrorCode output params on every SP?

This is the pattern I use at work and mirrors production systems. The advantages:
- Every SP has a consistent success/failure contract — no need to guess "did this work?"
- Specific error codes (`INSUFFICIENT_STOCK`, `QUOTE_NOT_FOUND`) allow the application layer to translate failures into user-friendly messages and appropriate HTTP status codes
- Prevents generic 500 errors from surfacing to the user when a business rule is violated
- The TRY/CATCH with explicit ROLLBACK ensures transactions are clean even if the SP crashes mid-way

---

## 10. Deployment Architecture

```
User Browser
    │
    ▼
GitHub Pages (HTTPS, static files)
https://amal-das.github.io/ConstructFlow/
    │
    │  HTTPS API calls (JWT in Authorization header)
    ▼
MonsterASP.NET (IIS, .NET 8)
https://constructflowapi.runasp.net
    │
    │  Internal datacenter network (no public internet)
    ▼
MonsterASP.NET SQL Server
db56787.databaseasp.net (local access, port 1433)
```

### GitHub Actions CI/CD

The `.github/workflows/deploy-pages.yml` workflow:
1. Triggers on every push to `master`
2. Sets up .NET 8 on Ubuntu runner
3. `dotnet publish` the Blazor project in Release mode
4. Adds `.nojekyll` (prevents GitHub from treating it as a Jekyll site)
5. Copies `index.html` to `404.html` (enables client-side routing on direct URL access)
6. Deploys the `publish-output/wwwroot` folder to GitHub Pages

This means every git push automatically redeploys the frontend. No manual steps.

### Production Config

```
appsettings.json — structure only, empty sensitive values (committed to git)
appsettings.Production.json — real connection string and JWT key (on server only, not in git)
.NET User Secrets — local dev secrets (stored in user profile, never in repo)
```

### CORS Configuration

The API's CORS policy explicitly allows:
- `https://amal-das.github.io` — the production Blazor client
- `https://localhost:xxxx` — local development

Without explicit CORS configuration, the browser refuses to make cross-origin requests to the API. CORS is enforced by the browser, not the server — the server just needs to tell the browser it's allowed.

---

## 11. What Was Built — Module by Module

### Auth Module
- Register endpoint with BCrypt hashing
- Login endpoint with JWT generation
- FluentValidation: email format, password strength (8+ chars, uppercase, number)
- Security: generic "invalid email or password" prevents email enumeration
- Blazor: Login page, Register page, custom auth state provider

### Projects Module
- Full CRUD (Create, Read list, Read by ID, Update)
- Status workflow: Planning → InProgress → OnHold → Completed / Cancelled
- Budget tracking with decimal precision
- Blazor: Projects list page (table with status badges), Create project form

### Vendors Module
- Vendor CRUD
- Active/Inactive status
- Blazor: Vendors list page, Create vendor form

### Purchase Requests Module
- Create with dynamic item list (TVP insert)
- Status workflow: Submitted → QuotesReceived → Awarded
- List view with project name joined
- Blazor: List page with "Quotes & Comparison" action, Create form with add/remove item rows

### Quote Comparison & Award Module
- Submit vendor quotes (TVP insert, auto-computes total)
- Three-result-set comparison query
- Side-by-side comparison grid (items as rows, vendors as columns)
- Award flow: resets all quotes, marks winner, updates PR status
- Blazor: The flagship screen — vendor selector, pricing form, comparison table, Award button

### Inventory Module
- Create items per project
- Stock In / Stock Out / Adjustment transactions
- Automatic low-stock detection (QuantityInStock ≤ MinimumStockLevel)
- Stock-out guard in SP (rejects transaction if insufficient stock, returns INSUFFICIENT_STOCK error code)
- Blazor: List filtered by project, modal for stock transactions, low-stock badge

---

## 12. Problems Solved & Lessons Learned

### Problem 1: AutoMapper version licensing
**What happened:** AutoMapper 16.x required a commercial license. AutoMapper 13+ changed DI registration patterns, 15+ required a license key.
**Fix:** Pinned to 12.0.1 + `AutoMapper.Extensions.Microsoft.DependencyInjection 12.0.1` — the last stable fully-open-source version.
**Lesson:** Always check licensing before adding dependencies to an open-source portfolio project.

### Problem 2: JWT token key too short
**What happened:** `IDX10720: key has 184 bits` — the initial dev secret was only 23 characters, below the 256-bit (32 character) minimum for HMAC-SHA256.
**Fix:** Used a 66+ character key.
**Lesson:** HMAC-SHA256 requires keys ≥ 32 characters. Each ASCII character = 8 bits, so 32 characters = 256 bits minimum.

### Problem 3: Generic 500 errors masking business rule failures
**What happened:** When a stock-out transaction exceeded available inventory, the SP returned `@ReturnStatus = 'FAILURE'` with `@ErrorCode = 'INSUFFICIENT_STOCK'`, but the repository threw a generic `InvalidOperationException`. This fell into the middleware's catch-all `_` case and returned a meaningless 500 to the client.
**Fix:** Created `BusinessRuleException` with an `ErrorCode` property. Updated all repositories to throw this instead of `InvalidOperationException`. Added a specific case in the middleware to map it to HTTP 400 with the error code in the response body.
**Lesson:** Every layer of the stack needs to preserve domain-specific error information rather than losing it in a generic exception. Define exception types that carry meaningful context.

### Problem 4: Blazor WASM missing auth redirect
**What happened:** When navigating directly to a protected URL while logged out, the `<NotAuthorized>` content in `App.razor` was just a static "You are not authorized" message — no redirect to login.
**Fix:** Created a `RedirectToLogin` component that calls `Navigation.NavigateTo("login", forceLoad: true)` in `OnInitialized`. Referenced this as the `<NotAuthorized>` content.
**Lesson:** `AuthorizeRouteView` handles authorization checks but doesn't redirect by itself — you have to provide your own `<NotAuthorized>` behavior.

### Problem 5: Token expiry not triggering logout
**What happened:** When a JWT expired mid-session, API calls started returning 401s silently, leaving the UI broken but no redirect to login.
**Fix:** Added `UnauthorizedResponseHandler` (DelegatingHandler) that intercepts every HTTP response — if it's a 401, it logs the user out and navigates to login.
**Lesson:** Auth state needs to be monitored reactively, not just checked on page load.

### Problem 6: appsettings.json encoding corruption on server
**What happened:** MonsterASP.NET's web file manager saved `appsettings.json` with a BOM (Byte Order Mark), causing `'0xC3' is an invalid start of a value` JsonReaderException on startup.
**Fix:** Republished from Visual Studio (which always produces clean UTF-8 without BOM), overwriting the corrupted file.
**Lesson:** Never edit config files through a web-based file manager if you can avoid it — publish tools are more reliable for getting file encoding right.

### Problem 7: GitHub Actions not triggering
**What happened:** The workflow file was placed at the repository root (`/deploy-pages.yml`) instead of the required `.github/workflows/deploy-pages.yml` path. GitHub simply ignored it.
**Fix:** Created `.github/workflows/` folder structure and moved the file there.
**Lesson:** GitHub Actions has a strict path requirement. The folder must be exactly `.github/workflows/` at the repository root. A file anywhere else is not a workflow.

### Problem 8: Base href breaking navigation on GitHub Pages
**What happened:** `Navigation.NavigateTo("/projects")` navigated to `amal-das.github.io/projects` instead of `amal-das.github.io/ConstructFlow/projects` — dropping the subpath entirely.
**Fix:** Changed all absolute paths (leading `/`) to relative paths (no leading `/`) in both `NavigateTo()` calls and `<a href>` tags. `NavLink` components were unaffected since they're Blazor-aware.
**Lesson:** When deploying Blazor WASM to a subpath (not domain root), all internal navigation must use relative paths. Blazor's `NavigationManager` combines relative paths with the base href automatically, but absolute paths (`/path`) bypass this and navigate to the domain root.

---

## 13. How to Present This in an Interview

### The 30-Second Pitch (when they ask "tell me about a project you've built")

> "I built ConstructFlow — a full-stack construction project management system. The backend is ASP.NET Core 8 with Clean Architecture, CQRS through MediatR, and Dapper talking to SQL Server through stored procedures and table-valued parameters. The frontend is Blazor WebAssembly with a custom auth system I built from scratch — no third-party auth library. It's fully deployed: the Blazor client on GitHub Pages with a GitHub Actions CI/CD pipeline, the API on a hosted .NET server. The centerpiece feature is a quote comparison screen where you submit prices from multiple vendors and compare them side-by-side in a grid, then award to the winner — all backed by a multi-result-set stored procedure."

### The 2-Minute Version (for a technical screen)

Add to the above:
- Walk through the Clean Architecture dependency direction
- Mention the MediatR validation pipeline behavior (validators fire automatically, handler never runs if validation fails)
- Explain the TVP pattern (pass a whole list in one SQL round trip)
- Mention the multi-result-set query for the comparison screen (3 result sets, one round trip)
- Mention custom exception types mapping to HTTP status codes
- Mention the deployment architecture and GitHub Actions

### The Technical Deep Dive (when they ask you to walk through the code)

Go to the Quote Comparison screen — it's the most technically interesting and shows the most patterns at once:
1. **Blazor component** — `[Inject]`, code-behind, `OnInitializedAsync`, `Task.WhenAll`
2. **MediatR query** — `GetQuoteComparisonQuery` → handler → repository
3. **Repository** — `QueryMultipleAsync`, three flat DTOs, assembly into nested DTO
4. **SP** — three SELECT statements, schema-prefixed, `SET NOCOUNT ON`
5. **Blazor rendering** — nested `@foreach` loops building a dynamic grid

Then walk through the Award flow — shows the update pattern, `@@ROWCOUNT` guard, status transitions.

### For Behavioral Questions ("tell me about a technical challenge you overcame")

Use the BCrypt work factor / key length story — it's concrete, shows you understand security fundamentals, and has a clear problem → investigation → root cause → fix structure.

Or use the GitHub Actions workflow path story — shows debugging discipline (checking what actually deployed, reading error output, systematic elimination of causes).

### Connecting It to Your Professional Experience

> "The patterns I used here are the same ones I use daily in my current role building a Planned Maintenance System for the maritime industry — CQRS with MediatR, Dapper with stored procedures, Blazor frontend. I deliberately built this in a different domain to avoid any IP overlap, but the architecture is identical. This project proves I can apply these patterns independently, outside the structure of my existing codebase."

---

## 14. Common Interview Questions & Answers

**Q: Why didn't you use Entity Framework?**
> "Deliberate choice. My professional work involves complex stored procedures, multi-result-set queries, and TVPs — patterns that are awkward with EF. This project mirrors that. Dapper gives full control over SQL, which matters when you need predictable execution plans and you're working alongside DBAs. EF is great for simple CRUD — the right tool depends on context."

**Q: How does CQRS help here?**
> "Every operation is either a Command (changes state) or a Query (reads). This separation means reads and writes can evolve independently. More practically: every command has a matching validator — when a command passes through MediatR's pipeline, FluentValidation fires automatically before the handler runs. The handler never has to check if the input is valid. And controllers stay thin — they just map HTTP to MediatR commands, no business logic."

**Q: How does authentication work?**
> "JWT-based. Login returns a signed token containing the user's ID, email, name, and role. The Blazor client stores this in localStorage and attaches it as a Bearer token on every API call via a DelegatingHandler. The API validates the signature on every request using the same secret key. Token expiry is handled reactively — a second DelegatingHandler watches for 401 responses and auto-logs out. No session state anywhere."

**Q: How do you handle errors?**
> "Three layers. FluentValidation catches bad input before the handler runs. Custom exception types (NotFoundException, BusinessRuleException, ValidationException) communicate specific failure modes up the stack. Global exception middleware catches everything and maps it to appropriate HTTP status codes with consistent JSON error bodies. No try-catch in controllers or handlers — exceptions propagate naturally and are handled in one place."

**Q: What would you do differently if starting again?**
> "Automated tests from the start. The Application layer is designed to be testable — interfaces everywhere, no static dependencies — but I didn't write tests during the build. The validators and handlers are the highest-value test targets. I'd also add a refresh token flow rather than hard expiry — a better user experience than getting kicked out mid-session."

**Q: How does the deployment pipeline work?**
> "GitHub Actions workflow triggers on every push to master. It checks out the code, sets up .NET 8, publishes the Blazor project in Release mode, adds a .nojekyll file and a 404.html fallback for client-side routing, then deploys the static output to GitHub Pages. The API is deployed manually via Visual Studio's Web Deploy to MonsterASP.NET — I'd automate this too with a second workflow targeting their WebDeploy endpoint."

**Q: What's the SSDT database project?**
> "It's a Visual Studio project that represents the entire database schema as version-controlled files — one .sql file per table, stored procedure, and TVP type. Rather than maintaining a folder of migration scripts, SSDT computes the diff between the current database and the desired state and generates the appropriate ALTER/CREATE statements. Publish to any target — local, staging, production — with one click, just changing the connection string."

---

## Summary

ConstructFlow demonstrates:

✅ **Clean Architecture** — proper dependency direction, no leaking of infrastructure concerns into business logic  
✅ **CQRS with MediatR** — commands, queries, pipeline behaviors  
✅ **Dapper with stored procedures** — TVPs, multi-result-set queries, output parameters  
✅ **JWT authentication** — generation, validation, Blazor state management  
✅ **BCrypt password hashing** — security fundamentals  
✅ **FluentValidation** — automatic pipeline validation  
✅ **Global exception handling** — consistent error contract  
✅ **Blazor WASM** — code-behind pattern, DelegatingHandlers, JS interop  
✅ **Custom CSS design system** — CSS variables, no UI library  
✅ **GitHub Actions CI/CD** — automated deployment on push  
✅ **SSDT database project** — version-controlled schema  
✅ **Production deployment** — live, HTTPS, real data  

---

*Document prepared: June 2026*  
*Project repository: https://github.com/Amal-Das/ConstructFlow*  
*Live demo: https://amal-das.github.io/ConstructFlow/*
