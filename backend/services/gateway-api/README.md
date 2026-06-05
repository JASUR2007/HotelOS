# gateway-api

## Folder pattern

```text
Controllers/
Services/
Repositories/
Models/
DTOs/
Events/
Validators/
Middleware/
Data/
Migrations/
Program.cs
appsettings.json
Dockerfile
```

## Purpose

- Public API entry point
- Forwards or aggregates traffic for the hotel operations dashboard
- Exposes auth and gateway endpoints for the frontend

## Example endpoints

- `GET /api/dashboard/metrics`
- `GET /api/rooms/overview`
- `POST /api/auth/login`
