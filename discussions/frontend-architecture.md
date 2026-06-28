# Feature-Based Frontend Architecture — Subul E-commerce
### Senior Frontend Architect Reference (Mid-2026)

> **Stack:** Next.js 15 (App Router) · TypeScript · Shadcn/UI · Tailwind CSS · React Query v5 · Zod · Axios

---

## Table of Contents

1. [Architectural Philosophy](#1-architectural-philosophy)
2. [Comparison With Your Existing Codebase](#2-comparison-with-your-existing-codebase)
3. [Root Folder Structure](#3-root-folder-structure)
4. [Feature Internal Structure Contract](#4-feature-internal-structure-contract)
5. [ProductFeature — Full Real Example](#5-productfeature--full-real-example)
6. [App Router Pages](#6-app-router-pages)
7. [CategoryFeature — Parallel Example](#7-categoryfeature--parallel-example)
8. [Shared Layer](#8-shared-layer)
9. [Feature vs. Shared Decision Matrix](#9-feature-vs-shared-decision-matrix)
10. [Inter-Feature Communication](#10-inter-feature-communication)
11. [Data Flow Diagram](#11-data-flow-diagram)
12. [Migration Guide for Your Existing Project](#12-migration-guide-for-your-existing-project)

---

## 1. Architectural Philosophy

### Why Feature-Based?

In a team working on multiple domain entities simultaneously, **feature-based architecture** co-locates every concern of a feature in one folder. The rule is simple:

> **"If it only exists for this feature, it lives inside this feature."**

This eliminates the classic problem of a global `services/`, `hooks/`, `types/` folder that grows to 60+ files with no clear ownership.

### Core Principles

| Principle | Meaning |
|-----------|---------|
| **Vertical slicing** | Each feature is a self-contained vertical slice: API → hooks → UI |
| **Colocation** | Types, validators, hooks, and components sit next to the code that uses them |
| **Single public surface** | Each feature exports only through its `index.ts` barrel |
| **Shared = truly global** | Code is moved to `shared/` only when 3+ features need it |
| **Server ↔ Client boundary** | RSCs fetch, Client Components mutate via React Query |
| **Zero cross-feature imports** | `ProductFeature` never imports from `CategoryFeature` directly |

### Alignment With Backend

Your backend uses `ProductFeature/CreateProduct/`, `ProductFeature/UpdateProduct/`, etc.
The frontend mirrors this:

```
backend: Features/ProductFeature/CreateProduct/CreateProductCommand.cs
frontend: features/product/api/product.api.ts        → createProduct()
frontend: features/product/hooks/useProductMutations.ts → useCreateProduct()
frontend: features/product/schemas/product.schema.ts    → createProductSchema
frontend: features/product/components/ProductForm.tsx
```

---

## 2. Comparison With Your Existing Codebase

Your existing project at `warehouse-management-system/client/src/` already has excellent instincts. Here is an honest, file-by-file comparison.

### Root `src/` — Side by Side

| Folder | Your WMS Project | Recommended Architecture | Verdict |
|--------|-----------------|--------------------------|---------|
| `app/` | ✅ App Router with `(auth)`, `(routes)` | Same | ✅ Keep |
| `features/` | ✅ Exists with feature subfolders | Same | ✅ Keep |
| `components/` | ✅ `ui/`, layout, shared UI | Same | ✅ Keep |
| `lib/` | ✅ api-client, utils, auth | Same | ✅ Keep |
| `hooks/` | ⚠️ Generic hooks only, no React Query | Add React Query hooks **inside features** | 🔧 Refactor |
| `types/` | ⚠️ Only global nav/auth types | Feature types belong inside features | ✅ Correct (already in features) |
| `constants/` | ✅ Exists | Feature constants belong inside features | ✅ Good |
| `providers/` | ✅ Exists | Same — QueryClientProvider, ThemeProvider | ✅ Keep |
| `config/` | ✅ Exists | Same | ✅ Keep |

### Feature Folder — Side by Side

| Sub-folder | Your WMS (`category/`) | Recommended (`product/`) | Verdict |
|------------|------------------------|--------------------------|---------|
| `components/` | ✅ `category-form.tsx`, `category-listing.tsx` | Same naming convention | ✅ Keep |
| `services/` | ⚠️ `category.service.ts` with `'use server'` | Rename to `api/` | 🔧 Rename |
| `types/` | ✅ `index.ts` with interfaces | Same | ✅ Keep |
| `utils/` | ✅ `index.ts` + `validators.ts` | Split into `utils/` + `schemas/` | 🔧 Split |
| `hooks/` | ❌ Missing entirely | Add `hooks/useProduct.ts` + `hooks/useProductMutations.ts` | ❌ Add |
| `constants/` | ❌ Missing | Add `constants/index.ts` | ❌ Add |
| `index.ts` | ✅ Barrel file exists | Same | ✅ Keep |

### The Biggest Gap: No React Query Hooks Inside Features

Your current `category-form.tsx` calls the server action **directly**:

```tsx
// ❌ Current: Direct server action — manual loading state, no cache
const handleSubmit = async (values: CategoryFormInput) => {
  setIsSubmitting(true);           // manual loading state
  await updateCategory(id, data);  // no caching, no invalidation
  router.refresh();                // manual cache bust
};
```

The recommended pattern:

```tsx
// ✅ Recommended: React Query mutation — automatic everything
const { mutate: updateProduct, isPending } = useUpdateProduct();
const handleSubmit = (values: ProductFormInput) => {
  updateProduct({ id, data: values }); // loading, error, cache invalidation: automatic
};
```

Benefits: automatic loading states · optimistic updates · automatic cache invalidation · retry logic · React Query Devtools · zero `useState(false)` boilerplate.

### The `services/` vs `api/` Naming

Your `category.service.ts` mixes concerns:

```ts
'use server'; // ← Next.js Server Action
// BUT also calls apiClient + does data mapping
```

**Recommended separation:**

| File | Concern | Runtime |
|------|---------|---------|
| `api/product.api.ts` | Pure HTTP calls — no React, no Next.js | Client + Server |
| `actions/product.actions.ts` | Next.js Server Actions — `revalidatePath` | Server only |
| `hooks/useProduct.ts` | React Query queries | Client |
| `hooks/useProductMutations.ts` | React Query mutations | Client |

---

## 3. Root Folder Structure

```
src/
├── app/                          # Next.js App Router (routing only)
│   ├── (auth)/
│   │   ├── login/page.tsx
│   │   └── layout.tsx
│   ├── (routes)/
│   │   ├── layout.tsx            # dashboard shell
│   │   ├── page.tsx              # redirect to /dashboard
│   │   ├── products/
│   │   │   ├── page.tsx          # list — RSC
│   │   │   ├── new/page.tsx      # create — RSC shell
│   │   │   └── [id]/
│   │   │       ├── page.tsx      # detail — RSC
│   │   │       └── edit/page.tsx # edit — RSC shell
│   │   └── categories/
│   │       ├── page.tsx
│   │       ├── new/page.tsx
│   │       └── [id]/edit/page.tsx
│   ├── api/auth/[...nextauth]/route.ts
│   ├── globals.css
│   ├── layout.tsx
│   └── not-found.tsx
│
├── features/                     # ← Domain features (one per backend feature)
│   ├── product/                  # ProductFeature
│   │   ├── api/product.api.ts
│   │   ├── hooks/
│   │   │   ├── useProduct.ts
│   │   │   └── useProductMutations.ts
│   │   ├── schemas/product.schema.ts
│   │   ├── components/
│   │   │   ├── ProductForm.tsx
│   │   │   ├── ProductTable.tsx
│   │   │   ├── ProductCard.tsx
│   │   │   ├── ProductListingPage.tsx
│   │   │   ├── ProductViewPage.tsx
│   │   │   └── index.ts
│   │   ├── types/index.ts
│   │   ├── constants/index.ts
│   │   ├── utils/index.ts
│   │   └── index.ts              # public barrel
│   │
│   └── category/                 # CategoryFeature
│       ├── api/category.api.ts
│       ├── hooks/
│       │   ├── useCategory.ts
│       │   └── useCategoryMutations.ts
│       ├── schemas/category.schema.ts
│       ├── components/
│       │   ├── CategoryForm.tsx
│       │   ├── CategoryTable.tsx
│       │   ├── CategoryTree.tsx
│       │   ├── CategoryListingPage.tsx
│       │   └── index.ts
│       ├── types/index.ts
│       ├── constants/index.ts
│       ├── utils/index.ts
│       └── index.ts
│
├── components/                   # Shared UI (no domain logic)
│   ├── ui/                       # Shadcn primitives
│   ├── layout/                   # Shell, sidebar, header
│   ├── data-table/               # Generic table primitives
│   └── icons.tsx
│
├── lib/                          # Shared infrastructure
│   ├── api-client.ts             # Axios instance
│   ├── query-client.ts           # React Query client factory
│   ├── utils.ts                  # cn(), formatters
│   ├── auth-options.ts
│   └── searchparams.ts
│
├── hooks/                        # Shared hooks (not feature-specific)
│   ├── use-debounce.ts
│   ├── use-media-query.ts
│   └── use-data-table.ts
│
├── types/                        # Global/shared types only
│   ├── api.ts                    # ApiResponse<T>, PaginatedResponse<T>
│   ├── next-auth.d.ts
│   └── index.ts
│
├── providers/
│   ├── QueryProvider.tsx
│   └── ThemeProvider.tsx
│
├── constants/index.ts            # Global constants
├── config/nav.ts                 # App configuration
└── middleware.ts
```

---

## 4. Feature Internal Structure Contract

Every feature follows this **identical** shape. This is the contract that makes the architecture scale — any developer can navigate any feature immediately.

```
features/{entity}/
├── api/                   ← Pure HTTP functions (no React, no Next.js)
│   └── {entity}.api.ts
├── hooks/                 ← React Query wrappers (Client only)
│   ├── use{Entity}.ts          (queries)
│   └── use{Entity}Mutations.ts (mutations)
├── schemas/               ← Zod schemas + inferred TypeScript types for forms
│   └── {entity}.schema.ts
├── components/            ← All React components for this feature
│   ├── {Entity}Form.tsx
│   ├── {Entity}Table.tsx
│   ├── {Entity}ListingPage.tsx
│   ├── {Entity}ViewPage.tsx
│   └── index.ts
├── types/                 ← DTO types from backend response shapes
│   └── index.ts
├── constants/             ← Feature-specific enums and static data
│   └── index.ts
├── utils/                 ← Pure functions specific to this feature
│   └── index.ts
└── index.ts               ← Public barrel: ONLY way to import from this feature
```

---

## 5. ProductFeature — Full Real Example

### 5.1 `types/index.ts`

**Why it exists:** Mirrors the backend `ProductDto` and command shapes. These are **wire format** types — not form types.

```ts
// features/product/types/index.ts

export type ProductStatus = 'active' | 'inactive' | 'draft' | 'archived';
export type ProductCurrency = 'SAR' | 'USD' | 'EUR';

// Mirrors: ProductFeature/GetByIdProduct/ProductDto.cs
export interface ProductDto {
  id: number;
  nameEn: string;
  nameAr: string;
  slug: string;
  descriptionEn: string | null;
  descriptionAr: string | null;
  price: number;
  currency: ProductCurrency;
  stock: number;
  sku: string | null;
  status: ProductStatus;
  categoryId: number;
  categoryNameEn: string;
  categoryNameAr: string;
  createdAt: string;
  updatedAt: string;
}

// Mirrors: ListProductPaginated query response (lighter shape for tables)
export interface ProductListItem {
  id: number;
  nameEn: string;
  nameAr: string;
  slug: string;
  price: number;
  currency: ProductCurrency;
  stock: number;
  status: ProductStatus;
  categoryNameEn: string;
  categoryNameAr: string;
}

// Mirrors: ListProductPaginatedQuery.cs fields
export interface ProductQueryParams {
  page?: number;
  limit?: number;
  search?: string;
  categoryId?: number;
  status?: ProductStatus;
  sortBy?: 'price' | 'stock' | 'createdAt' | 'nameEn';
  sortOrder?: 'asc' | 'desc';
}
```

---

### 5.2 `api/product.api.ts`

**Why it exists:** Single file for all HTTP calls to the backend `ProductFeature`. Pure functions — no React, no hooks, no server-only imports. Can be called from React Query hooks, RSCs, or Server Actions.

```ts
// features/product/api/product.api.ts

import apiClient from '@/lib/api-client';
import type { ProductDto, ProductListItem, ProductQueryParams } from '../types';
import type { ApiResponse, PaginatedResponse } from '@/types/api';

// ─── CreateProduct — mirrors CreateProductCommand.cs ─────────────────────────
export interface CreateProductPayload {
  nameEn: string;
  nameAr: string;
  descriptionEn?: string;
  descriptionAr?: string;
  price: number;
  currency: string;
  stock: number;
  sku?: string;
  categoryId: number;
}

export async function createProduct(payload: CreateProductPayload): Promise<ProductDto> {
  const { data } = await apiClient.post<ApiResponse<ProductDto>>('/products', payload);
  if (!data.success) throw new Error(data.message ?? 'Failed to create product');
  return data.data!;
}

// ─── UpdateProduct — mirrors UpdateProductCommand.cs ─────────────────────────
export interface UpdateProductPayload {
  nameEn?: string;
  nameAr?: string;
  descriptionEn?: string;
  descriptionAr?: string;
  price?: number;
  currency?: string;
  stock?: number;
  sku?: string;
  categoryId?: number;
  status?: string;
}

export async function updateProduct(
  id: number,
  payload: UpdateProductPayload,
): Promise<ProductDto> {
  const { data } = await apiClient.put<ApiResponse<ProductDto>>(`/products/${id}`, payload);
  if (!data.success) throw new Error(data.message ?? 'Failed to update product');
  return data.data!;
}

// ─── DeleteProduct — mirrors DeleteProductCommand.cs ─────────────────────────
export async function deleteProduct(id: number): Promise<void> {
  const { data } = await apiClient.delete<ApiResponse<null>>(`/products/${id}`);
  if (!data.success) throw new Error(data.message ?? 'Failed to delete product');
}

// ─── GetByIdProduct — mirrors GetByIdProductQuery.cs ─────────────────────────
export async function getProductById(id: number): Promise<ProductDto | null> {
  const { data } = await apiClient.get<ApiResponse<ProductDto>>(`/products/${id}`);
  if (!data.success) return null;
  return data.data ?? null;
}

// ─── ListProductPaginated — mirrors ListProductPaginatedQuery.cs ──────────────
export async function getProducts(
  params: ProductQueryParams,
): Promise<PaginatedResponse<ProductListItem>> {
  const { page = 1, limit = 10, search, categoryId, status, sortBy, sortOrder } = params;
  const { data } = await apiClient.get<ApiResponse<PaginatedResponse<ProductListItem>>>(
    '/products',
    {
      params: {
        page,
        limit,
        ...(search && { search }),
        ...(categoryId !== undefined && { categoryId }),
        ...(status && { status }),
        ...(sortBy && { sortBy }),
        ...(sortOrder && { sortOrder }),
      },
    },
  );
  if (!data.success) throw new Error(data.message ?? 'Failed to fetch products');
  return data.data!;
}
```

---

### 5.3 `hooks/useProduct.ts`

**Why it exists:** Wraps **query** (read-only) operations in React Query. Components that display data import from here — never call the API directly.

```ts
// features/product/hooks/useProduct.ts
'use client';

import { useQuery } from '@tanstack/react-query';
import { getProductById, getProducts } from '../api/product.api';
import type { ProductQueryParams } from '../types';
import { PRODUCT_QUERY_KEYS } from '../constants';

// Query Key Factory — centralising keys prevents typos and enables precise invalidation
export const productKeys = {
  all: PRODUCT_QUERY_KEYS.ALL,
  lists: () => [...productKeys.all, 'list'] as const,
  list: (params: ProductQueryParams) => [...productKeys.lists(), params] as const,
  details: () => [...productKeys.all, 'detail'] as const,
  detail: (id: number) => [...productKeys.details(), id] as const,
};

export function useProducts(params: ProductQueryParams = {}) {
  return useQuery({
    queryKey: productKeys.list(params),
    queryFn: () => getProducts(params),
    placeholderData: (prev) => prev, // keeps stale data visible while fetching next page
    staleTime: 30_000,
  });
}

export function useProduct(id: number, enabled = true) {
  return useQuery({
    queryKey: productKeys.detail(id),
    queryFn: () => getProductById(id),
    enabled: enabled && id > 0,
    staleTime: 60_000,
  });
}
```

---

### 5.4 `hooks/useProductMutations.ts`

**Why it exists:** Wraps **mutation** operations in React Query. Separated from queries to keep each file focused and make side effects obvious.

```ts
// features/product/hooks/useProductMutations.ts
'use client';

import { useMutation, useQueryClient } from '@tanstack/react-query';
import { toast } from 'sonner';
import {
  createProduct,
  updateProduct,
  deleteProduct,
  type CreateProductPayload,
  type UpdateProductPayload,
} from '../api/product.api';
import { productKeys } from './useProduct';

export function useCreateProduct() {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: (payload: CreateProductPayload) => createProduct(payload),
    onSuccess: (newProduct) => {
      queryClient.invalidateQueries({ queryKey: productKeys.lists() });
      queryClient.setQueryData(productKeys.detail(newProduct.id), newProduct);
      toast.success('Product created successfully');
    },
    onError: (error: Error) => {
      toast.error(error.message ?? 'Failed to create product');
    },
  });
}

export function useUpdateProduct() {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: ({ id, payload }: { id: number; payload: UpdateProductPayload }) =>
      updateProduct(id, payload),
    onSuccess: (updated) => {
      queryClient.invalidateQueries({ queryKey: productKeys.lists() });
      queryClient.setQueryData(productKeys.detail(updated.id), updated);
      toast.success('Product updated successfully');
    },
    onError: (error: Error) => {
      toast.error(error.message ?? 'Failed to update product');
    },
  });
}

export function useDeleteProduct() {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: (id: number) => deleteProduct(id),
    onMutate: async (id) => {
      // Optimistic update: remove from list immediately
      await queryClient.cancelQueries({ queryKey: productKeys.lists() });
      const previous = queryClient.getQueryData(productKeys.lists());
      return { previous, id };
    },
    onError: (_err, _id, context) => {
      if (context?.previous) {
        queryClient.setQueryData(productKeys.lists(), context.previous);
      }
      toast.error('Failed to delete product');
    },
    onSuccess: (_data, id) => {
      queryClient.invalidateQueries({ queryKey: productKeys.lists() });
      queryClient.removeQueries({ queryKey: productKeys.detail(id) });
      toast.success('Product deleted successfully');
    },
  });
}
```

---

### 5.5 `schemas/product.schema.ts`

**Why it exists:** Zod schemas for form validation + TypeScript type inference. Separate from `types/` because wire format ≠ form input format (e.g., `status` is excluded from create).

```ts
// features/product/schemas/product.schema.ts

import { z } from 'zod';

export const createProductSchema = z.object({
  nameEn: z
    .string()
    .min(2, 'English name must be at least 2 characters')
    .max(200, 'English name must be less than 200 characters'),
  nameAr: z
    .string()
    .min(2, 'Arabic name must be at least 2 characters')
    .max(200, 'Arabic name must be less than 200 characters'),
  descriptionEn: z.string().max(2000).optional(),
  descriptionAr: z.string().max(2000).optional(),
  price: z
    .number({ invalid_type_error: 'Price must be a number' })
    .positive('Price must be greater than 0')
    .multipleOf(0.01, 'Price can have at most 2 decimal places'),
  currency: z.enum(['SAR', 'USD', 'EUR'], {
    errorMap: () => ({ message: 'Select a valid currency' }),
  }),
  stock: z
    .number({ invalid_type_error: 'Stock must be a number' })
    .int('Stock must be a whole number')
    .min(0, 'Stock cannot be negative'),
  sku: z.string().max(100).optional().or(z.literal('')),
  categoryId: z
    .number({ invalid_type_error: 'Select a category' })
    .int()
    .positive('Select a valid category'),
});

export type CreateProductInput = z.infer<typeof createProductSchema>;

// Update: all fields optional + add status field
export const updateProductSchema = createProductSchema.partial().extend({
  status: z.enum(['active', 'inactive', 'draft', 'archived']).optional(),
});

export type UpdateProductInput = z.infer<typeof updateProductSchema>;

// Filter/query params schema for URL search params
export const productFilterSchema = z.object({
  page: z.coerce.number().int().min(1).optional().default(1),
  limit: z.coerce.number().int().min(1).max(100).optional().default(10),
  search: z.string().max(100).optional(),
  categoryId: z.coerce.number().int().positive().optional(),
  status: z.enum(['active', 'inactive', 'draft', 'archived']).optional(),
  sortBy: z.enum(['price', 'stock', 'createdAt', 'nameEn']).optional(),
  sortOrder: z.enum(['asc', 'desc']).optional().default('desc'),
});

export type ProductFilterInput = z.infer<typeof productFilterSchema>;
```

---

### 5.6 `components/ProductForm.tsx`

**Why it exists:** Single form component handling both create and update. Uses React Query mutations — the form is a Client Component while the parent page is a Server Component.

```tsx
// features/product/components/ProductForm.tsx
'use client';

import { useEffect } from 'react';
import { useRouter } from 'next/navigation';
import { useForm } from 'react-hook-form';
import { zodResolver } from '@hookform/resolvers/zod';
import {
  Form, FormControl, FormField, FormItem, FormLabel, FormMessage,
} from '@/components/ui/form';
import { Input } from '@/components/ui/input';
import { Textarea } from '@/components/ui/textarea';
import {
  Select, SelectContent, SelectItem, SelectTrigger, SelectValue,
} from '@/components/ui/select';
import { Button } from '@/components/ui/button';
import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card';
import { Loader2, Save, X } from 'lucide-react';
import { createProductSchema, type CreateProductInput } from '../schemas/product.schema';
import { useCreateProduct, useUpdateProduct } from '../hooks/useProductMutations';
import type { ProductDto } from '../types';
import { PRODUCT_CURRENCIES } from '../constants';

interface ProductFormProps {
  initialData?: ProductDto | null;
  categories: Array<{ id: number; nameEn: string; nameAr: string }>;
}

export function ProductForm({ initialData, categories }: ProductFormProps) {
  const router = useRouter();
  const { mutate: createProduct, isPending: isCreating } = useCreateProduct();
  const { mutate: updateProduct, isPending: isUpdating } = useUpdateProduct();
  const isPending = isCreating || isUpdating;

  const form = useForm<CreateProductInput>({
    resolver: zodResolver(createProductSchema),
    defaultValues: {
      nameEn: '', nameAr: '', descriptionEn: '', descriptionAr: '',
      price: 0, currency: 'SAR', stock: 0, sku: '', categoryId: 0,
    },
  });

  useEffect(() => {
    if (initialData) {
      form.reset({
        nameEn: initialData.nameEn,
        nameAr: initialData.nameAr,
        descriptionEn: initialData.descriptionEn ?? '',
        descriptionAr: initialData.descriptionAr ?? '',
        price: initialData.price,
        currency: initialData.currency,
        stock: initialData.stock,
        sku: initialData.sku ?? '',
        categoryId: initialData.categoryId,
      });
    }
  }, [initialData, form]);

  function onSubmit(values: CreateProductInput) {
    if (initialData) {
      updateProduct(
        { id: initialData.id, payload: values },
        { onSuccess: () => router.push('/products') },
      );
    } else {
      createProduct(values, { onSuccess: () => router.push('/products') });
    }
  }

  return (
    <Card>
      <CardHeader>
        <CardTitle>{initialData ? 'Edit Product' : 'Create Product'}</CardTitle>
      </CardHeader>
      <CardContent>
        <Form {...form}>
          <form onSubmit={form.handleSubmit(onSubmit)} className="space-y-6">
            <div className="grid grid-cols-1 gap-6 md:grid-cols-2">
              <FormField control={form.control} name="nameEn"
                render={({ field }) => (
                  <FormItem>
                    <FormLabel>English Name *</FormLabel>
                    <FormControl>
                      <Input placeholder="Product name in English" {...field} disabled={isPending} />
                    </FormControl>
                    <FormMessage />
                  </FormItem>
                )}
              />
              <FormField control={form.control} name="nameAr"
                render={({ field }) => (
                  <FormItem>
                    <FormLabel>الاسم بالعربية *</FormLabel>
                    <FormControl>
                      <Input placeholder="اسم المنتج بالعربية" dir="rtl" {...field} disabled={isPending} />
                    </FormControl>
                    <FormMessage />
                  </FormItem>
                )}
              />
              <FormField control={form.control} name="price"
                render={({ field }) => (
                  <FormItem>
                    <FormLabel>Price *</FormLabel>
                    <FormControl>
                      <Input type="number" step="0.01" min="0" placeholder="0.00"
                        {...field}
                        onChange={(e) => field.onChange(parseFloat(e.target.value))}
                        disabled={isPending}
                      />
                    </FormControl>
                    <FormMessage />
                  </FormItem>
                )}
              />
              <FormField control={form.control} name="currency"
                render={({ field }) => (
                  <FormItem>
                    <FormLabel>Currency *</FormLabel>
                    <Select onValueChange={field.onChange} value={field.value} disabled={isPending}>
                      <FormControl>
                        <SelectTrigger><SelectValue placeholder="Select currency" /></SelectTrigger>
                      </FormControl>
                      <SelectContent>
                        {PRODUCT_CURRENCIES.map((c) => (
                          <SelectItem key={c.value} value={c.value}>{c.label}</SelectItem>
                        ))}
                      </SelectContent>
                    </Select>
                    <FormMessage />
                  </FormItem>
                )}
              />
              <FormField control={form.control} name="stock"
                render={({ field }) => (
                  <FormItem>
                    <FormLabel>Stock *</FormLabel>
                    <FormControl>
                      <Input type="number" min="0" step="1" placeholder="0"
                        {...field}
                        onChange={(e) => field.onChange(parseInt(e.target.value, 10))}
                        disabled={isPending}
                      />
                    </FormControl>
                    <FormMessage />
                  </FormItem>
                )}
              />
              <FormField control={form.control} name="categoryId"
                render={({ field }) => (
                  <FormItem>
                    <FormLabel>Category *</FormLabel>
                    <Select
                      onValueChange={(v) => field.onChange(parseInt(v, 10))}
                      value={field.value ? String(field.value) : ''}
                      disabled={isPending}
                    >
                      <FormControl>
                        <SelectTrigger><SelectValue placeholder="Select a category" /></SelectTrigger>
                      </FormControl>
                      <SelectContent>
                        {categories.map((cat) => (
                          <SelectItem key={cat.id} value={String(cat.id)}>{cat.nameEn}</SelectItem>
                        ))}
                      </SelectContent>
                    </Select>
                    <FormMessage />
                  </FormItem>
                )}
              />
            </div>
            <FormField control={form.control} name="descriptionEn"
              render={({ field }) => (
                <FormItem>
                  <FormLabel>Description (English)</FormLabel>
                  <FormControl>
                    <Textarea placeholder="Product description in English"
                      className="min-h-[100px] resize-none" {...field} disabled={isPending} />
                  </FormControl>
                  <FormMessage />
                </FormItem>
              )}
            />
            <div className="flex gap-3">
              <Button type="submit" disabled={isPending} className="min-w-[120px]">
                {isPending ? (
                  <><Loader2 className="mr-2 h-4 w-4 animate-spin" />Saving...</>
                ) : (
                  <><Save className="mr-2 h-4 w-4" />{initialData ? 'Update' : 'Create'}</>
                )}
              </Button>
              <Button type="button" variant="outline" disabled={isPending}
                onClick={() => router.push('/products')}>
                <X className="mr-2 h-4 w-4" />Cancel
              </Button>
            </div>
          </form>
        </Form>
      </CardContent>
    </Card>
  );
}
```

---

### 5.7 `components/ProductTable.tsx`

**Why it exists:** Encapsulates table UI — columns, row actions — all product-specific. Client Component because it needs interactivity.

```tsx
// features/product/components/ProductTable.tsx
'use client';

import {
  ColumnDef, flexRender, getCoreRowModel, useReactTable,
} from '@tanstack/react-table';
import {
  Table, TableBody, TableCell, TableHead, TableHeader, TableRow,
} from '@/components/ui/table';
import { Badge } from '@/components/ui/badge';
import { Button } from '@/components/ui/button';
import { MoreHorizontal, Pencil, Trash2 } from 'lucide-react';
import {
  DropdownMenu, DropdownMenuContent, DropdownMenuItem, DropdownMenuTrigger,
} from '@/components/ui/dropdown-menu';
import Link from 'next/link';
import type { ProductListItem } from '../types';
import { useDeleteProduct } from '../hooks/useProductMutations';
import { PRODUCT_STATUS_COLORS } from '../constants';
import { formatPrice } from '../utils';

interface ProductTableProps {
  data: ProductListItem[];
  isLoading?: boolean;
}

export function ProductTable({ data, isLoading }: ProductTableProps) {
  const { mutate: deleteProduct } = useDeleteProduct();

  const columns: ColumnDef<ProductListItem>[] = [
    {
      accessorKey: 'nameEn',
      header: 'Name',
      cell: ({ row }) => (
        <div>
          <p className="font-medium">{row.original.nameEn}</p>
          <p className="text-muted-foreground text-xs" dir="rtl">{row.original.nameAr}</p>
        </div>
      ),
    },
    {
      accessorKey: 'price',
      header: 'Price',
      cell: ({ row }) => formatPrice(row.original.price, row.original.currency),
    },
    {
      accessorKey: 'stock',
      header: 'Stock',
      cell: ({ row }) => (
        <span className={row.original.stock === 0 ? 'text-destructive font-medium' : ''}>
          {row.original.stock}
        </span>
      ),
    },
    {
      accessorKey: 'status',
      header: 'Status',
      cell: ({ row }) => (
        <Badge variant="outline" className={PRODUCT_STATUS_COLORS[row.original.status]}>
          {row.original.status}
        </Badge>
      ),
    },
    { accessorKey: 'categoryNameEn', header: 'Category' },
    {
      id: 'actions',
      cell: ({ row }) => (
        <DropdownMenu>
          <DropdownMenuTrigger asChild>
            <Button variant="ghost" size="icon"><MoreHorizontal className="h-4 w-4" /></Button>
          </DropdownMenuTrigger>
          <DropdownMenuContent align="end">
            <DropdownMenuItem asChild>
              <Link href={`/products/${row.original.id}/edit`}>
                <Pencil className="mr-2 h-4 w-4" />Edit
              </Link>
            </DropdownMenuItem>
            <DropdownMenuItem
              className="text-destructive"
              onClick={() => {
                if (confirm('Delete this product?')) deleteProduct(row.original.id);
              }}
            >
              <Trash2 className="mr-2 h-4 w-4" />Delete
            </DropdownMenuItem>
          </DropdownMenuContent>
        </DropdownMenu>
      ),
    },
  ];

  const table = useReactTable({ data, columns, getCoreRowModel: getCoreRowModel() });

  if (isLoading) {
    return <div className="py-10 text-center text-sm text-muted-foreground">Loading products...</div>;
  }

  return (
    <div className="rounded-md border">
      <Table>
        <TableHeader>
          {table.getHeaderGroups().map((hg) => (
            <TableRow key={hg.id}>
              {hg.headers.map((h) => (
                <TableHead key={h.id}>
                  {h.isPlaceholder ? null : flexRender(h.column.columnDef.header, h.getContext())}
                </TableHead>
              ))}
            </TableRow>
          ))}
        </TableHeader>
        <TableBody>
          {table.getRowModel().rows.length ? (
            table.getRowModel().rows.map((row) => (
              <TableRow key={row.id}>
                {row.getVisibleCells().map((cell) => (
                  <TableCell key={cell.id}>
                    {flexRender(cell.column.columnDef.cell, cell.getContext())}
                  </TableCell>
                ))}
              </TableRow>
            ))
          ) : (
            <TableRow>
              <TableCell colSpan={columns.length} className="h-24 text-center text-muted-foreground">
                No products found.
              </TableCell>
            </TableRow>
          )}
        </TableBody>
      </Table>
    </div>
  );
}
```

---

### 5.8 `components/ProductCard.tsx`

**Why it exists:** Alternative grid/card display component for the same data.

```tsx
// features/product/components/ProductCard.tsx
import { Badge } from '@/components/ui/badge';
import { Card, CardContent, CardFooter, CardHeader } from '@/components/ui/card';
import { Button } from '@/components/ui/button';
import Link from 'next/link';
import { Pencil } from 'lucide-react';
import type { ProductListItem } from '../types';
import { formatPrice } from '../utils';
import { PRODUCT_STATUS_COLORS } from '../constants';

export function ProductCard({ product }: { product: ProductListItem }) {
  return (
    <Card className="transition-shadow hover:shadow-md">
      <CardHeader className="pb-2">
        <div className="flex items-start justify-between gap-2">
          <div className="min-w-0 flex-1">
            <h3 className="truncate font-semibold">{product.nameEn}</h3>
            <p className="text-muted-foreground truncate text-sm" dir="rtl">{product.nameAr}</p>
          </div>
          <Badge variant="outline" className={PRODUCT_STATUS_COLORS[product.status]}>
            {product.status}
          </Badge>
        </div>
      </CardHeader>
      <CardContent className="pb-2">
        <div className="text-muted-foreground flex items-center justify-between text-sm">
          <span className="text-foreground text-lg font-bold">
            {formatPrice(product.price, product.currency)}
          </span>
          <span>Stock: {product.stock}</span>
        </div>
        <p className="text-muted-foreground mt-1 text-xs">{product.categoryNameEn}</p>
      </CardContent>
      <CardFooter>
        <Button asChild variant="outline" size="sm" className="w-full">
          <Link href={`/products/${product.id}/edit`}>
            <Pencil className="mr-2 h-3 w-3" />Edit
          </Link>
        </Button>
      </CardFooter>
    </Card>
  );
}
```

---

### 5.9 `constants/index.ts`

**Why it exists:** Static data specific to the product domain. Global constants (app name, auth routes) belong in root `constants/`.

```ts
// features/product/constants/index.ts

import type { ProductStatus } from '../types';

// React Query key namespace — prevents key collisions between features
export const PRODUCT_QUERY_KEYS = {
  ALL: ['products'] as const,
};

export const PRODUCT_CURRENCIES = [
  { value: 'SAR', label: 'Saudi Riyal (SAR)' },
  { value: 'USD', label: 'US Dollar (USD)' },
  { value: 'EUR', label: 'Euro (EUR)' },
] as const;

export const PRODUCT_STATUS_OPTIONS: Array<{ value: ProductStatus; label: string }> = [
  { value: 'active', label: 'Active' },
  { value: 'inactive', label: 'Inactive' },
  { value: 'draft', label: 'Draft' },
  { value: 'archived', label: 'Archived' },
];

// Tailwind classes keyed by status — styling logic stays out of components
export const PRODUCT_STATUS_COLORS: Record<ProductStatus, string> = {
  active: 'bg-green-50 text-green-700 border-green-200',
  inactive: 'bg-gray-50 text-gray-700 border-gray-200',
  draft: 'bg-yellow-50 text-yellow-700 border-yellow-200',
  archived: 'bg-red-50 text-red-700 border-red-200',
};

export const PRODUCT_DEFAULT_PAGE_SIZE = 10;
export const PRODUCT_MAX_PAGE_SIZE = 100;
```

---

### 5.10 `utils/index.ts`

**Why it exists:** Pure functions that transform/compute data specific to products. No React, no API calls.

```ts
// features/product/utils/index.ts

import type { ProductListItem, ProductCurrency } from '../types';

export function formatPrice(price: number, currency: ProductCurrency): string {
  const localeMap: Record<ProductCurrency, string> = {
    SAR: 'ar-SA',
    USD: 'en-US',
    EUR: 'de-DE',
  };
  return new Intl.NumberFormat(localeMap[currency], {
    style: 'currency',
    currency,
    minimumFractionDigits: 2,
  }).format(price);
}

export function isOutOfStock(product: ProductListItem): boolean {
  return product.stock === 0;
}

export function isLowStock(product: ProductListItem, threshold = 5): boolean {
  return product.stock > 0 && product.stock <= threshold;
}

export function getProductName(
  product: Pick<ProductListItem, 'nameEn' | 'nameAr'>,
  locale: 'en' | 'ar' = 'en',
): string {
  return locale === 'ar' ? product.nameAr : product.nameEn;
}

export function buildProductQueryString(
  params: Record<string, string | number | undefined>,
): string {
  const sp = new URLSearchParams();
  for (const [key, value] of Object.entries(params)) {
    if (value !== undefined && value !== '') sp.set(key, String(value));
  }
  return sp.toString();
}
```

---

### 5.11 `index.ts` (barrel)

**Why it exists:** The **only** import gateway for anything outside the `product/` feature. Enforces encapsulation.

```ts
// features/product/index.ts

// Public Components
export { ProductForm } from './components/ProductForm';
export { ProductTable } from './components/ProductTable';
export { ProductCard } from './components/ProductCard';

// Public Hooks
export { useProducts, useProduct, productKeys } from './hooks/useProduct';
export { useCreateProduct, useUpdateProduct, useDeleteProduct } from './hooks/useProductMutations';

// Public Types
export type {
  ProductDto, ProductListItem, ProductQueryParams, ProductStatus, ProductCurrency,
} from './types';

// Public Schemas
export { createProductSchema, updateProductSchema } from './schemas/product.schema';
export type { CreateProductInput, UpdateProductInput } from './schemas/product.schema';

// Public Utils
export { formatPrice, getProductName, isOutOfStock, isLowStock } from './utils';
```

---

## 6. App Router Pages

Pages are **thin shells**: metadata + Suspense boundaries + pass server-fetched data as props to feature components. All display/interactive logic lives inside feature components.

### List Page — RSC

```tsx
// app/(routes)/products/page.tsx
import { Suspense } from 'react';
import type { Metadata } from 'next';
import Link from 'next/link';
import { Plus } from 'lucide-react';
import { buttonVariants } from '@/components/ui/button';
import { Heading } from '@/components/ui/heading';
import { Separator } from '@/components/ui/separator';
import { DataTableSkeleton } from '@/components/ui/table/data-table-skeleton';
import { ProductListingPage } from '@/features/product/components/ProductListingPage';
import { cn } from '@/lib/utils';
import type { SearchParams } from 'nuqs/server';

export const metadata: Metadata = {
  title: 'Products | Subul',
  description: 'Manage your product catalog',
};

export default async function ProductsPage({ searchParams }: { searchParams: Promise<SearchParams> }) {
  const params = await searchParams;

  return (
    <div className="flex flex-1 flex-col space-y-4">
      <div className="flex items-start justify-between">
        <Heading title="Products" description="Manage your product catalog" />
        <Link href="/products/new" className={cn(buttonVariants(), 'text-xs md:text-sm')}>
          <Plus className="mr-2 h-4 w-4" />Add Product
        </Link>
      </div>
      <Separator />
      <Suspense fallback={<DataTableSkeleton columnCount={6} rowCount={10} />}>
        <ProductListingPage searchParams={params} />
      </Suspense>
    </div>
  );
}
```

### Create Page — RSC

```tsx
// app/(routes)/products/new/page.tsx
import type { Metadata } from 'next';
import { getCategories } from '@/features/category/api/category.api';
import { ProductForm } from '@/features/product';

export const metadata: Metadata = { title: 'New Product | Subul' };

export default async function NewProductPage() {
  // Server-side: fetch category options for the form's select
  const categoriesData = await getCategories({ limit: 1000, sortBy: 'nameEn', sortOrder: 'asc' });

  return (
    <div className="mx-auto w-full max-w-3xl">
      <ProductForm
        categories={categoriesData.items.map((c) => ({
          id: c.id, nameEn: c.nameEn, nameAr: c.nameAr,
        }))}
      />
    </div>
  );
}
```

### Edit Page — RSC

```tsx
// app/(routes)/products/[id]/edit/page.tsx
import { notFound } from 'next/navigation';
import type { Metadata } from 'next';
import { getProductById } from '@/features/product/api/product.api';
import { getCategories } from '@/features/category/api/category.api';
import { ProductForm } from '@/features/product';

interface PageProps { params: Promise<{ id: string }> }

export async function generateMetadata({ params }: PageProps): Promise<Metadata> {
  const { id } = await params;
  const product = await getProductById(parseInt(id, 10));
  return { title: product ? `Edit: ${product.nameEn}` : 'Product Not Found' };
}

export default async function EditProductPage({ params }: PageProps) {
  const { id } = await params;
  const productId = parseInt(id, 10);
  if (isNaN(productId)) notFound();

  const [product, categoriesData] = await Promise.all([
    getProductById(productId),
    getCategories({ limit: 1000, sortBy: 'nameEn', sortOrder: 'asc' }),
  ]);

  if (!product) notFound();

  return (
    <div className="mx-auto w-full max-w-3xl">
      <ProductForm
        initialData={product}
        categories={categoriesData.items.map((c) => ({
          id: c.id, nameEn: c.nameEn, nameAr: c.nameAr,
        }))}
      />
    </div>
  );
}
```

---

## 7. CategoryFeature — Parallel Example

The pattern is identical — one-to-one mapping with the backend `CategoryFeature`.

```
features/category/
├── api/category.api.ts
├── hooks/
│   ├── useCategory.ts           ← useCategories, useCategory, useCategoryTree
│   └── useCategoryMutations.ts  ← useCreateCategory, useUpdateCategory, useDeleteCategory
├── schemas/category.schema.ts
├── components/
│   ├── CategoryForm.tsx
│   ├── CategoryTable.tsx
│   ├── CategoryTree.tsx         ← hierarchical tree view (unique to this feature)
│   ├── CategoryListingPage.tsx
│   └── index.ts
├── types/index.ts
├── constants/index.ts           ← CATEGORY_QUERY_KEYS, MAX_DEPTH
├── utils/index.ts               ← buildCategoryTree, isAncestor, flattenTree
└── index.ts
```

### `hooks/useCategory.ts`

```ts
// features/category/hooks/useCategory.ts
'use client';

import { useQuery } from '@tanstack/react-query';
import { getCategories, getCategoryById } from '../api/category.api';
import type { CategoryQueryParams } from '../types';
import { buildCategoryTree } from '../utils';
import { CATEGORY_QUERY_KEYS } from '../constants';

export const categoryKeys = {
  all: CATEGORY_QUERY_KEYS.ALL,
  lists: () => [...categoryKeys.all, 'list'] as const,
  list: (params: CategoryQueryParams) => [...categoryKeys.lists(), params] as const,
  details: () => [...categoryKeys.all, 'detail'] as const,
  detail: (id: number) => [...categoryKeys.details(), id] as const,
};

export function useCategories(params: CategoryQueryParams = {}) {
  return useQuery({
    queryKey: categoryKeys.list(params),
    queryFn: () => getCategories(params),
    staleTime: 60_000, // categories change rarely
  });
}

export function useCategory(id: number) {
  return useQuery({
    queryKey: categoryKeys.detail(id),
    queryFn: () => getCategoryById(id),
    enabled: id > 0,
  });
}

// Derived query — builds tree on client from flat list response
export function useCategoryTree() {
  const query = useCategories({ limit: 1000, sortBy: 'nameEn', sortOrder: 'asc' });
  return {
    ...query,
    data: query.data ? buildCategoryTree(query.data.items) : undefined,
  };
}
```

---

## 8. Shared Layer

### `lib/api-client.ts`

Global Axios instance used by every feature's `api/*.api.ts`.

```ts
// lib/api-client.ts
import axios, { type InternalAxiosRequestConfig } from 'axios';

const apiClient = axios.create({
  baseURL: process.env.NEXT_PUBLIC_API_URL ?? 'http://localhost:5101/api',
  timeout: 15_000,
  headers: { 'Content-Type': 'application/json' },
});

apiClient.interceptors.request.use(async (config: InternalAxiosRequestConfig) => {
  let token: string | undefined;

  if (typeof window === 'undefined') {
    const { getServerSession } = await import('next-auth');
    const { default: authOptions } = await import('@/lib/auth-options');
    const session = await getServerSession(authOptions);
    token = session?.accessToken;
  } else {
    const res = await fetch('/api/auth/session');
    const session = await res.json();
    token = session?.accessToken;
  }

  if (token) config.headers.Authorization = `Bearer ${token}`;
  return config;
});

apiClient.interceptors.response.use(
  (res) => res,
  (error) => {
    const message =
      error.response?.data?.message ??
      error.response?.data?.errors?.[0] ??
      error.message ??
      'An unexpected error occurred';
    return Promise.reject(new Error(message));
  },
);

export default apiClient;
```

### `lib/query-client.ts`

```ts
// lib/query-client.ts
import { QueryClient } from '@tanstack/react-query';

export function makeQueryClient(): QueryClient {
  return new QueryClient({
    defaultOptions: {
      queries: {
        staleTime: 30_000,
        gcTime: 5 * 60 * 1_000,
        retry: 1,
        refetchOnWindowFocus: false,
      },
      mutations: { retry: 0 },
    },
  });
}

let browserQueryClient: QueryClient | undefined;

export function getQueryClient(): QueryClient {
  if (typeof window === 'undefined') return makeQueryClient();
  if (!browserQueryClient) browserQueryClient = makeQueryClient();
  return browserQueryClient;
}
```

### `types/api.ts`

Shared generic types used by every feature's API functions. Mirrors the backend `ApiResponse<T>`.

```ts
// types/api.ts

// Mirrors: Common/ApiResponse.cs
export interface ApiResponse<T> {
  success: boolean;
  message: string | null;
  data: T | null;
  errors: string[] | null;
}

// Mirrors: backend paginated list handler response
export interface PaginatedResponse<T> {
  items: T[];
  total: number;
  page: number;
  limit: number;
  totalPages: number;
}
```

---

## 9. Feature vs. Shared Decision Matrix

| Question | Feature | Shared |
|----------|---------|--------|
| Used by only one feature? | ✅ Feature | ❌ |
| Domain type (e.g., `ProductDto`)? | ✅ Feature `types/` | ❌ |
| Generic UI primitive (Button, Input)? | ❌ | ✅ `components/ui/` |
| HTTP call to backend? | ✅ Feature `api/` | ❌ |
| React Query hook reading feature data? | ✅ Feature `hooks/` | ❌ |
| Used by 3+ features (e.g., `useDebounce`)? | ❌ | ✅ `hooks/` |
| Axios instance? | ❌ | ✅ `lib/api-client.ts` |
| QueryClient factory? | ❌ | ✅ `lib/query-client.ts` |
| `ApiResponse<T>` / `PaginatedResponse<T>` type? | ❌ | ✅ `types/api.ts` |
| Feature-specific utility (`formatPrice`)? | ✅ Feature `utils/` | ❌ |
| Generic utility (`cn()`, `formatDate()`)? | ❌ | ✅ `lib/utils.ts` |
| Navigation config / sidebar items? | ❌ | ✅ `config/nav.ts` |

### The Rule of Three

When a pattern appears in a **third** feature, extract it to `shared/`. Before that, duplication is acceptable and preferable over premature abstraction.

---

## 10. Inter-Feature Communication

Features **never** import directly from each other. Valid cross-feature patterns:

### Pattern 1: Props from RSC page (most common)

```tsx
// app/(routes)/products/new/page.tsx ← RSC orchestrates data
const categories = await getCategories({ limit: 1000 });
// Passes as props — no feature-to-feature import in components
<ProductForm categories={categories.items} />
```

### Pattern 2: React Query cache seeding

```ts
// In useCreateProduct mutation onSuccess:
// Seed the category cache as a side effect via queryClient — not via direct import
queryClient.setQueryData(categoryKeys.detail(product.categoryId), {
  id: product.categoryId,
  nameEn: product.categoryNameEn,
});
```

### Pattern 3: Shared types only

Both features use `ApiResponse<T>` and `PaginatedResponse<T>` from `types/api.ts` — not from each other.

---

## 11. Data Flow Diagram

```
┌─────────────────────────────────────────────────────────────┐
│  RSC Page (app/products/page.tsx)                           │
│  • Server-side render                                        │
│  • Calls feature api/ directly for initial data             │
│  • Passes data as props to Client Components                │
└──────────────────────┬──────────────────────────────────────┘
                       │ props
                       ▼
┌─────────────────────────────────────────────────────────────┐
│  Feature Listing Component (ProductListingPage.tsx)          │
│  'use client'                                                │
│  • Reads URL params (page, search, filters)                  │
│  • Calls useProducts() → React Query cache                   │
│  • Renders ProductTable + Pagination                         │
└────────────┬──────────────────┬───────────────────────────-─┘
             │                  │
    useProducts()        mutations (delete)
             │                  │
             ▼                  ▼
┌─────────────────────────────────────────────────────────────┐
│  React Query Layer                                           │
│  • Cache: queryClient                                        │
│  • productKeys.list({}) → GET /api/products                 │
│  • useDeleteProduct  → DELETE /api/products/:id             │
│  • Automatic invalidation after mutation                    │
└──────────────────────┬──────────────────────────────────────┘
                       │
                       ▼
┌─────────────────────────────────────────────────────────────┐
│  API Layer (product.api.ts)                                  │
│  • Pure functions: getProducts(), createProduct(), etc.     │
│  • Uses apiClient (Axios) from lib/                         │
│  • Unwraps ApiResponse<T>, throws on !success               │
└──────────────────────┬──────────────────────────────────────┘
                       │ HTTP
                       ▼
          ┌────────────────────────┐
          │  .NET Backend API       │
          │  /api/products          │
          │  ProductFeature/        │
          └────────────────────────┘
```

---

## 12. Migration Guide for Your Existing Project

### ✅ Keep As-Is

| What | Why |
|------|-----|
| `app/(auth)/` + `app/(routes)/` structure | Correct App Router pattern |
| `components/ui/` Shadcn components | Correct shared UI layer |
| `features/{entity}/types/index.ts` | Types already co-located |
| `features/{entity}/utils/` | Good pattern |
| `features/{entity}/index.ts` barrel | Correct encapsulation |
| `lib/api-client.ts` Axios instance | Correct global infrastructure |
| `hooks/` generic hooks (`use-debounce`, `use-media-query`) | Correct shared layer |

### 🔧 Rename/Refactor

| Current | Recommended | Reason |
|---------|-------------|--------|
| `features/category/services/category.service.ts` | `features/category/api/category.api.ts` | `api/` is clearer; separates from Next.js Server Actions |
| `features/category/utils/validators.ts` | `features/category/schemas/category.schema.ts` | Zod schemas are distinct from utility functions |
| `'use server'` at top of service file | Remove — split into `api/` (pure) + `actions/` (Server Actions) | API functions should be runtime-neutral |

### ❌ Add (Missing Pieces)

| What to Add | Where | Why |
|-------------|-------|-----|
| `features/{entity}/hooks/use{Entity}.ts` | Each feature | React Query queries — eliminates manual loading state |
| `features/{entity}/hooks/use{Entity}Mutations.ts` | Each feature | React Query mutations — automatic cache invalidation |
| `features/{entity}/constants/index.ts` | Each feature | Query keys, status maps, static config |
| `types/api.ts` | `src/types/` | Shared `ApiResponse<T>` / `PaginatedResponse<T>` |
| `lib/query-client.ts` | `src/lib/` | QueryClient factory for SSR safety |
| `providers/QueryProvider.tsx` | `src/providers/` | Wraps app in `QueryClientProvider` |

### Step-by-Step Migration (Zero Downtime)

1. **Step 1** — Add `types/api.ts` + `lib/query-client.ts` + `providers/QueryProvider.tsx`
2. **Step 2** — Add `features/{entity}/hooks/` folders with React Query hooks. Keep old service files.
3. **Step 3** — Migrate one component at a time to use hooks instead of direct service calls.
4. **Step 4** — Once a feature is fully on React Query, remove `'use server'` from the API file and rename `services/` → `api/`.
5. **Step 5** — Move validators to `schemas/` and update imports.
6. **Step 6** — Add `constants/` per feature and move query keys and status maps out of components.

> **Key Insight:** Your warehouse project already has the right instincts — feature folders, co-located types, barrel exports, Shadcn UI. The primary gaps are the **React Query hooks layer** inside each feature and the **`api/` vs `services/` separation**. Adding React Query mutations will immediately eliminate all `useState(false)` loading boilerplate and `router.refresh()` cache-busting.
