// features/category/index.ts
// Public barrel — the ONLY way to import from the category feature.
// External code (pages, other features) must import from here, never from internal paths.

// ── Components ────────────────────────────────────────────────────────────────
export { CategoryListingPage } from './components/pages/category-listing-page';
export { CategoryForm } from './components/pages/category-form';
export { CategoryView } from './components/pages/category-view';

// ── Hooks ─────────────────────────────────────────────────────────────────────
export {
  useCategories,
  useCategory,
  useCategoryTree,
  categoryKeys,
} from './hooks/useCategory';
export {
  useCreateCategory,
  useUpdateCategory,
  useDeleteCategory,
  useChangeCategoryStatus,
} from './hooks/useCategoryMutations';

// ── Types ─────────────────────────────────────────────────────────────────────
export type {
  CategoryDto,
  CategoryListItem,
  CategoryTreeNode,
  CategoryQueryParams,
  CategoryStatus,
} from './types';

// ── Schemas + inferred types ──────────────────────────────────────────────────
export { createCategorySchema, updateCategorySchema } from './schemas/category.schema';
export type {
  CreateCategoryInput,
  UpdateCategoryInput,
} from './schemas/category.schema';

// ── API (for RSC server-side calls) ──────────────────────────────────────────
export { getCategories, getCategoryById } from './api/category.api';
export { getCachedCategoryById } from './api/category.cached';

// ── Utils ─────────────────────────────────────────────────────────────────────
export { buildCategoryTree, flattenTree, getCategoryName, generateSlug } from './utils';

// ── URL search params (nuqs) ────────────────────────────────────────────────────
export {
  categoryListingParsers,
  categoryListingSearchParamsCache,
} from './search-params';
