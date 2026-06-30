// features/attribute-group/index.ts
// Public barrel — the ONLY way to import from the attribute-group feature.
// External code (pages, other features) must import from here, never from internal paths.

// ── Components ────────────────────────────────────────────────────────────────
export { AttributeGroupListingPage } from './components/pages/attribute-group-listing-page';
export { AttributeGroupForm } from './components/pages/attribute-group-form';
export { AttributeGroupView } from './components/pages/attribute-group-view';

// ── Hooks ─────────────────────────────────────────────────────────────────────
export { useAttributeGroups, useAttributeGroup, attributeGroupKeys } from './hooks/useAttributeGroup';
export {
  useCreateAttributeGroup,
  useUpdateAttributeGroup,
  useDeleteAttributeGroup,
} from './hooks/useAttributeGroupMutations';

// ── Types ─────────────────────────────────────────────────────────────────────
export type {
  AttributeGroupDto,
  AttributeGroupListItem,
  AttributeGroupQueryParams,
} from './types';

// ── Schemas + inferred types ──────────────────────────────────────────────────
export { createAttributeGroupSchema, updateAttributeGroupSchema } from './schemas/attribute-group.schema';
export type {
  CreateAttributeGroupFormValues,
  UpdateAttributeGroupFormValues,
} from './schemas/attribute-group.schema';

// ── API (for RSC server-side calls) ──────────────────────────────────────────
export {
  getAttributeGroups,
  getAttributeGroupById,
  createAttributeGroup,
  updateAttributeGroup,
  deleteAttributeGroup,
} from './api/attribute-group.api';
export { getCachedAttributeGroupById } from './api/attribute-group.cached';

// ── URL search params (nuqs) ────────────────────────────────────────────────────
export {
  attributeGroupListingParsers,
  attributeGroupListingSearchParamsCache,
} from './search-params';
