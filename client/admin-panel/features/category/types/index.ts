// features/category/types/index.ts
// Frontend domain types — kept separate from raw backend response shapes.
// Transforms from backend → these types happen in api/category.api.ts.

// ─── Status ───────────────────────────────────────────────────────────────────
// Frontend uses a string enum; backend uses isActive: bool. The API layer maps between them.

export type CategoryStatus = 'active' | 'inactive';

// ─── CategoryDto — full detail shape (GetById response) ──────────────────────

export interface CategoryDto {
  id: number;
  nameEn: string;
  nameAr: string | null;
  slug: string;
  descriptionEn: string | null;
  descriptionAr: string | null;
  parentId: number | null;
  parentNameEn: string | null;
  parentNameAr: string | null;
  sortOrder: number;
  status: CategoryStatus;
  createdAt: string;
  updatedAt: string | null;
}

// ─── CategoryListItem — lighter shape for the paginated table ─────────────────

export interface CategoryListItem {
  id: number;
  nameEn: string;
  nameAr: string | null;
  slug: string;
  parentId: number | null;
  parentNameEn: string | null;
  sortOrder: number;
  status: CategoryStatus;
}

// ─── CategoryTreeNode — CategoryListItem enriched with children and depth ─────
// depth is computed client-side by buildCategoryTree, not returned by the backend.

export interface CategoryTreeNode extends CategoryListItem {
  depth: number;
  children: CategoryTreeNode[];
}

// ─── Query params ─────────────────────────────────────────────────────────────

export interface CategoryQueryParams {
  page?: number;
  limit?: number;
  search?: string;
  parentId?: number | null;
  status?: CategoryStatus;
  sortBy?: 'nameEn' | 'createdAt';
  sortOrder?: 'asc' | 'desc';
}
