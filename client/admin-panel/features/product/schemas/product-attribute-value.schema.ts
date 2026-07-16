import { z } from 'zod';

import { messages } from '@/lib/messages.ar';

const v = messages.product.attributeValue.validation;

export const createProductAttributeValueSchema = z.object({
  attributeId: z.number().int().positive(v.attributeRequired),
  valueText: z.string().max(500, v.valueTextMax).optional().or(z.literal('')),
  valueNumber: z.coerce.number().nullable().optional(),
  valueBoolean: z.boolean().nullable().optional(),
});

export type CreateProductAttributeValueInput = z.infer<
  typeof createProductAttributeValueSchema
>;

export const updateProductAttributeValueSchema = createProductAttributeValueSchema;

export type UpdateProductAttributeValueInput = z.infer<
  typeof updateProductAttributeValueSchema
>;
