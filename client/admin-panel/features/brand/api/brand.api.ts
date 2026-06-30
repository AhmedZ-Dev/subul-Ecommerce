import apiClient, { postFormData } from '@/lib/api-client';
import type { BrandDto, BrandListItem, BrandQueryParams } from '../types';
import type { ApiResponse, PaginatedResponse } from '@/types/api';

interface BackendBrandItem {
  id: number;
  name: string;
  slug: string;
  logoUrl: string | null;
  bannerUrl: string | null;
  descriptionEn: string | null;
  descriptionAr: string | null;
  websiteUrl: string | null;
  isFeatured: boolean;
  isActive: boolean;
  sortOrder: number;
  createdAt: string;
  updatedAt: string | null;
}

function toListItem(raw: BackendBrandItem): BrandListItem {
  return {
    id: raw.id,
    name: raw.name,
    slug: raw.slug,
    logoUrl: raw.logoUrl,
    isFeatured: raw.isFeatured,
    sortOrder: raw.sortOrder,
    status: raw.isActive ? 'active' : 'inactive',
  };
}

function toDto(raw: BackendBrandItem): BrandDto {
  return {
    id: raw.id,
    name: raw.name,
    slug: raw.slug,
    logoUrl: raw.logoUrl,
    bannerUrl: raw.bannerUrl,
    descriptionEn: raw.descriptionEn,
    descriptionAr: raw.descriptionAr,
    websiteUrl: raw.websiteUrl,
    isFeatured: raw.isFeatured,
    sortOrder: raw.sortOrder,
    status: raw.isActive ? 'active' : 'inactive',
    createdAt: raw.createdAt,
    updatedAt: raw.updatedAt,
  };
}

export interface CreateBrandPayload {
  name: string;
  slug?: string;
  descriptionEn?: string;
  descriptionAr?: string;
  websiteUrl?: string;
  isFeatured?: boolean;
  status?: 'active' | 'inactive';
  sortOrder?: number;
}

export interface UpdateBrandPayload {
  name: string;
  slug?: string;
  descriptionEn?: string;
  descriptionAr?: string;
  websiteUrl?: string;
  isFeatured?: boolean;
  status?: 'active' | 'inactive';
  sortOrder?: number;
}

interface BrandImageAssetResponse {
  id: number;
  logoUrl: string | null;
  bannerUrl: string | null;
  updatedAt: string | null;
}

export async function createBrand(payload: CreateBrandPayload): Promise<BrandDto> {
  const { data } = await apiClient.post<ApiResponse<BackendBrandItem>>('/brands', {
    name: payload.name,
    slug: payload.slug ?? null,
    descriptionEn: payload.descriptionEn ?? null,
    descriptionAr: payload.descriptionAr ?? null,
    websiteUrl: payload.websiteUrl ?? null,
    isFeatured: payload.isFeatured ?? false,
    isActive: payload.status !== 'inactive',
    sortOrder: payload.sortOrder ?? 0,
  });
  if (!data.success) throw new Error(data.message ?? 'Failed to create brand');
  return toDto(data.data!);
}

export async function updateBrand(
  id: number,
  payload: UpdateBrandPayload,
): Promise<BrandDto> {
  const { data } = await apiClient.put<ApiResponse<BackendBrandItem>>(`/brands/${id}`, {
    name: payload.name,
    slug: payload.slug ?? null,
    descriptionEn: payload.descriptionEn ?? null,
    descriptionAr: payload.descriptionAr ?? null,
    websiteUrl: payload.websiteUrl ?? null,
    isFeatured: payload.isFeatured ?? false,
    isActive: payload.status !== 'inactive',
    sortOrder: payload.sortOrder ?? 0,
  });
  if (!data.success) throw new Error(data.message ?? 'Failed to update brand');
  return toDto(data.data!);
}

export async function deleteBrand(id: number): Promise<void> {
  const { data } = await apiClient.delete<ApiResponse<null>>(`/brands/${id}`);
  if (!data.success) throw new Error(data.message ?? 'Failed to delete brand');
}

export async function getBrandById(id: number): Promise<BrandDto | null> {
  const { data } = await apiClient.get<ApiResponse<BackendBrandItem>>(`/brands/${id}`);
  if (!data.success) return null;
  return data.data ? toDto(data.data) : null;
}

export async function getBrands(
  params: BrandQueryParams = {},
): Promise<PaginatedResponse<BrandListItem>> {
  const { page = 1, limit = 10, search, status, isFeatured, sortBy, sortOrder } = params;

  const { data } = await apiClient.get<ApiResponse<{ items: BackendBrandItem[]; total: number; page: number; limit: number; totalPages: number }>>(
    '/brands',
    {
      params: {
        page,
        limit,
        ...(search && { search }),
        ...(status !== undefined && { isActive: status === 'active' }),
        ...(isFeatured !== undefined && { isFeatured }),
        ...(sortBy && { sortBy }),
        ...(sortOrder && { sortOrder }),
      },
    },
  );

  if (!data.success) throw new Error(data.message ?? 'Failed to fetch brands');

  const raw = data.data!;
  return {
    items: raw.items.map(toListItem),
    total: raw.total,
    page: raw.page,
    limit: raw.limit,
    totalPages: raw.totalPages,
  };
}

export async function uploadBrandLogo(id: number, file: File): Promise<BrandImageAssetResponse> {
  const formData = new FormData();
  formData.append('Image', file);
  return postFormData<BrandImageAssetResponse>(`/brands/${id}/logo`, formData);
}

export async function uploadBrandBanner(id: number, file: File): Promise<BrandImageAssetResponse> {
  const formData = new FormData();
  formData.append('Image', file);
  return postFormData<BrandImageAssetResponse>(`/brands/${id}/banner`, formData);
}

export async function deleteBrandLogo(id: number): Promise<BrandImageAssetResponse> {
  const { data } = await apiClient.delete<ApiResponse<BrandImageAssetResponse>>(`/brands/${id}/logo`);
  if (!data.success) throw new Error(data.message ?? 'Failed to delete brand logo');
  return data.data!;
}

export async function deleteBrandBanner(id: number): Promise<BrandImageAssetResponse> {
  const { data } = await apiClient.delete<ApiResponse<BrandImageAssetResponse>>(`/brands/${id}/banner`);
  if (!data.success) throw new Error(data.message ?? 'Failed to delete brand banner');
  return data.data!;
}
