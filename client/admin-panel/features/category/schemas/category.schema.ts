import { z } from 'zod';

import { messages } from '@/lib/messages.ar';

const v = messages.category.form.validation;

export const createCategorySchema = z.object({
  nameEn: z
    .string()
    .min(2, v.nameEnMin)
    .max(100, v.nameEnMax),
  nameAr: z
    .string()
    .min(2, v.nameArMin)
    .max(100, v.nameArMax),
  slug: z
    .string()
    .min(2, v.slugMin)
    .max(100, v.slugMax)
    .regex(/^[a-z0-9]+(?:-[a-z0-9]+)*$/, v.slugFormat)
    .optional()
    .or(z.literal('')),
  descriptionEn: z.string().max(2000, v.descriptionMax).optional().or(z.literal('')),
  descriptionAr: z.string().max(2000, v.descriptionMax).optional().or(z.literal('')),
  parentId: z.number().int().positive().nullable().optional(),
  status: z.enum(['active', 'inactive']),
});

export type CreateCategoryInput = z.infer<typeof createCategorySchema>;

export const updateCategorySchema = createCategorySchema.partial();

export type UpdateCategoryInput = z.infer<typeof updateCategorySchema>;

export const categoryFilterSchema = z.object({
  page: z.coerce.number().int().min(1).optional().default(1),
  limit: z.coerce.number().int().min(1).max(100).optional().default(10),
  search: z.string().max(100).optional(),
  parentId: z.coerce.number().int().positive().optional(),
  status: z.enum(['active', 'inactive']).optional(),
  sortBy: z.enum(['nameEn', 'createdAt', 'depth']).optional(),
  sortOrder: z.enum(['asc', 'desc']).optional(),
});

export type CategoryFilterInput = z.infer<typeof categoryFilterSchema>;
