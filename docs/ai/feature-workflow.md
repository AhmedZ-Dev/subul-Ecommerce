# CRUD Feature Workflow

Template: `backend/Features/CategoryFeature/`. Tests: `backend.Tests/Features/CategoryFeature/`.
Copy file names, swap `Category`→`{Entity}`.

## Prerequisites

- [ ] Entity in `Domain/Entities/{Entity}.cs` — read required fields before writing tests
- [ ] Table in PostgreSQL + `DATABASE.md`

## Create 5 Operation Folders

```
Features/{Entity}Feature/
  Create{Entity}/       POST   api/{entities}     → 201
  List{Entity}Paginated/ GET   api/{entities}     → 200
  GetById{Entity}/       GET   api/{entities}/{id} → 200
  Update{Entity}/        PUT   api/{entities}/{id} → 200
  Delete{Entity}/        DELETE api/{entities}/{id} → 200
```

Each folder: `{Op}Endpoint.cs` (class `{Op}Controller`), `{Op}Handler.cs`, `{Op}Command.cs` or `{Op}Query.cs`.
Update adds `Update{Entity}Request` mapped in endpoint.

## Handler Checklist

**Create:** trim → uniqueness (`"already exists"`) → FK (`"not found"`) → slug → timestamps → SaveChanges

**List:** Page/Limit defaults → AsNoTracking → Search En/Ar → paginated response

**GetById:** AsNoTracking → `"not found"`

**Update:** load → re-validate uniqueness/FK excluding self → UpdatedAt → SaveChanges

**Delete:** load → children/FK guard → Remove

Copy from Category: slug (`CreateCategoryHandler`), pagination (`ListCategoryPaginatedHandler`), delete guards (`DeleteCategoryHandler`), update mapping (`UpdateCategoryEndpoint`).

## Tests (mirror Category)

```
backend.Tests/Features/{Entity}Feature/
  Create{Entity}HandlerTests.cs
  Update{Entity}HandlerTests.cs
  Delete{Entity}HandlerTests.cs
  GetById{Entity}HandlerTests.cs
  List{Entity}HandlerTests.cs
  {Entity}IntegrationTests.cs
```

Use `[Collection("Database")]` + `DatabaseFixture`. Handler tests instantiate handler directly.
Integration tests use `TestWebApplicationFactory`.

## Entity Extras

| Entity | See |
|--------|-----|
| Product | `productForExample.sql`, `Domain/Entities/Product.cs` (`Currency`, `Status` required) |
| Order | `order_status_history` in DATABASE.md |
| Cart | `carts.session_id` — session vs user |

## Done

- [ ] `dotnet build` in `backend/`
- [ ] `dotnet test backend.Tests/backend.Tests.csproj` (Docker running)
- [ ] All 5 routes in Scalar (`http://localhost:5101/scalar/v1`)
- [ ] `DATABASE.md` updated only if schema changed

No manual DI registration needed.
