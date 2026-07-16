import { z } from 'zod';
import { messages } from '@/lib/messages.ar';

export const loginSchema = z.object({
  email: z
    .string()
    .min(1, messages.auth.validation.emailRequired)
    .email(messages.auth.validation.emailInvalid),
  password: z.string().min(1, messages.auth.validation.passwordRequired),
});

export type LoginInput = z.infer<typeof loginSchema>;
