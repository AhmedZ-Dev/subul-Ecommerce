# 🛒 Subul-Ecommerce — Frontend Client Plan
## الخطة الشاملة لبناء الواجهة الأمامية (Admin Panel)

> **المرجع المستخدم:** warehouse-management-system/client (Next.js 15 + Tailwind v4 + shadcn + NextAuth v4)  
> **الهدف:** Admin Panel لإدارة المتجر الإلكتروني — ليس storefront للعملاء  
> **المسار:** `subul-Ecommerce/client/`

---

## 🧱 Tech Stack

| الأداة | الإصدار | الغرض |
|--------|---------|--------|
| Next.js | **16** (App Router) | Framework أساسي |
| React | **19** | UI Library |
| TypeScript | **^6** | Type Safety |
| Tailwind CSS | **v4 (latest)** | Styling |
| shadcn/ui | **latest (new-york style)** | Component Library |
| NextAuth.js | **v4** | Authentication |
| TanStack Query | **v5** | Server State Management |
| TanStack Table | **v8** | Data Tables |
| React Hook Form | **v7** | Form Management |
| Zod | **v3** | Schema Validation |
| Axios | **v1** | HTTP Client |
| Zustand | **v5** | Client State Management |
| Sonner | **latest** | Toast Notifications |
| Lucide React | **latest** | Icons |
| nuqs | **v2** | URL Search Params |
| date-fns | **v4** | Date Formatting |
| next-themes | **latest** | Dark/Light Mode |
| nextjs-toploader | **latest** | Page Loading Bar |
| motion | **v11** | Animations |

---

## 🔄 الفروقات عن مشروع warehouse-management-system

### ✅ ما يُحتفظ به من نفس النمط
- Feature-based architecture (نفس البنية بالضبط)
- `src/` directory structure مع نفس المجلدات
- App Router + Route Groups `(auth)` و `(routes)`
- Server Actions لـ CRUD operations
- shadcn/ui `new-york` style + `zinc` base color
- NextAuth v4 مع JWT strategy
- Axios `apiClient` مع interceptor يضيف Bearer token
- `middleware.ts` لحماية المسارات
- TanStack Table للجداول مع pagination
- `nuqs` لإدارة search params في URL
- `sonner` للـ toasts
- Dark/light mode via `next-themes`
- `zod` + `react-hook-form` للـ forms

### ❌ ما يُحذف (غير موجود في subul)

| العنصر | السبب |
|--------|--------|
| Prisma / `@prisma/client` | الـ Backend هو ASP.NET Core — لا Prisma |
| `prisma/` directory | لا يوجد |
| `pg` / `@prisma/adapter-pg` | لا DB مباشر من Next.js |
| `argon2` / `bcryptjs` | لا hash في Frontend |
| `crypto-js` / `jsonwebtoken` | لا signing في Frontend |
| `kbar` (command palette) | اختياري — يُضاف لاحقاً إن احتيج |
| `@dnd-kit/*` | اختياري — يُضاف للـ banners/collections لاحقاً |
| SSO credentials provider | subul يستخدم credentials provider عادي (email/password) |
| `csrf-protection.ts` | مبسّط — لا CSRF في هذه المرحلة |
| `rate-limit.ts` / `rate-limit-config.ts` | يُعالَج في Backend |
| `scripts/security-check.js` | اختياري |
| `husky` / `lint-staged` | اختياري (يُضاف لاحقاً) |
| `@fontsource/cairo` | نستخدم Google Fonts مباشرة عبر next/font |

### 🔧 ما يتغير (تعديل وليس حذف)

| العنصر | في WMS | في Subul |
|--------|--------|---------|
| `auth-options.ts` | SSO provider + custom fields | Credentials provider (email/password → backend JWT) |
| `next-auth.d.ts` | `UserRole` من Prisma | role من backend: `SUPER_ADMIN`, `ADMIN`, `MANAGER` |
| `api-client.ts` | basePath logic معقد | بسيط — `NEXT_PUBLIC_API_URL` |
| `middleware.ts` | multi-role redirect + CSRF | auth guard فقط (token → protected routes) |
| `next.config.ts` | basePath + Prisma transpile | بدون basePath — أبسط |
| `globals.css` | كاملة | تُعدّل للـ Ecommerce theme (ألوان subul) |

