export interface CollectionProduct {
  productId: number
  nameEn: string
  nameAr: string | null
  slug: string
  price: number
  currency: string
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
