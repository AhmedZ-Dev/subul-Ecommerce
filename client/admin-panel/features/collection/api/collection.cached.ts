import { cache } from 'react';
import { getCollectionById } from './collection.api';

/** Per-request deduplication for RSC (metadata + page body). */
export const getCachedCollectionById = cache(getCollectionById);
