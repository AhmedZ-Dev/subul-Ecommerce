'use client';
// features/collection/hooks/useCollectionMutations.ts

import { useMutation, useQueryClient } from '@tanstack/react-query';
import { toast } from 'sonner';
import { messages } from '@/lib/messages.ar';
import {
  createCollection,
  updateCollection,
  deleteCollection,
  changeCollectionStatus,
  type CreateCollectionPayload,
  type UpdateCollectionPayload,
} from '../api/collection.api';
import { collectionKeys } from './useCollection';
import type { CollectionDto, CollectionListItem } from '../types';
import type { PaginatedResponse } from '@/types/api';

export function useCreateCollection() {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: (payload: CreateCollectionPayload) => createCollection(payload),
    onSuccess: (newCollection) => {
      queryClient.invalidateQueries({ queryKey: collectionKeys.lists() });
      queryClient.setQueryData(collectionKeys.detail(newCollection.id), newCollection);
    },
    onError: (error: Error) => {
      toast.error(error.message ?? messages.collection.createError);
    },
  });
}

export function useUpdateCollection() {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: ({ id, payload }: { id: number; payload: UpdateCollectionPayload }) =>
      updateCollection(id, payload),
    onSuccess: (updated) => {
      queryClient.invalidateQueries({ queryKey: collectionKeys.lists() });
      queryClient.setQueryData(collectionKeys.detail(updated.id), updated);
    },
    onError: (error: Error) => {
      toast.error(error.message ?? messages.collection.updateError);
    },
  });
}

export function useDeleteCollection() {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: (id: number) => deleteCollection(id),
    onMutate: async (id) => {
      await queryClient.cancelQueries({ queryKey: collectionKeys.lists() });

      const previousLists = queryClient.getQueriesData<PaginatedResponse<CollectionListItem>>({
        queryKey: collectionKeys.lists(),
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
      toast.error(messages.collection.deleteError);
    },
    onSuccess: (_data, id) => {
      queryClient.invalidateQueries({ queryKey: collectionKeys.lists() });
      queryClient.removeQueries({ queryKey: collectionKeys.detail(id) });
      toast.success(messages.collection.deleteSuccess);
    },
  });
}

export function useChangeCollectionStatus() {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: ({
      id,
      isActive,
    }: {
      id: number;
      isActive: boolean;
    }) => changeCollectionStatus(id, { isActive }),
    onMutate: async ({ id, isActive }) => {
      await queryClient.cancelQueries({ queryKey: collectionKeys.lists() });
      await queryClient.cancelQueries({ queryKey: collectionKeys.detail(id) });

      const nextStatus = isActive ? 'active' : 'inactive';

      const previousLists = queryClient.getQueriesData<PaginatedResponse<CollectionListItem>>({
        queryKey: collectionKeys.lists(),
      });

      const previousDetail = queryClient.getQueryData<CollectionDto>(collectionKeys.detail(id));

      previousLists.forEach(([key, data]) => {
        if (!data) return;
        queryClient.setQueryData(key, {
          ...data,
          items: data.items.map((item) =>
            item.id === id ? { ...item, status: nextStatus } : item,
          ),
        });
      });

      if (previousDetail) {
        queryClient.setQueryData(collectionKeys.detail(id), {
          ...previousDetail,
          status: nextStatus,
        });
      }

      return { previousLists, previousDetail, id };
    },
    onError: (error: Error, _vars, context) => {
      context?.previousLists.forEach(([key, data]) => {
        queryClient.setQueryData(key, data);
      });
      if (context?.previousDetail) {
        queryClient.setQueryData(collectionKeys.detail(context.id), context.previousDetail);
      }
      toast.error(error.message ?? messages.collection.status.changeError);
    },
    onSuccess: (result) => {
      queryClient.invalidateQueries({ queryKey: collectionKeys.lists() });
      queryClient.setQueryData(
        collectionKeys.detail(result.id),
        (current: CollectionDto | undefined) =>
          current
            ? {
                ...current,
                status: result.status,
                updatedAt: result.updatedAt ?? current.updatedAt,
              }
            : current,
      );
      toast.success(messages.collection.status.changeSuccess);
    },
  });
}
