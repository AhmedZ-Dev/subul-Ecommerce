// features/product/types/index.ts
// Frontend domain types — kept separate from raw backend response shapes.

export type ProductStatus = 'active' | 'draft' | 'archived';

export interface ProductCategoryInfo {
  id: number;
  nameEn: string;
  nameAr: string | null;
  slug: string;
}

export interface ProductBrandInfo {
  id: number;
  name: string;
  slug: string;
}

export interface ProductVariantInfo {
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

export interface ProductAttributeValueInfo {
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

export interface ProductImageInfo {
  id: number;
  productId: number;
  variantId: number | null;
  imageUrl: string;
  altText: string | null;
  sortOrder: number;
  isPrimary: boolean;
  createdAt: string;
}

export interface ProductVariantQueryParams {
  page?: number;
  limit?: number;
  search?: string;
  isActive?: boolean;
  sortBy?: 'title' | 'sku' | 'price' | 'stockQuantity' | 'sortOrder' | 'createdAt' | 'updatedAt';
  sortOrder?: 'asc' | 'desc';
}

export interface ProductImageQueryParams {
  page?: number;
  limit?: number;
  variantId?: number;
  sortBy?: 'sortOrder' | 'createdAt';
  sortOrder?: 'asc' | 'desc';
}

export interface ProductAttributeValueQueryParams {
  page?: number;
  limit?: number;
  search?: string;
  attributeId?: number;
  sortBy?: 'attributeId' | 'valueText' | 'createdAt';
  sortOrder?: 'asc' | 'desc';
}

export interface ProductDto {
  id: number;
  nameEn: string;
  nameAr: string | null;
  slug: string;
  sku: string | null;
  barcode: string | null;
  descriptionEn: string | null;
  descriptionAr: string | null;
  shortDescriptionEn: string | null;
  shortDescriptionAr: string | null;
  price: number;
  compareAtPrice: number | null;
  costPrice: number | null;
  currency: string;
  stockQuantity: number;
  lowStockThreshold: number;
  minOrderQuantity: number;
  weight: number | null;
  status: ProductStatus;
  isFeatured: boolean;
  requiresShipping: boolean;
  warrantyMonths: number;
  warrantyDescription: string | null;
  totalSold: number;
  viewsCount: number;
  metaTitle: string | null;
  metaDescription: string | null;
  categoryId: number | null;
  brandId: number | null;
  category: ProductCategoryInfo | null;
  brand: ProductBrandInfo | null;
  variants: ProductVariantInfo[];
  attributeValues: ProductAttributeValueInfo[];
  createdAt: string;
  updatedAt: string | null;
}

export interface ProductListItem {
  id: number;
  nameEn: string;
  nameAr: string | null;
  slug: string;
  sku: string | null;
  price: number;
  currency: string;
  stockQuantity: number;
  status: ProductStatus;
  isFeatured: boolean;
  totalSold: number;
  category: ProductCategoryInfo | null;
  brand: ProductBrandInfo | null;
  createdAt: string;
}

export interface ProductQueryParams {
  page?: number;
  limit?: number;
  search?: string;
  categoryId?: number;
  brandId?: number;
  status?: ProductStatus;
  isFeatured?: boolean;
  sortBy?: 'nameEn' | 'price' | 'stockQuantity' | 'totalSold' | 'createdAt' | 'updatedAt';
  sortOrder?: 'asc' | 'desc';
}
