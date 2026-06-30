import { cache } from 'react';
import { getBrandById } from './brand.api';

/** Per-request deduplication for RSC (metadata + page body). */
export const getCachedBrandById = cache(getBrandById);
