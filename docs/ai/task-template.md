# Agent Workflow

## 1. Scope

Entity · layer (backend/frontend/tests/both) · operation (one op or full CRUD).

## 2. Read

Follow scan limits in `.cursor/rules/architecture.mdc`. Never full-repo scan.

## 3. Plan (before code)

List: files to touch · routes/endpoints · handler/API transforms · tests. Confirm with user if >5 files or schema change.

## 4. Implement

Copy `CategoryFeature/` or `features/category/`. Minimal diff. CRUD steps: `feature-workflow.md`.

## 5. Verify

```bash
cd backend && dotnet build && dotnet test ../backend.Tests/backend.Tests.csproj
cd client/admin-panel && npm run typecheck && npm run build
```

## 6. Stuck?

Nearest Category op · `DATABASE.md` table · `architecture.mdc` anti-patterns · ask user (don't guess auth/fields).
