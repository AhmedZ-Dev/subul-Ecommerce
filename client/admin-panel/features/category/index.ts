// features/category/index.ts
// Public barrel — the ONLY way to import from the category feature.
// External code (pages, other features) must import from here, never from internal paths.

// ── Components ────────────────────────────────────────────────────────────────
export { CategoryForm } from './components/category-form';
export { CategoryTable } from './components/category-tables';
export { categoryColumns } from './components/category-tables/columns';
export { CategoryCellAction } from './components/category-tables/cell-action';
export { CategoryTree } from './components/category-tree';
export { CategoryListingPage } from './components/category-listing-page';
export { CategoryView } from './components/category-view';
export { CategoryStatusBadge } from './components/category-status-badge';
export { CategoryStatusToggle } from './components/category-status-toggle';

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

// ── Utils ─────────────────────────────────────────────────────────────────────
export { buildCategoryTree, flattenTree, getCategoryName, generateSlug } from './utils';
