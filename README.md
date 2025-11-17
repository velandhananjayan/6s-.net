# Application Tracking System

Modern applicant tracking API built with ASP.NET Core. Technical roles advance automatically through a bot-driven workflow, while non-technical roles remain under explicit admin control.

## 1. Capabilities

- **Secure roles**: Applicant (self-service CRUD), Bot Mimic (automated progression), Admin (job roles and non-technical oversight).
- **Application lifecycle**: submission, detail view, history, notes, and activity logging.
- **Automation**: background service runs every five minutes and can be triggered manually.
- **Dashboards**: role-scoped metrics, status breakdowns, and recent events ready for any UI.

## 2. Architecture Overview

| Layer | Purpose |
| --- | --- |
| Controllers | REST endpoints (auth, applications, admin, bot mimic, dashboard) |
| Services | Business logic, automation rules, role checks |
| Data | EF Core context + PostgreSQL schema (SQL Server optional) |
| Security | JWT bearer auth, role policies, BCrypt passwords |

Directory layout:
```
ApplicationTrackingSystem.API/
├── Controllers/
├── Models/
│   └── DTOs/
├── Services/
├── Data/
└── Program.cs
```

## 3. Technology Stack

| Area | Choice |
| --- | --- |
| Framework | .NET 9 / ASP.NET Core Web API |
| ORM | Entity Framework Core |
| Database | PostgreSQL by default, SQL Server supported |
| Auth | JWT bearer tokens |
| Documentation | Swagger UI |

## 4. Setup

1. Install prerequisites: .NET 9 SDK, PostgreSQL 12+ (or SQL Server), preferred IDE.
2. Clone repository:
   ```bash
   git clone <repository-url>
   cd ApplicationTrackingSystem
   ```
3. Configure database:
   - PostgreSQL: create database `ApplicationTrackingSystem`, then set `ConnectionStrings:DefaultConnection` in `appsettings.json`.
   - SQL Server: update connection string and swap `UseNpgsql` for `UseSqlServer` in `Program.cs`.
4. Restore and build:
   ```bash
   dotnet restore
   dotnet build
   ```

## 5. Running Locally

```bash
cd ApplicationTrackingSystem.API
dotnet run
```

- Swagger UI: `https://localhost:5001` (or `http://localhost:5000`)
- Base API path: `/api`
- Open Swagger, click **Authorize**, and provide `Bearer <token>` from login.

## 6. Seed Accounts

| Role | Username | Password |
| --- | --- | --- |
| Applicant | applicant | applicant123 |
| Bot Mimic | botmimic | botmimic123 |
| Admin | admin | admin123 |

## 7. Key Endpoints

| Area | Endpoint |
| --- | --- |
| Authentication | `POST /api/auth/login` |
| Applicant applications | `POST /api/applications`, `GET /api/applications`, `GET /api/applications/{id}`, `GET /api/applications/{id}/history` |
| Bot Mimic automation | `POST /api/botmimic/process-all`, `POST /api/botmimic/process/{applicationId}` |
| Admin tools | `POST /api/admin/job-roles`, `GET /api/admin/job-roles`, `GET /api/admin/applications`, `PUT /api/admin/applications/{id}/status` |
| Dashboards | `GET /api/dashboard` |

## 8. Typical Workflows

| Applicant | Bot Mimic | Admin |
| --- | --- | --- |
| Login → submit application → monitor dashboard | Login → run `process-all` or per-app processing → review dashboard | Login → create/edit job roles → review non-technical applications → update status/comments |

Statuses follow `Applied → Reviewed → Interview → Offer/Rejected`. Admins can override non-technical flows; Bot Mimic advances technical ones.

## 9. Data Model

- Users: identity, credentials, role, timestamps.
- JobRoles: metadata, technical flag, author, activity state.
- Applications: applicant link, role link, status, notes, timestamps.
- ActivityLogs: action, status transition, comment, performing role/user, timestamp.

## 10. Security Notes

- JWT tokens expire after 24 hours; include `Bearer` prefix.
- Passwords stored via BCrypt.
- Policy-based authorization ensures users are limited to their role scope.

## 11. Troubleshooting

- **Database**: confirm server is running, credentials are valid, and connection string matches.
- **Authentication**: verify JWT key length (>=32 chars) and unexpired tokens.
- **Swagger/UI**: run in Development mode and check ports 5000/5001 (or 5059) are free.

## 12. Development Tips

- Default setup uses `EnsureCreated()`. For migrations:
  ```bash
  dotnet ef migrations add InitialCreate
  dotnet ef database update
  ```
- To extend functionality: add models, services, and controllers, then register services in `Program.cs`.

## 13. License & Support

This project is provided for assignment purposes. Consult course documentation for questions or escalation paths.