### ➕ ما يُضاف (غير موجود في WMS)

| العنصر | الغرض |
|--------|--------|
| `src/features/products/` | إدارة المنتجات (الأهم) |
| `src/features/orders/` | إدارة الطلبات |
| `src/features/brands/` | إدارة البراندات |
| `src/features/banners/` | بنرات الصفحة الرئيسية |
| `src/features/delivery-agents/` | مندوبو التوصيل |
| `src/features/settings/` | إعدادات المتجر |
| `src/features/shipping/` | مناطق الشحن والتسعيرة |
| `src/features/dashboard/` | KPIs + charts |
| Bilingual fields (AR/EN) | nameAr + nameEn في كل entity |
| RTL layout | `dir="rtl"` على مستوى الـ HTML |
| Cairo font | خط عربي احترافي |

---

## 📁 هيكل المجلدات الكامل

```
client/
├── .eslintrc.json
├── .gitignore
├── .prettierrc
├── components.json                    ← shadcn config
├── env.example.txt
├── next.config.ts
├── package.json
├── postcss.config.mjs
├── tsconfig.json
│
├── public/
│   └── logo.svg
│
└── src/
    ├── middleware.ts                  ← Auth guard
    │
    ├── app/
    │   ├── globals.css               ← Tailwind v4 + CSS Variables
    │   ├── layout.tsx                ← Root layout (providers)
    │   ├── not-found.tsx
    │   ├── page.tsx                  ← Redirect to /dashboard
    │   │
    │   ├── (auth)/
    │   │   ├── layout.tsx            ← Auth pages layout (centered)
    │   │   └── login/
    │   │       └── page.tsx
    │   │
    │   ├── (routes)/
    │   │   ├── layout.tsx            ← Dashboard layout (sidebar + header)
    │   │   │
    │   │   ├── dashboard/
    │   │   │   └── page.tsx          ← Overview / KPIs
    │   │   │
    │   │   ├── categories/
    │   │   │   ├── page.tsx          ← List
    │   │   │   ├── new/page.tsx      ← Create form
    │   │   │   └── [id]/
    │   │   │       └── edit/page.tsx ← Edit form
    │   │   │
    │   │   ├── brands/
    │   │   │   ├── page.tsx
    │   │   │   ├── new/page.tsx
    │   │   │   └── [id]/edit/page.tsx
    │   │   │
    │   │   ├── products/
    │   │   │   ├── page.tsx
    │   │   │   ├── new/page.tsx
    │   │   │   └── [id]/
    │   │   │       ├── page.tsx      ← Product detail
    │   │   │       └── edit/page.tsx
    │   │   │
    │   │   ├── orders/
    │   │   │   ├── page.tsx          ← All orders
    │   │   │   └── [id]/page.tsx     ← Order detail + status update
    │   │   │
    │   │   ├── users/
    │   │   │   ├── page.tsx
    │   │   │   └── [id]/page.tsx
    │   │   │
    │   │   ├── admin-users/
    │   │   │   ├── page.tsx
    │   │   │   ├── new/page.tsx
    │   │   │   └── [id]/edit/page.tsx
    │   │   │
    │   │   ├── delivery-agents/
    │   │   │   ├── page.tsx
    │   │   │   ├── new/page.tsx
    │   │   │   └── [id]/edit/page.tsx
    │   │   │
    │   │   ├── shipping/
    │   │   │   ├── zones/page.tsx
    │   │   │   └── rates/page.tsx
    │   │   │
    │   │   ├── banners/
    │   │   │   └── page.tsx
    │   │   │
    │   │   ├── settings/
    │   │   │   └── page.tsx
    │   │   │
    │   │   └── profile/
    │   │       └── page.tsx
    │   │
    │   └── api/
    │       └── auth/
    │           └── [...nextauth]/
    │               └── route.ts      ← NextAuth handler
    │
    ├── components/
    │   ├── ui/                       ← shadcn generated components
    │   │   ├── button.tsx
    │   │   ├── card.tsx
    │   │   ├── dialog.tsx
    │   │   ├── form.tsx
    │   │   ├── input.tsx
    │   │   ├── select.tsx
    │   │   ├── table.tsx
    │   │   ├── badge.tsx
    │   │   ├── skeleton.tsx
    │   │   ├── sonner.tsx
    │   │   ├── sidebar.tsx
    │   │   ├── sheet.tsx
    │   │   ├── dropdown-menu.tsx
    │   │   ├── separator.tsx
    │   │   ├── tooltip.tsx
    │   │   └── ...
    │   │
    │   ├── layout/
    │   │   ├── providers.tsx         ← Wraps all providers
    │   │   ├── app-sidebar.tsx       ← Main sidebar
    │   │   ├── header.tsx            ← Top header + breadcrumb
    │   │   ├── nav-main.tsx          ← Sidebar navigation items
    │   │   └── nav-user.tsx          ← User menu (logout, profile)
    │   │
    │   ├── breadcrumbs.tsx
    │   ├── data-table/
    │   │   ├── data-table.tsx        ← Reusable TanStack Table
    │   │   ├── data-table-toolbar.tsx
    │   │   ├── data-table-pagination.tsx
    │   │   └── data-table-column-header.tsx
    │   │
    │   ├── file-uploader.tsx         ← Image upload component
    │   ├── search-input.tsx
    │   ├── modal/
    │   │   ├── delete-confirm-modal.tsx
    │   │   └── alert-modal.tsx
    │   └── spinner.tsx
    │
    ├── features/
    │   │
    │   ├── categories/               ← TEMPLATE (mirror backend CategoryFeature)
    │   │   ├── index.ts              ← Barrel exports
    │   │   ├── components/
    │   │   │   ├── category-listing.tsx
    │   │   │   ├── category-view-page.tsx
    │   │   │   ├── category-form.tsx
    │   │   │   └── category-table/
    │   │   │       ├── index.tsx
    │   │   │       └── columns.tsx
    │   │   ├── services/
    │   │   │   └── category.service.ts
    │   │   ├── types/
    │   │   │   └── index.ts
    │   │   └── utils/
    │   │       └── validators.ts
    │   │
    │   ├── brands/
    │   │   ├── index.ts
    │   │   ├── components/
    │   │   │   ├── brand-listing.tsx
    │   │   │   ├── brand-view-page.tsx
    │   │   │   ├── brand-form.tsx
    │   │   │   └── brand-table/
    │   │   │       ├── index.tsx
    │   │   │       └── columns.tsx
    │   │   ├── services/
    │   │   │   └── brand.service.ts
    │   │   ├── types/
    │   │   │   └── index.ts
    │   │   └── utils/
    │   │       └── validators.ts
    │   │
    │   ├── products/
    │   │   ├── index.ts
    │   │   ├── components/
    │   │   │   ├── product-listing.tsx
    │   │   │   ├── product-view-page.tsx
    │   │   │   ├── product-form.tsx
    │   │   │   ├── product-images-uploader.tsx
    │   │   │   ├── product-variants-section.tsx
    │   │   │   ├── product-attributes-section.tsx
    │   │   │   └── product-table/
    │   │   │       ├── index.tsx
    │   │   │       └── columns.tsx
    │   │   ├── services/
    │   │   │   └── product.service.ts
    │   │   ├── types/
    │   │   │   └── index.ts
    │   │   └── utils/
    │   │       └── validators.ts
    │   │
    │   ├── orders/
    │   │   ├── index.ts
    │   │   ├── components/
    │   │   │   ├── order-listing.tsx
    │   │   │   ├── order-detail-page.tsx
    │   │   │   ├── order-status-badge.tsx
    │   │   │   ├── order-timeline.tsx
    │   │   │   └── order-table/
    │   │   │       ├── index.tsx
    │   │   │       └── columns.tsx
    │   │   ├── services/
    │   │   │   └── order.service.ts
    │   │   ├── types/
    │   │   │   └── index.ts
    │   │   └── utils/
    │   │       └── validators.ts
    │   │
    │   ├── users/
    │   │   ├── index.ts
    │   │   ├── components/
    │   │   │   ├── user-listing.tsx
    │   │   │   ├── user-detail-page.tsx
    │   │   │   └── user-table/
    │   │   │       ├── index.tsx
    │   │   │       └── columns.tsx
    │   │   ├── services/
    │   │   │   └── user.service.ts
    │   │   └── types/
    │   │       └── index.ts
    │   │
    │   ├── admin-users/
    │   │   ├── index.ts
    │   │   ├── components/
    │   │   │   ├── admin-user-listing.tsx
    │   │   │   ├── admin-user-view-page.tsx
    │   │   │   ├── admin-user-form.tsx
    │   │   │   └── admin-user-table/
    │   │   │       ├── index.tsx
    │   │   │       └── columns.tsx
    │   │   ├── services/
    │   │   │   └── admin-user.service.ts
    │   │   ├── types/
    │   │   │   └── index.ts
    │   │   └── utils/
    │   │       └── validators.ts
    │   │
    │   ├── delivery-agents/
    │   │   ├── index.ts
    │   │   ├── components/
    │   │   │   ├── delivery-agent-listing.tsx
    │   │   │   ├── delivery-agent-view-page.tsx
    │   │   │   ├── delivery-agent-form.tsx
    │   │   │   └── delivery-agent-table/
    │   │   │       ├── index.tsx
    │   │   │       └── columns.tsx
    │   │   ├── services/
    │   │   │   └── delivery-agent.service.ts
    │   │   ├── types/
    │   │   │   └── index.ts
    │   │   └── utils/
    │   │       └── validators.ts
    │   │
    │   ├── shipping/
    │   │   ├── index.ts
    │   │   ├── components/
    │   │   │   ├── shipping-zone-listing.tsx
    │   │   │   ├── shipping-zone-form.tsx
    │   │   │   ├── shipping-rate-listing.tsx
    │   │   │   ├── shipping-rate-form.tsx
    │   │   │   └── shipping-table/
    │   │   │       └── columns.tsx
    │   │   ├── services/
    │   │   │   └── shipping.service.ts
    │   │   ├── types/
    │   │   │   └── index.ts
    │   │   └── utils/
    │   │       └── validators.ts
    │   │
    │   ├── banners/
    │   │   ├── index.ts
    │   │   ├── components/
    │   │   │   ├── banner-listing.tsx
    │   │   │   ├── banner-form.tsx
    │   │   │   └── banner-table/
    │   │   │       └── columns.tsx
    │   │   ├── services/
    │   │   │   └── banner.service.ts
    │   │   ├── types/
    │   │   │   └── index.ts
    │   │   └── utils/
    │   │       └── validators.ts
    │   │
    │   ├── settings/
    │   │   ├── index.ts
    │   │   ├── components/
    │   │   │   └── settings-form.tsx
    │   │   ├── services/
    │   │   │   └── settings.service.ts
    │   │   └── types/
    │   │       └── index.ts
    │   │
    │   ├── dashboard/
    │   │   ├── index.ts
    │   │   ├── components/
    │   │   │   ├── stats-cards.tsx
    │   │   │   ├── recent-orders-table.tsx
    │   │   │   └── sales-chart.tsx
    │   │   ├── services/
    │   │   │   └── dashboard.service.ts
    │   │   └── types/
    │   │       └── index.ts
    │   │
    │   └── profile/
    │       ├── index.ts
    │       ├── components/
    │       │   └── profile-form.tsx
    │       └── services/
    │           └── profile.service.ts
    │
    ├── hooks/
    │   ├── use-media-query.ts
    │   └── use-debounce.ts
    │
    ├── lib/
    │   ├── api-client.ts             ← Axios instance + Bearer token interceptor
    │   ├── auth-options.ts           ← NextAuth config (Credentials provider)
    │   ├── font.ts                   ← Google Fonts (Cairo + Inter)
    │   ├── format.ts                 ← formatPrice (IQD), formatDate
    │   ├── utils.ts                  ← cn(), formatBytes()
    │   ├── parsers.ts                ← nuqs parsers
    │   ├── searchparams.ts           ← shared search param definitions
    │   └── theme-provider.tsx
    │
    ├── providers/
    │   ├── auth-provider.tsx         ← SessionProvider wrapper
    │   └── query-client-provider.tsx ← TanStack Query wrapper
    │
    └── types/
        ├── index.ts                  ← Shared types (ApiResponse, Paginated)
        └── next-auth.d.ts            ← Session augmentation
```

