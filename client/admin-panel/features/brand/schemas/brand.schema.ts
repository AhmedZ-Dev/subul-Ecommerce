import { z } from 'zod';

import { messages } from '@/lib/messages.ar';

const v = messages.brand.form.validation;

export const createBrandSchema = z.object({
  name: z
    .string()
    .min(2, v.nameMin)
    .max(100, v.nameMax),
  slug: z
    .string()
    .min(2, v.slugMin)
    .max(100, v.slugMax)
    .regex(/^[a-z0-9]+(?:-[a-z0-9]+)*$/, v.slugFormat)
    .optional()
    .or(z.literal('')),
  descriptionEn: z.string().max(2000, v.descriptionMax).optional().or(z.literal('')),
  descriptionAr: z.string().max(2000, v.descriptionMax).optional().or(z.literal('')),
  websiteUrl: z.string().url(v.urlFormat).optional().or(z.literal('')),
  isFeatured: z.boolean(),
  status: z.enum(['active', 'inactive']),
  sortOrder: z.coerce.number().int().min(0),
});

export type CreateBrandInput = z.infer<typeof createBrandSchema>;

export const updateBrandSchema = createBrandSchema.partial();

export type UpdateBrandInput = z.infer<typeof updateBrandSchema>;

export const brandFilterSchema = z.object({
  page: z.coerce.number().int().min(1).optional().default(1),
  limit: z.coerce.number().int().min(1).max(100).optional().default(10),
  search: z.string().max(100).optional(),
  status: z.enum(['active', 'inactive']).optional(),
  isFeatured: z.coerce.boolean().optional(),
  sortBy: z.enum(['name', 'createdAt', 'sortOrder']).optional(),
  sortOrder: z.enum(['asc', 'desc']).optional(),
});

export type BrandFilterInput = z.infer<typeof brandFilterSchema>;
