# BoardGameTracker

## About

Board game collection and session tracker application

## Planned features

- Organize board game collection
- Keep track of game session
- Generate statistics
- Board game recommendation based on time and player count
- BoardGameGeek integration
- Custom scoring by board game
- User login
- Frontend

## Getting started

Requires the .NET 10 SDK.

### Run locally

From the main directory run:

```bash
dotnet run --project src/BoardGameTrackerApi
```

The API listens on `http://localhost:5135`. Swagger is available at [http://localhost:5135/swagger](http://localhost:5135/swagger) (the default launch profile sets `ASPNETCORE_ENVIRONMENT=Development`).

### Database

The `DatabaseType` setting selects the database:

- `IN_MEMORY`: no setup needed. The database is seeded with sample data on startup and reset on every restart.
- `POSTGRES`: uses the `PostgresConfig` section of `appsettings.json`:

  | Setting    | Default              |
  | ---------- | -------------------- |
  | `Host`     | `localhost`          |
  | `Port`     | `5432`               |
  | `Username` | `postgres`           |
  | `Password` | (empty)              |
  | `Database` | `board_game_tracker` |

Settings can also be overridden with environment variables, e.g. `DatabaseType=IN_MEMORY` or `PostgresConfig__Port=5433`.

Before the first run against Postgres, apply the migrations. This needs the `dotnet-ef` tool (`dotnet tool install --global dotnet-ef`). From `src/BoardGameTrackerApi` run:

```bash
dotnet ef database update
```

See [docs/ef_commands.md](docs/ef_commands.md) for creating and reverting migrations.

## API conventions

- **Optimistic concurrency**: every entity has a `version`, returned in `metaData`. Updates and deletes require the current version as a query parameter (`PUT /Players/{id}?version=1`); a stale version returns 409.
- **Paging**: list endpoints take a 1-based `page` and a `pageSize` between 1 and 100 (default 10).
- **Sorting**: `orderBy` takes a property name (case-insensitive), with `orderDirection` `ASC` or `DESC`.
- **Soft delete**: deleted entities are hidden from all endpoints but kept in the database.
- **Names**: board game and player names are unique, ignoring case. A deleted entity's name can be reused.
- **Starting a session**: `POST /PlaySessions/Start` creates a play session that starts now. It takes `boardGameId` and `sessionType` (required), plus optional `usedExpansionIds` and `notes`. The server sets `startTime` to the current UTC time; `duration` can't be set at this point.
- **Finishing a session**: `POST /PlaySessions/{id}/Finish?version=1` sets `duration` to the time elapsed since `startTime`. It fails with 422 if the session is already finished, has no start time or hasn't started yet.
- **Player scores**: a player has at most one score per play session. A score's player and session are set on creation and cannot be changed. Deleting a player or a play session also deletes its scores.
- **Deleting a board game** also deletes its expansions, its play sessions and their scores.
- **Board game types**: a board game's type (`BASE_GAME` or `EXPANSION`) is set on creation and cannot be changed. An expansion must belong to a base game.
- **Errors** are returned as ProblemDetails:

  | Status | Meaning                                                             |
  | ------ | ------------------------------------------------------------------- |
  | 400    | Invalid request, e.g. failed validation or an unknown `orderBy`     |
  | 404    | Entity not found                                                    |
  | 409    | Version mismatch                                                    |
  | 422    | Business rule violation, e.g. a duplicate name or invalid expansion |
  | 500    | Unexpected server error                                             |

## Entity relationships

![image](docs/er_diagram.svg)

## Test

From src directory run:

```bash
dotnet test
```

This runs both test projects:

- `UnitTests`: mappings, filters and services in isolation.
- `IntegrationTests`: the full API in memory. Each test starts its own instance with a freshly seeded in-memory database and calls the endpoints over HTTP.

To run only one of them, pass the project, e.g. `dotnet test IntegrationTests`.

Or

```bash
bash run_tests.sh
```

### Coverage report

To generate a coverage report (merged over both test projects) with html output run:

```bash
bash run_tests.sh -c true
```

The generated html report can be found at: [index.html](src/coveragereport/index.html)

## Formatting

Code is formatted with [CSharpier](https://csharpier.com). From the main directory run:

```bash
dotnet tool restore
dotnet csharpier format <files>
```

Migrations, generated files and `.csproj` files are excluded via `.csharpierignore`.

## Docker

### Dockerize

From main directory run:

```bash
docker build -t board_game_tracker_api -f docker/Dockerfile .
```

### Run dockerized application

```bash
docker run \
	--name board_game_tracker \
	-e DatabaseType=IN_MEMORY \
	-e ASPNETCORE_ENVIRONMENT=Development \
	-p 8080:8080 \
	-d board_game_tracker_api
```

Swagger is only enabled when `ASPNETCORE_ENVIRONMENT=Development` is set, as above. It is then available at [http://localhost:8080/swagger](http://localhost:8080/swagger).
