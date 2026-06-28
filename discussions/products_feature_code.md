# `features/products/` — Complete Code

> Based directly on `backend/Features/ProductFeature/`.  
> Stack: **Next.js 16 · React 19 · TypeScript · TailwindCSS v4 · TanStack Query**

---

## Backend endpoints this feature talks to

| Method | URL | Backend operation |
|---|---|---|
| `GET` | `/api/products` | `ListProductPaginated` |
| `GET` | `/api/products/{id}` | `GetByIdProduct` |
| `POST` | `/api/products` | `CreateProduct` |
| `PUT` | `/api/products/{id}` | `UpdateProduct` |
| `DELETE` | `/api/products/{id}` | `DeleteProduct` |

---

## File 1 — `features/products/types.ts`

Mirrors every C# record from the backend **exactly** (camelCase in JSON).

```ts
// ─────────────────────────────────────────────────────────────────────────────
// Shared nested types
// ─────────────────────────────────────────────────────────────────────────────

export interface ProductCategoryInfo {
  id: number;
  nameEn: string;
  nameAr: string | null;
}

export interface ProductBrandInfo {
  id: number;
  name: string;
  slug: string;
}

// ─────────────────────────────────────────────────────────────────────────────
// LIST  →  mirrors ListProductPaginatedItemResponse
// ─────────────────────────────────────────────────────────────────────────────

export interface ProductListItem {
  id: number;
  categoryId: number | null;
  brandId: number | null;
  nameEn: string;
  nameAr: string | null;
  slug: string;
  sku: string | null;
  barcode: string | null;
  shortDescriptionEn: string | null;
  shortDescriptionAr: string | null;
  price: number;
  compareAtPrice: number | null;
  currency: string;
  stockQuantity: number;
  status: 'active' | 'draft' | 'archived';
  isFeatured: boolean;
  totalSold: number;
  viewsCount: number;
  createdAt: string;
  updatedAt: string | null;
  category: ProductCategoryInfo | null;
  brand: ProductBrandInfo | null;
}

export interface PaginatedProducts {
  items: ProductListItem[];
  total: number;
  page: number;
  limit: number;
  totalPages: number;
}

// ─────────────────────────────────────────────────────────────────────────────
// GET BY ID  →  mirrors GetByIdProductResponse
// ─────────────────────────────────────────────────────────────────────────────

export interface ProductVariantInfo {
  id: number;
  title: string | null;
  sku: string | null;
  barcode: string | null;
  price: number | null;
  compareAtPrice: number | null;
  costPrice: number | null;
  stockQuantity: number;
  weight: number | null;
  isActive: boolean;
  sortOrder: number;
}

export interface ProductAttributeValueInfo {
  id: number;
  attributeId: number;
  valueText: string | null;
  valueNumber: number | null;
  valueBoolean: boolean | null;
  attribute: {
    nameEn: string;
    nameAr: string | null;
    unit: string | null;
    inputType: string;
    sortOrder: number;
  };
}

export interface ProductDetail {
  id: number;
  categoryId: number | null;
  brandId: number | null;
  nameEn: string;
  nameAr: string | null;
  slug: string;
  sku: string | null;
  barcode: string | null;
  descriptionEn: string | null;
  descriptionAr: string | null;
  shortDescriptionEn: string | null;
  shortDescriptionAr: string | null;
  price: number;
  compareAtPrice: number | null;
  costPrice: number | null;
  currency: string;
  stockQuantity: number;
  lowStockThreshold: number;
  minOrderQuantity: number;
  weight: number | null;
  status: 'active' | 'draft' | 'archived';
  isFeatured: boolean;
  requiresShipping: boolean;
  warrantyMonths: number;
  warrantyDescription: string | null;
  totalSold: number;
  viewsCount: number;
  metaTitle: string | null;
  metaDescription: string | null;
  createdAt: string;
  updatedAt: string | null;
  category: ProductCategoryInfo | null;
  brand: ProductBrandInfo | null;
  variants: ProductVariantInfo[];
  attributeValues: ProductAttributeValueInfo[];
}

// ─────────────────────────────────────────────────────────────────────────────
// CREATE  →  mirrors CreateProductCommand
// ─────────────────────────────────────────────────────────────────────────────

export interface CreateProductPayload {
  nameEn: string;
  nameAr?: string;
  categoryId?: number;
  brandId?: number;
  slug?: string;
  sku?: string;
  barcode?: string;
  descriptionEn?: string;
  descriptionAr?: string;
  shortDescriptionEn?: string;
  shortDescriptionAr?: string;
  price: number;
  compareAtPrice?: number;
  costPrice?: number;
  currency?: string;          // default: "IQD"
  stockQuantity?: number;     // default: 0
  lowStockThreshold?: number; // default: 2
  minOrderQuantity?: number;  // default: 1
  weight?: number;
  status?: 'active' | 'draft' | 'archived'; // default: "active"
  isFeatured?: boolean;       // default: false
  requiresShipping?: boolean; // default: true
  warrantyMonths?: number;    // default: 12
  warrantyDescription?: string;
  metaTitle?: string;
  metaDescription?: string;
}

// ─────────────────────────────────────────────────────────────────────────────
// UPDATE  →  mirrors UpdateProductRequest (body) — same fields, all required
// ─────────────────────────────────────────────────────────────────────────────

export interface UpdateProductPayload {
  nameEn: string;
  nameAr?: string;
  categoryId?: number;
  brandId?: number;
  slug?: string;
  sku?: string;
  barcode?: string;
  descriptionEn?: string;
  descriptionAr?: string;
  shortDescriptionEn?: string;
  shortDescriptionAr?: string;
  price: number;
  compareAtPrice?: number;
  costPrice?: number;
  currency: string;
  stockQuantity: number;
  lowStockThreshold: number;
  minOrderQuantity: number;
  weight?: number;
  status: 'active' | 'draft' | 'archived';
  isFeatured: boolean;
  requiresShipping: boolean;
  warrantyMonths: number;
  warrantyDescription?: string;
  metaTitle?: string;
  metaDescription?: string;
}

// ─────────────────────────────────────────────────────────────────────────────
// FILTERS  →  mirrors ListProductPaginatedQuery params
// ─────────────────────────────────────────────────────────────────────────────

export interface ProductFilters {
  page?: number;
  limit?: number;
  search?: string;
  categoryId?: number;
  brandId?: number;
  status?: string;
  isFeatured?: boolean;
  sortBy?: 'createdAt' | 'updatedAt' | 'nameEn' | 'price' | 'stockQuantity' | 'totalSold';
  sortOrder?: 'asc' | 'desc';
}
```

