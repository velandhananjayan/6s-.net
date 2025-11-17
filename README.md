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

## 3. Architecture Diagram

<img width="749" height="819" alt="image" src="https://github.com/user-attachments/assets/4925f441-e0a1-402e-84e3-2e9c8bf55438" />


## 4. Technology Stack

| Area | Choice |
| --- | --- |
| Framework | .NET 9 / ASP.NET Core Web API |
| ORM | Entity Framework Core |
| Database | PostgreSQL by default, SQL Server supported |
| Auth | JWT bearer tokens |
| Documentation | Swagger UI |

## 5. Setup

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

## 6. Running Locally

```bash
cd ApplicationTrackingSystem.API
dotnet run
```

- Swagger UI: `https://localhost:5001` (or `http://localhost:5000`)
- Base API path: `/api`
- Open Swagger, click **Authorize**, and provide `Bearer <token>` from login.

## 7. Seed Accounts

| Role | Username | Password |
| --- | --- | --- |
| Applicant | applicant | applicant123 |
| Bot Mimic | botmimic | botmimic123 |
| Admin | admin | admin123 |

## 8. Key Endpoints

| Area | Endpoint |
| --- | --- |
| Authentication | `POST /api/auth/login` |
| Applicant applications | `POST /api/applications`, `GET /api/applications`, `GET /api/applications/{id}`, `GET /api/applications/{id}/history` |
| Bot Mimic automation | `POST /api/botmimic/process-all`, `POST /api/botmimic/process/{applicationId}` |
| Admin tools | `POST /api/admin/job-roles`, `GET /api/admin/job-roles`, `GET /api/admin/applications`, `PUT /api/admin/applications/{id}/status` |
| Dashboards | `GET /api/dashboard` |

## 9. Typical Workflows

| Applicant | Bot Mimic | Admin |
| --- | --- | --- |
| Login → submit application → monitor dashboard | Login → run `process-all` or per-app processing → review dashboard | Login → create/edit job roles → review non-technical applications → update status/comments |

Statuses follow `Applied → Reviewed → Interview → Offer/Rejected`. Admins can override non-technical flows; Bot Mimic advances technical ones.

## 10. Data Model

- Users: identity, credentials, role, timestamps.
- JobRoles: metadata, technical flag, author, activity state.
- Applications: applicant link, role link, status, notes, timestamps.
- ActivityLogs: action, status transition, comment, performing role/user, timestamp.

## 11. Security Notes

- JWT tokens expire after 24 hours; include `Bearer` prefix.
- Passwords stored via BCrypt.
- Policy-based authorization ensures users are limited to their role scope.

## 12. Troubleshooting

- **Database**: confirm server is running, credentials are valid, and connection string matches.
- **Authentication**: verify JWT key length (>=32 chars) and unexpired tokens.
- **Swagger/UI**: run in Development mode and check ports 5000/5001 (or 5059) are free.

## 13. Development Tips

- Default setup uses `EnsureCreated()`. For migrations:
  ```bash
  dotnet ef migrations add InitialCreate
  dotnet ef database update
  ```
- To extend functionality: add models, services, and controllers, then register services in `Program.cs`.

## 14. OUTPUT 

<img width="1919" height="956" alt="image" src="https://github.com/user-attachments/assets/f13de12c-bef5-443a-bee0-588d021b2fd0" />

<img width="1902" height="541" alt="image" src="https://github.com/user-attachments/assets/a4029739-dcfb-4a25-b70a-980e3be08b3e" />

<img width="1900" height="961" alt="image" src="https://github.com/user-attachments/assets/ea9ac6f9-4273-451b-9958-c08bfa9002a5" />







