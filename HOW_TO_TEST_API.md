# How to Test the API - Complete Guide

## Quick Answer

**The base URL `http://localhost:5059/api` is NOT a webpage** - it's just a prefix for all API endpoints. You need to access specific endpoints.

---

## Method 1: Swagger UI (RECOMMENDED - Easiest)

### Step 1: Make sure the application is running
```bash
cd ApplicationTrackingSystem.API
dotnet run
```

### Step 2: Open Swagger UI in browser
**URL**: `http://localhost:5059`

- Swagger UI will load automatically
- You'll see all available endpoints
- You can test them directly from the browser!

### Step 3: Test Login
1. Find `POST /api/auth/login` in Swagger
2. Click "Try it out"
3. Enter credentials:
   ```json
   {
     "username": "applicant",
     "password": "applicant123"
   }
   ```
4. Click "Execute"
5. Copy the `token` from response

### Step 4: Authorize
1. Click "Authorize" button (top right, lock icon)
2. Enter: `Bearer <paste-your-token-here>`
3. Click "Authorize", then "Close"
4. Now you can test all protected endpoints!

---

## Method 2: Test Specific Endpoints in Browser

### Test 1: Login Endpoint (POST - Use Postman/curl)
**URL**: `http://localhost:5059/api/auth/login`

**Note**: Browser can't do POST requests easily. Use:
- **Postman** (recommended)
- **curl** (command line)
- **Swagger UI** (easiest)

**Using curl:**
```bash
curl -X POST http://localhost:5059/api/auth/login `
  -H "Content-Type: application/json" `
  -d '{\"username\":\"applicant\",\"password\":\"applicant123\"}'
```

### Test 2: Swagger JSON (GET - Works in browser)
**URL**: `http://localhost:5059/swagger/v1/swagger.json`

- This will show the API documentation in JSON format
- If you see JSON, the API is working!

### Test 3: Swagger UI (GET - Works in browser)
**URL**: `http://localhost:5059/swagger/index.html`

- This should show the Swagger UI interface
- If you see the Swagger page, the API is running!

---

## Method 3: Using PowerShell/Command Line

### Check if API is running:
```powershell
# Test Swagger endpoint
Invoke-WebRequest -Uri "http://localhost:5059/swagger/index.html" -UseBasicParsing
```

### Test Login:
```powershell
$body = @{
    username = "applicant"
    password = "applicant123"
} | ConvertTo-Json

$response = Invoke-RestMethod -Uri "http://localhost:5059/api/auth/login" `
    -Method Post `
    -ContentType "application/json" `
    -Body $body

Write-Host "Token: $($response.token)"
```

### Test Dashboard (after getting token):
```powershell
$token = "your-token-here"
$headers = @{
    Authorization = "Bearer $token"
}

$dashboard = Invoke-RestMethod -Uri "http://localhost:5059/api/dashboard" `
    -Method Get `
    -Headers $headers

$dashboard | ConvertTo-Json
```

---

## Method 4: Using Postman

1. **Download Postman** (if not installed)
2. **Create new request**: `POST http://localhost:5059/api/auth/login`
3. **Set Headers**: `Content-Type: application/json`
4. **Set Body** (raw JSON):
   ```json
   {
     "username": "applicant",
     "password": "applicant123"
   }
   ```
5. **Send** - You'll get a token
6. **Create new request**: `GET http://localhost:5059/api/dashboard`
7. **Set Headers**: `Authorization: Bearer <your-token>`
8. **Send** - You'll get dashboard data

---

## How to Verify API is Working

### Quick Check (Browser):
1. Open: `http://localhost:5059/swagger/index.html`
2. **If you see Swagger UI** → API is working!
3. **If you see error/blank** → API is not running

### Detailed Check (PowerShell):
```powershell
# Check if server is listening
netstat -ano | findstr ":5059"

# Test Swagger
try {
    $response = Invoke-WebRequest -Uri "http://localhost:5059/swagger/index.html" -UseBasicParsing
    Write-Host "API is RUNNING! Status: $($response.StatusCode)" -ForegroundColor Green
} catch {
    Write-Host "API is NOT running. Error: $($_.Exception.Message)" -ForegroundColor Red
    Write-Host "Start it with: cd ApplicationTrackingSystem.API; dotnet run" -ForegroundColor Yellow
}
```

---

## Complete Testing Workflow

### Step-by-Step:

1. **Start the API**
   ```bash
   cd ApplicationTrackingSystem.API
   dotnet run
   ```
   Wait for: `Now listening on: http://localhost:5059`

2. **Open Swagger UI**
   - Browser: `http://localhost:5059`
   - Should see all endpoints listed

3. **Test Login**
   - Click `POST /api/auth/login`
   - Click "Try it out"
   - Enter credentials
   - Click "Execute"
   - Copy the token

4. **Authorize**
   - Click "Authorize" button
   - Enter: `Bearer <token>`
   - Click "Authorize"

5. **Test Other Endpoints**
   - Try `GET /api/dashboard`
   - Try `GET /api/applications`
   - All should work now!

---

## Summary

| What to Check | URL | What You Should See |
|---------------|-----|---------------------|
| **Swagger UI** | `http://localhost:5059` | Swagger interface with all endpoints |
| **Swagger JSON** | `http://localhost:5059/swagger/v1/swagger.json` | JSON documentation |
| **API Base** | `http://localhost:5059/api` | No content (just a prefix) |
| **Login** | `http://localhost:5059/api/auth/login` | Need POST request (use Swagger/Postman) |
| **Dashboard** | `http://localhost:5059/api/dashboard` | Need auth token (use Swagger) |

---

## Pro Tips

1. **Best Method**: Use Swagger UI at `http://localhost:5059`
   - No tools needed
   - Visual interface
   - Built-in authentication
   - Can test all endpoints

2. **For Development**: Use Postman
   - Save requests
   - Create collections
   - Environment variables

3. **For Automation**: Use curl/PowerShell
   - Scriptable
   - CI/CD integration

---

## Troubleshooting

### Problem: "Cannot connect" or "Connection refused"
**Solution**: API is not running. Start it with `dotnet run`

### Problem: Swagger shows but endpoints return 401
**Solution**: You need to login first and authorize with the token

### Problem: Swagger doesn't load
**Solution**: 
- Check if running in Development mode
- Verify port 5059 is not blocked
- Check firewall settings

---

**Remember**: The base URL `/api` is just a prefix. Always access specific endpoints like `/api/auth/login` or use Swagger UI!

