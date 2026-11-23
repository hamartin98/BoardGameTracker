---
name: smoke-test-api
description: Confirm real HTTP behavior of BoardGameTracker endpoints (status codes, ProblemDetails, versioning, Swagger) by writing and running integration tests against the in-memory API.
---

# Smoke-test the API with integration tests

Every check is an integration test in `src/IntegrationTests`. `BoardGameTrackerApiFactory` runs the real API in memory, in the Development environment. Each test gets its own freshly seeded in-memory database and its own fixed clock, so you don't start a server, pick a port or stop a process. Run all commands from `src/`.

1. **Find the test class** for the controller under test: `IntegrationTests/ControllerTests/<Controller>Tests.cs`, deriving from `IntegrationTestBase`. Create it if it doesn't exist; `PlaySessionsControllerTests.cs` is a full example.
2. **Write each check as a permanent test.** A check worth running once is worth keeping, so don't write throwaway tests.
   - Send requests with `Client.GetAsync` / `PostAsync` / `PutAsync` / `Client.DeleteAsync`, and read bodies with `ReadAsync<T>`.
   - Check errors with `AssertProblemAsync(response, status, "ExceptionType")`. For 400s from request validation, check only the status code.
   - Request bodies and expected values come from `TestData` (`ViewModelsTestData`), compared with the `Check*` helpers. Never build them inline in the test.
   - After a write, read the entity back to check it was stored, not just the write response.
   - Update and delete need `?version=<current version>`. Seeded entities start at version 1; otherwise read `MetaData.Version` from the earlier response.
   - For anything that depends on time, move `Clock.UtcNow`. It starts at `CommonTestData.DateTime`, the same time as the seeded data.
3. **Run only those tests**, then the whole suite:
   ```bash
   dotnet test IntegrationTests --filter "FullyQualifiedName~<Controller>Tests"
   dotnet test
   ```
4. **Report** each request covered with its expected status, and every failing test with its assertion message. A 500 is always a bug to report.

## Reference

Seed data (`TestData/DbDataSeeder.cs`, fixtures in `TestData/`), using the `CommonTestData` ids:
- `Guid1`: Scythe (base game, 1–5 players). Also Alice, the seeded play session, Alice's score and the Stars category.
- `Guid2`: the Scythe expansion Fenris. Also Bob, Bob's score and the Territories category.
- `Guid3`: the Fenris Bonus category.
- `DeletedGuid`: a soft-deleted board game. Expect 404.

The seeded session (Scythe with Fenris, Alice and Bob) started at `CommonTestData.DateTime` and lasted 2 hours.

Expected error codes (`Middlewares/ExceptionHandlerMiddleware.cs`), returned as ProblemDetails JSON:
- 400: `ApplicationException`, or request validation (DataAnnotations / `IValidatableObject`)
- 404: not found / invalid foreign key
- 409: wrong `?version=` (`VersionMismatchException`)
- 422: `BusinessRuleException`
- 500: anything else

Swagger: `IntegrationTests/OpenApiTests.cs` checks that `/swagger/v1/swagger.json` is generated and lists the paths. When you add an endpoint, add its path to the `DocumentListsPathTest` cases.

## Manual run (exception)

Use this only to look at the Swagger UI in a browser. For everything else, use a test.

```bash
P=Board; DatabaseType=IN_MEMORY dotnet run --project "${P}GameTrackerApi" > "$LOG" 2>&1 &
# Swagger UI: http://localhost:5135/swagger (the port is fixed by Properties/launchSettings.json)
pkill -f "${P}GameTrackerApi"
```

Never write the literal project name in a command that also runs `pkill -f`. The pattern then matches the shell running it, which gets killed (exit code 144).
