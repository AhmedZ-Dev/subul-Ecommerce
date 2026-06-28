# Architecture Decision: Admin Panel vs Storefront
## Why Two Separate Next.js Projects

---

## My Recommendation: Two Separate Next.js Projects

But first, let me show you *why*, based on what I actually found in your code.

---

## What Your Architecture Tells Me

### 1. You Have Two Completely Different User Types

```
backend/Domain/Entities/

AdminUser.cs          ← name, email, passwordHash, role, isActive
                        relations: ActivityLogs, CashCollections,
                                   OrderDeliveries, PurchaseOrders...

User.cs               ← email, passwordHash, firstName, lastName,
                        phone, storeCredit, acceptsMarketing
                        relations: Carts, Orders, Wishlists,
                                   Reviews, Notifications...
```

These are **two separate database tables** with **completely different relationships**. Sharing a single NextAuth session between them in one project would be a nightmare — you'd be constantly checking `session.type === 'admin' || 'customer'` everywhere.

---

### 2. Your Backend Already Separates the Concerns

Looking at your features built so far:

```
Features/
├── CategoryFeature/        ← admin writes, storefront reads
├── BrandFeature/           ← admin writes, storefront reads
├── ProductFeature/         ← admin writes, storefront reads
│                              (ListProductPaginated already filters by
│                               Status, IsFeatured, CategoryId...)
├── OrderFeature/
│   ├── CreateOrder/        ← storefront customer action
│   ├── UpdateOrder/        ← admin action (change status)
│   ├── TrackGuestOrder/    ← storefront public (no auth needed)
│   └── ListOrderPaginated/ ← admin view
├── CartFeature/            ← storefront only
└── ShippingZoneFeature/    ← admin manages, storefront reads for checkout
```

The backend is already naturally split between admin actions and customer actions. Your frontend should mirror that.

---

### 3. Your `ListProductPaginated` Handler Already Serves Both

```csharp
// It filters by: Status, IsFeatured, CategoryId, BrandId, Search
// Admin uses it with status filter (show all statuses)
// Storefront uses it with status=active filter only
```

**One API, two consumers.** This is the correct design.

---

## The Verdict: Two Projects

```
subul-Ecommerce/
│
├── backend/              ✅ Already built — ASP.NET Core on :5101
│
├── admin-panel/          ← Next.js 16 on :3001
│   └── (what FRONTEND_PLAN.md describes)
│
└── storefront/           ← Next.js 16 on :3000
    └── (separate project)
```

---

## Why NOT One Project (The Real Reasons for YOUR Case)

| Problem | Why it matters for subul |
|---------|--------------------------|
| **Auth collision** | `AdminUser` and `User` are different tables. In one project, NextAuth has one session type — you'd need hacks like `session.userType` everywhere |
| **`/api/products` serves two audiences** | Admin needs all statuses + cost price + stock details. Customer needs only `status=active` + no cost price. One project means one `features/products/` — which version do you build? |
| **Security exposure** | Admin panel should never be reachable from a public URL. Storefront is public. Mixing them in one project means one wrong middleware config exposes `/admin` |
| **SEO is critical for storefront** | Product pages need `generateMetadata()`, structured data, sitemap, og:tags. Admin never needs any of this. Mixing them pollutes both |
| **Bundle size** | Admin uses heavy table libraries (TanStack Table), rich form editors, charts. Customers shouldn't download any of that |
| **Order feature has 7 ops** | `CreateOrder` is storefront. `UpdateOrder` is admin. `TrackGuestOrder` is public. Different auth, different context — one project means complex route group logic |

---

## Exact Folder Structure to Use

