# Codebase Audit & Improvement Plan

The project is a Next.js 16 / React 19 admin panel for an e-commerce platform (Subul).
It currently has one fully built feature — **Category management** — plus a boilerplate dashboard.
The architecture is solid (feature-slice design, barrel exports, RSC + Client boundary respected),
but a handful of concrete, non-trivial issues surfaced during the audit.
Every item below is a **real problem**, not gold-plating.

---

## Summary of Findings

| # | Severity | Area | Issue |
|---|---|---|---|
| 1 | 🔴 High | `category-tree.tsx` | Hardcoded English strings & bare `confirm()` dialog |
| 2 | 🔴 High | `category-tables/index.tsx` | Hardcoded Arabic inline string |
| 3 | 🟡 Medium | `category-listing-page.tsx` | Both `useCategories` + `useCategoryTree` always fire, even when only one view is visible |
| 4 | 🟡 Medium | `cell-action.tsx` | Delete confirmation strings duplicated from `messages.ar.ts` inline |
| 5 | 🟡 Medium | `lib/search-params.ts` | Unused `tableSearchParams` / `tableSearchParamsCache` (orphan file) |
| 6 | 🟡 Medium | `view/page.tsx` | `getCategoryById` called **twice** (once in `generateMetadata`, once in the page body) |
| 7 | 🟡 Medium | `category-status-toggle.tsx` | Local `useState` for status duplicates what React Query already tracks optimistically |
| 8 | 🟢 Low | `category-form.tsx` | `form` object in `useEffect` deps → stale-closure warning in strict mode |
| 9 | 🟢 Low | `app/layout.tsx` | Metadata still references **"شركة أكمي"** (placeholder company), not "سبُل المستقبل" |
| 10 | 🟢 Low | `messages.ar.ts` | `statusLabels` / `typeLabels` + `translateStatus/translateType` functions are dead code |
| 11 | 🟢 Low | `category-tree.tsx` | `aria-label` for expand/collapse is English ("Collapse" / "Expand") |

---

## Proposed Changes

### 1. `category-tree.tsx` — Hardcoded strings & native `confirm()` [HIGH]

`TreeNode` uses raw English strings for its dropdown menu items ("Edit", "Delete") and calls the
browser's native `confirm()` instead of the `AlertDialog` used everywhere else.

#### [MODIFY] [category-tree.tsx](file:///home/ahmed/Desktop/INSS%20PROJECT/subul-Ecommerce/client/admin-panel/features/category/components/category-tree.tsx)

- Replace `"Edit"` / `"Delete"` menu item labels with `messages.common.edit` / `messages.common.delete`.
- Replace `confirm(...)` with an `AlertDialog` (same pattern as `cell-action.tsx` and `category-view.tsx`).
- Translate `aria-label="Open actions"` via `messages`.
- Translate `aria-label="Collapse"/"Expand"` using Arabic labels.
- Replace hardcoded `"No categories found."` with `messages.category.listing.emptyTitle` (or a new key).
- Translate the tree header labels: `"Name"`, `"Slug"`, `"Status"`.

---

### 2. `category-tables/index.tsx` — Hardcoded Arabic inline string [HIGH]

Line 172: `لا توجد فئات.` is hardcoded inline instead of using `messages`.

#### [MODIFY] [index.tsx (category-tables)](file:///home/ahmed/Desktop/INSS%20PROJECT/subul-Ecommerce/client/admin-panel/features/category/components/category-tables/index.tsx)

- Replace with `m.emptyTitle` or `messages.common.noResults`.

---

### 3. `category-listing-page.tsx` — Unnecessary double data fetch [MEDIUM]

Both `useCategories(queryParams)` and `useCategoryTree()` are always called at the top of
`CategoryListingPage`, regardless of which view is active. `useCategoryTree` internally fires
`getCategories({ limit: 1000 })`, so in **table mode** we fire two network requests.

#### [MODIFY] [category-listing-page.tsx](file:///home/ahmed/Desktop/INSS%20PROJECT/subul-Ecommerce/client/admin-panel/features/category/components/category-listing-page.tsx)

- Add `enabled: viewMode === 'tree'` to the `useCategoryTree` call (or conditionally include it).
- Alternatively, pass the `viewMode` into the hook so it gates its own `useQuery` call.

> [!NOTE]
> `useCategories` must still always run (needed for table pagination). Only `useCategoryTree` needs gating.

---

### 4. `cell-action.tsx` — Delete dialog strings duplicated inline [MEDIUM]

The `AlertDialog` inside `CategoryCellAction` hardcodes `"تأكيد الحذف"`, `"إلغاء"`, `"حذف"` and the
confirmation message — none of which go through `messages`.

#### [MODIFY] [cell-action.tsx](file:///home/ahmed/Desktop/INSS%20PROJECT/subul-Ecommerce/client/admin-panel/features/category/components/category-tables/cell-action.tsx)

- Replace all inline Arabic strings with references to `messages.category.view` and `messages.common`.

---

### 5. `lib/search-params.ts` — Orphan file [MEDIUM]

`tableSearchParams` and `tableSearchParamsCache` are defined but **never imported** anywhere in the
codebase. The category listing page manages its own URL params manually via `useSearchParams`.

#### [DELETE] [search-params.ts](file:///home/ahmed/Desktop/INSS%20PROJECT/subul-Ecommerce/client/admin-panel/lib/search-params.ts)

Delete the file. If `nuqs` server-side parsing is needed in the future, it can be re-created
per-feature (e.g., `features/category/search-params.ts`).

