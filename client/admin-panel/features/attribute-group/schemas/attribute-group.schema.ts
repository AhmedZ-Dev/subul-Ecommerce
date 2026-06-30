import { z } from 'zod';

// Define individual attribute schema
export const attributeSchema = z.object({
  id: z.number().nullable().optional(),
  nameEn: z.string().min(1, { message: 'Required' }),
  nameAr: z.string().nullable().optional(),
  slug: z.string().nullable().optional().or(z.literal('')),
  unit: z.string().nullable().optional(),
  inputType: z.enum(['text', 'select', 'boolean', 'number']).default('text'),
  isFilterable: z.boolean().default(true),
  sortOrder: z.coerce.number().int().default(0),
});

// Base schema for group
export const attributeGroupBaseSchema = z.object({
  nameEn: z.string().min(1, { message: 'Required' }),
  nameAr: z.string().nullable().optional(),
  slug: z
    .string()
    .regex(/^[a-z0-9]+(?:-[a-z0-9]+)*$/, {
      message: 'Slug must be lowercase alphanumeric and hyphens only',
    })
    .nullable()
    .optional()
    .or(z.literal('')),
  sortOrder: z.coerce.number().int().default(0),
  isFilterable: z.boolean().default(true),
  attributes: z.array(attributeSchema).default([]),
});

export const createAttributeGroupSchema = attributeGroupBaseSchema;
export const updateAttributeGroupSchema = attributeGroupBaseSchema;

export type AttributeFormValues = z.infer<typeof attributeSchema>;
export type CreateAttributeGroupFormValues = z.infer<typeof createAttributeGroupSchema>;
export type UpdateAttributeGroupFormValues = z.infer<typeof updateAttributeGroupSchema>;
