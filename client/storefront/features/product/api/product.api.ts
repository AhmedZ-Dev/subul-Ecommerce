import apiClient from "@/lib/api-client"
import type { ApiResponse, PaginatedResponse } from "@/types/api"
import type {
  ProductAttributeValueInfo,
  ProductBrandInfo,
  ProductCategoryInfo,
  ProductFilterOptions,
  ProductImageInfo,
  ProductQueryParams,
  ProductVariantInfo,
  StorefrontProductDetail,
  StorefrontProductListItem,
} from "../types"

interface BackendCategoryInfo {
  id: number
  nameEn: string
  nameAr: string | null
  slug: string
}

interface BackendBrandInfo {
  id: number
  name: string
  slug: string
}

interface BackendProductListItem {
  id: number
  nameEn: string
  nameAr: string | null
  slug: string
  price: number
  compareAtPrice: number | null
  currency: string
  stockQuantity: number
  isFeatured: boolean
  category: BackendCategoryInfo | null
  brand: BackendBrandInfo | null
  primaryImageUrl: string | null
}

interface BackendVariantInfo {
  id: number
  title: string | null
  sku: string | null
  price: number | null
  compareAtPrice: number | null
  stockQuantity: number
  isActive: boolean
  sortOrder: number
}

interface BackendAttributeValueInfo {
  id: number
  attributeId: number
  valueText: string | null
  valueNumber: number | null
  valueBoolean: boolean | null
  attribute: {
    nameEn: string
    nameAr: string | null
    unit: string | null
    inputType: string
    sortOrder: number
  }
}

interface BackendProductDetail extends BackendProductListItem {
  status: string
  descriptionEn: string | null
  descriptionAr: string | null
  shortDescriptionEn: string | null
  shortDescriptionAr: string | null
  minOrderQuantity: number
  weight: number | null
  requiresShipping: boolean
  warrantyMonths: number
  warrantyDescription: string | null
  metaTitle: string | null
  metaDescription: string | null
  categoryId: number | null
  brandId: number | null
  variants: BackendVariantInfo[]
  attributeValues: BackendAttributeValueInfo[]
}

interface BackendProductImage {
  id: number
  productId: number
  variantId: number | null
  imageUrl: string
  altText: string | null
  sortOrder: number
  isPrimary: boolean
}

function toListItem(raw: BackendProductListItem): StorefrontProductListItem {
  return {
    id: raw.id,
    nameEn: raw.nameEn,
    nameAr: raw.nameAr,
    slug: raw.slug,
    price: raw.price,
    compareAtPrice: raw.compareAtPrice,
    currency: raw.currency,
    stockQuantity: raw.stockQuantity,
    isFeatured: raw.isFeatured,
    category: raw.category,
    brand: raw.brand,
    primaryImageUrl: raw.primaryImageUrl ?? null,
  }
}

function toVariant(raw: BackendVariantInfo): ProductVariantInfo {
  return {
    id: raw.id,
    title: raw.title,
    sku: raw.sku,
    price: raw.price,
    compareAtPrice: raw.compareAtPrice,
    stockQuantity: raw.stockQuantity,
    isActive: raw.isActive,
    sortOrder: raw.sortOrder,
  }
}

function toAttributeValue(raw: BackendAttributeValueInfo): ProductAttributeValueInfo {
  return {
    id: raw.id,
    attributeId: raw.attributeId,
    valueText: raw.valueText,
    valueNumber: raw.valueNumber,
    valueBoolean: raw.valueBoolean,
    attribute: raw.attribute,
  }
}

function toDetail(raw: BackendProductDetail): StorefrontProductDetail {
  return {
    ...toListItem(raw),
    descriptionEn: raw.descriptionEn,
    descriptionAr: raw.descriptionAr,
    shortDescriptionEn: raw.shortDescriptionEn,
    shortDescriptionAr: raw.shortDescriptionAr,
    minOrderQuantity: raw.minOrderQuantity,
    weight: raw.weight,
    requiresShipping: raw.requiresShipping,
    warrantyMonths: raw.warrantyMonths,
    warrantyDescription: raw.warrantyDescription,
    metaTitle: raw.metaTitle,
    metaDescription: raw.metaDescription,
    categoryId: raw.categoryId,
    brandId: raw.brandId,
    variants: (raw.variants ?? []).map(toVariant),
    attributeValues: (raw.attributeValues ?? []).map(toAttributeValue),
  }
}