---

## File 2 — `features/products/api.ts`

One function per backend endpoint. No React — pure async functions only.

```ts
import { apiClient } from '@/lib/api-client';
import type {
  PaginatedProducts,
  ProductDetail,
  ProductListItem,
  ProductFilters,
  CreateProductPayload,
  UpdateProductPayload,
} from './types';

// ─── GET /api/products ────────────────────────────────────────────────────────
export async function fetchProducts(
  filters: ProductFilters = {}
): Promise<PaginatedProducts> {
  const params = new URLSearchParams();

  if (filters.page)       params.set('page',       String(filters.page));
  if (filters.limit)      params.set('limit',      String(filters.limit));
  if (filters.search)     params.set('search',     filters.search);
  if (filters.categoryId) params.set('categoryId', String(filters.categoryId));
  if (filters.brandId)    params.set('brandId',    String(filters.brandId));
  if (filters.status)     params.set('status',     filters.status);
  if (filters.sortBy)     params.set('sortBy',     filters.sortBy);
  if (filters.sortOrder)  params.set('sortOrder',  filters.sortOrder);
  if (filters.isFeatured !== undefined)
    params.set('isFeatured', String(filters.isFeatured));

  const query = params.toString();
  return apiClient<PaginatedProducts>(`/api/products${query ? `?${query}` : ''}`);
}

// ─── GET /api/products/{id} ───────────────────────────────────────────────────
export async function fetchProductById(id: number): Promise<ProductDetail> {
  return apiClient<ProductDetail>(`/api/products/${id}`);
}

// ─── POST /api/products ───────────────────────────────────────────────────────
export async function createProduct(
  payload: CreateProductPayload
): Promise<ProductListItem> {
  return apiClient<ProductListItem>('/api/products', {
    method: 'POST',
    body: JSON.stringify(payload),
  });
}

// ─── PUT /api/products/{id} ───────────────────────────────────────────────────
export async function updateProduct(
  id: number,
  payload: UpdateProductPayload
): Promise<ProductListItem> {
  return apiClient<ProductListItem>(`/api/products/${id}`, {
    method: 'PUT',
    body: JSON.stringify(payload),
  });
}

// ─── DELETE /api/products/{id} ────────────────────────────────────────────────
export async function deleteProduct(id: number): Promise<void> {
  return apiClient<void>(`/api/products/${id}`, { method: 'DELETE' });
}
```

