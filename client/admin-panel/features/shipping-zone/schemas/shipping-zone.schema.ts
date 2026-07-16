import { z } from 'zod';

import { messages } from '@/lib/messages.ar';

const v = messages.shippingZone?.form?.validation || {
  nameMin: 'الاسم يجب أن يكون حرفين على الأقل',
  nameMax: 'الاسم يجب ألا يتجاوز 100 حرفاً',
  priceMin: 'السعر يجب أن يكون صفراً أو أكثر',
};

export const shippingRateSchema = z.object({
  id: z.number().optional(),
  nameEn: z.string().max(100).optional().or(z.literal('')),
  nameAr: z.string().max(100).optional().or(z.literal('')),
  rateType: z.enum(['flat', 'weight_based', 'price_based']),
  price: z.coerce.number().min(0, v.priceMin),
  minOrderValue: z.coerce.number().min(0).nullable().optional(),
  maxOrderValue: z.coerce.number().min(0).nullable().optional(),
  freeShippingThreshold: z.coerce.number().min(0).nullable().optional(),
  estimatedDaysMin: z.coerce.number().int().min(0).nullable().optional(),
  estimatedDaysMax: z.coerce.number().int().min(0).nullable().optional(),
  isActive: z.boolean(),
});

export const createShippingZoneSchema = z.object({
  nameEn: z.string().min(2, v.nameMin).max(100, v.nameMax),
  nameAr: z.string().min(2, v.nameMin).max(100, v.nameMax).optional().or(z.literal('')),
  governorates: z.array(z.string()).optional().default([]),
  status: z.enum(['active', 'inactive']),
  shippingRates: z.array(shippingRateSchema).optional().default([]),
});

export type CreateShippingZoneInput = z.infer<typeof createShippingZoneSchema>;

export const updateShippingZoneSchema = createShippingZoneSchema;

export type UpdateShippingZoneInput = z.infer<typeof updateShippingZoneSchema>;
