---
name: add-entity
description: Wire a domain entity (e.g. Player, PlaySession, PlayerScore) end to end in BoardGameTracker - EF configuration, filter, service, DI, Mapster config, controller, tests and migration - following the existing BoardGame pattern.
---

# Add or finish wiring an entity

`BoardGame` is the reference implementation. For each step, open the BoardGame file named and mirror it. Paths are relative to `src/`.

All existing entities are fully wired; for a new one, add the entity, VMs, service interface and DI registration as well. Check what exists before creating anything.

## Steps

For an entity with a unique, case-insensitive `Name`, derive from `NamedEntity`, `NamedEntityFilter<T>` and `NamedEntityService<T, TFilter>` instead of the `Abstract*` classes. You then get `NormalizedName`, name search and the duplicate-name check (422) for free; only `DuplicateNameMessage` needs to be set. See `Player`.

1. **Entity configuration.** Copy `Database/EntityConfigurations/BoardGameConfiguration.cs`.
   - Add a `Configure<Entity>(this ModelBuilder)` extension in the same `partial class ModelBuilderExtensions`. It must call `AbstractEntityConfiguration.Configure<TEntity>(modelBuilder)` first; that sets the key, the `Version` concurrency token, the soft-delete query filter and indexes.
   - Configure relationships and indexes there, not with attributes.
   - Call it from `BoardGameContext.OnModelCreating` (`Database/BoardGameContext.cs`).
2. **Filter.**
   - Derive the model filter from `AbstractFilter<TEntity>` (`Models/Filters/BoardGameFilter.cs`). Override `FilterQuery`, call `base.FilterQuery(source)` first, then add conditions.
   - Add `ViewModels/Filters/<Entity>FilterVM.cs` deriving from `AbstractFilterVM`.
3. **Service.** Make the class derive from `AbstractService<TEntity, TFilter>` and keep its interface (`Services/PlayerService.cs`). `AbstractService` implements the CRUD methods, so delete the stub methods.
   - Put extra checks in `ValidateCreateAsync(model)` / `ValidateUpdateAsync(existing, model)`. The update one runs after the version check, with the stored entity loaded, so don't load it again.
   - Override `LoadRelations` (a no-op by default) to eager-load the navigations a get-by-id response needs. Override `LoadDeleteRelationsAsync` (defaults to `LoadRelations`) to load the dependents a delete must cascade to.
   - Check foreign keys before create/update with `ThrowIfNotExistsAsync<TReferenced>(id, ct)`.
   - Keep request-only data (ids of referenced entities, flags like `randomStartPlayer`) off the entity. Put it in create/update commands in `Models/Commands/` that wrap the stored values (`Values`), and use the 4-parameter `AbstractService<TEntity, TFilter, TCreate, TUpdate>`, `ICrudService<…, TCreate, TUpdate>` and the 9-parameter `CrudController`, with Mapster configs from the VMs to the commands. See `PlaySessionCommands.cs` and `PlaySessionService`. Resolve the ids in `ValidateCreateAsync`, sync the collection in `UpdateRelationsAsync` (mark the entity `Modified` when it changes, so the version bumps), and include it for list responses in `IncludeListRelations`.
   - Throw `BusinessRuleException` for business-rule violations (→ 422); `ThrowIfDuplicateAsync` already does for uniqueness. Don't use `InvalidOperationException`: it maps to 500, since EF throws it for internal failures. The middleware maps the other exception types.
4. **DI.** Confirm the registration in `BoardGameTrackerApi/Setup/ServiceSetupExtensions.cs`.
5. **Mapping.** Extend the `IRegister` class in `BoardGameTrackerApi/Mappers/Configs/` (`BoardGameMapperConfig.cs`).
   - Map `MetaData` from the entity for response VMs.
   - Map FilterVM → Filter and `PaginatedResponse<T>` → `<Entities>ResponseVM`.
   - `MapperConfigTests` compiles with `RequireDestinationMemberSource = true`, so every destination member needs a source or an explicit `Ignore`.
6. **Controller.** Derive from `CrudController<TEntity, TFilter, TCreateVM, TUpdateVM, TResponseVM, TListResponseVM, TFilterVM>` with `[Route("[controller]")]`, like `BoardGameTrackerApi/Controllers/PlayersController.cs`. It provides create / get-by-id / list / update / delete (update and delete take the version in `If-Match`; routes come out kebab-case).
   - Add extra endpoints to the derived controller with an XML doc comment and a `ProducesResponseType` for the success status, like `GetExpansions` in `BoardGamesController.cs`. Use `{id:guid}` in routes. The ProblemDetails error codes are added by `Setup/ProblemResponsesConvention.cs` from the HTTP method, an `{id}` route parameter and an `If-Match` parameter.
