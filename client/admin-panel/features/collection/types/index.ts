// features/collection/types/index.ts
// Frontend domain types — kept separate from raw backend response shapes.
// Transforms from backend → these types happen in api/collection.api.ts.

export type CollectionStatus = 'active' | 'inactive';
export type CollectionType = 'manual' | 'smart';

export interface CollectionDto {
  id: number;
  nameEn: string;
  nameAr: string | null;
  slug: string;
  descriptionEn: string | null;
  descriptionAr: string | null;
  imageUrl: string | null;
  bannerUrl: string | null;
  collectionType: CollectionType;
  status: CollectionStatus;
  sortOrder: number;
  metaTitle: string | null;
  metaDescription: string | null;
  createdAt: string;
  updatedAt: string | null;
}

export interface CollectionListItem {
  id: number;
  nameEn: string;
  nameAr: string | null;
  slug: string;
  collectionType: CollectionType;
  sortOrder: number;
  status: CollectionStatus;
  createdAt: string;
}

export interface CollectionQueryParams {
  page?: number;
  limit?: number;
  search?: string;
  status?: CollectionStatus;
  type?: CollectionType;
  sortBy?: 'nameEn' | 'createdAt' | 'sortOrder';
  sortOrder?: 'asc' | 'desc';
}
