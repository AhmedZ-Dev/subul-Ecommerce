# Agent Workflow

## 1. Scope

Entity? Backend or frontend? Which operation (Create/List/Get/Update/Delete)?

## 2. Read (minimal)

| Task | Files |
|------|-------|
| New CRUD | `CategoryFeature/` (all 5 ops), `Domain/Entities/{Entity}.cs`, one `DATABASE.md` section |
| Edit feature | That operation's 3 files only |
| Infra | Target file in `Common/` or `DependencyInjection/` |

Never: 51 entities, full `AppDbContext.cs`, unrelated features, frontend search.

## 3. Plan (3–5 bullets)

Files, routes, handler rules, response records. Confirm with user if >5 new files.

## 4. Implement

Copy `CategoryFeature` structure. Minimal diff. `Result<T>` + `ToActionResult()`.

CRUD checklist: `docs/ai/feature-workflow.md`

## 5. Verify

```bash
cd backend && dotnet build
```

Scalar: `http://localhost:5101/scalar/v1`

## 6. Stuck?

1. `CategoryFeature/` closest operation
2. `DATABASE.md` table section
3. Ask user — don't guess auth/schema

Anti-patterns: `.cursor/rules/architecture.mdc`