---

## File 3 — `features/products/hooks.ts`

TanStack Query hooks. Components call these — never call api.ts directly.

```ts
import {
  useQuery,
  useMutation,
  useQueryClient,
  keepPreviousData,
} from '@tanstack/react-query';
import {
  fetchProducts,
  fetchProductById,
  createProduct,
  updateProduct,
  deleteProduct,
} from './api';
import type {
  ProductFilters,
  CreateProductPayload,
  UpdateProductPayload,
} from './types';

// ─── Cache key factory ────────────────────────────────────────────────────────
// Keeps query keys consistent across the whole feature.
export const productKeys = {
  all:    ()                        => ['products'] as const,
  lists:  ()                        => ['products', 'list'] as const,
  list:   (filters: ProductFilters) => ['products', 'list', filters] as const,
  detail: (id: number)              => ['products', 'detail', id] as const,
};

// ─── READ: list ───────────────────────────────────────────────────────────────
export function useProducts(filters: ProductFilters = {}) {
  return useQuery({
    queryKey:    productKeys.list(filters),
    queryFn:     () => fetchProducts(filters),
    placeholderData: keepPreviousData, // keeps old data visible while new page loads
  });
}

// ─── READ: single ─────────────────────────────────────────────────────────────
export function useProduct(id: number) {
  return useQuery({
    queryKey: productKeys.detail(id),
    queryFn:  () => fetchProductById(id),
    enabled:  !!id,  // skip if id is 0 / undefined
  });
}

// ─── WRITE: create ────────────────────────────────────────────────────────────
export function useCreateProduct() {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: (payload: CreateProductPayload) => createProduct(payload),
    onSuccess: () => {
      // invalidate the list so the new product appears immediately
      queryClient.invalidateQueries({ queryKey: productKeys.lists() });
    },
  });
}

// ─── WRITE: update ────────────────────────────────────────────────────────────
export function useUpdateProduct(id: number) {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: (payload: UpdateProductPayload) => updateProduct(id, payload),
    onSuccess: () => {
      // refresh both the list and the specific product detail
      queryClient.invalidateQueries({ queryKey: productKeys.lists() });
      queryClient.invalidateQueries({ queryKey: productKeys.detail(id) });
    },
  });
}

// ─── WRITE: delete ────────────────────────────────────────────────────────────
export function useDeleteProduct() {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: (id: number) => deleteProduct(id),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: productKeys.lists() });
    },
  });
}
```

---

## File 4 — `features/products/utils.ts`

Small pure helpers scoped to the Products feature only.

```ts
// Format price with currency (e.g. "25,000 IQD")
export function formatPrice(price: number, currency: string): string {
  return `${price.toLocaleString('en-US')} ${currency}`;
}

// True if stock is at or below the low-stock threshold
export function isLowStock(quantity: number, threshold: number): boolean {
  return quantity <= threshold;
}

// Human-readable date for table cells
export function formatDate(isoString: string): string {
  return new Date(isoString).toLocaleDateString('en-US', {
    year: 'numeric',
    month: 'short',
    day: 'numeric',
  });
}

// Discount percentage shown next to compareAtPrice
export function calcDiscountPercent(
  price: number,
  compareAtPrice: number | null
): number | null {
  if (!compareAtPrice || compareAtPrice <= price) return null;
  return Math.round(((compareAtPrice - price) / compareAtPrice) * 100);
}
```

