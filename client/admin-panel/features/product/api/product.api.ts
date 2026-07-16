// features/product/api/product.api.ts
import apiClient from '@/lib/api-client';
import type {
  ProductAttributeValueInfo,
  ProductDto,
  ProductListItem,
  ProductQueryParams,
  ProductStatus,
  ProductVariantInfo,
} from '../types';
import type { ApiResponse, PaginatedResponse } from '@/types/api';

interface BackendCategoryInfo {
  id: number;
  nameEn: string;
  nameAr: string | null;
  slug: string;
}

interface BackendBrandInfo {
  id: number;
  name: string;
  slug: string;
}

interface BackendProductListItem {
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
  status: string;
  isFeatured: boolean;
  totalSold: number;
  viewsCount: number;
  createdAt: string;
  updatedAt: string | null;
  category: BackendCategoryInfo | null;
  brand: BackendBrandInfo | null;
}

interface BackendVariantInfo {
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

interface BackendAttributeValueInfo {
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

interface BackendProductDetail extends BackendProductListItem {
  descriptionEn: string | null;
  descriptionAr: string | null;
  costPrice: number | null;
  lowStockThreshold: number;
  minOrderQuantity: number;
  weight: number | null;
  requiresShipping: boolean;
  warrantyMonths: number;
  warrantyDescription: string | null;
  metaTitle: string | null;
  metaDescription: string | null;
  variants: BackendVariantInfo[];
  attributeValues: BackendAttributeValueInfo[];
}

function normalizeStatus(status: string): ProductStatus {
  const normalized = status.toLowerCase();
  if (normalized === 'draft' || normalized === 'archived') return normalized;
  return 'active';
}

function toListItem(raw: BackendProductListItem): ProductListItem {
  return {
    id: raw.id,
    nameEn: raw.nameEn,
    nameAr: raw.nameAr,
    slug: raw.slug,
    sku: raw.sku,
    price: raw.price,
    currency: raw.currency,
    stockQuantity: raw.stockQuantity,
    status: normalizeStatus(raw.status),
    isFeatured: raw.isFeatured,
    totalSold: raw.totalSold,
    category: raw.category,
    brand: raw.brand,
    createdAt: raw.createdAt,
  };
}

function toVariantInfo(raw: BackendVariantInfo): ProductVariantInfo {
  return {
    id: raw.id,
    title: raw.title,
    sku: raw.sku,
    barcode: raw.barcode,
    price: raw.price,
    compareAtPrice: raw.compareAtPrice,
    costPrice: raw.costPrice,
    stockQuantity: raw.stockQuantity,
    weight: raw.weight,
    isActive: raw.isActive,
    sortOrder: raw.sortOrder,
  };
}

function toAttributeValueInfo(raw: BackendAttributeValueInfo): ProductAttributeValueInfo {
  return {
    id: raw.id,
    attributeId: raw.attributeId,
    valueText: raw.valueText,
    valueNumber: raw.valueNumber,
    valueBoolean: raw.valueBoolean,
    attribute: raw.attribute,
  };
}

function toDto(raw: BackendProductDetail): ProductDto {
  return {
    id: raw.id,
    nameEn: raw.nameEn,
    nameAr: raw.nameAr,
    slug: raw.slug,
    sku: raw.sku,
    barcode: raw.barcode,
    descriptionEn: raw.descriptionEn,
    descriptionAr: raw.descriptionAr,
    shortDescriptionEn: raw.shortDescriptionEn,
    shortDescriptionAr: raw.shortDescriptionAr,
    price: raw.price,
    compareAtPrice: raw.compareAtPrice,
    costPrice: raw.costPrice,
    currency: raw.currency,
    stockQuantity: raw.stockQuantity,
    lowStockThreshold: raw.lowStockThreshold,
    minOrderQuantity: raw.minOrderQuantity,
    weight: raw.weight,
    status: normalizeStatus(raw.status),
    isFeatured: raw.isFeatured,
    requiresShipping: raw.requiresShipping,
    warrantyMonths: raw.warrantyMonths,
    warrantyDescription: raw.warrantyDescription,
    totalSold: raw.totalSold,
    viewsCount: raw.viewsCount,
    metaTitle: raw.metaTitle,
    metaDescription: raw.metaDescription,
    categoryId: raw.categoryId,
    brandId: raw.brandId,
    category: raw.category,
    brand: raw.brand,
    variants: (raw.variants ?? []).map(toVariantInfo),
    attributeValues: (raw.attributeValues ?? []).map(toAttributeValueInfo),
    createdAt: raw.createdAt,
    updatedAt: raw.updatedAt,
  };
}

export interface CreateProductPayload {
  nameEn: string;
  nameAr?: string;
  categoryId?: number | null;
  brandId?: number | null;
  slug?: string;
  sku?: string;
  barcode?: string;
  descriptionEn?: string;
  descriptionAr?: string;
  shortDescriptionEn?: string;
  shortDescriptionAr?: string;
  price?: number;
  compareAtPrice?: number | null;
  costPrice?: number | null;
  currency?: string;
  stockQuantity?: number;
  lowStockThreshold?: number;
  minOrderQuantity?: number;
  weight?: number | null;
  status?: ProductStatus;
  isFeatured?: boolean;
  requiresShipping?: boolean;
  warrantyMonths?: number;
  warrantyDescription?: string;
  metaTitle?: string;
  metaDescription?: string;
}

export interface UpdateProductPayload {
  nameEn: string;
  nameAr?: string | null;
  categoryId?: number | null;
  brandId?: number | null;
  slug?: string | null;
  sku?: string | null;
  barcode?: string | null;
  descriptionEn?: string | null;
  descriptionAr?: string | null;
  shortDescriptionEn?: string | null;
  shortDescriptionAr?: string | null;
  price: number;
  compareAtPrice?: number | null;
  costPrice?: number | null;
  currency: string;
  stockQuantity: number;
  lowStockThreshold: number;
  minOrderQuantity: number;
  weight?: number | null;
  status: ProductStatus;
  isFeatured: boolean;
  requiresShipping: boolean;
  warrantyMonths: number;
  warrantyDescription?: string | null;
  metaTitle?: string | null;
  metaDescription?: string | null;
}

function buildCreateBody(payload: CreateProductPayload) {
  return {
    nameEn: payload.nameEn,
    nameAr: payload.nameAr ?? null,
    categoryId: payload.categoryId ?? null,
    brandId: payload.brandId ?? null,
    slug: payload.slug ?? null,
    sku: payload.sku ?? null,
    barcode: payload.barcode ?? null,
    descriptionEn: payload.descriptionEn ?? null,
    descriptionAr: payload.descriptionAr ?? null,
    shortDescriptionEn: payload.shortDescriptionEn ?? null,
    shortDescriptionAr: payload.shortDescriptionAr ?? null,
    price: payload.price ?? 0,
    compareAtPrice: payload.compareAtPrice ?? null,
    costPrice: payload.costPrice ?? null,
    currency: payload.currency ?? 'IQD',
    stockQuantity: payload.stockQuantity ?? 0,
    lowStockThreshold: payload.lowStockThreshold ?? 2,
    minOrderQuantity: payload.minOrderQuantity ?? 1,
    weight: payload.weight ?? null,
    status: payload.status ?? 'active',
    isFeatured: payload.isFeatured ?? false,
    requiresShipping: payload.requiresShipping ?? true,
    warrantyMonths: payload.warrantyMonths ?? 12,
    warrantyDescription: payload.warrantyDescription ?? null,
    metaTitle: payload.metaTitle ?? null,
    metaDescription: payload.metaDescription ?? null,
  };
}

function buildUpdateBody(payload: UpdateProductPayload) {
  return {
    nameEn: payload.nameEn,
    nameAr: payload.nameAr ?? null,
    categoryId: payload.categoryId ?? null,
    brandId: payload.brandId ?? null,
    slug: payload.slug ?? null,
    sku: payload.sku ?? null,
    barcode: payload.barcode ?? null,
    descriptionEn: payload.descriptionEn ?? null,
    descriptionAr: payload.descriptionAr ?? null,
    shortDescriptionEn: payload.shortDescriptionEn ?? null,
    shortDescriptionAr: payload.shortDescriptionAr ?? null,
    price: payload.price,
    compareAtPrice: payload.compareAtPrice ?? null,
    costPrice: payload.costPrice ?? null,
    currency: payload.currency,
    stockQuantity: payload.stockQuantity,
    lowStockThreshold: payload.lowStockThreshold,
    minOrderQuantity: payload.minOrderQuantity,
    weight: payload.weight ?? null,
    status: payload.status,
    isFeatured: payload.isFeatured,
    requiresShipping: payload.requiresShipping,
    warrantyMonths: payload.warrantyMonths,
    warrantyDescription: payload.warrantyDescription ?? null,
    metaTitle: payload.metaTitle ?? null,
    metaDescription: payload.metaDescription ?? null,
  };
}

export async function createProduct(payload: CreateProductPayload): Promise<ProductDto> {
  const { data } = await apiClient.post<ApiResponse<BackendProductDetail>>(
    '/products',
    buildCreateBody(payload),
  );
  if (!data.success) throw new Error(data.message ?? 'Failed to create product');
  return toDto(data.data!);
}

export async function updateProduct(
  id: number,
  payload: UpdateProductPayload,
): Promise<ProductDto> {
  const { data } = await apiClient.put<ApiResponse<BackendProductDetail>>(
    `/products/${id}`,
    buildUpdateBody(payload),
  );
  if (!data.success) throw new Error(data.message ?? 'Failed to update product');
  return toDto(data.data!);
}

export async function deleteProduct(id: number): Promise<void> {
  const { data } = await apiClient.delete<ApiResponse<boolean>>(`/products/${id}`);
  if (!data.success) throw new Error(data.message ?? 'Failed to delete product');
}

export async function getProductById(id: number): Promise<ProductDto | null> {
  const { data } = await apiClient.get<ApiResponse<BackendProductDetail>>(`/products/${id}`);
  if (!data.success) return null;
  return data.data ? toDto(data.data) : null;
}

export async function getProducts(
  params: ProductQueryParams = {},
): Promise<PaginatedResponse<ProductListItem>> {
  const {
    page = 1,
    limit = 10,
    search,
    categoryId,
    brandId,
    status,
    isFeatured,
    sortBy,
    sortOrder,
  } = params;

  const { data } = await apiClient.get<
    ApiResponse<{
      items: BackendProductListItem[];
      total: number;
      page: number;
      limit: number;
      totalPages: number;
    }>
  >('/products', {
    params: {
      page,
      limit,
      ...(search && { search }),
      ...(categoryId != null && { categoryId }),
      ...(brandId != null && { brandId }),
      ...(status && { status }),
      ...(isFeatured !== undefined && { isFeatured }),
      ...(sortBy && { sortBy }),
      ...(sortOrder && { sortOrder }),
    },
  });

  if (!data.success) throw new Error(data.message ?? 'Failed to fetch products');

  const raw = data.data!;
  return {
    items: raw.items.map(toListItem),
    total: raw.total,
    page: raw.page,
    limit: raw.limit,
    totalPages: raw.totalPages,
  };
}
