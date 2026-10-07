# Sales Management

A small sales management application built as a technical assignment.

The application manages salespeople, sales districts, stores, and the relationship between salespeople and districts. Each district has exactly one primary salesperson and may have additional secondary salespeople.

## Technology

* **Backend:** ASP.NET Core / C#
* **Database:** SQL Server
* **Data access:** Dapper + native SQL
* **Frontend:** Angular
* **Testing:** xUnit, integration tests, API end-to-end tests, Angular/Jasmine tests
* **Logging:** Serilog

## Architecture

The solution is split into a small number of focused projects:

```text
SalesManagement
├── SalesManagement.Application
├── SalesManagement.Data
├── SalesManagement.Api
├── SalesManagement.Web
├── SalesManagement.UnitTests
├── SalesManagement.IntegrationTests
└── SalesManagement.EndToEndTests
```

### Application

Contains the application/domain models, DTOs, service interfaces, and application services.

The application layer does not depend on the database implementation.

### Data

Contains the Dapper-based repositories and SQL required to access SQL Server.

The assignment specifically requires native SQL rather than an ORM, so database access is implemented using Dapper and `Microsoft.Data.SqlClient`.

### API

Provides the REST API consumed by the Angular application.

### Web

Contains the Angular frontend. The browser communicates with the backend through the REST API rather than accessing the database directly.

### Tests

The solution contains unit tests, integration tests against a separate SQL Server database, API end-to-end tests, and Angular tests. All automated tests can be executed with dotnet test for the .NET solution and npm test -- --watch=false for the Angular application.

---

## Domain

The main entities are:

* **Salesperson** - a person who sells to stores.
* **District** - a sales district containing stores.
* **Store** - a store belonging to a district.
* **DistrictSalesperson** - the relationship between a salesperson and a district, including their role.

A salesperson may belong to multiple districts, and the same salesperson may be the primary salesperson for multiple districts.

A district must have exactly one primary salesperson. Other assigned salespeople are secondary.

---

## Database Design

The database contains four main tables:

```text
Salesperson
    |
    | 1..*
    |
DistrictSalesperson
    |
    | *..1
    |
District
    |
    | 1..*
    |
Store
```

`DistrictSalesperson` uses a composite primary key consisting of:

```text
DistrictId + SalespersonId
```

The database also enforces:

* valid foreign-key references
* unique district names
* unique salesperson/district relationships
* valid salesperson roles
* at most one primary salesperson per district

The "at most one primary" rule is enforced with a filtered unique index.

The application maintains the stronger invariant that every district has a primary salesperson.

### Primary salesperson changes

Changing the primary salesperson is performed inside a database transaction.

The operation:

1. Demotes the existing primary salesperson to secondary.
2. Updates an existing relationship or inserts a new relationship.
3. Commits both changes together.

This prevents a partially completed primary reassignment from being persisted.

The application prevents removal of the current primary salesperson because a district must always retain a primary salesperson.

---

## API

| Method | Endpoint                                                   | Description                               |
| ------ | ---------------------------------------------------------- | ----------------------------------------- |
| GET    | `/api/districts`                                           | Get all districts                         |
| GET    | `/api/districts/{districtId}`                              | Get district details                      |
| GET    | `/api/salespersons`                                        | Get all salespeople                       |
| PUT    | `/api/districts/{districtId}/salespersons/{salespersonId}` | Assign a salesperson or change their role |
| DELETE | `/api/districts/{districtId}/salespersons/{salespersonId}` | Remove a salesperson from a district      |

### Assigning a salesperson

The PUT endpoint accepts:

```json
{
  "role": "Primary"
}
```

or:

```json
{
  "role": "Secondary"
}
```

Assigning a salesperson as primary automatically makes the previous primary salesperson secondary.

### Error handling

The API returns:

* `200 OK` for successful GET requests
* `204 No Content` for successful assignment/removal operations
* `404 Not Found` when a referenced district or salesperson does not exist
* `409 Conflict` when an operation would violate the requirement that a district must have a primary salesperson

---

## Running the Application

### Prerequisites

* .NET SDK compatible with the solution
* SQL Server Developer Edition or another compatible SQL Server instance
* Node.js/npm compatible with the Angular project

### Database setup

The database scripts are located in the database setup directory.

They are intended to be executed in order:

```text
01-CreateDatabase.sql
02-CreateTables.sql
03-CreateConstraints.sql
04-SeedTestData.sql
```

A PowerShell setup script is also provided to create/configure the required database.

The application uses `SalesManagement` as the development database.

Integration tests use a separate `SalesManagement_Test` database so that test execution does not modify development data.

### Start the API

Configure the API connection string in the API application's configuration.

Then run:

```powershell
dotnet run --project SalesManagement.Api
```

### Start the Angular application

From the Angular project:

```powershell
npm install
npm start
```

The Angular development proxy forwards `/api` requests to the local ASP.NET Core API.

---

## Testing

Run the complete .NET test suite with:

```powershell
dotnet test
```

The test suite includes:

### Unit tests

Test application/service behavior independently of SQL Server.

### Integration tests

Execute the data layer against the dedicated `SalesManagement_Test` SQL Server database.

This verifies the actual SQL, constraints, transactions, and database behavior rather than mocking the database.

### End-to-end API tests

Exercise the API through the ASP.NET Core application and cover both read and mutation operations, including error cases.

### Angular tests

The Angular project contains tests for:

