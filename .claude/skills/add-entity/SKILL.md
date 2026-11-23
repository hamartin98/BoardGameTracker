---
name: add-entity
description: Add a new domain entity to BoardGameTracker end to end - entity, EF configuration, filter, service, DI, Mapster config, controller, tests, seed data and migration - following the existing BoardGame pattern.
---

# Add an entity

`BoardGame` is the reference implementation. For each step, open the file named and mirror it. Paths are relative to `src/`. The cross-cutting rules in CLAUDE.md (interceptors, snake_case, flag enums, Mapster strictness, error mapping, statistics) apply throughout and aren't repeated here.

For an entity with a unique, case-insensitive `Name`, derive from `NamedEntity`, `NamedEntityFilter<T>` and `NamedEntityService<T, TFilter>` instead of the `Abstract*` classes. You then get `NormalizedName`, name search and the duplicate-name check (422); only `DuplicateNameMessage` needs to be set. See `Player`.

## Steps

1. **Entity and VMs.** Add the entity in `Models/Entities/`, and the create/update/response/list-response VMs in `ViewModels/`.
2. **Entity configuration.** Copy `Database/EntityConfigurations/BoardGameConfiguration.cs`.
   - Add a `Configure<Entity>(this ModelBuilder)` extension in the same `partial class ModelBuilderExtensions`. It must call `AbstractEntityConfiguration.Configure<TEntity>(modelBuilder)` first; that sets the key, the `Version` concurrency token, the soft-delete query filter and indexes.
   - Configure relationships and indexes there, not with attributes.
   - Call it from `BoardGameContext.OnModelCreating` (`Database/BoardGameContext.cs`).
3. **Filter.**
   - Derive the model filter from `AbstractFilter<TEntity>` (`Models/Filters/BoardGameFilter.cs`). Override `FilterQuery`, call `base.FilterQuery(source)` first, then add conditions.
   - Add `ViewModels/Filters/<Entity>FilterVM.cs` deriving from `AbstractFilterVM`.
4. **Service.** Add an interface and a class deriving from `AbstractService<TEntity, TFilter>` (`Services/BoardGameService.cs`). `AbstractService` implements CRUD.
   - Put extra checks in `ValidateCreateAsync(model)` / `ValidateUpdateAsync(existing, model)`. The update one runs after the version check, with the stored entity loaded, so don't load it again.
   - Override `LoadRelations` (a no-op by default) to eager-load the navigations a get-by-id response needs. Override `LoadDeleteRelationsAsync` (defaults to `LoadRelations`) to load the dependents a delete must cascade to.
   - Check foreign keys before create/update with `ThrowIfNotExistsAsync<TReferenced>(id, ct)`.
   - Keep request-only data (ids of referenced entities, flags like `randomStartPlayer`) off the entity. Put it in create/update commands in `Models/Commands/` that wrap the stored values (`Values`), and use the 4-parameter `AbstractService<TEntity, TFilter, TCreate, TUpdate>`, `ICrudService<…, TCreate, TUpdate>` and the 9-parameter `CrudController`, with Mapster configs from the VMs to the commands. See `PlaySessionCommands.cs` and `PlaySessionService`. Resolve the ids in `ValidateCreateAsync`, sync the collection in `UpdateRelationsAsync` (mark the entity `Modified` when it changes, so the version bumps), and include it for list responses in `IncludeListRelations`.
   - Throw `BusinessRuleException` for business-rule violations (422). Throw the generic `EntityNotFoundException<T>` / `InvalidForeignKeyException<T>`; the middleware matches their non-generic bases.
5. **DI.** Register the service in `BoardGameTrackerApi/Setup/ServiceSetupExtensions.cs`.
6. **Mapping.** Add an `IRegister` class in `BoardGameTrackerApi/Mappers/Configs/` (`BoardGameMapperConfig.cs`).
   - Map `MetaData` from the entity for response VMs.
   - Map FilterVM → Filter and `PaginatedResponse<T>` → `<Entities>ResponseVM`.
7. **Controller.** Derive from `CrudController<TEntity, TFilter, TCreateVM, TUpdateVM, TResponseVM, TListResponseVM, TFilterVM>` with `[Route("[controller]")]`, like `BoardGameTrackerApi/Controllers/PlayersController.cs`. It provides create / get-by-id / list / update / delete.
   - Add extra endpoints to the derived controller with an XML doc comment and a `ProducesResponseType` for the success status, like `GetExpansions` in `BoardGamesController.cs`.
8. **Unit tests.**
   - Every object a test maps from or compares against lives in TestData, never inline in the test. That covers entities, create/update VMs, filter VMs and expected filters.
     - Models and model filters go in `TestData/Models/<Entity>.cs` (`partial class ModelsTestData`).
     - VMs and filter VMs go in `TestData/ViewModels/<Entity>VM.cs` (`partial class ViewModelsTestData`).
     - Expose them as `get => new() { ... }` properties. See `TestData/Models/Player.cs`.
     - Reuse the shared `CommonTestData.Guid1` / `Guid2` / `DeletedGuid` and `CommonTestData.DateTime` rather than adding per-entity ids. Ids only need to be unique within one entity type.
     - For response VMs, reuse `ViewModelsTestData.ResponseMetaDataVM1` / `ResponseMetaDataVM2`.
   - Put the assertions in `Check<Type>(expected, actual)` helpers next to the fixtures (e.g. `CheckPlayer`, `CheckPlayerFilter`, `CheckPlayerResponseVM`), not in the test body.
   - Add mapping tests as `public partial class MappingTests` in `UnitTests/MapperTests/<Entity>MappingTests.cs`. Each test only gets `expected`, maps the fixture and calls the `Check*` helper (see `PlayerMappingTests.cs`). Cover entity → response VM, create VM → entity, and filter VM → filter. Add `EnumMappingTests` cases for new enums.
9. **Seed data.** Add the seeded entities to `SeedData/SeededEntities.cs` and seed them in `SeedData/DbDataSeeder.cs`; forward them from `ModelsTestData` (`get => SeededEntities.X;`).
10. **Integration tests.** Add `IntegrationTests/ControllerTests/<Entities>ControllerTests.cs` following the `smoke-test-api` skill. Call every endpoint, covering success and each error status (400 / 404 / 412 / 422 / 428), and add the new paths to `IntegrationTests/OpenApiTests.cs`. Sanity check: temporarily break one rule in the service and confirm the matching test fails.
11. **Migration.** Use the `ef-migration` skill.
12. **Verify.** Run `dotnet build` and `dotnet test` from `src/`, then `dotnet csharpier format` on the touched files only.

## Gotchas

- A required navigation to an entity with a soft-delete query filter triggers EF's "global query filter ... required end of a relationship" warning at startup unless the dependent entity has the filter too, which step 2 adds. None are logged today, so a new one means a missing configuration.
- If deleting the principal should also soft-delete the dependents, load them in the principal's `LoadDeleteRelationsAsync`, including dependents of dependents (see `BoardGameService`, which loads sessions with their scores). Only tracked dependents are cascaded; anything not loaded stays visible.
