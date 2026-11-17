# EF commands

## Create migration

```bash
dotnet ef migrations add MIGRATION_NAME --project ../Migrations/Migrations.csproj
```

## Apply migrations

```bash
dotnet ef database update
```

## Revert latest migration (no database, migration files only)

```bash
dotnet ef migrations remove --project ../Migrations/Migrations.csproj
```

## Revert latest migration (database too)

```bash
dotnet ef migrations remove -f --project ../Migrations/Migrations.csproj
```
