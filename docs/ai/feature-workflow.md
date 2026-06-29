# CRUD Workflow

Templates: `backend/Features/CategoryFeature/` · `client/admin-panel/features/category/` · `backend.Tests/Features/CategoryFeature/`. Swap `Category`→`{Entity}`.

## Prerequisites

- [ ] `Domain/Entities/{Entity}.cs` + `DATABASE.md` section
- [ ] Backend working before frontend

## Backend

- [ ] 5 ops: Create/List/Get/Update/Delete under `Features/{Entity}Feature/`
- [ ] Handler rules per `backend.mdc` — copy slug/pagination/delete guards from Category
- [ ] 5 handler tests + `{Entity}IntegrationTests.cs` — `[Collection("Database")]`

## Frontend

- [ ] Copy `features/category/` → `features/{entity}/`
- [ ] Update `api/`, `types/`, `schemas/`, `search-params.ts`, hooks, `index.ts` barrel
- [ ] Routes: `app/(routes)/{entities}/` — list, new, `[id]/view`, `[id]/edit` (thin RSC shells)
- [ ] `config/navigation.ts` `navMain` + `lib/messages.ar.ts`

## Entity notes

| Entity | Read |
|--------|------|
| Product | `Product.cs` + `DATABASE.md` § `products` (`currency`, `status`) |
| Order | `Order.cs` + `DATABASE.md` § `orders`, `order_status_history` |
| Cart | `Cart.cs` + `DATABASE.md` § `carts` (`session_id`) |

## Done

- [ ] `dotnet build` + `dotnet test` (Docker)
- [ ] `npm run typecheck && npm run build`
- [ ] Scalar: `http://localhost:5101/scalar/v1`
