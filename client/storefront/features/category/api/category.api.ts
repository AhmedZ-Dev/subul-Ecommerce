import apiClient from "@/lib/api-client"
import type { ApiResponse, PaginatedResponse } from "@/types/api"
import type { CategoryDto, CategoryListItem, CategoryQueryParams } from "../types"

interface BackendParentInfo {
  id: number
  nameEn: string
  nameAr: string | null
}

interface BackendCategoryItem {
  id: number
  parentId: number | null
  nameEn: string
  nameAr: string | null
  slug: string
  descriptionEn: string | null
  descriptionAr: string | null
  imageUrl: string | null
  sortOrder: number
  isActive: boolean
  parent: BackendParentInfo | null
}

function toListItem(raw: BackendCategoryItem): CategoryListItem {
  return {
    id: raw.id,
    nameEn: raw.nameEn,
    nameAr: raw.nameAr,
    slug: raw.slug,
    parentId: raw.parentId,
    parentNameEn: raw.parent?.nameEn ?? null,
    sortOrder: raw.sortOrder,
    imageUrl: raw.imageUrl,
  }
}

function toDto(raw: BackendCategoryItem): CategoryDto {
  return {
    ...toListItem(raw),
    descriptionEn: raw.descriptionEn,
    descriptionAr: raw.descriptionAr,
  }
}

export async function getStorefrontCategories(
  params: CategoryQueryParams = {},
): Promise<PaginatedResponse<CategoryListItem>> {
  const {
    page = 1,
    limit = 100,
    search,
    parentId,
    isActive = true,
    sortBy,
    sortOrder,
  } = params

  const { data } = await apiClient.get<
    ApiResponse<{
      items: BackendCategoryItem[]
      total: number
      page: number
      limit: number
      totalPages: number
    }>
  >("/categories", {
    params: {
      page,
      limit,
      isActive,
      ...(search && { search }),
      ...(parentId != null && { parentId }),
      ...(sortBy && { sortBy }),
      ...(sortOrder && { sortOrder }),
    },
  })

  if (!data.success) throw new Error(data.message ?? "Failed to fetch categories")

  const raw = data.data!
  return {
    items: raw.items.map(toListItem),
    total: raw.total,
    page: raw.page,
    limit: raw.limit,
    totalPages: raw.totalPages,
  }
}

export async function getStorefrontCategoryById(
  id: number,
): Promise<CategoryDto | null> {
  const { data } = await apiClient.get<ApiResponse<BackendCategoryItem>>(
    `/categories/${id}`,
  )
  if (!data.success) return null
  if (!data.data?.isActive) return null
  return toDto(data.data)
}

export async function getStorefrontCategoryBySlug(
  slug: string,
): Promise<CategoryDto | null> {
  const normalized = slug.trim().toLowerCase()
  if (!normalized) return null

  const { data } = await apiClient.get<ApiResponse<BackendCategoryItem>>(
    `/categories/by-slug/${encodeURIComponent(normalized)}`,
  )
  if (!data.success) return null
  if (!data.data?.isActive) return null
  return toDto(data.data)
}

export async function getTopLevelCategories(): Promise<CategoryListItem[]> {
  const result = await getStorefrontCategories({
    limit: 100,
    sortBy: "sortOrder",
    sortOrder: "asc",
    isActive: true,
  })
  return result.items.filter((c) => c.parentId == null).slice(0, 12)
}
