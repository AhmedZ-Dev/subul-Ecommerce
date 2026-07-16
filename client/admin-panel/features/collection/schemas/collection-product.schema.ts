import { z } from 'zod';

import { messages } from '@/lib/messages.ar';

const v = messages.collection.product.validation;

export const addCollectionProductSchema = z.object({
  productId: z.coerce.number().int().positive(v.productRequired),
  sortOrder: z.coerce.number().int().min(0, v.sortOrderMin),
});

export type AddCollectionProductInput = z.infer<typeof addCollectionProductSchema>;

export const updateCollectionProductSchema = z.object({
  sortOrder: z.coerce.number().int().min(0, v.sortOrderMin),
});

export type UpdateCollectionProductInput = z.infer<typeof updateCollectionProductSchema>;
