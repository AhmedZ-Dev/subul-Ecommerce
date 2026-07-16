'use client';

import { useMutation, useQueryClient } from '@tanstack/react-query';
import { toast } from 'sonner';
import { messages } from '@/lib/messages.ar';
import {
  createPaymentMethod,
  updatePaymentMethod,
  deletePaymentMethod,
  changePaymentMethodStatus,
  type CreatePaymentMethodPayload,
  type UpdatePaymentMethodPayload,
} from '../api/payment-method.api';
import { paymentMethodKeys } from './usePaymentMethod';
import type { PaymentMethodDto, PaymentMethodListItem } from '../types';
import type { PaginatedResponse } from '@/types/api';

export function useCreatePaymentMethod() {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: (payload: CreatePaymentMethodPayload) => createPaymentMethod(payload),
    onSuccess: (created) => {
      queryClient.invalidateQueries({ queryKey: paymentMethodKeys.lists() });
      queryClient.setQueryData(paymentMethodKeys.detail(created.id), created);
      toast.success(messages.paymentMethod.createSuccess);
    },
    onError: (error: Error) => {
      toast.error(error.message ?? messages.paymentMethod.createError);
    },
  });
}

export function useUpdatePaymentMethod() {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: ({ id, payload }: { id: number; payload: UpdatePaymentMethodPayload }) =>
      updatePaymentMethod(id, payload),
    onSuccess: (updated) => {
      queryClient.invalidateQueries({ queryKey: paymentMethodKeys.lists() });
      queryClient.setQueryData(paymentMethodKeys.detail(updated.id), updated);
      toast.success(messages.paymentMethod.updateSuccess);
    },
    onError: (error: Error) => {
      toast.error(error.message ?? messages.paymentMethod.updateError);
    },
  });
}

export function useDeletePaymentMethod() {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: (id: number) => deletePaymentMethod(id),
    onMutate: async (id) => {
      await queryClient.cancelQueries({ queryKey: paymentMethodKeys.lists() });

      const previousLists = queryClient.getQueriesData<PaginatedResponse<PaymentMethodListItem>>({
        queryKey: paymentMethodKeys.lists(),
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
      toast.error(messages.paymentMethod.deleteError);
    },
    onSuccess: (_data, id) => {
      queryClient.invalidateQueries({ queryKey: paymentMethodKeys.lists() });
      queryClient.removeQueries({ queryKey: paymentMethodKeys.detail(id) });
      toast.success(messages.paymentMethod.deleteSuccess);
    },
  });
}

export function useChangePaymentMethodStatus() {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: ({ id, isActive }: { id: number; isActive: boolean }) =>
      changePaymentMethodStatus(id, { isActive }),
    onMutate: async ({ id, isActive }) => {
      await queryClient.cancelQueries({ queryKey: paymentMethodKeys.lists() });
      await queryClient.cancelQueries({ queryKey: paymentMethodKeys.detail(id) });

      const nextStatus = isActive ? 'active' : 'inactive';

      const previousLists = queryClient.getQueriesData<PaginatedResponse<PaymentMethodListItem>>({
        queryKey: paymentMethodKeys.lists(),
      });

      const previousDetail = queryClient.getQueryData<PaymentMethodDto>(
        paymentMethodKeys.detail(id),
      );

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
        queryClient.setQueryData(paymentMethodKeys.detail(id), {
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
        queryClient.setQueryData(paymentMethodKeys.detail(context.id), context.previousDetail);
      }
      toast.error(error.message ?? messages.paymentMethod.status.changeError);
    },
    onSuccess: (result) => {
      queryClient.invalidateQueries({ queryKey: paymentMethodKeys.lists() });
      queryClient.setQueryData(
        paymentMethodKeys.detail(result.id),
        (current: PaymentMethodDto | undefined) =>
          current
            ? {
                ...current,
                status: result.status,
                updatedAt: result.updatedAt ?? current.updatedAt,
              }
            : current,
      );
      toast.success(messages.paymentMethod.status.changeSuccess);
    },
  });
}
