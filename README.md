# 🎧 Calico Backend

![.NET 8](https://img.shields.io/badge/.NET-8.0-512BD4?style=for-the-badge&logo=dotnet&logoColor=white)
![C#](https://img.shields.io/badge/Language-C%23-239120?style=for-the-badge&logo=csharp&logoColor=white)
![EF Core](https://img.shields.io/badge/ORM-EF_Core-512BD4?style=for-the-badge&logo=efcore&logoColor=white)
![Database](https://img.shields.io/badge/Database-SQLite_/_SQL_Server-336791?style=for-the-badge&logo=sqlite&logoColor=white)
![Swagger](https://img.shields.io/badge/Docs-Swagger-85EA2D?style=for-the-badge&logo=swagger&logoColor=black)
![HealthChecks](https://img.shields.io/badge/Monitoring-HealthChecks-FF6F00?style=for-the-badge&logo=heartbeat&logoColor=white)
![Testing](https://img.shields.io/badge/Testing-NUnit_/_Moq_/_Shouldly-512BD4?style=for-the-badge&logo=testinglibrary&logoColor=white)
![Design](https://img.shields.io/badge/Design-UML_/_Figma-000000?style=for-the-badge&logo=figma&logoColor=white)
![CI](https://github.com/Nazmul5765/calico-backend/actions/workflows/ci.yml/badge.svg)

**🔗 Live API:** [calico-backend-production.up.railway.app/health](https://calico-backend-production.up.railway.app/health) — powers the frontend at [calico.nazmulhussain.co.uk](https://calico.nazmulhussain.co.uk)

Calico Backend is a production-style ASP.NET Core Web API built as part of a collaborative full-stack group project. It powers the Calico LoFi platform by providing robust media streaming, productivity tracking, and secure identity management through a highly maintainable, testable, and decoupled layered architecture.

This repository is my personal fork of the original group project, which I've since taken through a full security review, fixed and finished independently, and deployed — see [My Contribution](#-my-contribution) below for what that involved.

---

# 🚀 Overview
The backend is designed for real‑world maintainability and scalability, featuring:

* Layered architecture (Controllers → Services → Repositories → EF Core → Database)

* Dual‑database strategy (SQLite for development, SQL Server for production)

* Automatic schema creation/migration

* Health checks for API and database

* Swagger/OpenAPI documentation

* Fully tested business logic, controllers, and data access

# 🧰 Tech Stack

  | Area | Technologies |
| --- | --- |
| **Framework** | ASP.NET Core Web API (.NET 8) |
| **Language** | C# |
| **Authentication** | Supabase JWT (cookie‑based token extraction) |
| **ORM** | Entity Framework Core |
| **Databases** | SQLite (Dev), SQL Server (Prod) |
| **Testing** | NUnit, Moq, Shouldly |
| **Tools** | Swagger, HealthChecks, HttpClient |


# 🎯 Core Features

## User Management
* User CRUD

* Authentication integration

* Profile operations

## Music & Media
* Music endpoints

* YouTube integration via repository/service layer

## Playlists & Projects
* Playlist CRUD

* Project CRUD

## Task Timer
* Timer CRUD

* Productivity tracking

## System Features
* Health checks (/health)

* Swagger API documentation

# 🧑‍💻 My Contribution

The original group project got the API this far as a Northcoders bootcamp project. Since forking it for my own portfolio, I ran a full security review, fixed everything it found, restored a feature the group had to fake due to API quota limits, and deployed it independently. Everything below is my own work, done solo, on top of the original.

### Security fixes
* **Authentication wasn't actually enforced anywhere.** No controller had `[Authorize]`, and the JWT signature verification was built against an outdated Supabase token format that no longer worked. I rewrote the verification to fetch Supabase's public keys directly and added proper authorization checks across every controller.
* **Any logged-in user could edit anyone else's profile and self-promote to admin.** The edit-profile endpoint accepted a full user object from the request, including an `IsAdmin` flag, with no check that you were editing your own account. I added ownership checks and made sure `IsAdmin` is always read from the database, never trusted from the request.
* **The same self-promotion hole existed on account creation.** I closed it the same way, plus added proper role-based access control (admin-only endpoints) as a genuinely new feature — not something the original project had.
* **Projects and task timers had no ownership checks at all** — any authenticated user could view, edit, or delete any other user's projects and timers just by knowing (or guessing) an ID. I added ownership checks across every action on both.
* **Auth cookies had no security flags set**, meaning the login token was readable by JavaScript and had no same-site protection. Fixed with proper `HttpOnly`/`Secure`/`SameSite` settings.
* **A startup log was printing the full database connection string, password included,** to the console on every deploy. Removed it.

### Restored & fixed
* **Real YouTube search was replaced with a large hardcoded mock list** in the original project, due to running into API quota limits. I restored the real Google API call and added a caching layer (15-minute in-memory cache per search term) so repeat searches don't burn through quota unnecessarily.
* **Fixed a backwards bug in the user-update logic** that made editing an already-existing user's profile fail every time — the check meant to prevent duplicate accounts had been copy-pasted in the wrong place.
* **Fixed a bug in the Azure SQL connection** where the app would crash outright the first time it reconnected after the free-tier database auto-paused from inactivity, rather than retrying.
* Removed dead code found during the review: an abandoned, never-referenced controller, unused imports across ~20 files, and a duplicate constructor that silently did nothing.

### Testing & deployment
* Added test coverage for the areas the security fixes touched, plus a full suite for the authentication controller, which previously had none — **95 tests passing**.
* Set up a GitHub Actions CI pipeline that builds and runs the full test suite on every push.
* Containerised the API with Docker and deployed it to Railway, backed by its own Azure SQL Database.

# 🏗️ Architecture
Calico Backend follows a clean, decoupled layered architecture:

Controllers
    ↓
Services (Business Logic)
    ↓
Repositories (Data Access)
    ↓
Entity Framework Core
    ↓
Database (SQLite / SQL Server)

# 🧪 Testing

The backend includes automated test coverage across all major layers.

## Backend Tests
* Repository testing

* Service testing

* Controller testing

## Testing Tools
* NUnit — test framework

* Moq — mocking dependencies

* Shouldly — readable assertions

* In‑memory SQLite — integration‑style repository tests

## Database Configuration
* Development: SQLite (auto‑created with EnsureCreated())

* Production: SQL Server (migrations applied via Migrate())

## Health Checks
* ApiHealthCheck

* DatabaseHealthCheck
