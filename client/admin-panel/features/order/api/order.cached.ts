import { cache } from 'react';
import { getOrderById } from './order.api';

/** Per-request deduplication for RSC (metadata + page body). */
export const getCachedOrderById = cache(getOrderById);