---

## File 5 — `features/products/components/ProductStatusBadge.tsx`

A tiny presentational component used only inside the products feature.

```tsx
import { cn } from '@/lib/utils';

interface Props {
  status: 'active' | 'draft' | 'archived';
}

const styles: Record<string, string> = {
  active:   'bg-green-100  text-green-700  border-green-200',
  draft:    'bg-yellow-100 text-yellow-700 border-yellow-200',
  archived: 'bg-gray-100   text-gray-500   border-gray-200',
};

export function ProductStatusBadge({ status }: Props) {
  return (
    <span
      className={cn(
        'inline-flex items-center rounded-full border px-2.5 py-0.5 text-xs font-medium capitalize',
        styles[status]
      )}
    >
      {status}
    </span>
  );
}
```

---

## File 6 — `features/products/components/ProductFilters.tsx`

Search + filter controls. Calls no API directly — just updates URL search params
which the parent page reads and passes to `useProducts`.

```tsx
'use client';

import { useRouter, useSearchParams } from 'next/navigation';
import { useCallback } from 'react';

export function ProductFilters() {
  const router       = useRouter();
  const searchParams = useSearchParams();

  const setParam = useCallback(
    (key: string, value: string) => {
      const params = new URLSearchParams(searchParams.toString());
      if (value) params.set(key, value);
      else        params.delete(key);
      params.set('page', '1'); // reset to page 1 on any filter change
      router.push(`?${params.toString()}`);
    },
    [router, searchParams]
  );

  return (
    <div className="mb-4 flex flex-wrap gap-3">
      {/* Search */}
      <input
        type="text"
        placeholder="Search by name, SKU, barcode…"
        defaultValue={searchParams.get('search') ?? ''}
        onChange={(e) => setParam('search', e.target.value)}
        className="h-9 rounded-md border border-input bg-background px-3 text-sm"
      />

      {/* Status filter */}
      <select
        defaultValue={searchParams.get('status') ?? ''}
        onChange={(e) => setParam('status', e.target.value)}
        className="h-9 rounded-md border border-input bg-background px-3 text-sm"
      >
        <option value="">All statuses</option>
        <option value="active">Active</option>
        <option value="draft">Draft</option>
        <option value="archived">Archived</option>
      </select>

      {/* Sort */}
      <select
        defaultValue={searchParams.get('sortBy') ?? 'createdAt'}
        onChange={(e) => setParam('sortBy', e.target.value)}
        className="h-9 rounded-md border border-input bg-background px-3 text-sm"
      >
        <option value="createdAt">Newest first</option>
        <option value="price">Price</option>
        <option value="nameEn">Name</option>
        <option value="stockQuantity">Stock</option>
        <option value="totalSold">Best selling</option>
      </select>
    </div>
  );
}
```

---

## File 7 — `features/products/components/ProductsTable.tsx`

Reads filters from props, calls `useProducts`, renders the table.  
**Zero fetch calls here** — all data comes from the hook.

