export interface CollectionProduct {
  productId: number
  nameEn: string
  nameAr: string | null
  slug: string
  price: number
  compareAtPrice: number | null
  currency: string
  stockQuantity: number
  isFeatured: boolean
  brand: { id: number; name: string; slug: string } | null
  sortOrder: number
  primaryImageUrl: string | null
}

export interface CollectionListItem {
  id: number
  nameEn: string
  nameAr: string | null
  slug: string
  collectionType: string
  imageUrl: string | null
  productCount: number
}

export interface CollectionDto extends CollectionListItem {
  descriptionEn: string | null
  descriptionAr: string | null
  bannerUrl: string | null
  products: CollectionProduct[]
}
