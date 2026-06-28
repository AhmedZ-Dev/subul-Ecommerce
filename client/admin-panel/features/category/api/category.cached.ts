import { cache } from 'react';
import { getCategoryById } from './category.api';

/** Per-request deduplication for RSC (metadata + page body). */
export const getCachedCategoryById = cache(getCategoryById);
