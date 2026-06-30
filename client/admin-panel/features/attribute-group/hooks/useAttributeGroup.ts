import { useQuery } from '@tanstack/react-query';
import { getAttributeGroupById, getAttributeGroups } from '../api/attribute-group.api';
import type { AttributeGroupDto, AttributeGroupQueryParams } from '../types';

export const attributeGroupKeys = {
  all: ['attribute-groups'] as const,
  lists: () => [...attributeGroupKeys.all, 'list'] as const,
  list: (filters: string) => [...attributeGroupKeys.lists(), { filters }] as const,
  details: () => [...attributeGroupKeys.all, 'detail'] as const,
  detail: (id: number) => [...attributeGroupKeys.details(), id] as const,
};

export function useAttributeGroups(params: AttributeGroupQueryParams) {
  return useQuery({
    queryKey: attributeGroupKeys.list(JSON.stringify(params)),
    queryFn: () => getAttributeGroups(params),
    staleTime: 60 * 1000,
  });
}

export function useAttributeGroup(
  id: number,
  options?: { enabled?: boolean; initialData?: AttributeGroupDto },
) {
  return useQuery({
    queryKey: attributeGroupKeys.detail(id),
    queryFn: () => getAttributeGroupById(id),
    enabled: (options?.enabled ?? true) && id > 0,
    initialData: options?.initialData ?? undefined,
    staleTime: 60 * 1000,
  });
}