---

## 🔌 API Contract Integration

### الـ API Response الموحّد (من backend ASP.NET Core)

```typescript
// src/types/index.ts

export type ApiResponse<T> = {
  success: boolean;
  message: string | null;
  data: T | null;
  errors: string[] | null;
};

export type Paginated<T> = {
  items: T[];
  total: number;
  page: number;
  limit: number;
  totalPages: number;
};
```

> **مهم:** دائماً تحقق من `success` قبل قراءة `data`.
> عند `success: false` → اعرض `message` أو أول عنصر في `errors[]`.

### Bilingual Support

كل entity فيها `nameEn` + `nameAr`. القاعدة:
- في الجداول: اعرض `nameAr` أولاً، ثم `nameEn` كـ subtitle
- في الـ forms: حقل لكل منهما
- Admin Panel بالعربي فقط (لا locale switching في هذه المرحلة)

---

## 🔐 Authentication Flow

### NextAuth Config (`src/lib/auth-options.ts`)

```typescript
// استخدام CredentialsProvider عادي — email + password
// يرسل credentials للـ backend API → يستقبل JWT token
// يخزّن token في JWT session

providers: [
  CredentialsProvider({
    credentials: {
      email: { type: 'email' },
      password: { type: 'password' }
    },
    async authorize(credentials) {
      const res = await fetch(`${process.env.NEXT_PUBLIC_API_URL}/auth/login`, {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({
          email: credentials.email,
          password: credentials.password
        })
      });
      const result = await res.json(); // ApiResponse<{ token, adminUser }>
      if (!result.success) return null;
      return {
        id: String(result.data.adminUser.id),
        email: result.data.adminUser.email,
        name: result.data.adminUser.name,
        role: result.data.adminUser.role,
        accessToken: result.data.token
      };
    }
  })
]
```

