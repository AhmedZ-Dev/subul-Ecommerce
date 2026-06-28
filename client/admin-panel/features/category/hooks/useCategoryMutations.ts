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

function withParentNames(
  dto: CategoryDto,
  queryClient: ReturnType<typeof useQueryClient>,
): CategoryDto {
  if (!dto.parentId || dto.parentNameEn || dto.parentNameAr) return dto;

  const previous = queryClient.getQueryData<CategoryDto>(categoryKeys.detail(dto.id));
  if (
    previous?.parentId === dto.parentId &&
    (previous.parentNameEn || previous.parentNameAr)
  ) {
    return {
      ...dto,
      parentNameEn: previous.parentNameEn,
      parentNameAr: previous.parentNameAr,
    };
  }

  const parent = queryClient.getQueryData<CategoryDto>(categoryKeys.detail(dto.parentId));
  if (parent) {
    return {
      ...dto,
      parentNameEn: parent.nameEn,
      parentNameAr: parent.nameAr,
    };
  }

  const listQueries = queryClient.getQueriesData<PaginatedResponse<CategoryListItem>>({
    queryKey: categoryKeys.lists(),
  });
  for (const [, data] of listQueries) {
    const item = data?.items.find((i) => i.id === dto.parentId);
    if (item) {
      return {
        ...dto,
        parentNameEn: item.nameEn,
        parentNameAr: item.nameAr,
      };
    }
  }

  return dto;
}

// ─── useCreateCategory ────────────────────────────────────────────────────────

export function useCreateCategory() {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: (payload: CreateCategoryPayload) => createCategory(payload),
    onSuccess: (newCategory) => {
      // Invalidate all list queries so the listing page re-fetches
      queryClient.invalidateQueries({ queryKey: categoryKeys.lists() });
      const enriched = withParentNames(newCategory, queryClient);
      queryClient.setQueryData(categoryKeys.detail(newCategory.id), enriched);
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
      const enriched = withParentNames(updated, queryClient);
      queryClient.setQueryData(categoryKeys.detail(updated.id), enriched);
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
      await queryClient.cancelQueries({ queryKey: categoryKeys.lists() });

      const previousLists = queryClient.getQueriesData<PaginatedResponse<CategoryListItem>>({
        queryKey: categoryKeys.lists(),
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
