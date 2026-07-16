// features/collection/index.ts
// Public barrel — the ONLY way to import from the collection feature.
// External code (pages, other features) must import from here, never from internal paths.

// ── Components ────────────────────────────────────────────────────────────────
export { CollectionListingPage } from './components/pages/collection-listing-page';
export { CollectionForm } from './components/pages/collection-form';
export { CollectionView } from './components/pages/collection-view';

// ── Hooks ─────────────────────────────────────────────────────────────────────
export { useCollections, useCollection, collectionKeys } from './hooks/useCollection';
export {
  useCreateCollection,
  useUpdateCollection,
  useDeleteCollection,
  useChangeCollectionStatus,
} from './hooks/useCollectionMutations';

// ── Types ─────────────────────────────────────────────────────────────────────
export type {
  CollectionDto,
  CollectionListItem,
  CollectionQueryParams,
  CollectionStatus,
  CollectionType,
} from './types';

// ── Schemas + inferred types ──────────────────────────────────────────────────
export { createCollectionSchema, updateCollectionSchema } from './schemas/collection.schema';
export type {
  CreateCollectionInput,
  UpdateCollectionInput,
} from './schemas/collection.schema';

// ── API (for RSC server-side calls) ──────────────────────────────────────────
export { getCollections, getCollectionById } from './api/collection.api';
export { getCachedCollectionById } from './api/collection.cached';

// ── URL search params (nuqs) ──────────────────────────────────────────────────
export {
  collectionListingParsers,
  collectionListingSearchParamsCache,
} from './search-params';

// ── Collection Products ───────────────────────────────────────────────────────
export type { CollectionProductInfo, CollectionProductQueryParams } from './types';
export {
  collectionProductKeys,
  useCollectionProducts,
  useCollectionProduct,
} from './hooks/useCollectionProduct';
export {
  useAddCollectionProduct,
  useUpdateCollectionProduct,
  useDeleteCollectionProduct,
} from './hooks/useCollectionProductMutations';
export {
  addCollectionProductSchema,
  updateCollectionProductSchema,
} from './schemas/collection-product.schema';
export type {
  AddCollectionProductInput,
  UpdateCollectionProductInput,
} from './schemas/collection-product.schema';
export {
  getCollectionProducts,
  getCollectionProductById,
  addCollectionProduct,
} from './api/collection-product.api';
