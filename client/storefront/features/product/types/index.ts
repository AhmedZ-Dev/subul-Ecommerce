export interface ProductCategoryInfo {
  id: number
  nameEn: string
  nameAr: string | null
  slug: string
}

export interface ProductBrandInfo {
  id: number
  name: string
  slug: string
}

export interface ProductVariantInfo {
  id: number
  title: string | null
  sku: string | null
  price: number | null
  compareAtPrice: number | null
  stockQuantity: number
  isActive: boolean
  sortOrder: number
}

export interface ProductAttributeValueInfo {
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

export interface ProductImageInfo {
  id: number
  productId: number
  variantId: number | null
  imageUrl: string
  altText: string | null
  sortOrder: number
  isPrimary: boolean
}

export interface StorefrontProductListItem {
  id: number
  nameEn: string
  nameAr: string | null
  slug: string
  price: number
  compareAtPrice: number | null
  currency: string
  stockQuantity: number
  isFeatured: boolean
  category: ProductCategoryInfo | null
  brand: ProductBrandInfo | null
  primaryImageUrl: string | null
}

export interface StorefrontProductDetail extends StorefrontProductListItem {
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
  variants: ProductVariantInfo[]
  attributeValues: ProductAttributeValueInfo[]
}

export interface ProductQueryParams {
  page?: number
  limit?: number
  search?: string
  categoryId?: number
  brandId?: number
  brandIds?: number[]
  minPrice?: number
  maxPrice?: number
  inStockOnly?: boolean
  attrs?: Record<string, string[]>
  isFeatured?: boolean
  sortBy?: "nameEn" | "price" | "createdAt" | "totalSold"
  sortOrder?: "asc" | "desc"
}

export interface BrandFacet {
  id: number
  name: string
  slug: string
  count: number
}

export interface PriceRangeFacet {
  min: number
  max: number
}

export interface AttributeValueFacet {
  value: string
  count: number
}

export interface AttributeGroupFacet {
  id: number
  nameEn: string
  nameAr: string | null
  values: AttributeValueFacet[]
}

export interface ProductFilterOptions {
  brands: BrandFacet[]
  priceRange: PriceRangeFacet
  attributeGroups: AttributeGroupFacet[]
}
