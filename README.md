# KMC Events — Kandy Municipal Council Event Platform

A service-oriented community event management platform developed for the **CSE5013 — Service-Oriented Computing** module.

The system allows members of the public to discover and register for community events, while event organizers can create and manage their own events through an authenticated dashboard.

The solution is implemented as **two independently running applications** — a RESTful Web API and an MVC web application — demonstrating service-oriented architecture principles.

---

## Features

### Public Users

* Browse available community events
* Search events by keyword
* Filter events by event type and date
* View individual event details
* Register for events without creating an account
* Receive validation when an event reaches its capacity

### Organizers

* Create an organizer account
* Log in securely
* Create new events
* View and manage their own events
* Edit their own events
* Delete their own events
* View the list of registered attendees for their events

### Security

* JWT-based authentication
* Ownership-based authorization — organizers can only modify or view registrations for events they created
* Organizer ownership validation
* Event modification restricted to the event owner
* API-level authorization rather than relying only on frontend restrictions

---

## Service-Oriented Architecture

The system is intentionally separated into two projects:

```text
┌─────────────────────────┐
│       KMC.Web           │
│  ASP.NET Core MVC       │
│                         │
│  Razor Views            │
│  Controllers            │
│  ApiClient.cs           │
└────────────┬────────────┘
             │
             │ HTTP / REST
             ▼
┌─────────────────────────┐
│       KMC.Api           │
│  ASP.NET Core Web API   │
│                         │
│  Controllers            │
│  Services               │
│  DTOs                   │
│  Authentication         │
└────────────┬────────────┘
             │
             ▼
┌─────────────────────────┐
│     SQLite Database     │
│      EF Core            │
└─────────────────────────┘
```

### KMC.Api

The REST API is responsible for:

* Authentication and authorization
* Event management
* Event registrations
* Business logic
* Ownership validation
* Database operations
* Data persistence

The API is the **only application that directly accesses the database**.

### KMC.Web

The MVC website provides the user-facing interface.

It does **not** directly access the database. All data retrieval and operations are performed through `ApiClient.cs`, which communicates with the REST API over HTTP.

This separation demonstrates the service-oriented principle of separating the **service layer** from the **consumer/application layer**.

---

## How to Run

### Prerequisites

* [.NET 10 SDK](https://dotnet.microsoft.com/)
* Visual Studio 2022 or another compatible IDE
* Git

### Clone the repository

```bash
git clone <YOUR-REPOSITORY-URL>
cd KMC-EventPlatform
```

### Restore dependencies

```bash
dotnet restore
```

### Build the solution

```bash
dotnet build
```

### Run the API

Open a terminal:

```bash
cd KMC.Api
dotnet run
```

The API will run at:

```text
https://localhost:7050
```

Swagger documentation is available at:

```text
https://localhost:7050/swagger
```

### Run the Web Application

Open a second terminal:

```bash
cd KMC.Web
dotnet run
```

The website will run at:

```text
https://localhost:7100
```

The SQLite database is created automatically when the API is started.

---

## Running with Visual Studio

1. Open `KMC.sln`
2. Right-click the solution
3. Select **Configure Startup Projects**
4. Select **Multiple startup projects**
5. Set both `KMC.Api` and `KMC.Web` to **Start**
6. Click **Apply**
7. Run the solution with `Ctrl + F5`

---

## API Documentation

Swagger/OpenAPI is included with the API to make the available REST endpoints easy to explore and test.

```text
https://localhost:7050/swagger
```

---

## Security Considerations

The application uses JWT authentication for protected API operations.

Ownership checks are performed at the service/API level. For example, an organizer cannot modify or delete another organizer's event simply by changing the event ID in an HTTP request.

The website also restricts access to organizer functionality, but the API remains the authoritative security boundary.

> **Important:** The JWT secret included in the development configuration is intended only for local development. Before deploying the application, replace it with a strong randomly generated secret and store it securely using an appropriate secrets-management solution.

---

## Current Limitations

The current implementation has a few planned improvements:

* Event type is currently stored as a text value rather than a dedicated enumeration/dropdown.
* Event images currently use category-based placeholder images.
* Individual event image uploads are not implemented yet.
* The application has only been tested in a local development environment.
* Production deployment and cloud hosting have not yet been implemented.

---

## Future Improvements

Potential future improvements include:

* Individual event image uploads
* Dedicated event categories
* Email notifications for registrations
* Advanced organizer analytics
* Pagination for large event lists
* Cloud deployment
* Centralized secrets management
* Automated testing and CI/CD
* Containerization using Docker

---

## Academic Context

**Module:** CSE5013 — Service-Oriented Computing

**Assessment:** Task 2 — Service-Oriented Solution

The project was developed to demonstrate service-oriented computing concepts through a RESTful API consumed by a separate web application.

The accompanying academic submission includes the comparison between a traditional monolithic architecture and the implemented service-oriented solution, together with supporting system and architecture diagrams.

---

## Author

**Ruzaik Siyadh**

Software Engineering & Computing Student

---

## License

This project was developed as an academic project for educational purposes.