```tsx
'use client';

import Link from 'next/link';
import { useDeleteProduct, useProducts } from '../hooks';
import { ProductStatusBadge } from './ProductStatusBadge';
import { formatPrice, formatDate, isLowStock } from '../utils';
import type { ProductFilters } from '../types';

interface Props {
  filters: ProductFilters;
}

export function ProductsTable({ filters }: Props) {
  const { data, isLoading, isError } = useProducts(filters);
  const deleteMutation = useDeleteProduct();

  // ── Loading state ──────────────────────────────────────────────────────────
  if (isLoading) {
    return (
      <div className="flex h-48 items-center justify-center text-muted-foreground">
        Loading products…
      </div>
    );
  }

  // ── Error state ────────────────────────────────────────────────────────────
  if (isError) {
    return (
      <div className="flex h-48 items-center justify-center text-red-500">
        Failed to load products. Please try again.
      </div>
    );
  }

  // ── Empty state ────────────────────────────────────────────────────────────
  if (!data || data.items.length === 0) {
    return (
      <div className="flex h-48 items-center justify-center text-muted-foreground">
        No products found.
      </div>
    );
  }

  // ── Table ──────────────────────────────────────────────────────────────────
  return (
    <div>
      <table className="w-full text-sm">
        <thead>
          <tr className="border-b text-left text-muted-foreground">
            <th className="py-3 pr-4">Name</th>
            <th className="py-3 pr-4">SKU</th>
            <th className="py-3 pr-4">Price</th>
            <th className="py-3 pr-4">Stock</th>
            <th className="py-3 pr-4">Status</th>
            <th className="py-3 pr-4">Category</th>
            <th className="py-3 pr-4">Created</th>
            <th className="py-3">Actions</th>
          </tr>
        </thead>
        <tbody>
          {data.items.map((product) => (
            <tr key={product.id} className="border-b hover:bg-muted/40">
              <td className="py-3 pr-4 font-medium">{product.nameEn}</td>
              <td className="py-3 pr-4 text-muted-foreground">
                {product.sku ?? '—'}
              </td>
              <td className="py-3 pr-4">
                {formatPrice(product.price, product.currency)}
              </td>
              <td
                className={`py-3 pr-4 ${
                  isLowStock(product.stockQuantity, 5)
                    ? 'font-semibold text-red-500'
                    : ''
                }`}
              >
                {product.stockQuantity}
              </td>
              <td className="py-3 pr-4">
                <ProductStatusBadge status={product.status} />
              </td>
              <td className="py-3 pr-4 text-muted-foreground">
                {product.category?.nameEn ?? '—'}
              </td>
              <td className="py-3 pr-4 text-muted-foreground">
                {formatDate(product.createdAt)}
              </td>
              <td className="py-3">
                <div className="flex items-center gap-2">
                  <Link
                    href={`/products/${product.id}/edit`}
                    className="rounded px-2 py-1 text-xs font-medium text-blue-600 hover:bg-blue-50"
                  >
                    Edit
                  </Link>
                  <button
                    onClick={() => {
                      if (confirm('Delete this product?')) {
                        deleteMutation.mutate(product.id);
                      }
                    }}
                    disabled={deleteMutation.isPending}
                    className="rounded px-2 py-1 text-xs font-medium text-red-600 hover:bg-red-50 disabled:opacity-50"
                  >
                    Delete
                  </button>
                </div>
              </td>
            </tr>
          ))}
        </tbody>
      </table>

      {/* Pagination */}
      <div className="mt-4 flex items-center justify-between text-sm text-muted-foreground">
        <span>
          {data.total} product{data.total !== 1 ? 's' : ''} total
        </span>
        <span>
          Page {data.page} of {data.totalPages}
        </span>
      </div>
    </div>
  );
}
```

---

## File 8 — `features/products/components/ProductForm.tsx`

Shared by both Create (`/products/new`) and Edit (`/products/[id]/edit`).  
Detects mode automatically from the `defaultValues` prop.

