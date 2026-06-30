'use client';

import { useMutation, useQueryClient } from '@tanstack/react-query';
import { toast } from 'sonner';
import { messages } from '@/lib/messages.ar';
import {
  createBrand,
  updateBrand,
  deleteBrand,
  type CreateBrandPayload,
  type UpdateBrandPayload,
} from '../api/brand.api';
import { brandKeys } from './useBrand';
import type { BrandListItem } from '../types';
import type { PaginatedResponse } from '@/types/api';

export function useCreateBrand() {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: (payload: CreateBrandPayload) => createBrand(payload),
    onSuccess: (newBrand) => {
      queryClient.invalidateQueries({ queryKey: brandKeys.lists() });
      queryClient.setQueryData(brandKeys.detail(newBrand.id), newBrand);
    },
    onError: (error: Error) => {
      toast.error(error.message ?? messages.brand.createError);
    },
  });
}

export function useUpdateBrand() {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: ({ id, payload }: { id: number; payload: UpdateBrandPayload }) =>
      updateBrand(id, payload),
    onSuccess: (updated) => {
      queryClient.invalidateQueries({ queryKey: brandKeys.lists() });
      queryClient.setQueryData(brandKeys.detail(updated.id), updated);
    },
    onError: (error: Error) => {
      toast.error(error.message ?? messages.brand.updateError);
    },
  });
}

export function useDeleteBrand() {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: (id: number) => deleteBrand(id),
    onMutate: async (id) => {
      await queryClient.cancelQueries({ queryKey: brandKeys.lists() });

      const previousLists = queryClient.getQueriesData<PaginatedResponse<BrandListItem>>({
        queryKey: brandKeys.lists(),
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
      toast.error(messages.brand.deleteError);
    },
    onSuccess: (_data, id) => {
      queryClient.invalidateQueries({ queryKey: brandKeys.lists() });
      queryClient.removeQueries({ queryKey: brandKeys.detail(id) });
      toast.success(messages.brand.deleteSuccess);
    },
  });
}