### Session Type (`src/types/next-auth.d.ts`)

```typescript
type AdminRole = 'SUPER_ADMIN' | 'ADMIN' | 'MANAGER';

declare module 'next-auth' {
  interface Session {
    accessToken?: string;
    role?: AdminRole;
    user: {
      id: string;
      email?: string | null;
      name?: string | null;
    };
  }
  interface User {
    id: string;
    accessToken?: string;
    role?: AdminRole;
  }
}

declare module 'next-auth/jwt' {
  interface JWT {
    accessToken?: string;
    role?: AdminRole;
    accessTokenExpires?: number;
  }
}
```

### Middleware (`src/middleware.ts`)

الفلسفة: بسيط — إذا لا يوجد token → redirect إلى /login.
لا role-based redirect في هذه المرحلة.

```typescript
import { getToken } from 'next-auth/jwt';
import { NextResponse } from 'next/server';

const PUBLIC_PATHS = ['/login', '/api/auth'];

export async function middleware(request) {
  const { pathname } = request.nextUrl;
  if (PUBLIC_PATHS.some((p) => pathname.startsWith(p))) {
    return NextResponse.next();
  }
  const token = await getToken({ req: request, secret: process.env.NEXTAUTH_SECRET });
  if (!token) {
    return NextResponse.redirect(new URL('/login', request.url));
  }
  return NextResponse.next();
}

export const config = {
  matcher: ['/((?!_next/static|_next/image|favicon.ico|public).*)'],
};
```

