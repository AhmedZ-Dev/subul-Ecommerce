export interface AttributeDto {
  id: number;
  nameEn: string;
  nameAr: string | null;
  slug: string;
  unit: string | null;
  inputType: string; // "text", "select", "boolean", "number"
  isFilterable: boolean;
  sortOrder: number;
  createdAt: string;
}

export interface AttributeGroupDto {
  id: number;
  nameEn: string;
  nameAr: string | null;
  slug: string;
  sortOrder: number;
  isFilterable: boolean;
  createdAt: string;
  attributes: AttributeDto[];
}

export interface AttributeGroupListItem {
  id: number;
  nameEn: string;
  nameAr: string | null;
  slug: string;
  sortOrder: number;
  isFilterable: boolean;
  createdAt: string;
  attributeCount: number;
}

export interface AttributeGroupQueryParams {
  page?: number;
  limit?: number;
  search?: string;
  isFilterable?: boolean;
  sortBy?: string;
  sortOrder?: 'asc' | 'desc';
}
