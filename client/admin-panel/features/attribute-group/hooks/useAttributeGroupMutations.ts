'use client';

import { useMutation, useQueryClient } from '@tanstack/react-query';
import { toast } from 'sonner';
import { messages } from '@/lib/messages.ar';
import {
  createAttributeGroup,
  updateAttributeGroup,
  deleteAttributeGroup,
} from '../api/attribute-group.api';
import type { CreateAttributeGroupPayload, UpdateAttributeGroupPayload } from '../api/attribute-group.api';
import { attributeGroupKeys } from './useAttributeGroup';
import type { AttributeGroupListItem } from '../types';
import type { PaginatedResponse } from '@/types/api';

export function useCreateAttributeGroup() {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: (payload: CreateAttributeGroupPayload) => createAttributeGroup(payload),
    onSuccess: (newGroup) => {
      queryClient.invalidateQueries({ queryKey: attributeGroupKeys.lists() });
      queryClient.setQueryData(attributeGroupKeys.detail(newGroup.id), newGroup);
    },
    onError: (error: Error) => {
      toast.error(error.message ?? messages.attributeGroup.createError);
    },
  });
}

export function useUpdateAttributeGroup() {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: ({ id, payload }: { id: number; payload: UpdateAttributeGroupPayload }) =>
      updateAttributeGroup(id, payload),
    onSuccess: (updated) => {
      queryClient.invalidateQueries({ queryKey: attributeGroupKeys.lists() });
      queryClient.setQueryData(attributeGroupKeys.detail(updated.id), updated);
    },
    onError: (error: Error) => {
      toast.error(error.message ?? messages.attributeGroup.updateError);
    },
  });
}

export function useDeleteAttributeGroup() {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: (id: number) => deleteAttributeGroup(id),
    onMutate: async (id) => {
      await queryClient.cancelQueries({ queryKey: attributeGroupKeys.lists() });

      const previousLists = queryClient.getQueriesData<PaginatedResponse<AttributeGroupListItem>>({
        queryKey: attributeGroupKeys.lists(),
      });

      previousLists.forEach(([key, data]) => {
        if (!data) return;
        queryClient.setQueryData(key, {
          ...data,
          items: data.items.filter((item) => item.id !== id),
          total: Math.max(0, data.total - 1),
        });
      });

      return { previousLists, id };
    },
    onError: (_err, _id, context) => {
      context?.previousLists.forEach(([key, data]) => {
        queryClient.setQueryData(key, data);
      });
      toast.error(messages.attributeGroup.deleteError);
    },
    onSuccess: (_data, id) => {
      queryClient.invalidateQueries({ queryKey: attributeGroupKeys.lists() });
      queryClient.removeQueries({ queryKey: attributeGroupKeys.detail(id) });
      toast.success(messages.attributeGroup.deleteSuccess);
    },
  });
}