---

## 🎨 Design System

### Tailwind v4 CSS Variables (`src/app/globals.css`)

- Base color: **Zinc** (من shadcn)
- RTL: `dir="rtl"` على `<html>`
- Dark mode: class-based via `next-themes`

### Fonts (`src/lib/font.ts`)

```typescript
import { Cairo, Inter } from 'next/font/google';

export const cairo = Cairo({
  subsets: ['arabic', 'latin'],
  variable: '--font-cairo',
  display: 'swap',
});

export const inter = Inter({
  subsets: ['latin'],
  variable: '--font-inter',
  display: 'swap',
});

export const fontVariables = `${cairo.variable} ${inter.variable}`;
```

---

## 📋 Feature Pattern (Template لكل Entity)

### 1. Types (`types/index.ts`)

```typescript
// Raw API DTO (camelCase من backend)
export type CategoryDto = {
  id: number;                // Backend يرجع long → number في TS
  nameEn: string;
  nameAr: string;
  slug: string;
  parentId: number | null;
  parent: { id: number; nameEn: string; nameAr: string } | null;
  createdAt: string;         // ISO string → convert بـ date-fns
  _count: { products: number; children: number };
};

// Form input (ما يُرسل للـ backend)
export interface CategoryFormData {
  nameEn: string;
  nameAr: string;
  parentId?: number | null;
}

// Pagination query params
export interface CategoryQueryParams {
  page?: number;
  limit?: number;
  search?: string;
}
```

