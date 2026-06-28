# Admin Panel — Template

Next.js 16 admin panel template with RTL Arabic support, shadcn/ui, and a wired ASP.NET Core backend. The `features/category/` folder is the canonical pattern — clone it when adding a new feature.

---

## Stack

| Layer | Tech |
|---|---|
| Framework | Next.js 16 (App Router), React 19 |
| Language | TypeScript (strict) |
| Styling | Tailwind CSS v4, shadcn/ui (radix-nova), RTL |
| Server state | TanStack Query v5 |
| URL state | nuqs v2 |
| HTTP client | Axios → `NEXT_PUBLIC_API_URL` |
| Forms | react-hook-form + Zod |
| Notifications | Sonner |
| Command palette | kbar |
| Theming | next-themes (`d` key toggles dark/light) |
| Backend | ASP.NET Core 10 at `localhost:5101` |

---

## Getting started

```bash
cp .env.example .env.local   # fill in NEXT_PUBLIC_API_URL
npm install
npm run dev                  # http://localhost:3000 → redirects to /dashboard
```

---

## Project structure

```
admin-panel/
├── app/
│   ├── layout.tsx              # Root layout — fonts, metadata, providers
│   ├── page.tsx                # Redirects to /dashboard
│   ├── error.tsx               # Global error boundary
│   ├── not-found.tsx           # Custom 404 page
│   └── (routes)/
│       ├── layout.tsx          # Sidebar + header layout for all admin routes
│       ├── loading.tsx         # Route-level skeleton loader
│       ├── dashboard/          # Demo dashboard (charts, KPI cards)
│       └── categories/         # Category CRUD (real API)
│
├── components/
│   ├── app-providers.tsx       # Wraps QueryClient, Theme, RTL, KBar, Toaster
│   ├── app-sidebar.tsx         # Sidebar — reads nav from config/navigation.ts
│   ├── site-header.tsx         # Top header with sidebar trigger
│   ├── kbar/                   # Command palette UI
│   ├── layout/page-container.tsx
│   └── ui/                     # shadcn/ui primitives (do not edit manually)
│
├── config/
│   └── navigation.ts           # SINGLE source of truth for all nav items + KBar
│
├── features/
│   └── category/               # ← TEMPLATE for new features (see below)
│       ├── api/                # Axios calls
│       ├── hooks/              # useQuery + useMutation hooks
│       ├── components/         # Page, table, form
│       ├── schemas/            # Zod validation schemas
│       ├── types/              # TypeScript interfaces
│       └── constants/          # Shared constants (e.g. query keys)
│
├── lib/
│   ├── api-client.ts           # Axios instance (reads NEXT_PUBLIC_API_URL)
│   ├── messages.ar.ts          # All Arabic UI strings + i18n helpers
│   ├── search-params.ts        # Shared nuqs pagination/search params
│   └── utils.ts                # cn() helper
│
├── providers/
│   └── query-client-provider.tsx
│
├── hooks/
│   └── use-mobile.ts           # useIsMobile() — 768px breakpoint
│
└── types/
    └── api.ts                  # ApiResponse<T>, PaginatedResponse<T>
```

---

## Adding a new feature

1. Copy `features/category/` → `features/{entity}/`
2. Replace every occurrence of `category`/`Category` with your entity name
3. Update `features/{entity}/api/{entity}.api.ts` with the correct endpoints
4. Update `features/{entity}/types/index.ts` with your entity shape
5. Update `features/{entity}/schemas/{entity}.schema.ts` with Zod validation
6. Add route pages under `app/(routes)/{entities}/`
7. Add the new route to `config/navigation.ts` in `navMain` — it automatically appears in the sidebar and KBar

---

## Navigation

`config/navigation.ts` is the single source of truth:

- **`navMain`** — main sidebar links; items with a real `url` (not `"#"`) are auto-added to KBar
- **`navSecondary`** — secondary links at the bottom of the sidebar
- **`navDocuments`** — document-type links mid-sidebar
- **`kbarNavItems`** — derived automatically from `navMain` (no manual sync needed)

To add a new route to both sidebar and KBar: add one entry to `navMain`.

---

## Scripts

```bash
npm run dev        # Development server
npm run build      # Production build
npm run start      # Production server
npm run lint       # ESLint
npm run typecheck  # tsc --noEmit
npm run format     # Prettier (all .ts/.tsx)
```

---

## Adding shadcn/ui components

```bash
npx shadcn@latest add <component-name>
```

Components are placed in `components/ui/`.

---

## Environment variables

| Variable | Description |
|---|---|
| `NEXT_PUBLIC_API_URL` | Base URL for the ASP.NET Core API (e.g. `http://localhost:5101/api`) |

Copy `.env.example` to `.env.local` to get started.
