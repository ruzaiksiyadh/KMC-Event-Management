# KMC Community Event Platform — Starter Solution

A scaffold for the CSE5013 (Service-Oriented Computing) assignment: Kandy Municipal
Council event platform, built as a REST API + a Razor/MVC client that consumes it.

## Structure

```
KMC-EventPlatform/
  KMC.sln
  KMC.Api/        ASP.NET Core Web API — the "service" layer
    Controllers/   AuthController, EventsController, RegistrationsController
    Services/      IEventService/EventService, IRegistrationService/RegistrationService, ITokenService/TokenService
    Models/        Event, Registration, ApplicationUser
    Data/          ApplicationDbContext (EF Core + SQLite)
  KMC.Web/         ASP.NET Core MVC (Razor) — the client, talks ONLY to the API
    Controllers/   Home, Events (public), Account (login/register), Dashboard (organizer, [Authorize])
    Services/      ApiClient.cs — every API call goes through this one class
    Views/         Razor pages for browsing, registering, and managing events
```

## Why this design demonstrates SOC

- **Separation of concerns**: business logic lives in `Services/` behind interfaces, never
  directly in controllers. Controllers are thin — they just translate HTTP <-> service calls.
  This is exactly the seam you'd cut along if you split `EventService`, `RegistrationService`
  into independently deployable microservices later — which is a good point to make in your
  written comparison (Task 1).
- **The MVC client never touches the database.** It only calls the API over HTTP through
  `ApiClient.cs`. Swap KMC.Web for a mobile app or another council's system and the API
  doesn't change — that's the core SOA argument for maintainability/scalability vs a monolith.
- **Ownership enforcement**: `EventService.UpdateAsync`/`DeleteAsync` throw
  `UnauthorizedAccessException` if the caller isn't the event's creator — satisfies the
  "only the organizer who created it can edit it" requirement.
- **Public vs authenticated split**: search/browse/register endpoints are `[AllowAnonymous]`;
  create/edit/delete/view-registrations require a JWT — matches "public users search/register,
  organizers manage."

## Running it locally

You'll need the [.NET 10 SDK](https://dotnet.microsoft.com/download) installed (this
sandbox doesn't have it, so the code hasn't been compiled here — read it over once before
your first run).

```bash
cd KMC-EventPlatform

# 1. Restore & build both projects
dotnet restore
dotnet build

# 2. Run the API first (creates kmc_events.db automatically on first run)
cd KMC.Api
dotnet run
# Swagger UI opens at https://localhost:7050/swagger — use it to sanity-check
# endpoints before touching the web client.

# 3. In a second terminal, run the web client
cd ../KMC.Web
dotnet run
# Browse to https://localhost:7100
```

If the ports are already taken on your machine, edit `applicationUrl` in each project's
`Properties/launchSettings.json`, and update `ApiBaseUrl` in `KMC.Web/appsettings.json` to match.

**Important before you submit:** replace the placeholder JWT secret in
`KMC.Api/appsettings.json` (`Jwt:Key`) with your own random 32+ character string, and don't
commit real secrets if this goes in a public repo.

## What's already working

- Organizer registration/login issuing a JWT (`AuthController`)
- Public search & filter by keyword/type/date range (`GET /api/events`)
- Event CRUD with ownership checks (`EventsController`)
- Participant registration with a capacity check (`RegistrationsController`)
- MVC client: public browse/search/details/register pages, organizer login/register,
  and a dashboard to create/edit/delete events

## What you should still add or customize

1. **Data validation** — add `[Required]`/`[StringLength]` attributes to the DTOs and
   view models, and show validation errors properly in the Razor views
   (`asp-validation-for`, plus the jQuery unobtrusive validation scripts in `_Layout.cshtml`
   if you want live client-side validation).
2. **A registrations-list view** for organizers (`GET /api/events/{id}/registrations`
   already exists in the API — just needs a Dashboard action + view to display it).
3. **Styling/branding** — swap in Kandy MC colours/logo, adjust the Bootstrap theme.
4. **Testing** — a few unit tests against `EventService`/`RegistrationService` (they take
   an `ApplicationDbContext`, so an in-memory EF Core provider makes this easy) will
   strengthen your submission and are easy marks if the module rewards test coverage.
5. **Deployment note for your write-up** — mention how `KMC.Api` and `KMC.Web` could be
   deployed and scaled independently (e.g. containers), which is the practical payoff of
   the SOA argument you're making in Task 1.

## Task 1 (written comparison) — a starting structure

Not code, but worth planning before you write it:

1. Briefly define monolithic vs SOA.
2. Map the case study's own requirements (multiple departments/organizers, public search,
   growth in usage, need to integrate with other council systems later) onto the
   *maintainability* and *scalability* criteria specifically — don't just recite generic
   pros/cons.
3. Recommend SOA/microservices-oriented API design (which is what this scaffold sets up),
   and justify it using the seams already visible in this codebase (Event service vs
   Registration service vs Auth) as a concrete example.
4. Note trade-offs honestly (operational complexity, network latency, harder local
   debugging) — a report that only lists advantages tends to lose marks for not showing
   critical evaluation.