### 2. Service (`services/category.service.ts`)

```typescript
'use server';

import apiClient from '@/lib/api-client';
import { revalidatePath } from 'next/cache';
import type { ApiResponse, Paginated } from '@/types';
import type { CategoryDto, CategoryFormData, CategoryQueryParams } from '../types';

export async function getCategories(params: CategoryQueryParams) {
  const { data: res } = await apiClient.get<ApiResponse<Paginated<CategoryDto>>>(
    '/categories', { params }
  );
  if (!res.success) throw new Error(res.message ?? 'فشل تحميل الفئات');
  return res.data!;
}

export async function getCategoryById(id: number) {
  const { data: res } = await apiClient.get<ApiResponse<CategoryDto>>(`/categories/${id}`);
  if (!res.success) return null;
  return res.data;
}

export async function createCategory(data: CategoryFormData) {
  const { data: res } = await apiClient.post<ApiResponse<CategoryDto>>('/categories', data);
  if (!res.success) throw new Error(res.message ?? 'فشل إنشاء الفئة');
  revalidatePath('/categories');
  return res.data!;
}

export async function updateCategory(id: number, data: Partial<CategoryFormData>) {
  const { data: res } = await apiClient.put<ApiResponse<CategoryDto>>(`/categories/${id}`, data);
  if (!res.success) throw new Error(res.message ?? 'فشل تحديث الفئة');
  revalidatePath('/categories');
  revalidatePath(`/categories/${id}/edit`);
  return res.data!;
}

export async function deleteCategory(id: number) {
  const { data: res } = await apiClient.delete<ApiResponse<null>>(`/categories/${id}`);
  if (!res.success) throw new Error(res.message ?? 'فشل حذف الفئة');
  revalidatePath('/categories');
  return true;
}
```

### 3. Validators (`utils/validators.ts`)

```typescript
import { z } from 'zod';

export const categoryFormSchema = z.object({
  nameEn: z.string().min(1, 'اسم الفئة بالإنجليزي مطلوب').max(100),
  nameAr: z.string().min(1, 'اسم الفئة بالعربي مطلوب').max(100),
  parentId: z.number().nullable().optional(),
});

export type CategoryFormInput = z.infer<typeof categoryFormSchema>;
```

### 4. API Client (`src/lib/api-client.ts`)

```typescript
import axios from 'axios';

const apiClient = axios.create({
  baseURL: process.env.NEXT_PUBLIC_API_URL || 'http://localhost:5101/api',
});

apiClient.interceptors.request.use(async (config) => {
  let token: string | undefined;

  if (typeof window === 'undefined') {
    // Server side
    const { getServerSession } = await import('next-auth');
    const { default: authOptions } = await import('@/lib/auth-options');
    const session = await getServerSession(authOptions);
    token = session?.accessToken;
  } else {
    // Client side
    const res = await fetch('/api/auth/session');
    const session = await res.json();
    token = session?.accessToken;
  }

  if (token) {
    config.headers.Authorization = `Bearer ${token}`;
  }
  return config;
});

export default apiClient;
```

