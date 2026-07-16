'use client';

import { useMutation, useQueryClient } from '@tanstack/react-query';
import { toast } from 'sonner';
import { messages } from '@/lib/messages.ar';
import {
  createShippingZone,
  updateShippingZone,
  deleteShippingZone,
  type CreateShippingZonePayload,
  type UpdateShippingZonePayload,
} from '../api/shipping-zone.api';
import { shippingZoneKeys } from './useShippingZone';
import type { ShippingZoneDto, ShippingZoneListItem } from '../types';
import type { PaginatedResponse } from '@/types/api';

export function useCreateShippingZone() {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: (payload: CreateShippingZonePayload) => createShippingZone(payload),
    onSuccess: (newZone) => {
      queryClient.invalidateQueries({ queryKey: shippingZoneKeys.lists() });
      queryClient.setQueryData(shippingZoneKeys.detail(newZone.id), newZone);
    },
    onError: (error: Error) => {
      toast.error(error.message ?? messages.shippingZone.createError);
    },
  });
}

export function useUpdateShippingZone() {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: ({ id, payload }: { id: number; payload: UpdateShippingZonePayload }) =>
      updateShippingZone(id, payload),
    onSuccess: (updated) => {
      queryClient.invalidateQueries({ queryKey: shippingZoneKeys.lists() });
      queryClient.setQueryData(shippingZoneKeys.detail(updated.id), updated);
    },
    onError: (error: Error) => {
      toast.error(error.message ?? messages.shippingZone.updateError);
    },
  });
}

export function useDeleteShippingZone() {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: (id: number) => deleteShippingZone(id),
    onMutate: async (id) => {
      await queryClient.cancelQueries({ queryKey: shippingZoneKeys.lists() });

      const previousLists = queryClient.getQueriesData<PaginatedResponse<ShippingZoneListItem>>({
        queryKey: shippingZoneKeys.lists(),
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
      toast.error(messages.shippingZone.deleteError);
    },
    onSuccess: (_data, id) => {
      queryClient.invalidateQueries({ queryKey: shippingZoneKeys.lists() });
      queryClient.removeQueries({ queryKey: shippingZoneKeys.detail(id) });
      toast.success(messages.shippingZone.deleteSuccess);
    },
  });
}
