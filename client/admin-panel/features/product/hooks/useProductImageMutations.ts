'use client';

import { useMutation, useQueryClient } from '@tanstack/react-query';
import { toast } from 'sonner';
import { messages } from '@/lib/messages.ar';
import {
  createProductImage,
  updateProductImage,
  deleteProductImage,
  type CreateProductImagePayload,
  type UpdateProductImagePayload,
} from '../api/product-image.api';
import { productImageKeys } from './useProductImage';
import { productKeys } from './useProduct';
import type { ProductImageInfo } from '../types';
import type { PaginatedResponse } from '@/types/api';

export function useCreateProductImage(productId: number) {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: (payload: CreateProductImagePayload) =>
      createProductImage(productId, payload),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: productImageKeys.lists() });
      queryClient.invalidateQueries({ queryKey: productKeys.detail(productId) });
    },
    onError: (error: Error) => {
      toast.error(error.message ?? messages.product.image.createError);
    },
  });
}

export function useUpdateProductImage(productId: number) {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: ({
      imageId,
      payload,
    }: {
      imageId: number;
      payload: UpdateProductImagePayload;
    }) => updateProductImage(productId, imageId, payload),
    onSuccess: (updated) => {
      queryClient.invalidateQueries({ queryKey: productImageKeys.lists() });
      queryClient.setQueryData(
        productImageKeys.detail(productId, updated.id),
        updated,
      );
      queryClient.invalidateQueries({ queryKey: productKeys.detail(productId) });
    },
    onError: (error: Error) => {
      toast.error(error.message ?? messages.product.image.updateError);
    },
  });
}

export function useDeleteProductImage(productId: number) {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: (imageId: number) => deleteProductImage(productId, imageId),
    onMutate: async (imageId) => {
      await queryClient.cancelQueries({ queryKey: productImageKeys.lists() });

      const previousLists = queryClient.getQueriesData<
        PaginatedResponse<ProductImageInfo>
      >({ queryKey: productImageKeys.lists() });

      previousLists.forEach(([key, data]) => {
        if (!data) return;
        queryClient.setQueryData(key, {
          ...data,
          items: data.items.filter((item) => item.id !== imageId),
          total: Math.max(0, data.total - 1),
        });
      });

      return { previousLists, imageId };
    },
    onError: (_err, _id, context) => {
      context?.previousLists.forEach(([key, data]) => {
        queryClient.setQueryData(key, data);
      });
      toast.error(messages.product.image.deleteError);
    },
    onSuccess: (_data, imageId) => {
      queryClient.invalidateQueries({ queryKey: productImageKeys.lists() });
      queryClient.removeQueries({
        queryKey: productImageKeys.detail(productId, imageId),
      });
      queryClient.invalidateQueries({ queryKey: productKeys.detail(productId) });
      toast.success(messages.product.image.deleteSuccess);
    },
  });
}
