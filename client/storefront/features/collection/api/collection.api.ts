import apiClient from "@/lib/api-client"
import type { ApiResponse, PaginatedResponse } from "@/types/api"
import type { CollectionDto, CollectionListItem } from "../types"

interface BackendCollectionProduct {
  productId: number
  nameEn: string
  nameAr: string | null
  slug: string
  price: number
  currency: string
  sortOrder: number
  primaryImageUrl: string | null
}

interface BackendCollectionListItem {
  id: number
  nameEn: string
  nameAr: string | null
  slug: string
  collectionType: string
  imageUrl: string | null
  productCount: number
}

interface BackendCollectionDetail extends BackendCollectionListItem {
  descriptionEn: string | null
  descriptionAr: string | null
  bannerUrl: string | null
  products: BackendCollectionProduct[]
}

function toListItem(raw: BackendCollectionListItem): CollectionListItem {
  return {
    id: raw.id,
    nameEn: raw.nameEn,
    nameAr: raw.nameAr,
    slug: raw.slug,
    collectionType: raw.collectionType,
    imageUrl: raw.imageUrl,
    productCount: raw.productCount,
  }
}

function toDto(raw: BackendCollectionDetail): CollectionDto {
  return {
    ...toListItem(raw),
    descriptionEn: raw.descriptionEn,
    descriptionAr: raw.descriptionAr,
    bannerUrl: raw.bannerUrl,
    products: (raw.products ?? []).map((p) => ({
      productId: p.productId,
      nameEn: p.nameEn,
      nameAr: p.nameAr,
      slug: p.slug,
      price: p.price,
      currency: p.currency ?? "IQD",
      sortOrder: p.sortOrder,
      primaryImageUrl: p.primaryImageUrl ?? null,
    })),
  }
}

export async function getActiveCollections(): Promise<CollectionListItem[]> {
  const { data } = await apiClient.get<
    ApiResponse<{
      items: BackendCollectionListItem[]
      total: number
      page: number
      limit: number
      totalPages: number
    }>
  >("/collections", {
    params: { isActive: true, limit: 20, sortBy: "sortOrder", sortOrder: "asc" },
  })

  if (!data.success) throw new Error(data.message ?? "Failed to fetch collections")
  return (data.data?.items ?? []).map(toListItem)
}

export async function getCollectionById(id: number): Promise<CollectionDto | null> {
  const { data } = await apiClient.get<ApiResponse<BackendCollectionDetail>>(
    `/collections/${id}`,
  )
  if (!data.success) return null
  if (!data.data) return null
  return toDto(data.data)
}

export async function getCollectionBySlug(slug: string): Promise<CollectionDto | null> {
  const normalized = slug.trim().toLowerCase()
  if (!normalized) return null

  const { data } = await apiClient.get<ApiResponse<BackendCollectionDetail>>(
    `/collections/by-slug/${encodeURIComponent(normalized)}`,
  )
  if (!data.success) return null
  if (!data.data) return null
  return toDto(data.data)
}