7. **Tests.**
   - Every object a test maps from or compares against lives in TestData, never inline in the test. That covers entities, create/update VMs, filter VMs and expected filters.
     - Models and model filters go in `TestData/Models/<Entity>.cs` (`partial class ModelsTestData`).
     - VMs and filter VMs go in `TestData/ViewModels/<Entity>VM.cs` (`partial class ViewModelsTestData`).
     - Expose them as `get => new() { ... }` properties, with no comments. See `TestData/Models/Player.cs`.
     - Reuse the shared `CommonTestData.Guid1` / `Guid2` / `DeletedGuid` and `CommonTestData.DateTime` rather than adding per-entity ids. Ids only need to be unique within one entity type.
     - For response VMs, reuse `ViewModelsTestData.ResponseMetaDataVM1` / `ResponseMetaDataVM2`.
   - Put the assertions in `Check<Type>(expected, actual)` helpers next to the fixtures (e.g. `CheckPlayer`, `CheckPlayerFilter`, `CheckPlayerResponseVM`), not in the test body.
   - Add mapping tests as `public partial class MappingTests` in `UnitTests/MapperTests/<Entity>MappingTests.cs`. Each test only gets `expected`, maps the fixture and calls the `Check*` helper (see `PlayerMappingTests.cs`). Cover entity → response VM, create VM → entity, and filter VM → filter.
   - Add enum mapping cases if the entity has enums.
8. **Seed data.** Add fixtures to `TestData/DbDataSeeder.cs`. The integration tests use the seed data as their test data, and it's also what manual testing sees.
   - **Integration tests.** Add `IntegrationTests/ControllerTests/<Entities>ControllerTests.cs` deriving from `IntegrationTestBase`, like `BoardGamesControllerTests.cs`. Each test gets its own app and freshly seeded in-memory database, so tests may create, update and delete freely. Services get the injected `TimeProvider` from the factory; it starts at `CommonTestData.DateTime`, and tests move it with `Clock.UtcNow`.
     - Call every endpoint, covering both success and each error status (400 / 404 / 412 / 422 / 428). Use `AssertProblemAsync(response, status, "ExceptionType")` for errors.
     - Start from the seeded entities, using `CommonTestData` ids in URLs. Request bodies and expected responses come from `ViewModelsTestData`, compared with the `Check*` helpers, as in the unit tests.
     - After a write, read the entity back to check it was stored, not just the write response.
     - Sanity check: temporarily break one rule in the service and confirm the matching test fails.
9. **Migration.** Use the `ef-migration` skill.
10. **Verify.**
    - Run `dotnet build` and `dotnet test` from `src/`. This runs both the unit and integration tests.
    - Format only the files this change touched (`dotnet csharpier format <files>`), not the whole tree. Formatting everything rewrites unrelated files.
    - Then follow the `smoke-test-api` skill: confirm the integration tests cover create / get / update with `If-Match` / delete, a 404, a 412 and a 428, and add the new paths to `IntegrationTests/OpenApiTests.cs`.

## Gotchas

- Never set `Version`, `UpdatedAt` or `IsDeleted` in services. `UpdateInterceptor` and `SoftDeleteInterceptor` own them, and `Remove()` becomes a soft delete.
- Column names are snake_case in Postgres (`EFCore.NamingConventions`). Raw filters and `HasFilter(...)` strings must use snake_case.
- A required navigation to an entity with a soft-delete query filter triggers EF's "global query filter ... required end of a relationship" warning at startup unless the dependent entity has the filter too, which step 1 adds. None are logged today, so a new one means a missing configuration.
- If deleting the principal should also soft-delete the dependents, load them in the principal's `LoadDeleteRelationsAsync`, including dependents of dependents (see `BoardGameService`, which loads sessions with their scores). Only tracked dependents are cascaded; anything not loaded stays visible.
- Flag enums are stored as one value and exposed as a list of VM enums. Follow the `BoardGameCategory` mapping in `BoardGameMapperConfig`.
- Throw the generic `EntityNotFoundException<T>` / `InvalidForeignKeyException<T>`. The middleware matches their non-generic bases.
