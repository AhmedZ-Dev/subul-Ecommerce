import { cache } from 'react';
import { getProductById } from './product.api';

/** Per-request deduplication for RSC (metadata + page body). */
export const getCachedProductById = cache(getProductById);
