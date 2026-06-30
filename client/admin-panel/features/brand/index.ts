// features/brand/index.ts
// Public barrel — the ONLY way to import from the brand feature.
// External code (pages, other features) must import from here, never from internal paths.

// ── Components ────────────────────────────────────────────────────────────────
export { BrandListingPage } from './components/pages/brand-listing-page';
export { BrandForm } from './components/pages/brand-form';
export { BrandView } from './components/pages/brand-view';

// ── Hooks ─────────────────────────────────────────────────────────────────────
export { useBrands, useBrand, brandKeys } from './hooks/useBrand';
export {
  useCreateBrand,
  useUpdateBrand,
  useDeleteBrand,
} from './hooks/useBrandMutations';

// ── Types ─────────────────────────────────────────────────────────────────────
export type {
  BrandDto,
  BrandListItem,
  BrandQueryParams,
  BrandStatus,
} from './types';

// ── Schemas + inferred types ──────────────────────────────────────────────────
export { createBrandSchema, updateBrandSchema } from './schemas/brand.schema';
export type {
  CreateBrandInput,
  UpdateBrandInput,
} from './schemas/brand.schema';

// ── API (for RSC server-side calls) ──────────────────────────────────────────
export { getBrands, getBrandById, uploadBrandLogo, uploadBrandBanner, deleteBrandLogo, deleteBrandBanner } from './api/brand.api';
export { getCachedBrandById } from './api/brand.cached';

// ── URL search params (nuqs) ────────────────────────────────────────────────────
export {
  brandListingParsers,
  brandListingSearchParamsCache,
} from './search-params';
