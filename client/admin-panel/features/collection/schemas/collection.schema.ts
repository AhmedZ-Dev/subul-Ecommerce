import { z } from 'zod';

import { messages } from '@/lib/messages.ar';

const v = messages.collection?.form?.validation || {
  nameMin: "الاسم يجب أن يكون حرفين على الأقل",
  nameMax: "الاسم يجب ألا يتجاوز 100 حرفاً",
  slugMin: "الرابط المختصر يجب أن يكون حرفين على الأقل",
  slugMax: "الرابط المختصر يجب ألا يتجاوز 100 حرفاً",
  slugFormat: "الرابط المختصر: أحرف إنجليزية صغيرة وأرقام وشرطات فقط",
  descriptionMax: "الوصف يجب ألا يتجاوز 2000 حرفاً",
};

export const createCollectionSchema = z.object({
  nameEn: z
    .string()
    .min(2, v.nameMin)
    .max(100, v.nameMax),
  nameAr: z
    .string()
    .min(2, v.nameMin)
    .max(100, v.nameMax)
    .optional()
    .or(z.literal('')),
  slug: z
    .string()
    .min(2, v.slugMin)
    .max(100, v.slugMax)
    .regex(/^[a-z0-9]+(?:-[a-z0-9]+)*$/, v.slugFormat)
    .optional()
    .or(z.literal('')),
  descriptionEn: z.string().max(2000, v.descriptionMax).optional().or(z.literal('')),
  descriptionAr: z.string().max(2000, v.descriptionMax).optional().or(z.literal('')),
  collectionType: z.enum(['manual', 'smart']),
  status: z.enum(['active', 'inactive']),
  sortOrder: z.coerce.number().int().min(0).optional().default(0),
  metaTitle: z.string().max(100).optional().or(z.literal('')),
  metaDescription: z.string().max(2000).optional().or(z.literal('')),
});

export type CreateCollectionInput = z.infer<typeof createCollectionSchema>;

export const updateCollectionSchema = createCollectionSchema.partial();

export type UpdateCollectionInput = z.infer<typeof updateCollectionSchema>;

export const collectionFilterSchema = z.object({
  page: z.coerce.number().int().min(1).optional().default(1),
  limit: z.coerce.number().int().min(1).max(100).optional().default(10),
  search: z.string().max(100).optional(),
  status: z.enum(['active', 'inactive']).optional(),
  type: z.enum(['manual', 'smart']).optional(),
  sortBy: z.enum(['nameEn', 'createdAt', 'sortOrder']).optional(),
  sortOrder: z.enum(['asc', 'desc']).optional(),
});

export type CollectionFilterInput = z.infer<typeof collectionFilterSchema>;
