# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Overview

ASP.NET Core (.NET 10) Web API for tracking a board game collection and play sessions. The solution file is `src/BoardGameTracker.slnx`; run `dotnet` commands from `src/` unless noted.

## Commands

```bash
# from src/
dotnet run --project BoardGameTrackerApi          # Swagger UI only when ASPNETCORE_ENVIRONMENT=Development
DatabaseType=IN_MEMORY dotnet run --project BoardGameTrackerApi   # no Postgres needed; seeds sample data

dotnet test --filter "FullyQualifiedName~BoardGameMappingTests"   # single class
dotnet test --filter "FullyQualifiedName~BoardGameMappingTests.SomeTestName"  # single test
bash run_tests.sh -c true                          # tests + coverage; HTML report in src/coveragereport/ (needs reportgenerator)

# Formatting (local tool from repo-root dotnet-tools.json)
dotnet tool restore
dotnet csharpier format .
```

EF Core migrations (separate `Migrations` project, needs Postgres): use the `ef-migration` skill.

To run the API and check endpoints with curl, use the `smoke-test-api` skill (port 5135 is fixed by `launchSettings.json`).

Docker (from repo root): `docker build -t board_game_tracker_api -f docker/Dockerfile .`, then run with `-e DatabaseType=IN_MEMORY -p 8080:8080`.

## Configuration

- `DatabaseType` (`IN_MEMORY` | `POSTGRES`) selects the provider in `Setup/DatabaseSetupExtensions.cs`. `IN_MEMORY` also triggers `DbDataSeeder.Seed` at startup.
- Postgres settings come from the `PostgresConfig` section (`Configuration/PostgresConfiguration.cs`). Npgsql is configured with `MigrationsAssembly("Migrations")` and snake_case naming (`EFCore.NamingConventions`), so DB column names are snake_case (e.g. query filters/index filters use `is_deleted`).

## Cross-cutting conventions

- **Optimistic concurrency via `Version`**: every entity has an integer `Version` (concurrency token). Update and delete endpoints take `?version=` as a query parameter; the service throws `VersionMismatchException` (→ 409) if it doesn't match.
- **Interceptors manage audit fields**: `UpdateInterceptor` increments `Version` and sets `UpdatedAt` on modified entities; `SoftDeleteInterceptor` turns deletes into `IsDeleted = true`. A global query filter hides soft-deleted rows. Don't set these fields manually in services.
- **Filtering/pagination**: `AbstractFilter<T>` handles paging (1-based `Page`; `Page` and `PageSize` are clamped in their setters, PageSize to 1–100, default 10), `CreatedAt` range, and dynamic `OrderBy` by property name string (`Models/Filters/IOrderedQueryable.cs`), always tie-breaking on `id`. Entity-specific filters override `FilterQuery` and call `base`.
- **Flags enums**: `BoardGameCategory` is a bitflag stored as a single value; the API exposes it as a `List<BoardGameCategoryVM>`, converted in `BoardGameMapperConfig`. Domain and VM enum values must stay in sync (covered by `EnumMappingTests`).
- **Mapster configs** are `IRegister` classes discovered by assembly scan in `Setup/MapperSetupExtensions.cs`. `MapperConfigTests` compiles configs with `RequireDestinationMemberSource = true`, so every destination member needs a source.
- **Errors**: services throw exceptions; `Middlewares/ExceptionHandlerMiddleware.cs` maps them to ProblemDetails status codes (`ApplicationException` 400, not-found 404, `VersionMismatchException` 409, `InvalidOperationException` 422, else 500).

## Current state

Only `BoardGame` is fully wired (controller + entity configuration). `Player`, `PlaySession`, and `PlayerScore` have entities, interfaces, VMs and stub services, but no controllers or `ModelBuilder` configuration yet. Use the `add-entity` skill to wire one up.
