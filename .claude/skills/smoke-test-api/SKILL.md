---
name: smoke-test-api
description: Run the BoardGameTracker API against the in-memory database and call endpoints with curl to confirm real HTTP behavior (status codes, ProblemDetails, versioning).
---

# Smoke-test the API

1. **Build** from `src/`: `dotnet build`. Stop if it fails.
2. **Start** in the background. Log to a scratch file, not the repo:
   ```bash
   P=Board; DatabaseType=IN_MEMORY dotnet run --no-build --project "${P}GameTrackerApi" > "$LOG" 2>&1 &
   ```
   - The port is **5135**. `Properties/launchSettings.json` sets it and overrides `ASPNETCORE_URLS`, so don't pass a different port.
   - The environment is Development, so Swagger is at `/swagger`.
3. **Wait** until `curl -s -o /dev/null http://localhost:5135/BoardGames` succeeds. Poll for up to ~30 s and read `$LOG` if it never answers.
4. **Call endpoints** with `curl -s -w "\nHTTP %{http_code}\n"`. Known seed data, from `Database/DbDataSeeder.cs` and `TestData/`:
   - `77934a16-a70a-4ac3-9c0e-000000000001`: Scythe (base game)
   - `77934a16-a70a-4ac3-9c0e-200000000001`: Scythe expansion (Fenris)
   - `77934a16-a70a-4ac3-9c0e-900000000000`: soft-deleted. Expect 404.

   Expected error codes (`Middlewares/ExceptionHandlerMiddleware.cs`), returned as ProblemDetails JSON:
   - 400: `ApplicationException`
   - 404: not found / invalid foreign key
   - 409: wrong `?version=`
   - 422: `BusinessRuleException`
   - 500: anything else (a 500 is a bug to report)

   Update and delete require `?version=<current version>`. Read it from the `metaData.version` of a GET.
5. **Stop** the server every time, even after failures: `pkill -f "${P}GameTrackerApi"`. Never write the literal project name in a command that also runs `pkill -f`: the pattern then matches the shell running it, which gets killed (exit code 144) and skips every later command in that call.

Report each request with its status code and the relevant part of the body. The in-memory database resets on every start.
