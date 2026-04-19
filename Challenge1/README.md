# Challenge Weather API

A production-ready Weather microservice built with .NET 10 / ASP.NET Core, following Clean Architecture. Provides real-time weather data, 4-day forecasts, historical records, and threshold-based email alerts for 42 Singapore locations — all powered by [data.gov.sg](https://data.gov.sg) and [OpenWeatherMap](https://openweathermap.org/api).

---

## Table of Contents

- [Architecture](#architecture)
- [Features](#features)
- [Prerequisites](#prerequisites)
- [Getting Started](#getting-started)
- [Authentication](#authentication)
- [API Reference](#api-reference)
- [Configuration](#configuration)
- [Running Tests](#running-tests)
- [Docker](#docker)
- [CI/CD](#cicd)
- [Project Structure](#project-structure)

---

## Architecture

```
Challenge.Application.API          ← ASP.NET Core Web API (controllers, middleware, seeders)
Challenge.Application.Bll          ← Business Logic Layer
Challenge.Application.DTO          ← Request/response DTOs
Challenge.Application.Filter       ← Query filter models
Challenge.Common.Extension         ← DI registration extensions
Challenge.Common.Logging           ← Logging abstraction
Challenge.Common.Mapper            ← AutoMapper profile
Challenge.Common.Util              ← HTTP clients, secrets, middleware, helpers
Challenge.Common.EmailService      ← SMTP email service with background queue
Challenge.Database.Models          ← EF Core entity models
Challenge.Database.Repositories    ← Repositories and DbContext
Challenge.UnitTest                 ← NUnit tests (26 tests, SQLite in-memory)
```

**Request flow:** `Controller → BLL → Repository → DbContext`

---

## Features

- **Real-time weather** — fetches current conditions from data.gov.sg 2-hour forecast + OpenWeatherMap, merges and persists
- **4-day forecasts** — sourced from data.gov.sg 4-day outlook
- **Historical records** — queryable time-series storage with CSV export
- **Location auto-create** — name-based endpoints automatically create locations on first call
- **42 seeded Singapore locations** — pre-loaded on first Development startup (matches data.gov.sg area names exactly)
- **Weather alerts** — subscribe by email, threshold types: `temperature_high`, `temperature_low`, `rainfall`, `aqi`
- **Background alert checker** — checks all active alert thresholds every 15 minutes, sends HTML email on breach
- **JWT authentication** — Bearer token required on all weather endpoints
- **Rate limiting** — 100 requests/minute per authenticated user or IP
- **Security headers** — CSP, X-Frame-Options, Referrer-Policy via NWebsec
- **SQLite dev mode** — zero-config local development, no external database needed
- **PostgreSQL production** — connection string via AWS Secrets Manager

---

## Prerequisites

| Tool | Version |
|------|---------|
| .NET SDK | 10.0+ |
| Docker (optional) | 24+ |

No database setup required for local development — SQLite is used automatically.

---

## Getting Started

### 1. Clone and run

```bash
git clone <repo-url>
cd Challenge.Template

# Set development environment
export ASPNETCORE_ENVIRONMENT=Development   # Linux/macOS
$env:ASPNETCORE_ENVIRONMENT="Development"  # PowerShell

cd Challenge.Application.API
dotnet run
```

On first startup, the API:
- Creates `dev.db` (SQLite) in the output directory
- Seeds 42 Singapore weather locations
- Seeds a default admin user

### 2. Open Swagger UI

```
http://localhost:3000/swagger
```

### 3. Login and get a token

```
POST http://localhost:3000/Common/Auth/Login
Content-Type: application/json

{
  "loginId": "admin",
  "password": "Admin@1234"
}
```

### 4. Try the recommended starting endpoint

In Swagger UI, click **Authorize** and enter `Bearer <token>`, then call:

```
GET /Weather/Weather/Summary?locationName=Tampines
```

This returns current conditions and a 4-day forecast in one response. The location is auto-created if it does not exist.

### 5. Add your OpenWeatherMap API key (optional)

For live temperature/wind/humidity data, add your key to `appsettings.json`:

```json
"WeatherApi": {
  "OpenWeatherMapApiKey": "your-key-here"
}
```

The API gracefully degrades — if no key is configured, data.gov.sg data is still returned for forecasts and PSI readings.

---

## Authentication

All weather endpoints require a valid JWT Bearer token.

### Login

```http
POST /Common/Auth/Login
```

**Request body:**
```json
{
  "loginId": "admin",
  "password": "Admin@1234"
}
```

**Response:**
```json
{
  "token": "eyJhbGci...",
  "loginId": "admin",
  "fullName": "Administrator",
  "expiresAt": "2026-04-19T08:00:00Z"
}
```

Tokens are valid for **8 hours**. Pass the token in the `Authorization` header:

```
Authorization: Bearer eyJhbGci...
```

---

## API Reference

### Auth

| Method | Endpoint | Description | Auth |
|--------|----------|-------------|------|
| POST | `/Common/Auth/Login` | Login and receive JWT token | Public |

### Weather

All endpoints require a valid JWT token.

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/Weather/Weather/Locations` | List all locations (filter by `region`, `locationName`, `isActive`) |
| POST | `/Weather/Weather/Locations/Create` | Create a new location |
| GET | `/Weather/Weather/Summary?locationName=` | **Recommended start** — current weather + 4-day forecast |
| GET | `/Weather/Weather/CurrentByName?locationName=` | Current weather by name (auto-creates location) |
| GET | `/Weather/Weather/Current?locationId=` | Current weather by Guid |
| GET | `/Weather/Weather/ForecastByName?locationName=&days=` | Forecast by name (1–7 days, default 4) |
| GET | `/Weather/Weather/Forecast?locationId=&days=` | Forecast by Guid |
| POST | `/Weather/Weather/Historical` | Historical records with date-range and location filter |
| POST | `/Weather/Weather/Export` | Export historical records as CSV download |

### Weather Alerts

All endpoints require a valid JWT token.

| Method | Endpoint | Description |
|--------|----------|-------------|
| POST | `/Weather/WeatherAlert/Subscribe` | Subscribe to a threshold alert |
| POST | `/Weather/WeatherAlert/Unsubscribe` | Unsubscribe from an alert |
| POST | `/Weather/WeatherAlert/GetAlerts` | List active/inactive alerts with filter |

**Alert types:** `temperature_high`, `temperature_low`, `rainfall`, `aqi`

**Subscribe example:**
```json
{
  "locationId": "<guid>",
  "subscriberEmail": "you@example.com",
  "subscriberName": "Your Name",
  "alertType": "temperature_high",
  "thresholdValue": 35.0
}
```

---

## Configuration

### appsettings.json

```json
{
  "WeatherApi": {
    "OpenWeatherMapApiKey": "",
    "AlertCheckIntervalMinutes": 15
  },
  "Jwt": {
    "Secret": "dev-jwt-secret-key-must-be-at-least-32-characters-long!",
    "Issuer": "api",
    "Audience": "clients"
  }
}
```

### Environment Variables

| Variable | Description |
|----------|-------------|
| `ASPNETCORE_ENVIRONMENT` | Set to `Development` for SQLite + Swagger; anything else uses PostgreSQL |
| `CERT_PASSWORD` | Password for `cert.pfx` (HTTPS certificate) |
| `ENVIRONMENT` | App environment label: `LOCAL`, `DEV`, `UAT`, `PRD` |

### Secrets (Production)

Production secrets are read from **AWS Secrets Manager**. Required keys:

| Key | Description |
|-----|-------------|
| `ConnectionStrings` | PostgreSQL connection string |
| `jwt_secret` | JWT signing key (minimum 32 characters) |
| `openweathermap_api_key` | OpenWeatherMap API key |

---

## Running Tests

```bash
dotnet test Challenge.UnitTest/Challenge.UnitTest.csproj
```

**26 tests** across 4 test files, all using SQLite in-memory — no external database or network required.

| Test File | Coverage |
|-----------|----------|
| `WeatherBllTests.cs` | Location CRUD, current weather, forecast merging, CSV export, summary endpoint |
| `WeatherAlertBllTests.cs` | Subscribe/unsubscribe, all alert types, invalid location handling |
| `WeatherLocationRepositoryTests.cs` | CRUD, name lookup, region filtering |
| `WeatherRecordRepositoryTests.cs` | Create, latest-by-location, date-range filtering |

External API calls (data.gov.sg, OpenWeatherMap) are mocked with Moq so tests run fully offline.

---

## Docker

### Build and run locally

```bash
# Generate a self-signed certificate
openssl req -x509 -newkey rsa:4096 -keyout key.pem -out cert.pem -days 365 -nodes -subj "/CN=localhost"
openssl pkcs12 -export -out Challenge.Application.API/cert.pfx -inkey key.pem -in cert.pem -passout pass:""

# Build and run
docker build -f Challenge.Application.API/Dockerfile -t challenge-weather-api .
docker run -e ASPNETCORE_ENVIRONMENT=Development -p 3000:3000 challenge-weather-api
```

### Ports

| Port | Protocol | Purpose |
|------|----------|---------|
| 3000 | HTTP | ALB → container routing |
| 3080 | HTTPS (TLS 1.3) | ALB → container routing |
| 80 / 443 | HTTP/HTTPS | Local Docker Desktop testing |

The container runs as a non-root user (`oshd_user`) for security.

---

## CI/CD

GitHub Actions workflow at [.github/workflows/ci.yml](.github/workflows/ci.yml) runs on every push and pull request to `main` or `develop`.

```
push / pull_request
        │
        ▼
┌─────────────────┐
│  build-and-test │  .NET 10 build + 26 unit tests + coverage report
└────────┬────────┘
         │
    ┌────┴─────┐
    ▼           ▼
┌──────────┐  ┌───────────────┐
│  docker  │  │ security-scan │
│  build   │  │               │
└──────────┘  └───────────────┘
```

| Job | Description |
|-----|-------------|
| `build-and-test` | Restore, build, run tests with Cobertura coverage, upload artifacts |
| `docker-build` | Multi-platform build, push to GHCR (`ghcr.io/<owner>/weather-api`) on `main` |
| `security-scan` | `dotnet list package --vulnerable --include-transitive` audit |

Docker images are tagged with commit SHA, branch name, and `latest` (on `main` only).

---

## Project Structure

```
Challenge.Template/
├── Challenge.Application.API/
│   ├── Controllers/
│   │   ├── Common/AuthController.cs
│   │   └── Weather/
│   │       ├── WeatherController.cs
│   │       └── WeatherAlertController.cs
│   ├── BackgroundServices/
│   │   └── WeatherAlertBackgroundService.cs
│   ├── DataSeeder/
│   │   ├── WeatherDataSeeder.cs        ← 42 Singapore locations
│   │   └── AuthDataSeeder.cs           ← default admin user
│   ├── Dockerfile
│   └── Program.cs
├── Challenge.Application.Bll/
│   ├── Auth/AuthBll.cs
│   └── Weather/
│       ├── WeatherBll.cs
│       └── WeatherAlertBll.cs
├── Challenge.Common.Util/
│   └── WeatherClients/
│       ├── DataGovSgClient.cs          ← 2hr, 24hr, 4-day, PSI
│       └── OpenWeatherMapClient.cs     ← current, 5-day
├── Challenge.Database.Models/
│   └── Weather/
│       ├── WeatherLocation.cs
│       ├── WeatherRecord.cs
│       ├── WeatherForecast.cs
│       └── WeatherAlert.cs
├── Challenge.UnitTest/
│   └── Weather/
│       ├── WeatherBllTests.cs
│       ├── WeatherAlertBllTests.cs
│       ├── WeatherLocationRepositoryTests.cs
│       └── WeatherRecordRepositoryTests.cs
└── .github/workflows/ci.yml
```

---

## Data Sources

| Source | Data |
|--------|------|
| [data.gov.sg 2-hour forecast](https://data.gov.sg/datasets/d_0a34640fc9a2b4f8f6a9e5e5c543a9b4/view) | Area-level short-term forecast (used as priority weather description) |
| [data.gov.sg 24-hour forecast](https://data.gov.sg/datasets/d_91ffc58b7bf3c8428b6f88da2c6c98e0/view) | Island-wide day/night forecast |
| [data.gov.sg 4-day outlook](https://data.gov.sg/datasets/d_1efe4728b0f4b4b3b1a2e6d31d53cd3b/view) | Temperature, humidity, wind ranges per day |
| [data.gov.sg PSI](https://data.gov.sg/datasets/d_bfa9260d9cb0f88c82a0fe1a7b8c087e/view) | Air quality index (PM2.5, PSI) |
| [OpenWeatherMap Current](https://openweathermap.org/current) | Temperature, feels-like, humidity, wind speed/direction |
| [OpenWeatherMap 5-day Forecast](https://openweathermap.org/forecast5) | 3-hour step forecast (used when data.gov.sg outlook unavailable) |
