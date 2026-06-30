// features/brand/types/index.ts

export type BrandStatus = 'active' | 'inactive';

export interface BrandDto {
  id: number;
  name: string;
  slug: string;
  logoUrl: string | null;
  bannerUrl: string | null;
  descriptionEn: string | null;
  descriptionAr: string | null;
  websiteUrl: string | null;
  isFeatured: boolean;
  sortOrder: number;
  status: BrandStatus;
  createdAt: string;
  updatedAt: string | null;
}

export interface BrandListItem {
  id: number;
  name: string;
  slug: string;
  logoUrl: string | null;
  isFeatured: boolean;
  sortOrder: number;
  status: BrandStatus;
}

export interface BrandQueryParams {
  page?: number;
  limit?: number;
  search?: string;
  status?: BrandStatus;
  isFeatured?: boolean;
  sortBy?: 'name' | 'createdAt' | 'sortOrder';
  sortOrder?: 'asc' | 'desc';
}
