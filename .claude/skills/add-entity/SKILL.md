---
name: add-entity
description: Wire a domain entity (e.g. Player, PlaySession, PlayerScore) end to end in BoardGameTracker - EF configuration, filter, service, DI, Mapster config, controller, tests and migration - following the existing BoardGame pattern.
---

# Add or finish wiring an entity

`BoardGame` is the reference implementation. For each step, open the BoardGame file named and mirror it. Paths are relative to `src/`.

Entities, VMs, service interfaces, DI registrations and basic mapper configs already exist for `Player`, `PlaySession` and `PlayerScore`. Their services are `NotImplementedException` stubs. Check what exists before creating anything.

## Steps

1. **Entity configuration.** Copy `Database/EntityConfigurations/BoardGameConfiguration.cs`.
   - Add a `Configure<Entity>(this ModelBuilder)` extension in the same `partial class ModelBuilderExtensions`. It must call `AbstractEntityConfiguration.Configure<TEntity>(modelBuilder)` first; that sets the key, the `Version` concurrency token, the soft-delete query filter and indexes.
   - Configure relationships and indexes there, not with attributes.
   - Call it from `BoardGameContext.OnModelCreating` (`Database/BoardGameContext.cs`).
2. **Filter.**
   - Derive the model filter from `AbstractFilter<TEntity>` (`Models/Filters/BoardGameFilter.cs`). Override `FilterQuery`, call `base.FilterQuery(source)` first, then add conditions.
   - Add `ViewModels/Filters/<Entity>FilterVM.cs` deriving from `AbstractFilterVM`.
3. **Service.** Make the class derive from `AbstractService<TEntity, TFilter>` and keep its interface (`Services/BoardGameService.cs`).
   - Implement `LoadRelations` to eager-load the navigations a get-by-id response or a cascading delete needs.
   - Check foreign keys before create/update with `ThrowIfNotExists<TReferenced>(id, ct)`.
   - Throw `InvalidOperationException` for business-rule violations (→ 422). The middleware maps the other exception types.
4. **DI.** Confirm the registration in `BoardGameTrackerApi/Setup/ServiceSetupExtensions.cs`.
5. **Mapping.** Extend the `IRegister` class in `BoardGameTrackerApi/Mappers/Configs/` (`BoardGameMapperConfig.cs`).
   - Map `MetaData` from the entity for response VMs.
   - Map FilterVM → Filter and `PaginatedResponse<T>` → `<Entities>ResponseVM`.
   - `MapperConfigTests` compiles with `RequireDestinationMemberSource = true`, so every destination member needs a source or an explicit `Ignore`.
6. **Controller.** Copy `BoardGameTrackerApi/Controllers/BoardGamesController.cs`, including its XML doc comments and `ProducesResponseType` attributes.
   - Update and delete take `[FromQuery] int version`.
   - Create returns `CreatedAtAction(nameof(GetById), ...)`.
   - Give the get-by-id route a unique `Name`. `"GetById"` is already taken by BoardGames.
7. **Tests.**
   - Add test fixtures under `TestData/Models` and `TestData/ViewModels`, using fixed Guids like `CommonTestData`.
   - Add mapping tests as `public partial class MappingTests` in `UnitTests/MapperTests/`.
   - Add enum mapping cases if the entity has enums.
8. **Seed data (optional).** For manual testing, add fixtures to `Database/DbDataSeeder.cs`.
9. **Migration.** Use the `ef-migration` skill.
10. **Verify.**
    - Run `dotnet build`, `dotnet test` and `dotnet csharpier format .` from `src/`.
    - Then use the `smoke-test-api` skill for create / get / update with `?version=` / delete, and for a 404 and a 409.

## Gotchas

- Never set `Version`, `UpdatedAt` or `IsDeleted` in services. `UpdateInterceptor` and `SoftDeleteInterceptor` own them, and `Remove()` becomes a soft delete.
- Column names are snake_case in Postgres (`EFCore.NamingConventions`). Raw filters and `HasFilter(...)` strings must use snake_case.
- A required navigation to an entity with a soft-delete query filter triggers EF's "global query filter ... required end of a relationship" warning (already logged for `BoardGame` ↔ `PlaySession`). Make the navigation optional, or make sure both sides have matching filters.
- Flag enums are stored as one value and exposed as a list of VM enums. Follow the `BoardGameCategory` mapping in `BoardGameMapperConfig`.
- Throw the generic `EntityNotFoundException<T>` / `InvalidForeignKeyException<T>`. The middleware matches their non-generic bases.