---

## 🧭 Navigation Structure (Sidebar)

```typescript
const navItems = [
  { title: 'لوحة التحكم', href: '/dashboard', icon: 'LayoutDashboard' },
  {
    title: 'المنتجات',
    icon: 'Package',
    children: [
      { title: 'قائمة المنتجات', href: '/products' },
      { title: 'الفئات', href: '/categories' },
      { title: 'البراندات', href: '/brands' },
    ]
  },
  { title: 'الطلبات', href: '/orders', icon: 'ShoppingCart' },
  { title: 'العملاء', href: '/users', icon: 'Users' },
  {
    title: 'التوصيل',
    icon: 'Truck',
    children: [
      { title: 'المندوبون', href: '/delivery-agents' },
      { title: 'مناطق الشحن', href: '/shipping/zones' },
      { title: 'تسعيرة الشحن', href: '/shipping/rates' },
    ]
  },
  {
    title: 'المحتوى',
    icon: 'Image',
    children: [
      { title: 'البنرات', href: '/banners' },
    ]
  },
  {
    title: 'الإدارة',
    icon: 'Shield',
    children: [
      { title: 'المدراء', href: '/admin-users' },
      { title: 'الإعدادات', href: '/settings' },
    ]
  }
];
```

---

## ⚙️ `package.json` — Dependencies المحددة

### Production Dependencies (الرئيسية)
```json
{
  "next": "^16.0.0",
  "react": "^19.0.0",
  "react-dom": "^19.0.0",
  "next-auth": "^4.24.11",
  "next-themes": "^0.4.6",
  "nextjs-toploader": "^3.7.15",
  "@tanstack/react-query": "^5.85.0",
  "@tanstack/react-table": "^8.21.2",
  "react-hook-form": "^7.54.1",
  "@hookform/resolvers": "^3.9.1",
  "zod": "^3.24.1",
  "axios": "^1.16.0",
  "zustand": "^5.0.2",
  "tailwindcss": "^4.0.0",
  "@tailwindcss/postcss": "^4.0.0",
  "tailwind-merge": "^3.0.2",
  "tailwindcss-animate": "^1.0.7",
  "class-variance-authority": "^0.7.1",
  "clsx": "^2.1.1",
  "lucide-react": "^0.476.0",
  "sonner": "^1.7.1",
  "nuqs": "^2.4.1",
  "date-fns": "^4.1.0",
  "motion": "^11.18.2",
  "recharts": "^2.15.1",
  "react-dropzone": "^14.3.5",
  "react-day-picker": "^8.10.1",
  "vaul": "^1.1.2",
  "cmdk": "^1.1.1",
  "input-otp": "^1.4.2"
}
```

### Dev Dependencies
```json
{
  "typescript": "^6.0.0",
  "@types/node": "^22.0.0",
  "@types/react": "^19.0.0",
  "@types/react-dom": "^19.0.0",
  "eslint": "^9.0.0",
  "eslint-config-next": "^16.0.0",
  "prettier": "^3.4.0",
  "prettier-plugin-tailwindcss": "^0.6.0"
}
```

---

## 🌍 Environment Variables

```bash
# .env.local
NEXTAUTH_SECRET=<generate: openssl rand -base64 32>
NEXTAUTH_URL=http://localhost:3000

NEXT_PUBLIC_API_URL=http://localhost:5101/api
```

> Backend يعمل على port `5101` (subul-Ecommerce backend).

---

## 📐 Naming Conventions

| Item | Pattern | Example |
|------|---------|---------|
| Feature folder | `kebab-case` | `delivery-agents/` |
| Component file | `{feature}-{type}.tsx` | `category-form.tsx` |
| Service file | `{feature}.service.ts` | `category.service.ts` |
| Types file | `index.ts` inside `types/` | `types/index.ts` |
| Validator file | `validators.ts` inside `utils/` | `utils/validators.ts` |
| Table columns file | `columns.tsx` | `category-table/columns.tsx` |
| Route segment | `kebab-case` | `/delivery-agents/[id]/edit` |
| Component class name | `{Feature}{Type}` | `CategoryForm`, `CategoryListing` |
| ID type from backend | `number` (backend `long`) | `id: number` |

