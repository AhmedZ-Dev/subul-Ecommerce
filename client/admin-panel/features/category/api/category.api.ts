// features/category/api/category.api.ts
// Pure HTTP functions — transform backend shapes ↔ frontend domain types.
// No React, no hooks, no 'use server' — callable from RSCs, hooks, or actions.

import apiClient from '@/lib/api-client';
import type { CategoryDto, CategoryListItem, CategoryQueryParams } from '../types';
import type { ApiResponse, PaginatedResponse } from '@/types/api';

// ─── Raw backend response shapes ─────────────────────────────────────────────
// Mirrors the C# record definitions exactly (camelCase after JSON serialization).

interface BackendParentInfo {
  id: number;
  nameEn: string;
  nameAr: string | null;
}

interface BackendCount {
  product: number;
  subCategory: number;
}

interface BackendCategoryItem {
  id: number;
  parentId: number | null;
  nameEn: string;
  nameAr: string | null;
  slug: string;
  descriptionEn: string | null;
  descriptionAr: string | null;
  imageUrl: string | null;
  bannerUrl: string | null;
  sortOrder: number;
  isActive: boolean;
  seoTitle: string | null;
  seoDescription: string | null;
  createdAt: string;
  updatedAt: string | null;
  parent: BackendParentInfo | null;
  _count: BackendCount;
}

// ─── Transform helpers ────────────────────────────────────────────────────────

function toListItem(raw: BackendCategoryItem): CategoryListItem {
  return {
    id: raw.id,
    nameEn: raw.nameEn,
    nameAr: raw.nameAr,
    slug: raw.slug,
    parentId: raw.parentId,
    parentNameEn: raw.parent?.nameEn ?? null,
    sortOrder: raw.sortOrder,
    status: raw.isActive ? 'active' : 'inactive',
  };
}

function toDto(raw: BackendCategoryItem): CategoryDto {
  return {
    id: raw.id,
    nameEn: raw.nameEn,
    nameAr: raw.nameAr,
    slug: raw.slug,
    descriptionEn: raw.descriptionEn,
    descriptionAr: raw.descriptionAr,
    parentId: raw.parentId,
    parentNameEn: raw.parent?.nameEn ?? null,
    parentNameAr: raw.parent?.nameAr ?? null,
    sortOrder: raw.sortOrder,
    status: raw.isActive ? 'active' : 'inactive',
    createdAt: raw.createdAt,
    updatedAt: raw.updatedAt,
  };
}

// ─── Payload shapes — mirror backend command/request contracts ────────────────

export interface CreateCategoryPayload {
  nameEn: string;
  nameAr?: string;
  slug?: string;
  descriptionEn?: string;
  descriptionAr?: string;
  parentId?: number | null;
  status?: 'active' | 'inactive';
}

export interface UpdateCategoryPayload {
  nameEn: string;
  nameAr?: string;
  slug?: string;
  descriptionEn?: string;
  descriptionAr?: string;
  parentId?: number | null;
  status?: 'active' | 'inactive';
  sortOrder?: number;
}

export interface ChangeCategoryStatusPayload {
  isActive: boolean;
}

export interface ChangeCategoryStatusResult {
  id: number;
  status: 'active' | 'inactive';
  updatedAt: string | null;
}

// ─── CreateCategory ───────────────────────────────────────────────────────────

export async function createCategory(payload: CreateCategoryPayload): Promise<CategoryDto> {
  const { data } = await apiClient.post<ApiResponse<BackendCategoryItem>>('/categories', {
    nameEn: payload.nameEn,
    nameAr: payload.nameAr,
    slug: payload.slug,
    descriptionEn: payload.descriptionEn,
    descriptionAr: payload.descriptionAr,
    parentId: payload.parentId ?? null,
    isActive: payload.status !== 'inactive',
    sortOrder: 0,
  });
  if (!data.success) throw new Error(data.message ?? 'Failed to create category');
  return toDto(data.data!);
}

// ─── UpdateCategory ───────────────────────────────────────────────────────────

export async function updateCategory(
  id: number,
  payload: UpdateCategoryPayload,
): Promise<CategoryDto> {
  const { data } = await apiClient.put<ApiResponse<BackendCategoryItem>>(`/categories/${id}`, {
    nameEn: payload.nameEn,
    nameAr: payload.nameAr ?? null,
    slug: payload.slug ?? null,
    descriptionEn: payload.descriptionEn ?? null,
    descriptionAr: payload.descriptionAr ?? null,
    parentId: payload.parentId ?? null,
    isActive: payload.status !== 'inactive',
    sortOrder: payload.sortOrder ?? 0,
    imageUrl: null,
    bannerUrl: null,
    seoTitle: null,
    seoDescription: null,
  });
  if (!data.success) throw new Error(data.message ?? 'Failed to update category');
  return toDto(data.data!);
}

// ─── DeleteCategory ───────────────────────────────────────────────────────────

export async function deleteCategory(id: number): Promise<void> {
  const { data } = await apiClient.delete<ApiResponse<null>>(`/categories/${id}`);
  if (!data.success) throw new Error(data.message ?? 'Failed to delete category');
}

// ─── ChangeCategoryStatus ─────────────────────────────────────────────────────

export async function changeCategoryStatus(
  id: number,
  payload: ChangeCategoryStatusPayload,
): Promise<ChangeCategoryStatusResult> {
  const { data } = await apiClient.put<
    ApiResponse<{
      id: number;
      isActive: boolean;
      updatedAt: string | null;
    }>
  >(`/categories/${id}/status`, {
    isActive: payload.isActive,
  });

  if (!data.success) {
    throw new Error(data.message ?? 'Failed to change category status');
  }

  const raw = data.data!;
  return {
    id: raw.id,
    status: raw.isActive ? 'active' : 'inactive',
    updatedAt: raw.updatedAt,
  };
}

// ─── GetByIdCategory ──────────────────────────────────────────────────────────

export async function getCategoryById(id: number): Promise<CategoryDto | null> {
  const { data } = await apiClient.get<ApiResponse<BackendCategoryItem>>(`/categories/${id}`);
  if (!data.success) return null;
  return data.data ? toDto(data.data) : null;
}

// ─── ListCategoryPaginated ────────────────────────────────────────────────────

export async function getCategories(
  params: CategoryQueryParams = {},
): Promise<PaginatedResponse<CategoryListItem>> {
  const { page = 1, limit = 10, search, parentId, status, sortBy, sortOrder } = params;

  const { data } = await apiClient.get<ApiResponse<{ items: BackendCategoryItem[]; total: number; page: number; limit: number; totalPages: number }>>(
    '/categories',
    {
      params: {
        page,
        limit,
        ...(search && { search }),
        ...(parentId != null && { parentId }),
        // map status → isActive for the backend filter
        ...(status !== undefined && { isActive: status === 'active' }),
        ...(sortBy && { sortBy }),
        ...(sortOrder && { sortOrder }),
      },
    },
  );

  if (!data.success) throw new Error(data.message ?? 'Failed to fetch categories');

  const raw = data.data!;
  return {
    items: raw.items.map(toListItem),
    total: raw.total,
    page: raw.page,
    limit: raw.limit,
    totalPages: raw.totalPages,
  };
}
