# Database Setup Instructions

## Option 1: PostgreSQL (Current Default)

PostgreSQL service is running. You need to:

1. **Find your PostgreSQL password** - It was set during installation. Common defaults:
   - `postgres`
   - The password you entered during installation

2. **Update the connection string** in `appsettings.json`:
   ```json
   "ConnectionStrings": {
     "DefaultConnection": "Host=localhost;Database=ApplicationTrackingSystem;Username=postgres;Password=YOUR_PASSWORD;Port=5432"
   }
   ```

3. **The database will be created automatically** when you run the application (if it doesn't exist).

### If you need to create the database manually:

Find PostgreSQL installation path (usually `C:\Program Files\PostgreSQL\18\bin\`) and run:
```powershell
& "C:\Program Files\PostgreSQL\18\bin\psql.exe" -U postgres -c "CREATE DATABASE ApplicationTrackingSystem;"
```

Or use pgAdmin (GUI tool that comes with PostgreSQL).

---

## Option 2: SQL Server (Easier on Windows)

If you have SQL Server installed (or SQL Server Express):

1. **Update `appsettings.json`**:
   ```json
   "Database": {
     "UseSqlServer": true
   },
   "ConnectionStrings": {
     "DefaultConnection": "Server=localhost;Database=ApplicationTrackingSystem;Trusted_Connection=True;TrustServerCertificate=True;"
   }
   ```

2. **The database will be created automatically** when you run the application.

### SQL Server Express (Free):
- Download from: https://www.microsoft.com/en-us/sql-server/sql-server-downloads
- Choose "Express" edition (free)
- During installation, choose "Mixed Mode Authentication" or "Windows Authentication"

---

## Quick Test

Try running the application:
```bash
cd ApplicationTrackingSystem.API
dotnet run
```

If you get a connection error, check:
1. **PostgreSQL**: Is the service running? (We confirmed it is)
2. **PostgreSQL**: Is the password correct in appsettings.json?
3. **SQL Server**: Is SQL Server running? Check Services

---

## Recommended: Use SQL Server for Easier Setup

Since you're on Windows, SQL Server might be easier. Just:
1. Install SQL Server Express (if not installed)
2. Set `"UseSqlServer": true` in appsettings.json
3. Run the application

The database will be created automatically!

