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
import type { ProductListItem } from '../../../types';
import { useDeleteProduct } from '../../../hooks/useProductMutations';

interface ProductCellActionProps {
  product: ProductListItem;
}

export function ProductCellAction({ product }: ProductCellActionProps) {
  const [deleteOpen, setDeleteOpen] = useState(false);
  const { mutate: deleteProduct, isPending: isDeleting } = useDeleteProduct();

  const displayName = product.nameAr ?? product.nameEn;
  const isPending = isDeleting;

  function handleConfirmDelete() {
    deleteProduct(product.id, {
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
                id={`product-view-${product.id}`}
                aria-label={messages.common.view}
              >
                <Link href={`/products/${product.id}/view`}>
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
                id={`product-edit-${product.id}`}
                aria-label={messages.common.edit}
              >
                <Link href={`/products/${product.id}/edit`}>
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
                id={`product-delete-${product.id}`}
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
            <AlertDialogTitle>{messages.product.view.deleteTitle}</AlertDialogTitle>
            <AlertDialogDescription>
              {messages.product.view.deleteDescription(displayName)}
            </AlertDialogDescription>
          </AlertDialogHeader>
          <AlertDialogFooter>
            <AlertDialogCancel disabled={isPending}>
              {messages.product.form.cancel}
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
