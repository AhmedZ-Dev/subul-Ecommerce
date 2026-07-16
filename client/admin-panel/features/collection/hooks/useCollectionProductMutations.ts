'use client';

import { useMutation, useQueryClient } from '@tanstack/react-query';
import { toast } from 'sonner';
import { messages } from '@/lib/messages.ar';
import {
  addCollectionProduct,
  updateCollectionProduct,
  deleteCollectionProduct,
  type AddCollectionProductPayload,
  type UpdateCollectionProductPayload,
} from '../api/collection-product.api';
import { collectionProductKeys } from './useCollectionProduct';
import { collectionKeys } from './useCollection';
import type { CollectionProductInfo } from '../types';
import type { PaginatedResponse } from '@/types/api';

export function useAddCollectionProduct(collectionId: number) {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: (payload: AddCollectionProductPayload) =>
      addCollectionProduct(collectionId, payload),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: collectionProductKeys.lists() });
      queryClient.invalidateQueries({ queryKey: collectionKeys.detail(collectionId) });
      toast.success(messages.collection.product.createSuccess);
    },
    onError: (error: Error) => {
      toast.error(error.message ?? messages.collection.product.createError);
    },
  });
}

export function useUpdateCollectionProduct(collectionId: number) {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: ({
      id,
      payload,
    }: {
      id: number;
      payload: UpdateCollectionProductPayload;
    }) => updateCollectionProduct(collectionId, id, payload),
    onSuccess: (updated) => {
      queryClient.invalidateQueries({ queryKey: collectionProductKeys.lists() });
      queryClient.setQueryData(
        collectionProductKeys.detail(collectionId, updated.id),
        updated,
      );
      queryClient.invalidateQueries({ queryKey: collectionKeys.detail(collectionId) });
      toast.success(messages.collection.product.updateSuccess);
    },
    onError: (error: Error) => {
      toast.error(error.message ?? messages.collection.product.updateError);
    },
  });
}

export function useDeleteCollectionProduct(collectionId: number) {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: (id: number) => deleteCollectionProduct(collectionId, id),
    onMutate: async (id) => {
      await queryClient.cancelQueries({ queryKey: collectionProductKeys.lists() });

      const previousLists = queryClient.getQueriesData<
        PaginatedResponse<CollectionProductInfo>
      >({ queryKey: collectionProductKeys.lists() });

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
      toast.error(messages.collection.product.deleteError);
    },
    onSuccess: (_data, id) => {
      queryClient.invalidateQueries({ queryKey: collectionProductKeys.lists() });
      queryClient.removeQueries({
        queryKey: collectionProductKeys.detail(collectionId, id),
      });
      queryClient.invalidateQueries({ queryKey: collectionKeys.detail(collectionId) });
      toast.success(messages.collection.product.deleteSuccess);
    },
  });
}
