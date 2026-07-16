// features/product/index.ts
// Public barrel — the ONLY way to import from the product feature.

// ── Components ────────────────────────────────────────────────────────────────
export { ProductListingPage } from './components/pages/product-listing-page';
export { ProductForm } from './components/pages/product-form';
export { ProductView } from './components/pages/product-view';

// ── Hooks ─────────────────────────────────────────────────────────────────────
export { useProducts, useProduct, productKeys } from './hooks/useProduct';
export {
  useCreateProduct,
  useUpdateProduct,
  useDeleteProduct,
} from './hooks/useProductMutations';
export {
  useProductVariants,
  useProductVariant,
  productVariantKeys,
} from './hooks/useProductVariant';
export {
  useCreateProductVariant,
  useUpdateProductVariant,
  useDeleteProductVariant,
} from './hooks/useProductVariantMutations';
export { useProductImages, useProductImage, productImageKeys } from './hooks/useProductImage';
export {
  useCreateProductImage,
  useUpdateProductImage,
  useDeleteProductImage,
} from './hooks/useProductImageMutations';
export {
  useProductAttributeValues,
  useProductAttributeValue,
  productAttributeValueKeys,
} from './hooks/useProductAttributeValue';
export {
  useCreateProductAttributeValue,
  useUpdateProductAttributeValue,
  useDeleteProductAttributeValue,
} from './hooks/useProductAttributeValueMutations';

// ── Types ─────────────────────────────────────────────────────────────────────
export type {
  ProductDto,
  ProductListItem,
  ProductQueryParams,
  ProductStatus,
  ProductVariantInfo,
  ProductAttributeValueInfo,
  ProductImageInfo,
  ProductVariantQueryParams,
  ProductImageQueryParams,
  ProductAttributeValueQueryParams,
} from './types';

// ── Schemas + inferred types ──────────────────────────────────────────────────
export { createProductSchema, updateProductSchema } from './schemas/product.schema';
export type {
  CreateProductInput,
  UpdateProductInput,
} from './schemas/product.schema';
export {
  createProductVariantSchema,
  updateProductVariantSchema,
} from './schemas/product-variant.schema';
export type {
  CreateProductVariantInput,
  UpdateProductVariantInput,
} from './schemas/product-variant.schema';
export {
  createProductAttributeValueSchema,
  updateProductAttributeValueSchema,
} from './schemas/product-attribute-value.schema';
export type {
  CreateProductAttributeValueInput,
  UpdateProductAttributeValueInput,
} from './schemas/product-attribute-value.schema';

// ── API (for RSC server-side calls) ──────────────────────────────────────────
export { getProducts, getProductById } from './api/product.api';
export { getCachedProductById } from './api/product.cached';
export {
  getProductVariants,
  getProductVariantById,
} from './api/product-variant.api';
export { getProductImages, getProductImageById } from './api/product-image.api';
export {
  getProductAttributeValues,
  getProductAttributeValueById,
} from './api/product-attribute-value.api';

// ── URL search params (nuqs) ────────────────────────────────────────────────────
export {
  productListingParsers,
  productListingSearchParamsCache,
} from './search-params';
