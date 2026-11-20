---
name: ef-migration
description: Create, apply or revert an EF Core migration for BoardGameTracker (migrations live in the separate Migrations project and target Postgres).
disable-model-invocation: true
---

# EF Core migrations

Run every command from `src/BoardGameTrackerApi/`. Migrations are generated into the separate `Migrations` project, which the Npgsql provider uses via `MigrationsAssembly("Migrations")`.

## Before running

- `dotnet ef --version` should work. If not: `dotnet tool install --global dotnet-ef`.
- Commands use the Postgres provider. `DatabaseType` in `appsettings.json` must be `POSTGRES`, or set the environment variable `DatabaseType=POSTGRES` for the command.
- `database update` and `migrations remove -f` need Postgres reachable with the `PostgresConfig` settings in `appsettings.json` (host `localhost`, port `5432` by default). Ask the user before touching their database.
- Run `dotnet build` from `src/` first, so failures are compile errors rather than EF errors.

## Commands

```bash
# create (name in PascalCase describing the change, e.g. AddPlayerConfiguration)
dotnet ef migrations add MIGRATION_NAME --project ../Migrations/Migrations.csproj

# apply to the database
dotnet ef database update

# remove the latest migration (files only, not yet applied)
dotnet ef migrations remove --project ../Migrations/Migrations.csproj

# remove the latest migration and revert it in the database
dotnet ef migrations remove -f --project ../Migrations/Migrations.csproj
```

## After creating

- Read the generated `Up`/`Down` in `src/Migrations/Migrations/`. Look for unintended `DropTable`, `DropColumn` or type changes, and check that column and index names are snake_case.
- `BoardGameContextModelSnapshot.cs` must be updated in the same change.
- Run `dotnet build` and `dotnet test` from `src/`.