```
subul-Ecommerce/
├── backend/                        ✅ exists
│
├── admin-panel/                    ← Build this FIRST
│   ├── src/
│   │   ├── app/
│   │   │   ├── (auth)/login/       ← Calls POST /api/auth/admin/login
│   │   │   └── (routes)/           ← Protected: dashboard, products, orders...
│   │   ├── features/
│   │   │   ├── categories/
│   │   │   ├── brands/
│   │   │   ├── products/           ← Shows ALL statuses, cost price, stock mgmt
│   │   │   ├── orders/             ← UpdateOrder (status change), assign delivery
│   │   │   ├── delivery-agents/
│   │   │   ├── shipping/
│   │   │   └── banners/
│   │   └── middleware.ts           ← Guards all (routes)/ with AdminUser JWT
│   └── .env.local                  ← NEXT_PUBLIC_API_URL=http://localhost:5101/api
│
└── storefront/                     ← Build this SECOND (after backend is ready)
    ├── src/
    │   ├── app/
    │   │   ├── (public)/           ← No auth required
    │   │   │   ├── page.tsx        ← Homepage: banners, featured products
    │   │   │   ├── products/
    │   │   │   │   ├── page.tsx    ← Calls GET /api/products?status=active
    │   │   │   │   └── [slug]/     ← Product detail page (SEO critical)
    │   │   │   ├── categories/[slug]/
    │   │   │   ├── brands/[slug]/
    │   │   │   └── track/page.tsx  ← Calls GET /api/orders/track (TrackGuestOrder)
    │   │   │
    │   │   └── (account)/          ← Protected: Customer JWT
    │   │       ├── cart/
    │   │       ├── checkout/       ← Calls POST /api/orders (CreateOrder)
    │   │       ├── orders/         ← Calls GET /api/orders?userId=...
    │   │       ├── wishlist/
    │   │       └── profile/
    │   │
    │   ├── features/
    │   │   ├── products/           ← Shows ONLY active products, no cost price
    │   │   ├── cart/               ← Local state + sync with API
    │   │   ├── checkout/
    │   │   ├── customer-orders/    ← Customer's own orders only
    │   │   └── auth/               ← register, login (User table)
    │   │
    │   └── middleware.ts           ← Guards /account only
    └── .env.local                  ← NEXT_PUBLIC_API_URL=http://localhost:5101/api
```

---

## The Shared `features/products/` Problem — Solved

This is the most concrete reason to split. Look at what each side needs from your `Product` entity:

| Field | Admin Panel | Storefront |
|-------|------------|------------|
| `nameEn`, `nameAr`, `slug` | ✅ | ✅ |
| `price`, `compareAtPrice` | ✅ | ✅ |
| `costPrice` | ✅ (profit margin) | ❌ **NEVER** |
| `status` (all values) | ✅ (draft, inactive) | ❌ only `active` |
| `stockQuantity` | ✅ (exact number) | ✅ (show "In Stock" / "Low Stock") |
| `lowStockThreshold` | ✅ | ❌ |
| `totalSold`, `viewsCount` | ✅ (analytics) | ❌ |
| Form: create/edit | ✅ full form | ❌ read-only |
| Bulk actions | ✅ | ❌ |
| SEO meta tags for page | ❌ | ✅ critical |
| Structured data (JSON-LD) | ❌ | ✅ |

Two totally different feature implementations. Sharing a folder would mean constant `if (isAdmin)` checks everywhere.

---

## Build Order

```
Phase 1:  backend/          ✅ in progress
Phase 2:  admin-panel/      ← start now (matches FRONTEND_PLAN.md exactly)
Phase 3:  storefront/       ← start after admin + backend are stable
```

**Don't build both simultaneously.** Get the admin panel working so you can manage your data, then build the storefront that displays it.

---

## Bottom Line

Your own backend entities (`AdminUser` vs `User`, `UpdateOrder` vs `CreateOrder`,
`TrackGuestOrder` as public) are already telling you the answer — **two separate projects**.

- `FRONTEND_PLAN.md` → covers `admin-panel/` completely
- When ready for the storefront → a fresh plan will be created for it

| Project | Port | Auth Table | Next.js Route Group | Deployment |
|---------|------|------------|---------------------|------------|
| `admin-panel/` | 3001 | `admin_users` | `(routes)/` protected | Internal / VPN |
| `storefront/` | 3000 | `users` | `(account)/` protected, `(public)/` open | Public internet |
| `backend/` | 5101 | both | — | Server |
