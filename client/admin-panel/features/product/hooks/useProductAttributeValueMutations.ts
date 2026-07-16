'use client';

import { useMutation, useQueryClient } from '@tanstack/react-query';
import { toast } from 'sonner';
import { messages } from '@/lib/messages.ar';
import {
  createProductAttributeValue,
  updateProductAttributeValue,
  deleteProductAttributeValue,
  type CreateProductAttributeValuePayload,
  type UpdateProductAttributeValuePayload,
} from '../api/product-attribute-value.api';
import { productAttributeValueKeys } from './useProductAttributeValue';
import { productKeys } from './useProduct';
import type { ProductAttributeValueInfo } from '../types';
import type { PaginatedResponse } from '@/types/api';

export function useCreateProductAttributeValue(productId: number) {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: (payload: CreateProductAttributeValuePayload) =>
      createProductAttributeValue(productId, payload),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: productAttributeValueKeys.lists() });
      queryClient.invalidateQueries({ queryKey: productKeys.detail(productId) });
    },
    onError: (error: Error) => {
      toast.error(error.message ?? messages.product.attributeValue.createError);
    },
  });
}

export function useUpdateProductAttributeValue(productId: number) {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: ({
      valueId,
      payload,
    }: {
      valueId: number;
      payload: UpdateProductAttributeValuePayload;
    }) => updateProductAttributeValue(productId, valueId, payload),
    onSuccess: (updated) => {
      queryClient.invalidateQueries({ queryKey: productAttributeValueKeys.lists() });
      queryClient.setQueryData(
        productAttributeValueKeys.detail(productId, updated.id),
        updated,
      );
      queryClient.invalidateQueries({ queryKey: productKeys.detail(productId) });
    },
    onError: (error: Error) => {
      toast.error(error.message ?? messages.product.attributeValue.updateError);
    },
  });
}

export function useDeleteProductAttributeValue(productId: number) {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: (valueId: number) => deleteProductAttributeValue(productId, valueId),
    onMutate: async (valueId) => {
      await queryClient.cancelQueries({ queryKey: productAttributeValueKeys.lists() });

      const previousLists = queryClient.getQueriesData<
        PaginatedResponse<ProductAttributeValueInfo>
      >({ queryKey: productAttributeValueKeys.lists() });

      previousLists.forEach(([key, data]) => {
        if (!data) return;
        queryClient.setQueryData(key, {
          ...data,
          items: data.items.filter((item) => item.id !== valueId),
          total: Math.max(0, data.total - 1),
        });
      });

      return { previousLists, valueId };
    },
    onError: (_err, _id, context) => {
      context?.previousLists.forEach(([key, data]) => {
        queryClient.setQueryData(key, data);
      });
      toast.error(messages.product.attributeValue.deleteError);
    },
    onSuccess: (_data, valueId) => {
      queryClient.invalidateQueries({ queryKey: productAttributeValueKeys.lists() });
      queryClient.removeQueries({
        queryKey: productAttributeValueKeys.detail(productId, valueId),
      });
      queryClient.invalidateQueries({ queryKey: productKeys.detail(productId) });
      toast.success(messages.product.attributeValue.deleteSuccess);
    },
  });
}
