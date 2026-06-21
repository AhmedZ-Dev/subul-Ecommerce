# CRUD Feature Workflow

Template: `backend/Features/CategoryFeature/`. Copy file names, swap `Category`→`{Entity}`.

## Prerequisites

- [ ] Entity in `Domain/Entities/{Entity}.cs`
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

Each folder: `{Op}Endpoint.cs`, `{Op}Handler.cs`, `{Op}Command.cs` or `{Op}Query.cs`.

## Handler Checklist (per operation)

**Create:** trim inputs → uniqueness (`"already exists"`) → FK check (`"not found"`) → set timestamps → Add + SaveChanges → Success response

**List:** Page/Limit defaults → AsNoTracking → Search both En/Ar → Sort switch → paginated response

**GetById:** AsNoTracking → `"not found"` if missing

**Update:** load entity → re-validate uniqueness/FK excluding self → UpdatedAt → SaveChanges

**Delete:** load → check children/FK relations → Remove → `"not found"` / guard messages

Copy logic from matching Category handler:
- Slug: `CreateCategoryHandler.cs`
- Pagination: `ListCategoryPaginatedHandler.cs`
- Delete guards: `DeleteCategoryHandler.cs`
- Update mapping: `UpdateCategoryEndpoint.cs`

## Entity Extras

| Entity | See |
|--------|-----|
| Product | `productForExample.sql` (variants, images, attributes) |
| Order | `order_status_history` in DATABASE.md |
| Cart | `carts.session_id` — session vs user |

## Done

- [ ] `dotnet build`
- [ ] All 5 routes in Scalar
- [ ] `DATABASE.md` updated only if schema changed

No manual DI registration needed.
