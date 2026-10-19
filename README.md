# RoomReserve

A web application for booking meeting rooms in an office. People can browse rooms, book them, cancel or end meetings early – and the office can see **which rooms are often booked but rarely used**.

Built as the technical assignment for the IDT team internship at Present Connection.

**Stack:** ASP.NET Core (.NET 10) · EF Core In-Memory · React 19 · TypeScript · Vite · Tailwind CSS · shadcn/ui · TanStack Query

---

## Contents

- [Quick start](#quick-start)
- [Features](#features)
- [Architecture](#architecture)
- [Booking rules](#booking-rules)
- [Room usage report](#room-usage-report)
- [API](#api)
- [Configuration](#configuration)
- [Testing](#testing)
- [Decisions and assumptions](#decisions-and-assumptions)
- [Known limitations and next steps](#known-limitations-and-next-steps)
- [Project structure](#project-structure)

---

## Quick start

**Prerequisites:** [.NET 10 SDK](https://dotnet.microsoft.com/download) and [Node.js 20.19+](https://nodejs.org)

```bash
git clone https://github.com/<your-username>/RoomReserve.git
cd RoomReserve
npm install     # installs root and frontend packages
npm run dev     # starts the API and the web app together
```

| What                  | URL                          |
| --------------------- | ---------------------------- |
| Web app               | http://localhost:5173        |
| API explorer (Scalar) | http://localhost:5080/scalar |

Other commands, all from the repository root:

| Command         | Does                                                      |
| --------------- | --------------------------------------------------------- |
| `npm test`      | Runs the backend unit tests                               |
| `npm run build` | Builds the backend and a production build of the frontend |

> The database is **in-memory** and is re-seeded with sample rooms, users and reservations on every start. There is nothing to set up – and nothing persists between runs. Seed reservations are placed relative to the current date, so there is always a meeting in progress, upcoming bookings and recent history to look at.

<details>
<summary>Running the two parts separately</summary>

```bash
# terminal 1 – API on http://localhost:5080
dotnet run --project backend/src/RoomReserve.Api

# terminal 2 – web app on http://localhost:5173
cd frontend
npm run dev
```

The Vite dev server proxies `/api/*` to the API, so the browser only talks to one origin and no CORS configuration is needed.

</details>

---

## Features

### Required

| Page             | What it does                                                                                                                                                                                                                                                                                     |
| ---------------- | ------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------ |
| **Rooms**        | All rooms with ID, capacity, floor/location, equipment and active status. Filter by minimum capacity, floor and required equipment (a room must have _all_ selected items).                                                                                                                      |
| **Users**        | All users with their number of **active** reservations (booked and not yet ended).                                                                                                                                                                                                               |
| **Reservations** | All reservations with room, organizer, title, time, attendees and status. Filter by room, organizer, status and date range. **Create** a reservation, **cancel** an upcoming one, or **end a meeting early**. Every booking rule violation is shown in the dialog with the server's own message. |
| **Usage**        | Booked vs. actually used time per room, with rooms that are _often booked but rarely used_ flagged.                                                                                                                                                                                              |

### Nice-to-haves

|                                   | Status                                                                        |
| --------------------------------- | ----------------------------------------------------------------------------- |
| Unit tests in the backend         | ✅ 34 tests (xUnit) – booking rules, reservation lifecycle, usage calculation |
| Dependency injection              | ✅ ASP.NET Core container, registered per layer                               |
| Mapper package or custom solution | ✅ Custom – explicit extension methods (`ToDto()`)                            |
| TypeScript                        | ✅ The whole frontend, including types mirroring the API's DTOs               |
| Database seeders                  | ✅ `DbSeeder` with dates relative to "now"                                    |
| Export of the usage report to PDF | ✅ Print-optimised page → browser "Save as PDF"                               |

---

## Architecture

### Backend – layered ("clean") architecture

```
                 ┌──────────────────────────┐
                 │     RoomReserve.Api      │  controllers, error responses, Program.cs
                 └──────┬────────────┬──────┘
                        │            │
                        ▼            ▼
   ┌──────────────────────────┐   ┌──────────────────────────────┐
   │  RoomReserve.Application │◄──│  RoomReserve.Infrastructure  │  EF Core DbContext, seeder
   │  services, booking rules,│   └──────────────┬───────────────┘
   │  DTOs, report logic      │                  │
   └────────────┬─────────────┘                  │
                ▼                                ▼
   ┌───────────────────────────────────────────────────────────┐
   │                    RoomReserve.Domain                      │  entities, enums, domain errors
   └───────────────────────────────────────────────────────────┘
```

Dependencies point **inwards** and are enforced by project references – the compiler rejects, for example, a booking rule that tries to use EF's `AppDbContext` directly.

- **Domain** – `Room`, `User`, `Reservation` and their rules about _themselves_: a reservation knows its effective status (`GetStatus(now)`) and whether it can be cancelled or ended early.
- **Application** – use cases: services, the booking rules engine, the usage calculation, DTOs and mappings. It depends on an `IAppDbContext` **interface**, not on EF's implementation (dependency inversion).
- **Infrastructure** – EF Core with the in-memory provider, implementing `IAppDbContext`, plus the seeder.
- **Api** – thin controllers: bind input, call one service method, translate the result to HTTP.

A request, end to end:

```
GET /api/rooms?floor=2 → RoomsController → IRoomService → IAppDbContext (EF In-Memory)
                                                │
                    JSON ◄── RoomDto ◄── ToDto() mapping
```

### Frontend

```
src/
  api/           client.ts – the only place that calls fetch; types.ts – mirrors the C# DTOs
  components/    shared UI (Layout, ErrorAlert, ConfirmButton, …) and shadcn/ui primitives in ui/
  features/      one folder per page: rooms, users, reservations, usage
                 each with api.ts (TanStack Query hooks) and its components
  lib/           formatting, dates, labels, print helper
```

- **Feature folders** mirror the backend's `Rooms/`, `Users/`, `Reservations/`, `Reports/`.
- **Components never call `fetch`.** Hooks such as `useReservations(filter)` wrap TanStack Query, which handles caching, loading states and retries. The filter is part of the query key, so each filter combination is cached separately.
- **One error path.** Every failed request becomes an `ApiError` carrying the server's error codes and messages; `ErrorAlert` and toasts display them.
- **No business logic in the browser.** The UI only checks that a form is filled in. Statuses, whether a meeting can be cancelled or ended early (`canCancel` / `canEndEarly`), active counts and all report numbers come from the API, as the assignment requires.
- **Mutations invalidate the cache** – after creating, cancelling or ending a meeting, both the reservation list and the users' active counts refetch.

### Design patterns used

| Pattern                      | Where                                                                                                 |
| ---------------------------- | ----------------------------------------------------------------------------------------------------- |
| Layered / clean architecture | The four backend projects                                                                             |
| Dependency injection         | `AddApplication()` / `AddInfrastructure()`; constructor injection everywhere                          |
| Dependency inversion         | `IAppDbContext` defined in Application, implemented in Infrastructure                                 |
| **Strategy**                 | Each booking rule is an `IBookingRule`; `BookingValidator` runs all registered rules                  |
| Template method              | `SyncBookingRule` – rules without database access implement one synchronous method                    |
| Options pattern              | `BookingOptions`, `ReportOptions` bound from `appsettings.json`, validated on start                   |
| Result pattern               | Services return `Result<T>` – success or a list of errors – instead of throwing for expected failures |
| Rich domain model            | `Reservation.Cancel(now)` / `EndEarly(now)` guard their own state transitions                         |
| DTO + custom mapper          | Entities never leave the Application layer; extension-method mappings                                 |
| Unit of work / repository    | Provided by EF Core's `DbContext`, behind the `IAppDbContext` abstraction                             |

---

## Booking rules

Every new reservation is validated in the backend. **All** violated rules are returned together, so the user can fix everything at once.

| #   | Rule                                                                               | Error code                            |
| --- | ---------------------------------------------------------------------------------- | ------------------------------------- |
| R1  | A reservation cannot last longer than **7 days** (exactly 7 is allowed)            | `DURATION_TOO_LONG`                   |
| R2  | A room cannot have two overlapping active reservations; cancelled ones don't block | `ROOM_OVERLAP`                        |
| R3  | An organizer cannot have two overlapping meetings, even in different rooms         | `ORGANIZER_OVERLAP`                   |
| R4  | Start must be in the future (and the end after the start)                          | `START_IN_PAST`, `INVALID_TIME_RANGE` |
| R5  | Attendees must not exceed the room's capacity                                      | `CAPACITY_EXCEEDED`                   |
| R6  | A reservation cannot start more than **90 days** ahead                             | `BEYOND_BOOKING_HORIZON`              |
| R7  | Inactive rooms cannot be booked                                                    | `ROOM_INACTIVE`                       |

Both limits are [configurable](#configuration).

**How it's built:** each rule is a small class implementing `IBookingRule` and returning either `null` (passed) or a `BookingError`. They are registered in DI, and `BookingValidator` receives them all as `IEnumerable<IBookingRule>`. Adding a rule means adding one class and one registration line – no existing code changes.

**Overlap** uses half-open intervals `[start, end)`: two bookings overlap only if each starts before the other ends, so **back-to-back meetings (10:00–11:00 and 11:00–12:00) are allowed**. Only reservations with status _Booked_ block a slot – cancelled meetings and meetings that were ended early free the room. The validator accepts an "exclude this reservation" id, so the same rules would also cover editing an existing reservation.

### Reservation lifecycle

| Status        | Meaning                                                                                |
| ------------- | -------------------------------------------------------------------------------------- |
| **Booked**    | Created and not yet over                                                               |
| **Completed** | Ended early, _or_ its end time has passed (derived on read – no background job needed) |
| **Cancelled** | Cancelled before it started                                                            |

| Action    | Allowed when                       | Otherwise                                    |
| --------- | ---------------------------------- | -------------------------------------------- |
| Cancel    | The meeting **hasn't started** yet | `409 ALREADY_STARTED` – end it early instead |
| End early | The meeting is **in progress**     | `409 NOT_STARTED` – cancel it instead        |

Ending early records the actual end time, which is what the usage report is built on.

---

## Room usage report

Answers the assignment's question: _which rooms are often booked but rarely used?_

| Term             | Definition                                                                                                                                         |
| ---------------- | -------------------------------------------------------------------------------------------------------------------------------------------------- |
| **Booked hours** | Scheduled time of meetings that weren't cancelled, clipped to the report period                                                                    |
| **Used hours**   | The same, but ending at the moment the meeting was ended early, if it was                                                                          |
| **Utilization**  | Used ÷ booked (empty when nothing was booked)                                                                                                      |
| **Rarely used**  | Utilization below **60%** with at least **3 meetings** in the period (both configurable) – the minimum stops a single meeting from flagging a room |

Rules behind the numbers:

- **Only elapsed time counts.** Next week's bookings aren't "unused" yet, so the period is cut off at the current moment.
- **Cancelled meetings are counted separately.** They released the room, so they are neither booked nor used time.
- The calculation is a **pure function** (`RoomUsageCalculator`) – no database, no clock – which makes it easy to read and to unit test.

With the seed data, **Baltic** is flagged: booked for 10.5 h over the last 30 days but used for only about 4 h, because three of its meetings were ended early.

**PDF export:** _Export PDF_ opens the browser's print dialog on a print-optimised version of the page (no navigation or filters, a report header with the period, A4 landscape, colours preserved, rows never split across pages). Choosing _Save as PDF_ produces the file, named after the report period.

---

## API

Interactive documentation is available at **http://localhost:5080/scalar** while the API runs (OpenAPI document at `/openapi/v1.json`).

| Method | Endpoint                                                   | Description                                                       |
| ------ | ---------------------------------------------------------- | ----------------------------------------------------------------- |
| `GET`  | `/api/rooms?minCapacity=&floor=&equipment=`                | Rooms; `equipment` can be repeated                                |
| `GET`  | `/api/users`                                               | Users with active reservation counts                              |
| `GET`  | `/api/reservations?roomId=&organizerId=&status=&from=&to=` | Reservations; `from`/`to` return everything overlapping the range |
| `POST` | `/api/reservations`                                        | Create (validated against R1–R7)                                  |
| `POST` | `/api/reservations/{id}/cancel`                            | Cancel an upcoming reservation                                    |
| `POST` | `/api/reservations/{id}/end-early`                         | End an in-progress meeting now                                    |
| `GET`  | `/api/reports/room-usage?from=&to=`                        | Usage report; defaults to the last 30 days                        |

Enums are serialized as strings (`"Booked"`, `"Projector"`), times as ISO 8601 in UTC.

### Errors

Every error – a broken booking rule, invalid input, an unknown id – uses the same [RFC 9457 Problem Details](https://www.rfc-editor.org/rfc/rfc9457) shape with a list of codes and messages, so the frontend has a single error path. Unexpected server errors also return Problem Details (status 500) instead of an HTML error page.

Example – booking a slot that is already taken:

```json
{
  "status": 409,
  "title": "Conflict",
  "detail": "2 problems were found.",
  "errors": [
    { "code": "ROOM_OVERLAP", "message": "Room 'Baltic' is already booked for 'Sprint planning' at that time." },
    { "code": "ORGANIZER_OVERLAP", "message": "The organizer already has an overlapping meeting: 'Sprint planning'." }
  ]
}
```

| Status | When                                                                            |
| ------ | ------------------------------------------------------------------------------- |
| `201`  | Reservation created                                                             |
| `400`  | Invalid input (`INVALID_INPUT`, `INVALID_PERIOD`) or a booking rule violation   |
| `404`  | Unknown room, organizer or reservation                                          |
| `409`  | Overlap with an existing booking, or an action not allowed in the current state |

---

## Configuration

`backend/src/RoomReserve.Api/appsettings.json`:

```json
"Booking": {
  "MaxDurationDays": 7,
  "BookingHorizonDays": 90
},
"Reports": {
  "LowUtilizationThreshold": 0.6,
  "MinMeetingsToFlag": 3,
  "DefaultPeriodDays": 30
}
```

Values are bound with the Options pattern and **validated at startup** – an invalid value (e.g. a negative limit) stops the app with a clear message instead of misbehaving later. Like any ASP.NET Core setting, they can also be overridden with environment variables, e.g. `Booking__MaxDurationDays=14`.

---

## Testing

```bash
npm test          # or: dotnet test backend
```

34 xUnit tests cover:

- **Each booking rule**, including the boundaries the spec calls out – exactly 7 days, exactly 90 days ahead, attendees equal to capacity, back-to-back bookings, cancelled and ended-early reservations not blocking, and an edited reservation not conflicting with itself.
- **The validator** reporting several violations at once.
- **The reservation lifecycle** – create, reject with rule errors (nothing saved), cancel, end early, and status becoming _Completed_ once the end time passes.
- **The usage calculation** – full use, early endings, cancellations, clipping to the period, the underuse flag and its minimum meeting count, and future bookings not counting as unused.

Two things make the tests deterministic:

- **Time is injected.** Services take a `TimeProvider` instead of calling `DateTime.UtcNow`; tests use `FakeTimeProvider` and move time forward (`clock.Advance(...)`) to put a meeting in progress or in the past.
- **Each test gets its own in-memory database** (a random name per test class), so tests never see each other's data.

---

## Decisions and assumptions

The assignment leaves some points open; these are the interpretations taken.

**Scope**

- Section 1 of the assignment is titled "Users page" but describes rooms – treated as the **Rooms page**.
- The intro asks to show "how rooms are actually used", without a specific requirement – implemented as the **usage report**, built on the _end meeting early_ action.
- No authentication (as specified): the organizer is chosen from the list of users when booking.
- Editing reservations isn't among the listed actions, so there is no edit endpoint; the rules engine already supports it via the excluded-reservation id.

**Time**

- All times are stored and exchanged in **UTC**; the browser shows local time.
- Overlap uses half-open intervals, so back-to-back bookings are allowed.
- R4 is applied strictly (start must be after _now_), and additionally the end must be after the start.

**Statuses**

- _Completed_ is **derived** for booked meetings whose end time has passed, so data never goes stale and no scheduled job is needed.
- An **active** reservation is one that is _Booked_ and hasn't ended yet – used for the Users page count.
- **Cancel** is only possible before a meeting starts; a meeting in progress should be **ended early** instead, so the usage report keeps the fact that the room was used.

**User interface**

- Inactive rooms are left out of the booking dialog's room list for convenience, but R7 is still enforced by the API.
- PDF export uses the browser's print engine rather than a server-side PDF library – no extra dependencies, and the PDF matches exactly what's on screen. A generator such as QuestPDF would be the next step if reports had to be produced without a browser (e.g. emailed on a schedule).

**Technical choices**

- **`IAppDbContext` instead of repositories.** EF Core's `DbContext` already implements the repository and unit-of-work patterns; another layer of wrappers would be boilerplate at this size. With multiple data sources, repositories would be introduced.
- **Custom mapping over a mapper library.** A handful of explicit `ToDto()` extension methods are easy to read and debug, and errors show up at compile time rather than at runtime.
- **Vite + React instead of Next.js.** The backend is a separate API, so a client-side SPA is the simpler fit.
- **shadcn/ui** components are copied into the project (`src/components/ui`) rather than installed as a package, so they can be adjusted freely; app-specific styling is kept outside those files.

---

## Known limitations and next steps

- **Concurrent double booking.** The in-memory provider has no real transactions, so two simultaneous requests for the same slot could both pass validation. With a relational database: a serializable transaction or a concurrency token on the room.
- **Calendar / timeline view** (optional, not implemented). Planned approach: a day/week grid per room using the existing reservations endpoint. Because R2 guarantees bookings in one room never overlap, each block is positioned by start time and duration alone – no overlap layout algorithm needed. Multi-day bookings would be split into one segment per day.
- **Pagination** for the reservations list as data grows.
- **Generated API types** – derive the frontend's TypeScript types from the OpenAPI document so backend changes surface as compile errors.
- **More tests** – API integration tests with `WebApplicationFactory`, and frontend tests (Vitest) for the date helpers.
- **Persistent database** (SQL Server or PostgreSQL) – only the `AddInfrastructure()` registration would change.
- **Authentication**, so the organizer is the signed-in user.

---

## Project structure

```
RoomReserve/
├── package.json                  # root scripts: dev, test, build
├── global.json                   # pins the .NET SDK (10.0.x)
├── backend/
│   ├── RoomReserve.slnx
│   ├── src/
│   │   ├── RoomReserve.Domain/           # Entities/, Enums/, Errors/
│   │   ├── RoomReserve.Application/      # Rooms/, Users/, Reservations/ (+ Booking/Rules/), Reports/, Common/
│   │   ├── RoomReserve.Infrastructure/   # Persistence/ (AppDbContext, DbSeeder)
│   │   └── RoomReserve.Api/              # Controllers/, Errors/, Program.cs, appsettings.json
│   └── tests/
│       └── RoomReserve.Tests/            # Booking/, Reservations/, Reports/
└── frontend/
    └── src/
        ├── api/                          # fetch wrapper, DTO types
        ├── components/                   # shared components + ui/ (shadcn)
        ├── features/                     # rooms/, users/, reservations/, usage/
        └── lib/                          # dates, formatting, labels, print
```
