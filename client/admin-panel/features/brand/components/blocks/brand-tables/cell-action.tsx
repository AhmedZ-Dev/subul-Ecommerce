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
import type { BrandListItem } from '../../../types';
import { useDeleteBrand } from '../../../hooks/useBrandMutations';

interface BrandCellActionProps {
  brand: BrandListItem;
}

export function BrandCellAction({ brand }: BrandCellActionProps) {
  const [deleteOpen, setDeleteOpen] = useState(false);
  const { mutate: deleteBrand, isPending: isDeleting } = useDeleteBrand();

  const displayName = brand.name;
  const isPending = isDeleting;

  function handleConfirmDelete() {
    deleteBrand(brand.id, {
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
                id={`brand-view-${brand.id}`}
                aria-label={messages.common.view}
              >
                <Link href={`/brands/${brand.id}/view`}>
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
                id={`brand-edit-${brand.id}`}
                aria-label={messages.common.edit}
              >
                <Link href={`/brands/${brand.id}/edit`}>
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
                id={`brand-delete-${brand.id}`}
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
            <AlertDialogTitle>{messages.brand.view.deleteTitle}</AlertDialogTitle>
            <AlertDialogDescription>
              {messages.brand.view.deleteDescription(displayName)}
            </AlertDialogDescription>
          </AlertDialogHeader>
          <AlertDialogFooter>
            <AlertDialogCancel disabled={isPending}>
              {messages.brand.form.cancel}
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
