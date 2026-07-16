import { cache } from 'react';
import { getShippingZoneById } from './shipping-zone.api';

/** Per-request deduplication for RSC (metadata + page body). */
export const getCachedShippingZoneById = cache(getShippingZoneById);
