'use client';

import { useMutation, useQueryClient } from '@tanstack/react-query';
import { toast } from 'sonner';
import { messages } from '@/lib/messages.ar';
import { updateOrder, type UpdateOrderPayload } from '../api/order.api';
import { orderKeys } from './useOrder';

export function useUpdateOrder() {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: ({ id, payload }: { id: number; payload: UpdateOrderPayload }) =>
      updateOrder(id, payload),
    onSuccess: (updated) => {
      queryClient.invalidateQueries({ queryKey: orderKeys.lists() });
      queryClient.setQueryData(orderKeys.detail(updated.id), updated);
      toast.success(messages.order.updateSuccess);
    },
    onError: (error: Error) => {
      toast.error(error.message ?? messages.order.updateError);
    },
  });
}
