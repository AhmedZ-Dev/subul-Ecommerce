import { cache } from 'react';
import { getPaymentMethodById } from './payment-method.api';

/** Per-request deduplication for RSC (metadata + page body). */
export const getCachedPaymentMethodById = cache(getPaymentMethodById);
