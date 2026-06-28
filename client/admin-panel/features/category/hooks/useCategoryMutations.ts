'use client';
// features/category/hooks/useCategoryMutations.ts
// React Query mutation hooks. Separated from queries to make side effects obvious.

import { useMutation, useQueryClient } from '@tanstack/react-query';
import { toast } from 'sonner';
import { messages } from '@/lib/messages.ar';
import {
  createCategory,
  updateCategory,
  deleteCategory,
  changeCategoryStatus,
  type CreateCategoryPayload,
  type UpdateCategoryPayload,
} from '../api/category.api';
import { categoryKeys } from './useCategory';
import type { CategoryDto, CategoryListItem } from '../types';
import type { PaginatedResponse } from '@/types/api';

// ─── useCreateCategory ────────────────────────────────────────────────────────

export function useCreateCategory() {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: (payload: CreateCategoryPayload) => createCategory(payload),
    onSuccess: (newCategory) => {
      // Invalidate all list queries so the listing page re-fetches
      queryClient.invalidateQueries({ queryKey: categoryKeys.lists() });
      // Seed the detail cache immediately — avoids a network round-trip on navigation
      queryClient.setQueryData(categoryKeys.detail(newCategory.id), newCategory);
      toast.success(messages.category.createSuccess);
    },
    onError: (error: Error) => {
      toast.error(error.message ?? messages.category.createError);
    },
  });
}

// ─── useUpdateCategory ────────────────────────────────────────────────────────

export function useUpdateCategory() {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: ({ id, payload }: { id: number; payload: UpdateCategoryPayload }) =>
      updateCategory(id, payload),
    onSuccess: (updated) => {
      queryClient.invalidateQueries({ queryKey: categoryKeys.lists() });
      queryClient.setQueryData(categoryKeys.detail(updated.id), updated);
      toast.success(messages.category.updateSuccess);
    },
    onError: (error: Error) => {
      toast.error(error.message ?? messages.category.updateError);
    },
  });
}

// ─── useDeleteCategory ────────────────────────────────────────────────────────
// Uses optimistic update: removes the row from the list immediately,
// then rolls back if the server returns an error.

export function useDeleteCategory() {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: (id: number) => deleteCategory(id),
    onMutate: async (id) => {
      // Cancel any in-flight list queries to prevent overwrites
      await queryClient.cancelQueries({ queryKey: categoryKeys.lists() });
      const previous = queryClient.getQueryData(categoryKeys.lists());
      return { previous, id };
    },
    onError: (_err, _id, context) => {
      // Rollback on failure
      if (context?.previous) {
        queryClient.setQueryData(categoryKeys.lists(), context.previous);
      }
      toast.error(messages.category.deleteError);
    },
    onSuccess: (_data, id) => {
      queryClient.invalidateQueries({ queryKey: categoryKeys.lists() });
      queryClient.removeQueries({ queryKey: categoryKeys.detail(id) });
      toast.success(messages.category.deleteSuccess);
    },
  });
}

// ─── useChangeCategoryStatus ────────────────────────────────────────────────────

export function useChangeCategoryStatus() {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: ({
      id,
      isActive,
    }: {
      id: number;
      isActive: boolean;
    }) => changeCategoryStatus(id, { isActive }),
    onMutate: async ({ id, isActive }) => {
      await queryClient.cancelQueries({ queryKey: categoryKeys.lists() });
      await queryClient.cancelQueries({ queryKey: categoryKeys.detail(id) });

      const nextStatus = isActive ? 'active' : 'inactive';

      const previousLists = queryClient.getQueriesData<PaginatedResponse<CategoryListItem>>({
        queryKey: categoryKeys.lists(),
      });

      const previousDetail = queryClient.getQueryData<CategoryDto>(categoryKeys.detail(id));

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
        queryClient.setQueryData(categoryKeys.detail(id), {
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
        queryClient.setQueryData(categoryKeys.detail(context.id), context.previousDetail);
      }
      toast.error(error.message ?? messages.category.status.changeError);
    },
    onSuccess: (result) => {
      queryClient.invalidateQueries({ queryKey: categoryKeys.lists() });
      queryClient.setQueryData(
        categoryKeys.detail(result.id),
        (current: CategoryDto | undefined) =>
          current
            ? {
                ...current,
                status: result.status,
                updatedAt: result.updatedAt ?? current.updatedAt,
              }
            : current,
      );
      toast.success(messages.category.status.changeSuccess);
    },
  });
}
