'use client';

import { useMutation, useQueryClient } from '@tanstack/react-query';
import { toast } from 'sonner';
import { messages } from '@/lib/messages.ar';
import {
  createProductVariant,
  updateProductVariant,
  deleteProductVariant,
  type CreateProductVariantPayload,
  type UpdateProductVariantPayload,
} from '../api/product-variant.api';
import { productVariantKeys } from './useProductVariant';
import { productKeys } from './useProduct';
import type { ProductVariantInfo } from '../types';
import type { PaginatedResponse } from '@/types/api';

export function useCreateProductVariant(productId: number) {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: (payload: CreateProductVariantPayload) =>
      createProductVariant(productId, payload),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: productVariantKeys.lists() });
      queryClient.invalidateQueries({ queryKey: productKeys.detail(productId) });
    },
    onError: (error: Error) => {
      toast.error(error.message ?? messages.product.variant.createError);
    },
  });
}

export function useUpdateProductVariant(productId: number) {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: ({
      variantId,
      payload,
    }: {
      variantId: number;
      payload: UpdateProductVariantPayload;
    }) => updateProductVariant(productId, variantId, payload),
    onSuccess: (updated) => {
      queryClient.invalidateQueries({ queryKey: productVariantKeys.lists() });
      queryClient.setQueryData(
        productVariantKeys.detail(productId, updated.id),
        updated,
      );
      queryClient.invalidateQueries({ queryKey: productKeys.detail(productId) });
    },
    onError: (error: Error) => {
      toast.error(error.message ?? messages.product.variant.updateError);
    },
  });
}

export function useDeleteProductVariant(productId: number) {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: (variantId: number) => deleteProductVariant(productId, variantId),
    onMutate: async (variantId) => {
      await queryClient.cancelQueries({ queryKey: productVariantKeys.lists() });

      const previousLists = queryClient.getQueriesData<
        PaginatedResponse<ProductVariantInfo>
      >({ queryKey: productVariantKeys.lists() });

      previousLists.forEach(([key, data]) => {
        if (!data) return;
        queryClient.setQueryData(key, {
          ...data,
          items: data.items.filter((item) => item.id !== variantId),
          total: Math.max(0, data.total - 1),
        });
      });

      return { previousLists, variantId };
    },
    onError: (_err, _id, context) => {
      context?.previousLists.forEach(([key, data]) => {
        queryClient.setQueryData(key, data);
      });
      toast.error(messages.product.variant.deleteError);
    },
    onSuccess: (_data, variantId) => {
      queryClient.invalidateQueries({ queryKey: productVariantKeys.lists() });
      queryClient.removeQueries({
        queryKey: productVariantKeys.detail(productId, variantId),
      });
      queryClient.invalidateQueries({ queryKey: productKeys.detail(productId) });
      toast.success(messages.product.variant.deleteSuccess);
    },
  });
}
