'use client';

import { useState } from 'react';
import Link from 'next/link';
import { Eye, Loader2, Pencil, Power, PowerOff, Trash2 } from 'lucide-react';
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
import { cn } from '@/lib/utils';
import type { CategoryListItem } from '../../../types';
import {
  useChangeCategoryStatus,
  useDeleteCategory,
} from '../../../hooks/useCategoryMutations';

interface CategoryCellActionProps {
  category: CategoryListItem;
}

export function CategoryCellAction({ category }: CategoryCellActionProps) {
  const [deleteOpen, setDeleteOpen] = useState(false);
  const { mutate: deleteCategory, isPending: isDeleting } = useDeleteCategory();
  const { mutate: changeStatus, isPending: isChangingStatus } =
    useChangeCategoryStatus();

  const displayName = category.nameAr ?? category.nameEn;
  const isPending = isDeleting || isChangingStatus;
  const nextIsActive = category.status !== 'active';
  const statusTooltip = nextIsActive
    ? messages.category.status.toggleActive
    : messages.category.status.toggleInactive;
  const StatusIcon = nextIsActive ? Power : PowerOff;

  function handleConfirmDelete() {
    deleteCategory(category.id, {
      onSettled: () => setDeleteOpen(false),
    });
  }

  function handleToggleStatus() {
    changeStatus({ id: category.id, isActive: nextIsActive });
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
                id={`category-view-${category.id}`}
                aria-label={messages.common.view}
              >
                <Link href={`/categories/${category.id}/view`}>
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
                id={`category-edit-${category.id}`}
                aria-label={messages.common.edit}
              >
                <Link href={`/categories/${category.id}/edit`}>
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
                type="button"
                variant="outline"
                size="icon-sm"
                onClick={handleToggleStatus}
                disabled={isPending}
                id={`category-status-${category.id}`}
                aria-label={statusTooltip}
                className={cn(
                  category.status === 'active'
                    ? 'text-amber-700 hover:bg-amber-50 hover:text-amber-800 dark:text-amber-400 dark:hover:bg-amber-950/40'
                    : 'text-green-700 hover:bg-green-50 hover:text-green-800 dark:text-green-400 dark:hover:bg-green-950/40',
                )}
              >
                {isChangingStatus ? (
                  <Loader2 className="animate-spin" />
                ) : (
                  <StatusIcon />
                )}
                <span className="sr-only">{statusTooltip}</span>
              </Button>
            </span>
          </TooltipTrigger>
          <TooltipContent side="top">{statusTooltip}</TooltipContent>
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
                id={`category-delete-${category.id}`}
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
            <AlertDialogTitle>{messages.category.view.deleteTitle}</AlertDialogTitle>
            <AlertDialogDescription>
              {messages.category.view.deleteDescription(displayName)}
            </AlertDialogDescription>
          </AlertDialogHeader>
          <AlertDialogFooter>
            <AlertDialogCancel disabled={isPending}>
              {messages.category.form.cancel}
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