```tsx
'use client';

import { useRouter } from 'next/navigation';
import { useCreateProduct, useUpdateProduct } from '../hooks';
import type { CreateProductPayload, ProductDetail, UpdateProductPayload } from '../types';

interface Props {
  // If provided → Edit mode. If undefined → Create mode.
  defaultValues?: ProductDetail;
}

export function ProductForm({ defaultValues }: Props) {
  const router       = useRouter();
  const isEditMode   = !!defaultValues;

  const createMutation = useCreateProduct();
  const updateMutation = useUpdateProduct(defaultValues?.id ?? 0);

  // Pick the correct mutation based on mode
  const mutation = isEditMode ? updateMutation : createMutation;

  function handleSubmit(e: React.FormEvent<HTMLFormElement>) {
    e.preventDefault();
    const fd = new FormData(e.currentTarget);
    const get = (key: string) => fd.get(key) as string;

    if (isEditMode) {
      const payload: UpdateProductPayload = {
        nameEn:           get('nameEn'),
        nameAr:           get('nameAr') || undefined,
        price:            Number(get('price')),
        sku:              get('sku') || undefined,
        barcode:          get('barcode') || undefined,
        descriptionEn:    get('descriptionEn') || undefined,
        descriptionAr:    get('descriptionAr') || undefined,
        currency:         get('currency') || 'IQD',
        stockQuantity:    Number(get('stockQuantity')),
        lowStockThreshold: Number(get('lowStockThreshold')),
        minOrderQuantity: Number(get('minOrderQuantity')),
        status:           get('status') as 'active' | 'draft' | 'archived',
        isFeatured:       fd.get('isFeatured') === 'true',
        requiresShipping: fd.get('requiresShipping') === 'true',
        warrantyMonths:   Number(get('warrantyMonths')),
        metaTitle:        get('metaTitle') || undefined,
        metaDescription:  get('metaDescription') || undefined,
      };
      updateMutation.mutate(payload, {
        onSuccess: () => router.push('/products'),
      });
    } else {
      const payload: CreateProductPayload = {
        nameEn:  get('nameEn'),
        nameAr:  get('nameAr') || undefined,
        price:   Number(get('price')),
        sku:     get('sku') || undefined,
        status:  get('status') as 'active' | 'draft' | 'archived',
      };
      createMutation.mutate(payload, {
        onSuccess: () => router.push('/products'),
      });
    }
  }

  return (
    <form onSubmit={handleSubmit} className="grid gap-4 max-w-2xl">

      {/* Name EN — required */}
      <div className="grid gap-1">
        <label htmlFor="nameEn" className="text-sm font-medium">
          Product Name (EN) <span className="text-red-500">*</span>
        </label>
        <input
          id="nameEn"
          name="nameEn"
          required
          defaultValue={defaultValues?.nameEn}
          className="rounded-md border border-input px-3 py-2 text-sm"
          placeholder="e.g. Wireless Headphones"
        />
      </div>

      {/* Name AR */}
      <div className="grid gap-1">
        <label htmlFor="nameAr" className="text-sm font-medium">
          Product Name (AR)
        </label>
        <input
          id="nameAr"
          name="nameAr"
          dir="rtl"
          defaultValue={defaultValues?.nameAr ?? ''}
          className="rounded-md border border-input px-3 py-2 text-sm"
          placeholder="اسم المنتج بالعربي"
        />
      </div>

      {/* Price */}
      <div className="grid gap-1">
        <label htmlFor="price" className="text-sm font-medium">
          Price <span className="text-red-500">*</span>
        </label>
        <input
          id="price"
          name="price"
          type="number"
          required
          min={0}
          step="0.01"
          defaultValue={defaultValues?.price ?? 0}
          className="rounded-md border border-input px-3 py-2 text-sm"
        />
      </div>

      {/* SKU */}
      <div className="grid gap-1">
        <label htmlFor="sku" className="text-sm font-medium">SKU</label>
        <input
          id="sku"
          name="sku"
          defaultValue={defaultValues?.sku ?? ''}
          className="rounded-md border border-input px-3 py-2 text-sm"
          placeholder="e.g. WH-001"
        />
      </div>

      {/* Stock Quantity */}
      {isEditMode && (
        <div className="grid gap-1">
          <label htmlFor="stockQuantity" className="text-sm font-medium">
            Stock Quantity
          </label>
          <input
            id="stockQuantity"
            name="stockQuantity"
            type="number"
            min={0}
            defaultValue={defaultValues?.stockQuantity ?? 0}
            className="rounded-md border border-input px-3 py-2 text-sm"
          />
        </div>
      )}

      {/* Status */}
      <div className="grid gap-1">
        <label htmlFor="status" className="text-sm font-medium">Status</label>
        <select
          id="status"
          name="status"
          defaultValue={defaultValues?.status ?? 'draft'}
          className="rounded-md border border-input px-3 py-2 text-sm"
        >
          <option value="active">Active</option>
          <option value="draft">Draft</option>
          <option value="archived">Archived</option>
        </select>
      </div>

      {/* Description EN */}
      <div className="grid gap-1">
        <label htmlFor="descriptionEn" className="text-sm font-medium">
          Description (EN)
        </label>
        <textarea
          id="descriptionEn"
          name="descriptionEn"
          rows={4}
          defaultValue={defaultValues?.descriptionEn ?? ''}
          className="rounded-md border border-input px-3 py-2 text-sm"
        />
      </div>

      {/* Submit */}
      <div className="flex gap-3">
        <button
          type="submit"
          disabled={mutation.isPending}
          className="rounded-md bg-primary px-4 py-2 text-sm font-medium text-primary-foreground disabled:opacity-60"
        >
          {mutation.isPending
            ? 'Saving…'
            : isEditMode
            ? 'Update Product'
            : 'Create Product'}
        </button>
        <button
          type="button"
          onClick={() => router.back()}
          className="rounded-md border px-4 py-2 text-sm font-medium"
        >
          Cancel
        </button>
      </div>

      {/* Error message from backend */}
      {mutation.isError && (
        <p className="text-sm text-red-500">
          {mutation.error?.message ?? 'Something went wrong.'}
        </p>
      )}
    </form>
  );
}
```

