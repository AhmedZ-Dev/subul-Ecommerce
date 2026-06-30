import apiClient from '@/lib/api-client';
import type { AttributeGroupDto, AttributeGroupListItem, AttributeGroupQueryParams } from '../types';
import type { ApiResponse, PaginatedResponse } from '@/types/api';

// ─── Raw backend response shapes ─────────────────────────────────────────────

interface BackendAttribute {
  id: number;
  nameEn: string;
  nameAr: string | null;
  slug: string;
  unit: string | null;
  inputType: string;
  isFilterable: boolean;
  sortOrder: number;
  createdAt: string;
}

interface BackendAttributeGroupItem {
  id: number;
  nameEn: string;
  nameAr: string | null;
  slug: string;
  sortOrder: number;
  isFilterable: boolean;
  createdAt: string;
  attributes: BackendAttribute[];
}

interface BackendAttributeGroupListItem {
  id: number;
  nameEn: string;
  nameAr: string | null;
  slug: string;
  sortOrder: number;
  isFilterable: boolean;
  createdAt: string;
  attributeCount: number;
}

// ─── Transform helpers ────────────────────────────────────────────────────────

function toListItem(raw: BackendAttributeGroupListItem): AttributeGroupListItem {
  return {
    id: raw.id,
    nameEn: raw.nameEn,
    nameAr: raw.nameAr,
    slug: raw.slug,
    sortOrder: raw.sortOrder,
    isFilterable: raw.isFilterable,
    createdAt: raw.createdAt,
    attributeCount: raw.attributeCount,
  };
}

function toDto(raw: BackendAttributeGroupItem): AttributeGroupDto {
  return {
    id: raw.id,
    nameEn: raw.nameEn,
    nameAr: raw.nameAr,
    slug: raw.slug,
    sortOrder: raw.sortOrder,
    isFilterable: raw.isFilterable,
    createdAt: raw.createdAt,
    attributes: raw.attributes.map(attr => ({
      ...attr,
    })),
  };
}

// ─── Payload shapes ───────────────────────────────────────────────────────────

export interface AttributePayload {
  id?: number | null;
  nameEn: string;
  nameAr?: string | null;
  slug?: string | null;
  unit?: string | null;
  inputType: string;
  isFilterable: boolean;
  sortOrder: number;
}

export interface CreateAttributeGroupPayload {
  nameEn: string;
  nameAr?: string | null;
  slug?: string | null;
  sortOrder: number;
  isFilterable: boolean;
  attributes: AttributePayload[];
}

export interface UpdateAttributeGroupPayload {
  nameEn: string;
  nameAr?: string | null;
  slug?: string | null;
  sortOrder: number;
  isFilterable: boolean;
  attributes: AttributePayload[];
}

// ─── API Methods ──────────────────────────────────────────────────────────────

export async function createAttributeGroup(payload: CreateAttributeGroupPayload): Promise<AttributeGroupDto> {
  const { data } = await apiClient.post<ApiResponse<BackendAttributeGroupItem>>('/attribute-groups', {
    nameEn: payload.nameEn,
    nameAr: payload.nameAr ?? null,
    slug: payload.slug ?? null,
    sortOrder: payload.sortOrder,
    isFilterable: payload.isFilterable,
    attributes: payload.attributes,
  });
  if (!data.success) throw new Error(data.message ?? 'Failed to create attribute group');
  return toDto(data.data!);
}

export async function updateAttributeGroup(
  id: number,
  payload: UpdateAttributeGroupPayload,
): Promise<AttributeGroupDto> {
  const { data } = await apiClient.put<ApiResponse<BackendAttributeGroupItem>>(`/attribute-groups/${id}`, {
    nameEn: payload.nameEn,
    nameAr: payload.nameAr ?? null,
    slug: payload.slug ?? null,
    sortOrder: payload.sortOrder,
    isFilterable: payload.isFilterable,
    attributes: payload.attributes,
  });
  if (!data.success) throw new Error(data.message ?? 'Failed to update attribute group');
  return toDto(data.data!);
}

export async function deleteAttributeGroup(id: number): Promise<void> {
  const { data } = await apiClient.delete<ApiResponse<null>>(`/attribute-groups/${id}`);
  if (!data.success) throw new Error(data.message ?? 'Failed to delete attribute group');
}

export async function getAttributeGroupById(id: number): Promise<AttributeGroupDto | null> {
  const { data } = await apiClient.get<ApiResponse<BackendAttributeGroupItem>>(`/attribute-groups/${id}`);
  if (!data.success) return null;
  return data.data ? toDto(data.data) : null;
}

export async function getAttributeGroups(
  params: AttributeGroupQueryParams = {},
): Promise<PaginatedResponse<AttributeGroupListItem>> {
  const { page = 1, limit = 10, search, isFilterable, sortBy, sortOrder } = params;

  const { data } = await apiClient.get<ApiResponse<{ items: BackendAttributeGroupListItem[]; total: number; page: number; limit: number; totalPages: number }>>(
    '/attribute-groups',
    {
      params: {
        page,
        limit,
        ...(search && { search }),
        ...(isFilterable !== undefined && { isFilterable }),
        ...(sortBy && { sortBy }),
        ...(sortOrder && { sortOrder }),
      },
    },
  );

  if (!data.success) throw new Error(data.message ?? 'Failed to fetch attribute groups');

  const raw = data.data!;
  return {
    items: raw.items.map(toListItem),
    total: raw.total,
    page: raw.page,
    limit: raw.limit,
    totalPages: raw.totalPages,
  };
}
