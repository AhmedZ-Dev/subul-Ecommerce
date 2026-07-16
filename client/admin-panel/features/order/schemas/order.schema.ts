import { z } from 'zod';
import { messages } from '@/lib/messages.ar';
import {
  ORDER_FULFILLMENT_STATUSES,
  ORDER_PAYMENT_STATUSES,
  ORDER_STATUSES,
} from '../types';

const v = messages.order.form.validation;

export const updateOrderSchema = z.object({
  status: z.enum(ORDER_STATUSES).optional(),
  paymentStatus: z.enum(ORDER_PAYMENT_STATUSES).optional(),
  fulfillmentStatus: z.enum(ORDER_FULFILLMENT_STATUSES).optional(),
  trackingNumber: z.string().max(200, v.trackingMax).optional().or(z.literal('')),
  notes: z.string().max(1000, v.notesMax).optional().or(z.literal('')),
  cancelReason: z.string().max(500, v.cancelReasonMax).optional().or(z.literal('')),
});

export type UpdateOrderInput = z.infer<typeof updateOrderSchema>;