---

## Supporting file — `lib/api-client.ts`

Base fetch wrapper used by **all** features. Lives in `lib/`, not inside any feature.

```ts
// Base URL comes from .env  →  NEXT_PUBLIC_API_URL=http://localhost:5000
const BASE_URL = process.env.NEXT_PUBLIC_API_URL ?? 'http://localhost:5000';

export async function apiClient<T>(
  path: string,
  options?: RequestInit
): Promise<T> {
  const res = await fetch(`${BASE_URL}${path}`, {
    headers: {
      'Content-Type': 'application/json',
      // When you add auth: 'Authorization': `Bearer ${getToken()}`
    },
    ...options,
  });

  if (!res.ok) {
    // Backend returns { success: false, errors: [...] }
    const body = await res.json().catch(() => ({}));
    const message =
      body?.errors?.[0] ??
      body?.message ??
      `Request failed with status ${res.status}`;
    throw new Error(message);
  }

  // 204 No Content (DELETE)
  if (res.status === 204) return undefined as T;

  // Backend wraps everything in { success: true, data: {...} }
  const json = await res.json();
  return (json?.data ?? json) as T;
}
```

---

## The page file — `app/(dashboard)/products/page.tsx`

**Thin**. Reads URL params → passes as filters → composes feature components.

```tsx
import { Suspense } from 'react';
import { ProductFilters } from '@/features/products/components/ProductFilters';
import { ProductsTable } from '@/features/products/components/ProductsTable';
import Link from 'next/link';

interface Props {
  searchParams: { [key: string]: string | undefined };
}

export default function ProductsPage({ searchParams }: Props) {
  const filters = {
    page:   Number(searchParams.page  ?? 1),
    limit:  Number(searchParams.limit ?? 10),
    search:  searchParams.search,
    status:  searchParams.status,
    sortBy:  searchParams.sortBy   as any,
    sortOrder: searchParams.sortOrder as any,
  };

  return (
    <div className="p-6">
      {/* Header */}
      <div className="mb-6 flex items-center justify-between">
        <h1 className="text-2xl font-semibold">Products</h1>
        <Link
          href="/products/new"
          className="rounded-md bg-primary px-4 py-2 text-sm font-medium text-primary-foreground"
        >
          + Add Product
        </Link>
      </div>

      {/* Filters */}
      <Suspense>
        <ProductFilters />
      </Suspense>

      {/* Table */}
      <ProductsTable filters={filters} />
    </div>
  );
}
```

---

## Final folder map

```
features/products/
├── types.ts                         ← TypeScript types (mirrors backend DTOs)
├── api.ts                           ← fetch functions (one per endpoint)
├── hooks.ts                         ← TanStack Query hooks (cache, mutations)
├── utils.ts                         ← pure helpers (formatPrice, isLowStock…)
└── components/
    ├── ProductStatusBadge.tsx        ← tiny, presentational
    ├── ProductFilters.tsx            ← search + filter controls
    ├── ProductsTable.tsx             ← data table (uses useProducts hook)
    └── ProductForm.tsx               ← create + edit form (uses useCreateProduct / useUpdateProduct)
```