---

### 6. `view/page.tsx` — Double network call for `getCategoryById` [MEDIUM]

`getCategoryById(categoryId)` is called in both `generateMetadata` and the page body — two separate
RSC invocations, two HTTP requests to the backend.

#### [MODIFY] [view/page.tsx](file:///home/ahmed/Desktop/INSS%20PROJECT/subul-Ecommerce/client/admin-panel/app/%28routes%29/categories/%5Bid%5D/view/page.tsx)

Use Next.js's `React.cache` (or the built-in per-request deduplication via `fetch`) to deduplicate
the call. The cleanest fix: wrap `getCategoryById` with `cache()` from React in a shared utility,
or restructure so both `generateMetadata` and the page share one cached call.

---

### 7. `category-status-toggle.tsx` — Redundant local state [MEDIUM]

The component maintains its own `useState<CategoryStatus>` that shadows the optimistic update
already done in `useChangeCategoryStatus.onMutate`. The `useEffect` that syncs `initialStatus`
back into local state creates a potential flash when the prop changes.

#### [MODIFY] [category-status-toggle.tsx](file:///home/ahmed/Desktop/INSS%20PROJECT/subul-Ecommerce/client/admin-panel/features/category/components/category-status-toggle.tsx)

- Remove local `useState` and `useEffect`.
- Read `status` directly from the `initialStatus` prop (React Query already handles the optimistic
  value at the cache level; the prop will reflect it immediately).

---

### 8. `category-form.tsx` — `form` in `useEffect` deps [LOW]

`form` (from `useForm`) is included in two `useEffect` dependency arrays. The `useForm` return
value is not a stable reference — including it can suppress lint warnings but may cause unexpected
re-runs. The conventional fix is to destructure stable methods (`form.reset`, `form.setValue`,
`form.getValues`) into the dep array.

#### [MODIFY] [category-form.tsx](file:///home/ahmed/Desktop/INSS%20PROJECT/subul-Ecommerce/client/admin-panel/features/category/components/category-form.tsx)

- `useEffect([initialData, form.reset])` — use `form.reset` instead of `form`.
- `useEffect([watchedNameEn, isEditMode, form.setValue, form.getValues])`.

---

### 9. `app/layout.tsx` — Wrong company name in metadata [LOW]

`metadata.title` and `applicationName` still say **"شركة أكمي"** (the shadcn starter placeholder).
`messages.ar.ts` already defines `messages.common.companyName = "سبُل المستقبل"`.

#### [MODIFY] [layout.tsx](file:///home/ahmed/Desktop/INSS%20PROJECT/subul-Ecommerce/client/admin-panel/app/layout.tsx)

- Import `messages` and use `messages.common.companyName` for all three metadata fields.

> [!WARNING]
> `metadata` in Next.js root layout must be a plain object evaluated at build time.
> Since `messages.ar.ts` is a plain TS object (not async), a direct import works fine.

---

### 10. `messages.ar.ts` — Dead code at the bottom [LOW]

`statusLabels`, `typeLabels`, `translateStatus()`, and `translateType()` (lines 313–340) are
generic translations built for the boilerplate `DataTable` component on the dashboard.
They are not used by any category logic and will become stale as features evolve.

#### [MODIFY] [messages.ar.ts](file:///home/ahmed/Desktop/INSS%20PROJECT/subul-Ecommerce/client/admin-panel/lib/messages.ar.ts)

- Move these helpers into `components/data-table.tsx` or a co-located `data-table.messages.ts`
  file, since they belong to that component's domain, not the global message store.

---

### 11. `category-tree.tsx` — English aria-labels [LOW]

`aria-label="Collapse"` / `"Expand"` on the toggle button are English strings in an Arabic UI.

#### [MODIFY] [category-tree.tsx](file:///home/ahmed/Desktop/INSS%20PROJECT/subul-Ecommerce/client/admin-panel/features/category/components/category-tree.tsx)

- Add `treeExpand: "توسيع"` / `treeCollapse: "طي"` to `messages.category.listing` (or `table`).
- Use these in the `aria-label`.

*(Covered by change #1 above — combined in same file.)*

---

## Files Changed Summary

| File | Action |
|---|---|
| `features/category/components/category-tree.tsx` | Modify |
| `features/category/components/category-tables/index.tsx` | Modify |
| `features/category/components/category-tables/cell-action.tsx` | Modify |
| `features/category/components/category-listing-page.tsx` | Modify |
| `features/category/components/category-status-toggle.tsx` | Modify |
| `features/category/components/category-form.tsx` | Modify |
| `app/(routes)/categories/[id]/view/page.tsx` | Modify |
| `app/layout.tsx` | Modify |
| `lib/messages.ar.ts` | Modify (move dead code out) |
| `lib/search-params.ts` | **Delete** |
| `components/data-table.tsx` | Modify (receive moved translations) |

---

## Verification Plan

### After Each Change
- `npm run typecheck` — zero errors
- `npm run lint` — zero warnings on modified files
- `npm run format` — no unformatted output

### Manual Verification
- Visit `/categories` → table view: no double network call in DevTools Network tab
- Switch to tree view → tree renders in Arabic, `confirm()` is gone, `AlertDialog` appears
- Open `/categories/[id]/view` → Network tab shows one `GET /categories/:id`, not two
- Open `/categories/new` → form metadata says "سبُل المستقبل" in browser tab
- Toggle a status in the tree → no flicker, no double state
