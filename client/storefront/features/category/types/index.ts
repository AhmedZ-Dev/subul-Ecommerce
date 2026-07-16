export interface CategoryListItem {
  id: number
  nameEn: string
  nameAr: string | null
  slug: string
  parentId: number | null
  parentNameEn: string | null
  sortOrder: number
  imageUrl: string | null
}

export interface CategoryDto extends CategoryListItem {
  descriptionEn: string | null
  descriptionAr: string | null
}

export interface CategoryTreeNode extends CategoryListItem {
  depth: number
  children: CategoryTreeNode[]
}

export interface CategoryQueryParams {
  page?: number
  limit?: number
  search?: string
  parentId?: number | null
  isActive?: boolean
  sortBy?: "nameEn" | "sortOrder" | "createdAt"
  sortOrder?: "asc" | "desc"
}
