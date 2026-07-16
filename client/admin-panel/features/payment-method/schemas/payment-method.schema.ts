import { z } from 'zod';

import { messages } from '@/lib/messages.ar';

const v = messages.paymentMethod.form.validation;

export const createPaymentMethodSchema = z.object({
  name: z
    .string()
    .min(2, v.nameMin)
    .max(100, v.nameMax)
    .regex(/^[a-z0-9_-]+$/, v.nameFormat),
  labelEn: z.string().max(100, v.labelMax).optional().or(z.literal('')),
  labelAr: z.string().max(100, v.labelMax).optional().or(z.literal('')),
  type: z.enum(['offline', 'online']).optional().or(z.literal('')),
  gateway: z.string().max(100, v.gatewayMax).optional().or(z.literal('')),
  gatewayConfig: z.string().max(5000, v.gatewayConfigMax).optional().or(z.literal('')),
  iconUrl: z.string().max(500, v.iconUrlMax).optional().or(z.literal('')),
  instructionsEn: z.string().max(2000, v.instructionsMax).optional().or(z.literal('')),
  instructionsAr: z.string().max(2000, v.instructionsMax).optional().or(z.literal('')),
  status: z.enum(['active', 'inactive']),
  sortOrder: z.coerce.number().int().min(0),
});

export type CreatePaymentMethodInput = z.infer<typeof createPaymentMethodSchema>;

export const updatePaymentMethodSchema = createPaymentMethodSchema.partial();

export type UpdatePaymentMethodInput = z.infer<typeof updatePaymentMethodSchema>;

export const paymentMethodFilterSchema = z.object({
  page: z.coerce.number().int().min(1).optional().default(1),
  limit: z.coerce.number().int().min(1).max(100).optional().default(10),
  search: z.string().max(100).optional(),
  type: z.enum(['offline', 'online']).optional(),
  status: z.enum(['active', 'inactive']).optional(),
  sortBy: z.enum(['name', 'createdAt', 'sortOrder']).optional(),
  sortOrder: z.enum(['asc', 'desc']).optional(),
});

export type PaymentMethodFilterInput = z.infer<typeof paymentMethodFilterSchema>;
