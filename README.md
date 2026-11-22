# BoardGameTracker

## About

Board game collection and session tracker application

## Planned features

- [x] Organize board game collection
- [x] Keep track of game session
- [ ] Generate statistics
- [x] Board game recommendation based on time and player count
- [ ] BoardGameGeek integration
- [x] Custom scoring by board game
- [ ] User login
- [ ] Frontend

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
- **Names**: board game and player names are unique, ignoring case. Scoring category names are unique within their board game. A deleted entity's name can be reused.
- **Session players**: a play session has a list of players (`playerIds`) and an optional `startPlayerId`, which must be one of them. Both can be set when a session is created, started or updated; an update replaces the player list. On create and start, `randomStartPlayer: true` picks a random start player from the players instead (it can't be combined with `startPlayerId`). The order of the players isn't stored.
- **Starting a session**: `POST /PlaySessions/Start` creates a play session that starts now. It takes `boardGameId` and `sessionType` (required), plus optional `usedExpansionIds`, `playerIds`, `startPlayerId`, `randomStartPlayer` and `notes`. The server sets `startTime` to the current UTC time; `duration` can't be set at this point.
- **Finishing a session**: `POST /PlaySessions/{id}/Finish?version=1` sets `duration` to the time elapsed since `startTime`. It fails with 422 if the session is already finished, has no start time or hasn't started yet.
- **Scoring categories**: a board game, base game or expansion, has an ordered list of scoring categories (`/ScoringCategories`). A new category is added to the end of its game's order; `GET /BoardGames/{id}/ScoringCategories` lists them in order and `PUT /BoardGames/{id}/ScoringCategories/Order` sets a new order from the full list of their ids. A category with recorded scores can't be deleted.
- **Session scoring categories**: a session is scored in its board game's categories followed by those of each used expansion; `GET /PlaySessions/{id}/ScoringCategories` lists them.
- **Player scores**: a player has at most one score per play session, and must be one of the session's players. A score's player and session are set on creation and cannot be changed. The score lists the points for every scoring category of the session (`categoryScores`), each exactly once; `score` is their sum and can't be set directly. In a game without scoring categories, a score only records the win. Scores saved before scoring categories existed keep their total. Deleting a player or a play session also deletes its scores.
- **Changing a session with scores**: a player who has a score can't be removed from the session, and neither can the board game or a used expansion whose categories have scores (422).
- **Recommendations**: `GET /Recommendations?playerCount=2` recommends base games that support the player count, best first. Optional `minPlayTime`, `maxPlayTime` and `categories` make fitting games score higher without excluding the others, and games not played for a long time score higher. The time fit uses both the game's play time and the average duration of finished sessions with that player count. Each result shows the parts of its score. `limit` sets the number of results (default 10, at most 50). The weights are set in the `RecommendationConfig` section of `appsettings.json`.
- **Deleting a board game** also deletes its expansions, its play sessions and their scores, and the scoring categories of the game and its expansions.
- **Board game types**: a board game's type (`BASE_GAME` or `EXPANSION`) is set on creation and cannot be changed. An expansion must belong to a base game.
- **Errors** are returned as ProblemDetails:

  | Status | Meaning                                                                                            |
  | ------ | -------------------------------------------------------------------------------------------------- |
  | 400    | Invalid request, e.g. failed validation or an unknown `orderBy`                                    |
  | 404    | Entity not found                                                                                   |
  | 409    | Version mismatch                                                                                   |
  | 422    | Business rule violation, e.g. a duplicate name, an invalid expansion or a missing scoring category |
  | 500    | Unexpected server error                                                                            |

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
