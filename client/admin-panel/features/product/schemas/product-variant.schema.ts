import { z } from 'zod';

import { messages } from '@/lib/messages.ar';

const v = messages.product.variant.validation;

export const createProductVariantSchema = z.object({
  title: z.string().max(200, v.titleMax).optional().or(z.literal('')),
  sku: z.string().max(100, v.skuMax).optional().or(z.literal('')),
  barcode: z.string().max(100, v.barcodeMax).optional().or(z.literal('')),
  price: z.coerce.number().min(0, v.priceMin).nullable().optional(),
  compareAtPrice: z.coerce.number().min(0, v.priceMin).nullable().optional(),
  costPrice: z.coerce.number().min(0, v.priceMin).nullable().optional(),
  stockQuantity: z.coerce.number().int().min(0, v.stockMin),
  weight: z.coerce.number().min(0, v.weightMin).nullable().optional(),
  isActive: z.boolean(),
  sortOrder: z.coerce.number().int().min(0, v.sortOrderMin),
});

export type CreateProductVariantInput = z.infer<typeof createProductVariantSchema>;

export const updateProductVariantSchema = createProductVariantSchema;

export type UpdateProductVariantInput = z.infer<typeof updateProductVariantSchema>;
