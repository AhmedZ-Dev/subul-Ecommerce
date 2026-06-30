import { unstable_cache } from 'next/cache';
import { getAttributeGroupById } from './attribute-group.api';

export const getCachedAttributeGroupById = unstable_cache(
  async (id: number) => getAttributeGroupById(id),
  ['attribute-group-details'],
  {
    revalidate: 60,
    tags: ['attribute-groups'],
  }
);
