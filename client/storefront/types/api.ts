// Mirrors backend Common/Responses/ApiResponse.cs
export interface ApiResponse<T> {
  success: boolean;
  message: string | null;
  data: T | null;
  errors: string[] | null;
}

// Mirrors backend ListCategoryPaginatedResponse shape (used across all paginated features)
export interface PaginatedResponse<T> {
  items: T[];
  total: number;
  page: number;
  limit: number;
  totalPages: number;
}