---

## 🚫 Anti-Patterns

| لا تفعل | افعل بدلاً منه |
|--------|---------------|
| إضافة Prisma أو DB مباشر | Backend فقط — API calls عبر Axios |
| import من feature أخرى | كل feature مستقلة — shared → `components/ui/` |
| Duplicate backend validation | Zod للشكل فقط — business rules في Backend |
| قراءة `id` كـ string من Backend | Backend يرجع `long` → `number` في TS |
| إخفاء `nameAr` في الجداول | اعرض دائماً كلاهما (nameAr + nameEn) |
| hardcode الـ API URL | دائماً `process.env.NEXT_PUBLIC_API_URL` |
| إضافة storefront pages | هذا Admin Panel فقط — لا `/shop` أو `/cart` |
| استخدام `DateTime.UtcNow` patterns | `date-fns` للـ formatting فقط |
| بناء Frontend قبل backend API | API first — تأكد من endpoint قبل البناء |

---

## 🚀 MVP Build Order

### Phase 1 — Foundation (الأساس)
1. إنشاء المشروع: `npx create-next-app@latest ./` (App Router + TypeScript + Tailwind)
2. تثبيت الـ dependencies
3. إعداد shadcn: `npx shadcn@latest init` (new-york style, zinc, RSC: true)
4. `globals.css` — Tailwind v4 + CSS variables + RTL
5. `src/lib/font.ts` — Cairo + Inter
6. `src/types/index.ts` — ApiResponse + Paginated
7. `src/types/next-auth.d.ts` — Session augmentation
8. `src/lib/api-client.ts` — Axios instance
9. `src/lib/auth-options.ts` — NextAuth CredentialsProvider
10. `src/app/api/auth/[...nextauth]/route.ts`
11. `src/middleware.ts` — Auth guard
12. `src/providers/` — auth-provider + query-client-provider

### Phase 2 — Layout (الهيكل)
13. `src/components/layout/providers.tsx`
14. `src/app/layout.tsx` — Root layout
15. `src/app/(auth)/login/page.tsx` — Login page
16. `src/components/layout/app-sidebar.tsx`
17. `src/components/layout/header.tsx`
18. `src/app/(routes)/layout.tsx` — Dashboard layout

### Phase 3 — First Feature (Template)
19. `src/features/categories/` — كامل (template للباقي)
20. صفحات `src/app/(routes)/categories/`

### Phase 4 — Remaining CRUD Features
21. `brands/`
22. `products/` (الأكثر تعقيداً — images, variants, attributes)
23. `orders/` (read + status update)
24. `users/` (read-only)
25. `admin-users/`
26. `delivery-agents/`
27. `shipping/`
28. `banners/`

### Phase 5 — Dashboard & Polish
29. `dashboard/` — stats + charts (recharts)
30. `settings/`
31. `profile/`
32. Loading skeletons
33. Error boundaries
34. Dark mode polish

---

## ⚠️ ملاحظات مهمة

> **Next.js 16:** قد يكون في مرحلة canary/RC. استخدم `next@latest` أو `next@canary`. إذا لم يكن stable، استخدم Next.js 15.x (App Router) حتى الاستقرار — البنية متطابقة.

> **NextAuth v4:** مستقر ومجرَّب في WMS. اختيار آمن. لا تنتقل لـ v5 (Auth.js) الآن.

> **Backend IDs:** الـ backend يستخدم `long` (C#) → في TypeScript اعتبره `number`. لا تستخدم `string` للـ IDs.

> **ابدأ بـ `categories`** كـ template. بعد اكتمالها، انسخها لكل entity وعدّل الـ types فقط.

> **لا تربط `client/` بـ `backend/` عبر filesystem.** التواصل الوحيد هو HTTP عبر `NEXT_PUBLIC_API_URL`.