* HTTP service requests
* district loading
* salesperson filtering
* salesperson assignment
* primary salesperson changes
* error handling

---

## Architectural Decisions

### Dapper and native SQL

The assignment calls for native SQL rather than an ORM. Dapper provides lightweight parameter binding and object mapping while leaving the SQL explicit and visible.

This also keeps the data access implementation straightforward for the relatively small domain.

### Separate application and data layers

The application layer defines the required repository/service abstractions while the data layer provides the SQL Server implementation.

This keeps database-specific concerns out of the application logic without introducing unnecessary abstraction layers.

### Three queries for district details

District details contain salespeople and stores. Fetching both collections through one large join would produce repeated combinations of salespeople and stores.

The repository therefore retrieves the district, salespeople, and stores separately and combines them into the response model.

This keeps the SQL simple and avoids accidental duplication.

### Primary salesperson invariant

The database enforces the easy-to-enforce part of the invariant — there can never be more than one primary salesperson for a district.

The application enforces the business rule that a district cannot lose its only primary salesperson.

Primary reassignment is transactional so that the old and new roles change together.

### Separate integration database

Integration tests use a dedicated database rather than the development database. This prevents tests from modifying development data and makes the test environment predictable.

---

## Production Considerations

The implementation is intentionally scoped to the assignment rather than attempting to reproduce a complete production platform.

For a production deployment, additional concerns would need to be addressed.

### Security

The API would require authentication and authorization. Access to salesperson and district data would need to be restricted according to the application's business and organizational requirements.

### Configuration and secrets

Connection strings and other secrets should be supplied through an appropriate secret-management mechanism rather than committed configuration.

### Observability

Production deployments would benefit from centralized structured logging, metrics, tracing, health checks, and alerting.

### CI/CD

A production solution would typically have automated build, test, deployment, and database migration processes.

### Infrastructure

Depending on scale and availability requirements, containerization, orchestration, load balancing, database high availability, and automated infrastructure management could be introduced.

### Messaging and distributed processing

The assignment does not require asynchronous messaging or distributed processing. Technologies such as RabbitMQ or Kafka could be appropriate if future requirements introduced asynchronous workflows, integration with other systems, or event-driven processing.

### Scaling

The current application is intentionally a simple API backed by SQL Server. If usage grew significantly, caching, horizontal API scaling, database optimization, and potentially read replicas could be considered based on actual performance requirements.

These concerns are intentionally documented rather than implemented because introducing them into this assignment would add infrastructure and complexity without improving the core solution being evaluated.

## Requirements Coverage

The implementation addresses the main requirements of the assignment as follows.

| Requirement                           | Implementation                                                                                                                                                                                                            |
| ------------------------------------- | ------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| MSSQL database                        | SQL Server database with `Salesperson`, `District`, `Store`, and `DistrictSalesperson` tables.                                                                                                                            |
| Data model and business rules         | Foreign keys, primary keys, check constraints, unique constraints, and a filtered unique index enforce database-level rules. Application logic enforces the requirement that a district always has a primary salesperson. |
| ASP.NET Core Web API                  | REST API implemented with ASP.NET Core controllers.                                                                                                                                                                       |
| API design                            | Resource-oriented endpoints for districts and salespeople, including assignment, promotion, and removal operations.                                                                                                       |
| Separation of database and API models | Database access is isolated in the Data layer. Application/domain models and API DTOs are separate from the database representation.                                                                                      |
| Layering                              | API → Application → Data. Controllers handle HTTP concerns, application services handle business rules, and repositories handle SQL/database access.                                                                      |
| Native SQL / Dapper                   | Database access uses explicit SQL with Dapper and `Microsoft.Data.SqlClient`; no ORM is used.                                                                                                                             |
| API error handling                    | Application-specific exceptions are translated into appropriate HTTP responses, including `404 Not Found` and `409 Conflict` for business-rule violations.                                                                |
| Automated API testing                 | Unit tests, integration tests against a separate SQL Server test database, and end-to-end API tests cover normal and error scenarios.                                                                                     |
| Angular UI                            | Angular application consumes the REST API and provides district, salesperson, store, assignment, promotion, and removal functionality.                                                                                    |
| MVVM / separation of concerns         | Angular's component/service architecture is used instead of reproducing WPF-style MVVM literally. Components manage presentation state and user interaction while services encapsulate API communication.                 |
| UI/API model separation               | Angular uses TypeScript interfaces representing the API contract rather than accessing backend/database models directly. Server-side business rules remain authoritative.                                                 |
| UI testing                            | Angular unit tests cover HTTP services, district loading, salesperson filtering, assignment, primary salesperson changes, and error handling.                                                                             |

### Angular and MVVM

The UI uses Angular rather than WPF, so the implementation does not reproduce WPF's traditional MVVM structure literally. Angular components provide the presentation and view-model responsibilities, while Angular services isolate communication with the REST API.

The application intentionally avoids introducing a separate client-side domain model where it would only duplicate server-side business logic. Business invariants such as primary salesperson assignment are enforced by the API and database, keeping the server authoritative.

---

## Scope and Tradeoffs

The implementation prioritizes:

* clear separation of concerns
* explicit SQL
* transactional business operations
* database integrity
* testability
* a simple REST API
* a functional Angular client
* maintainable code with limited abstraction

It deliberately avoids adding infrastructure or abstractions that are not required to demonstrate these qualities.

The result is intended to be a small, understandable implementation that could be extended as requirements grow rather than a fully productionized distributed system.
