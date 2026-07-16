'use client';

import { useState } from 'react';
import Link from 'next/link';
import { Eye, Pencil, Trash2 } from 'lucide-react';
import { Button } from '@/components/ui/button';
import {
  AlertDialog,
  AlertDialogAction,
  AlertDialogCancel,
  AlertDialogContent,
  AlertDialogDescription,
  AlertDialogFooter,
  AlertDialogHeader,
  AlertDialogTitle,
} from '@/components/ui/alert-dialog';
import {
  Tooltip,
  TooltipContent,
  TooltipTrigger,
} from '@/components/ui/tooltip';
import { messages } from '@/lib/messages.ar';
import type { PaymentMethodListItem } from '../../../types';
import { useDeletePaymentMethod } from '../../../hooks/usePaymentMethodMutations';

interface PaymentMethodCellActionProps {
  paymentMethod: PaymentMethodListItem;
}

function getDisplayLabel(item: PaymentMethodListItem): string {
  return item.labelAr ?? item.labelEn ?? item.name;
}

export function PaymentMethodCellAction({ paymentMethod }: PaymentMethodCellActionProps) {
  const [deleteOpen, setDeleteOpen] = useState(false);
  const { mutate: deletePaymentMethod, isPending: isDeleting } = useDeletePaymentMethod();

  const displayName = getDisplayLabel(paymentMethod);
  const isPending = isDeleting;

  function handleConfirmDelete() {
    deletePaymentMethod(paymentMethod.id, {
      onSettled: () => setDeleteOpen(false),
    });
  }

  return (
    <>
      <div className="flex items-center justify-start gap-1">
        <Tooltip>
          <TooltipTrigger asChild>
            <span className="inline-flex">
              <Button
                asChild
                variant="outline"
                size="icon-sm"
                disabled={isPending}
                id={`payment-method-view-${paymentMethod.id}`}
                aria-label={messages.common.view}
              >
                <Link href={`/payment-methods/${paymentMethod.id}/view`}>
                  <Eye />
                  <span className="sr-only">{messages.common.view}</span>
                </Link>
              </Button>
            </span>
          </TooltipTrigger>
          <TooltipContent side="top">{messages.common.view}</TooltipContent>
        </Tooltip>

        <Tooltip>
          <TooltipTrigger asChild>
            <span className="inline-flex">
              <Button
                asChild
                variant="outline"
                size="icon-sm"
                disabled={isPending}
                id={`payment-method-edit-${paymentMethod.id}`}
                aria-label={messages.common.edit}
              >
                <Link href={`/payment-methods/${paymentMethod.id}/edit`}>
                  <Pencil />
                  <span className="sr-only">{messages.common.edit}</span>
                </Link>
              </Button>
            </span>
          </TooltipTrigger>
          <TooltipContent side="top">{messages.common.edit}</TooltipContent>
        </Tooltip>

        <Tooltip>
          <TooltipTrigger asChild>
            <span className="inline-flex">
              <Button
                variant="outline"
                size="icon-sm"
                className="text-destructive hover:bg-destructive/10 hover:text-destructive"
                onClick={() => setDeleteOpen(true)}
                disabled={isPending}
                id={`payment-method-delete-${paymentMethod.id}`}
                aria-label={messages.common.delete}
              >
                <Trash2 />
                <span className="sr-only">{messages.common.delete}</span>
              </Button>
            </span>
          </TooltipTrigger>
          <TooltipContent side="top">{messages.common.delete}</TooltipContent>
        </Tooltip>
      </div>

      <AlertDialog open={deleteOpen} onOpenChange={setDeleteOpen}>
        <AlertDialogContent>
          <AlertDialogHeader>
            <AlertDialogTitle>{messages.paymentMethod.view.deleteTitle}</AlertDialogTitle>
            <AlertDialogDescription>
              {messages.paymentMethod.view.deleteDescription(displayName)}
            </AlertDialogDescription>
          </AlertDialogHeader>
          <AlertDialogFooter>
            <AlertDialogCancel disabled={isPending}>
              {messages.paymentMethod.form.cancel}
            </AlertDialogCancel>
            <AlertDialogAction
              variant="destructive"
              onClick={handleConfirmDelete}
              disabled={isPending}
            >
              {messages.common.delete}
            </AlertDialogAction>
          </AlertDialogFooter>
        </AlertDialogContent>
      </AlertDialog>
    </>
  );
}
