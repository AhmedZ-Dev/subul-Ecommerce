# Agent Workflow

## 1. Scope

Entity? Backend, tests, or frontend? Which operation (Create/List/Get/Update/Delete)?

## 2. Read (minimal)

| Task | Files |
|------|-------|
| New CRUD | `CategoryFeature/` (5 ops), `Domain/Entities/{Entity}.cs`, one `DATABASE.md` section |
| Edit feature | That operation's 3 files only |
| Add tests | Matching file in `backend.Tests/Features/CategoryFeature/` + `Infrastructure/DatabaseFixture.cs` |
| Infra | Target file in `Common/` or `DependencyInjection/` |

Never: 51 entities, full `AppDbContext.cs`, unrelated features, frontend search, `bin/`/`obj/`.

## 3. Plan (3–5 bullets)

Files, routes, handler rules, test cases. Confirm with user if >5 new files.

## 4. Implement

Copy `CategoryFeature` structure. Minimal diff. `Result<T>` + `ToActionResult()`.
CRUD checklist: `docs/ai/feature-workflow.md`

## 5. Verify

```bash
cd backend && dotnet build
dotnet test ../backend.Tests/backend.Tests.csproj
```

Scalar: `http://localhost:5101/scalar/v1` (tests need Docker).

## 6. Stuck?

1. Closest operation in `CategoryFeature/`
2. `DATABASE.md` table section
3. Ask user — don't guess auth/schema/entity fields

Anti-patterns: `.cursor/rules/architecture.mdc`
