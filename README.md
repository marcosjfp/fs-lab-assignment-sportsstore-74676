# SportsStore — Change Log & Feature Overview

Marcos Jose Fernandes Pinheiro - nº 74676

> **Stack:** ASP.NET Core 10 · Blazor Server · Razor Pages · MVC · EF Core 10 · SQL Server

---

## 1. Stripe Payment Integration

**Files added/changed:** `IPaymentService.cs`, `StripePaymentService.cs`, `MockPaymentService.cs`, `OrderController.cs`, `Order.cs`, `appsettings*.json`

- Defined `IPaymentService` with two methods: `CreatePaymentIntentAsync` and `GetPaymentStatusAsync`.
- `StripePaymentService` — live implementation using `Stripe.net` v50; creates a PaymentIntent and verifies its status via Stripe's API.
- `MockPaymentService` — swap-in for local dev; returns fake `pi_mock_*` IDs instantly without hitting Stripe.
- `Order` model extended with `PaymentIntentId` and `PaymentStatus` (`[BindNever]`).
- `OrderController` checkout flow split into three steps:
  1. `POST /Order/Checkout` → creates PaymentIntent → renders **Payment** view with Stripe Elements.
  2. `GET /Order/PaymentReturn` → verifies Stripe status → saves order → redirects to `/Completed`.
  3. `POST /Order/ConfirmMockPayment` → mock-only shortcut, bypasses Stripe redirect.
- `PaymentFailed` view and action handle cancelled / failed payments with `TempData` error messages.
- Secrets (`PublishableKey`, `SecretKey`) are **never** committed — stored via `dotnet user-secrets`.
- `Stripe:UseMock` flag in `appsettings.Development.json` controls which implementation is injected.

> Set `"Stripe:UseMock": true` in `appsettings.Development.json` to skip real Stripe calls locally.

---

## 2. Structured Logging with Serilog

**Files added/changed:** `Program.cs`, `appsettings.json`, `appsettings.Development.json`, `appsettings.Production.json`

- Replaced the default ASP.NET Core logger with **Serilog** (`Serilog.AspNetCore` v10).
- Bootstrap logger captures startup/crash events before the host is built.
- Configuration-driven via `appsettings.json` (`ReadFrom.Configuration`) — no sink code in `Program.cs`.

| Sink | Purpose |
|---|---|
| **Console** | ANSI-coloured output with `CorrelationId` in template |
| **File** | Rolling daily logs under `Logs/`, 7-day retention |
| **Seq** | Structured log server for querying (see §4) |

- Enrichers: `FromLogContext`, `WithMachineName`, `WithEnvironmentName`.
- `UseSerilogRequestLogging` logs every HTTP request with method, path, status code, and elapsed time.
- `Log.Fatal` catches unhandled exceptions at the top level; `Log.CloseAndFlush()` in `finally`.

**Environment overrides:**
- `Development` → minimum level `Debug`; EF Core at `Information`.
- `Production` → minimum level `Information`; EF Core silenced to `Error`.

---

## 3. Correlation ID Middleware

**File added:** `Infrastructure/CorrelationIdMiddleware.cs`

- On every request: reads `X-Correlation-ID` header or generates a new GUID.
- Echoes the ID back in the response header.
- Pushes it into `Serilog.Context.LogContext` so **every log line within the request** carries `CorrelationId`.
- Registered in `Program.cs` after `UseSerilogRequestLogging`.

---

## 4. Seq Log Server Integration

**Files changed:** `SportsStore.csproj`, `appsettings.json`

- Added `Serilog.Sinks.Seq` v9.0.0.
- Seq sink configured to `http://localhost:5341` (UI at `http://localhost:8080`).

---

## 5. Unit Tests

**File added:** `SportsStore.Tests/OrderControllerTests.cs`

| Test | Asserts |
|---|---|
| `Cannot_Checkout_Empty_Cart` | `SaveOrder` never called; model state invalid |
| `Cannot_Checkout_Invalid_ShippingDetails` | `SaveOrder` never called; default view returned |
| `Checkout_With_Valid_Cart_Initiates_Payment` | PaymentIntent created once; `Payment` view returned |
| `PaymentReturn_Saves_Order_When_Payment_Succeeds` | `SaveOrder` called once; redirects to `/Completed` |
| `PaymentReturn_Does_Not_Save_Order_When_Payment_Fails` | `SaveOrder` never called; redirects to `PaymentFailed` |

Uses **xUnit** + **Moq**; `FakeSession` replaces `ISession` in-process.

---
