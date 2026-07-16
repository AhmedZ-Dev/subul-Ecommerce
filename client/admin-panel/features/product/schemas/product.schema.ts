import { z } from 'zod';

import { messages } from '@/lib/messages.ar';

const v = messages.product.form.validation;

export const createProductSchema = z.object({
  nameEn: z.string().min(2, v.nameEnMin).max(200, v.nameEnMax),
  nameAr: z.string().max(200, v.nameArMax).optional().or(z.literal('')),
  slug: z
    .string()
    .min(2, v.slugMin)
    .max(100, v.slugMax)
    .regex(/^[a-z0-9]+(?:-[a-z0-9]+)*$/, v.slugFormat)
    .optional()
    .or(z.literal('')),
  sku: z.string().max(100, v.skuMax).optional().or(z.literal('')),
  barcode: z.string().max(100, v.barcodeMax).optional().or(z.literal('')),
  descriptionEn: z.string().max(5000, v.descriptionMax).optional().or(z.literal('')),
  descriptionAr: z.string().max(5000, v.descriptionMax).optional().or(z.literal('')),
  shortDescriptionEn: z.string().max(500, v.shortDescriptionMax).optional().or(z.literal('')),
  shortDescriptionAr: z.string().max(500, v.shortDescriptionMax).optional().or(z.literal('')),
  price: z.coerce.number().min(0, v.priceMin),
  compareAtPrice: z.coerce.number().min(0, v.priceMin).nullable().optional(),
  costPrice: z.coerce.number().min(0, v.priceMin).nullable().optional(),
  currency: z.string().min(1, v.currencyRequired).max(10, v.currencyMax),
  stockQuantity: z.coerce.number().int().min(0, v.stockMin),
  lowStockThreshold: z.coerce.number().int().min(0, v.stockMin),
  minOrderQuantity: z.coerce.number().int().min(1, v.minOrderMin),
  weight: z.coerce.number().min(0, v.weightMin).nullable().optional(),
  status: z.enum(['active', 'draft', 'archived']),
  isFeatured: z.boolean(),
  requiresShipping: z.boolean(),
  warrantyMonths: z.coerce.number().int().min(0, v.warrantyMin),
  warrantyDescription: z.string().max(500, v.warrantyDescMax).optional().or(z.literal('')),
  categoryId: z.number().int().positive().nullable().optional(),
  brandId: z.number().int().positive().nullable().optional(),
  metaTitle: z.string().max(100, v.metaTitleMax).optional().or(z.literal('')),
  metaDescription: z.string().max(300, v.metaDescriptionMax).optional().or(z.literal('')),
});

export type CreateProductInput = z.infer<typeof createProductSchema>;

export const updateProductSchema = createProductSchema;

export type UpdateProductInput = z.infer<typeof updateProductSchema>;

export const productFilterSchema = z.object({
  page: z.coerce.number().int().min(1).optional().default(1),
  limit: z.coerce.number().int().min(1).max(100).optional().default(10),
  search: z.string().max(100).optional(),
  categoryId: z.coerce.number().int().positive().optional(),
  brandId: z.coerce.number().int().positive().optional(),
  status: z.enum(['active', 'draft', 'archived']).optional(),
  isFeatured: z.coerce.boolean().optional(),
  sortBy: z
    .enum(['nameEn', 'price', 'stockQuantity', 'totalSold', 'createdAt', 'updatedAt'])
    .optional(),
  sortOrder: z.enum(['asc', 'desc']).optional(),
});

export type ProductFilterInput = z.infer<typeof productFilterSchema>;
