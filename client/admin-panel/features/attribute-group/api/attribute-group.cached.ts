import { cache } from 'react';
import { getAttributeGroupById } from './attribute-group.api';

/** Per-request deduplication for RSC (metadata + page body). */
export const getCachedAttributeGroupById = cache(getAttributeGroupById);