export async function getStorefrontProducts(
  params: ProductQueryParams = {},
): Promise<PaginatedResponse<StorefrontProductListItem>> {
  const {
    page = 1,
    limit = 24,
    search,
    categoryId,
    brandId,
    brandIds,
    minPrice,
    maxPrice,
    inStockOnly,
    attrs,
    isFeatured,
    sortBy,
    sortOrder,
  } = params

  const hasBrandIds = brandIds != null && brandIds.length > 0
  const hasAttrs = attrs != null && Object.keys(attrs).length > 0

  const { data } = await apiClient.get<
    ApiResponse<{
      items: BackendProductListItem[]
      total: number
      page: number
      limit: number
      totalPages: number
    }>
  >("/products", {
    params: {
      page,
      limit,
      status: "active",
      ...(search && { search }),
      ...(categoryId != null && { categoryId }),
      ...(hasBrandIds
        ? { brandIds }
        : brandId != null
          ? { brandId }
          : {}),
      ...(minPrice != null && { minPrice }),
      ...(maxPrice != null && { maxPrice }),
      ...(inStockOnly === true && { inStockOnly: true }),
      ...(hasAttrs && { attrs: JSON.stringify(attrs) }),
      ...(isFeatured !== undefined && { isFeatured }),
      ...(sortBy && { sortBy }),
      ...(sortOrder && { sortOrder }),
    },
  })

  if (!data.success) throw new Error(data.message ?? "Failed to fetch products")

  const raw = data.data!
  return {
    items: raw.items.map(toListItem),
    total: raw.total,
    page: raw.page,
    limit: raw.limit,
    totalPages: raw.totalPages,
  }
}

interface BackendBrandFacet {
  id: number
  name: string
  slug: string
  count: number
}

interface BackendPriceRangeFacet {
  min: number
  max: number
}

interface BackendAttributeValueFacet {
  value: string
  count: number
}

interface BackendAttributeGroupFacet {
  id: number
  nameEn: string
  nameAr: string | null
  values: BackendAttributeValueFacet[]
}

interface BackendProductFilterOptions {
  brands: BackendBrandFacet[]
  priceRange: BackendPriceRangeFacet
  attributeGroups: BackendAttributeGroupFacet[]
}

export async function getProductFilterOptions(
  categoryId?: number,
): Promise<ProductFilterOptions> {
  const { data } = await apiClient.get<ApiResponse<BackendProductFilterOptions>>(
    "/products/filter-options",
    {
      params: {
        ...(categoryId != null && { categoryId }),
      },
    },
  )

  if (!data.success) throw new Error(data.message ?? "Failed to fetch filter options")

  const raw = data.data!
  return {
    brands: raw.brands,
    priceRange: raw.priceRange,
    attributeGroups: raw.attributeGroups,
  }
}

export async function getStorefrontProductById(
  id: number,
): Promise<StorefrontProductDetail | null> {
  const { data } = await apiClient.get<ApiResponse<BackendProductDetail>>(
    `/products/${id}`,
  )
  if (!data.success) return null
  if (!data.data || data.data.status?.toLowerCase() !== "active") return null
  return toDetail(data.data)
}

export async function getStorefrontProductBySlug(
  slug: string,
): Promise<StorefrontProductDetail | null> {
  const normalized = slug.trim().toLowerCase()
  if (!normalized) return null

  const { data } = await apiClient.get<ApiResponse<BackendProductDetail>>(
    `/products/by-slug/${encodeURIComponent(normalized)}`,
  )
  if (!data.success) return null
  if (!data.data || data.data.status?.toLowerCase() !== "active") return null
  return toDetail(data.data)
}

export async function getStorefrontProductImages(
  productId: number,
): Promise<ProductImageInfo[]> {
  const { data } = await apiClient.get<
    ApiResponse<{
      items: BackendProductImage[]
      total: number
      page: number
      limit: number
      totalPages: number
    }>
  >(`/products/${productId}/images`, {
    params: { sortBy: "sortOrder", sortOrder: "asc", limit: 50 },
  })

  if (!data.success) return []
  return (data.data?.items ?? []).map((img) => ({
    id: img.id,
    productId: img.productId,
    variantId: img.variantId,
    imageUrl: img.imageUrl,
    altText: img.altText,
    sortOrder: img.sortOrder,
    isPrimary: img.isPrimary,
  }))
}
