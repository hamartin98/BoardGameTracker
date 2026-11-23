# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Overview

ASP.NET Core (.NET 10) Web API for tracking a board game collection and play sessions. The solution file is `src/BoardGameTracker.slnx`; run `dotnet` commands from `src/` unless noted.

## Commands

```bash
# from src/
dotnet run --project BoardGameTrackerApi          # Swagger UI only when ASPNETCORE_ENVIRONMENT=Development
DatabaseType=IN_MEMORY dotnet run --project BoardGameTrackerApi   # no Postgres needed; seeds sample data

dotnet test                                        # unit + integration tests
dotnet test IntegrationTests                       # only integration tests (or UnitTests)
dotnet test --filter "FullyQualifiedName~BoardGameMappingTests"   # single class
dotnet test --filter "FullyQualifiedName~BoardGameMappingTests.SomeTestName"  # single test
bash run_tests.sh -c true                          # tests + coverage; HTML report in src/coveragereport/ (needs reportgenerator)

# Formatting (local tool from repo-root dotnet-tools.json)
dotnet tool restore
dotnet csharpier format <files>                    # only the files you changed; `.` rewrites unrelated files
```

EF Core migrations (separate `Migrations` project, needs Postgres): use the `ef-migration` skill.

To check real HTTP behavior of endpoints, use the `smoke-test-api` skill: checks are written and run as integration tests against the in-memory API, with no server to start.

Docker (from repo root): `docker build -t board_game_tracker_api -f docker/Dockerfile .`, then run with `-e DatabaseType=IN_MEMORY -p 8080:8080`.

## Configuration

- `DatabaseType` (`IN_MEMORY` | `POSTGRES`) selects the provider in `Setup/DatabaseSetupExtensions.cs`. `IN_MEMORY` also triggers `DbDataSeeder.Seed` at startup.
- Postgres settings come from the `PostgresConfig` section (`Configuration/PostgresConfiguration.cs`). Npgsql is configured with `MigrationsAssembly("Migrations")` and snake_case naming (`EFCore.NamingConventions`), so DB column names are snake_case (e.g. query filters/index filters use `is_deleted`).
- `POSTGRES` runs `PrepareDatabaseAsync` (`Setup/DatabaseStartupExtensions.cs`) at startup: retries transient connection errors for 30 s, then applies pending migrations, or throws when `PostgresConfig:ApplyMigrationsOnStartup` is false. It runs after `Build()`, so `dotnet ef` design-time runs are unaffected.
- Health endpoints (`Setup/HealthCheckSetupExtensions.cs`): `/health/live` (no checks) and `/health/ready` (`BoardGameContext` connection).
- `ImportConfig:TimeZone` (default `Europe/Budapest`): time zone of the local times in imported BoardRecord backups.

## Cross-cutting conventions

- **Optimistic concurrency via `Version`**: every entity has an integer `Version` (concurrency token). Single-entity responses return it as the `ETag` header (`"3"`); update, delete and finish endpoints take it in `If-Match`, parsed by `Controllers/EntityTags.cs` (missing → `PreconditionRequiredException` 428, malformed → `InvalidIfMatchException` 400). The service throws `VersionMismatchException` (→ 412) if it doesn't match.
- **Routes are kebab-case**: `[controller]` goes through `KebabCaseParameterTransformer` (`BoardGames` → `/board-games`); write literal route segments in kebab-case too, and constrain ids (`{id:guid}/scoring-categories`).
- **Swagger error codes** come from `Setup/ProblemResponsesConvention.cs`, per action: 400 and 500 always, 404 for `{id}` routes and writes, 422 for writes, 412 and 428 when the action takes `If-Match`.
- **Interceptors manage audit fields**, using the injected `TimeProvider`: `CreateInterceptor` sets `CreatedAt`/`UpdatedAt` on added entities when they're unset (fixtures with explicit dates keep them); `UpdateInterceptor` increments `Version` and sets `UpdatedAt` on modified entities; `SoftDeleteInterceptor` turns deletes into `IsDeleted = true`. A global query filter hides soft-deleted rows. Don't set these fields manually in services.
- **Filtering/pagination**: `AbstractFilter<T>` handles paging (1-based `Page`; `Page` and `PageSize` are clamped in their setters, PageSize to 1–100, default 10), `CreatedAt` range, and dynamic `OrderBy` by property name string (`Models/Filters/QueryableOrderingExtensions.cs`), always tie-breaking on `id`. Entity-specific filters override `FilterQuery` and call `base`.
- **Flags enums**: `BoardGameCategory` is a bitflag stored as a single value; the API exposes it as a `List<BoardGameCategoryVM>`, converted in `BoardGameMapperConfig`. Domain and VM enum values must stay in sync (covered by `EnumMappingTests`).
- **Mapster configs** are `IRegister` classes discovered by assembly scan in `Setup/MapperSetupExtensions.cs`. `MapperConfigTests` compiles configs with `RequireDestinationMemberSource = true`, so every destination member needs a source.
- **Calculated statistics**: `BoardGame.Statistics` and `Player.Statistics` are not stored (`entity.Ignore` in their EF configurations). Services fill them in the `AbstractService.LoadCalculatedValuesAsync` hook, which runs after get, list, create and update, through `IStatisticsService`. That service loads the sessions in one query, and the pure calculators in `Services/Statistics/` aggregate them in memory. Win rates and scores only count `COMPETITIVE` sessions. Board games nested in other responses (`expansionOf`, recommendations) have `null` statistics. Request mappings must ignore `Statistics`.
- **Errors**: services throw exceptions; `Middlewares/ExceptionHandlerMiddleware.cs` maps them to ProblemDetails status codes (`ApplicationException` 400, not-found 404, `VersionMismatchException` 412, `BusinessRuleException` 422, `PreconditionRequiredException` 428, else 500 — including `InvalidOperationException`).

## Tests

- `UnitTests` test mappings, filters and services directly; services run against an in-memory `BoardGameContext` from `UnitTests/ServiceTests/TestDbContextFactory.cs`.
- `IntegrationTests` run the real API through `WebApplicationFactory<Program>` (`IntegrationTests/BoardGameTrackerApiFactory.cs`). Every test gets its own app and its own in-memory database (unique `InMemoryDatabaseName` setting), seeded by `DbDataSeeder` at startup. The seed data is the integration test data, so changing a seeded entity can break integration tests.
- Fixtures and `Check*` helpers for both live in `TestData` (references xunit; tests only). The seeded entities, their ids (`SeedConstants`) and `DbDataSeeder` live in `SeedData`, the only one of the two the API references, so it must not use xunit or test-only types. `ModelsTestData` forwards the seeded fixtures (`Alice => SeededEntities.Alice`) and `CommonTestData` ids forward to `SeedConstants`.

## Current state

CRUD entities: `BoardGame`, `Player`, `PlaySession`, `PlayerScore`, `ScoringCategory` (`CategoryScore` is the `PlayerScore`–`ScoringCategory` join, written through `PlayerScore`; no controller). `RecommendationsController` is read-only, backed by `RecommendationService`. `POST /imports/board-record` (`Services/Imports/BoardRecordImportService.cs`) syncs a BoardRecord backup: imported records are tracked by their source UUID in `ExternalReference`, so re-importing only adds new records. Use the `add-entity` skill for a new entity. Every controller has integration tests in `IntegrationTests/ControllerTests/`.
