'use client';

import { useEffect, useState } from 'react';
import { useRouter } from 'next/navigation';
import { useForm } from 'react-hook-form';
import { zodResolver } from '@hookform/resolvers/zod';
import {
  Form,
  FormControl,
  FormDescription,
  FormField,
  FormItem,
  FormLabel,
  FormMessage,
} from '@/components/ui/form';
import { Input } from '@/components/ui/input';
import { Textarea } from '@/components/ui/textarea';
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from '@/components/ui/select';
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
import { Button } from '@/components/ui/button';
import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card';
import { FormActionsBar } from '@/components/layout/form-actions-bar';
import { StatusIndicator } from '@/components/ui/status-indicator';
import { Loader2, Save, X } from 'lucide-react';
import { messages } from '@/lib/messages.ar';
import { createCategorySchema, type CreateCategoryInput } from '../schemas/category.schema';
import { useCreateCategory, useUpdateCategory } from '../hooks/useCategoryMutations';
import { CategoryParentSelect } from './category-parent-select';
import type { CategoryDto } from '../types';
import { generateSlug } from '../utils';
import { CATEGORY_STATUS_TONES } from '../constants';

const m = messages.category.form;

interface CategoryFormProps {
  initialData?: CategoryDto | null;
  parentOptions: Array<{ id: number; nameEn: string; nameAr: string | null; depth: number }>;
}

export function CategoryForm({ initialData, parentOptions }: CategoryFormProps) {
  const router = useRouter();
  const { mutate: createCategory, isPending: isCreating } = useCreateCategory();
  const { mutate: updateCategory, isPending: isUpdating } = useUpdateCategory();
  const isPending = isCreating || isUpdating;
  const isEditMode = !!initialData;
  const [leaveOpen, setLeaveOpen] = useState(false);

  const form = useForm<CreateCategoryInput>({
    resolver: zodResolver(createCategorySchema),
    defaultValues: {
      nameEn: '',
      nameAr: '',
      slug: '',
      descriptionEn: '',
      descriptionAr: '',
      parentId: null,
      status: 'active',
    },
  });

  const { isDirty } = form.formState;
  const watchedSlug = form.watch('slug');
  const previewSlug = (watchedSlug || generateSlug(form.watch('nameEn') || '')).trim();

  useEffect(() => {
    if (initialData) {
      form.reset({
        nameEn: initialData.nameEn,
        nameAr: initialData.nameAr ?? '',
        slug: initialData.slug,
        descriptionEn: initialData.descriptionEn ?? '',
        descriptionAr: initialData.descriptionAr ?? '',
        parentId: initialData.parentId ?? null,
        status: initialData.status,
      });
    }
  }, [initialData, form]);

  const watchedNameEn = form.watch('nameEn');
  useEffect(() => {
    if (!isEditMode && watchedNameEn) {
      const currentSlug = form.getValues('slug');
      if (!currentSlug) {
        form.setValue('slug', generateSlug(watchedNameEn), { shouldValidate: false });
      }
    }
  }, [watchedNameEn, isEditMode, form]);

  useEffect(() => {
    if (!isDirty) return;
    const onBeforeUnload = (e: BeforeUnloadEvent) => {
      e.preventDefault();
    };
    window.addEventListener('beforeunload', onBeforeUnload);
    return () => window.removeEventListener('beforeunload', onBeforeUnload);
  }, [isDirty]);

  function onSubmit(values: CreateCategoryInput) {
    if (isEditMode && initialData) {
      updateCategory(
        {
          id: initialData.id,
          payload: {
            nameEn: values.nameEn,
            nameAr: values.nameAr || undefined,
            slug: values.slug || undefined,
            descriptionEn: values.descriptionEn || undefined,
            descriptionAr: values.descriptionAr || undefined,
            parentId: values.parentId ?? null,
            status: values.status,
            sortOrder: initialData.sortOrder,
          },
        },
        { onSuccess: () => router.push('/categories') },
      );
    } else {
      createCategory(
        {
          nameEn: values.nameEn,
          nameAr: values.nameAr || undefined,
          slug: values.slug || undefined,
          descriptionEn: values.descriptionEn || undefined,
          descriptionAr: values.descriptionAr || undefined,
          parentId: values.parentId ?? null,
          status: values.status,
        },
        { onSuccess: () => router.push('/categories') },
      );
    }
  }

  function handleCancel() {
    if (isDirty) {
      setLeaveOpen(true);
      return;
    }
    router.push('/categories');
  }

  const availableParents = parentOptions.filter((c) => c.id !== initialData?.id);

  return (
    <>
      <Form {...form}>
        <form onSubmit={form.handleSubmit(onSubmit)} className="space-y-6">
          <div className="grid gap-6 lg:grid-cols-[1fr_300px]">
            {/* ── Main column ─────────────────────────────────────────── */}
            <div className="space-y-6">
              <Card className="shadow-xs">
                <CardHeader>
                  <CardTitle className="text-base">{m.sections.names}</CardTitle>
                </CardHeader>
                <CardContent className="grid grid-cols-1 gap-4 md:grid-cols-2">
                  <FormField
                    control={form.control}
                    name="nameEn"
                    render={({ field }) => (
                      <FormItem>
                        <FormLabel>{m.nameEn} *</FormLabel>
                        <FormControl>
                          <Input
                            id="category-name-en"
                            placeholder={m.nameEnPlaceholder}
                            dir="ltr"
                            {...field}
                            disabled={isPending}
                          />
                        </FormControl>
                        <FormMessage />
                      </FormItem>
                    )}
                  />
                  <FormField
                    control={form.control}
                    name="nameAr"
                    render={({ field }) => (
                      <FormItem>
                        <FormLabel>{m.nameAr} *</FormLabel>
                        <FormControl>
                          <Input
                            id="category-name-ar"
                            placeholder={m.nameArPlaceholder}
                            dir="rtl"
                            {...field}
                            disabled={isPending}
                          />
                        </FormControl>
                        <FormMessage />
                      </FormItem>
                    )}
                  />
                </CardContent>
              </Card>

              <Card className="shadow-xs">
                <CardHeader>
                  <CardTitle className="text-base">{m.sections.descriptions}</CardTitle>
                </CardHeader>
                <CardContent className="grid grid-cols-1 gap-4 md:grid-cols-2">
                  <FormField
                    control={form.control}
                    name="descriptionEn"
                    render={({ field }) => (
                      <FormItem>
                        <FormLabel>{m.descriptionEn}</FormLabel>
                        <FormControl>
                          <Textarea
                            id="category-desc-en"
                            placeholder={m.descriptionEnPlaceholder}
                            dir="ltr"
                            className="min-h-[120px] resize-none"
                            {...field}
                            value={field.value ?? ''}
                            disabled={isPending}
                          />
                        </FormControl>
                        <FormDescription>{m.descriptionHelp}</FormDescription>
                        <FormMessage />
                      </FormItem>
                    )}
                  />
                  <FormField
                    control={form.control}
                    name="descriptionAr"
                    render={({ field }) => (
                      <FormItem>
                        <FormLabel>{m.descriptionAr}</FormLabel>
                        <FormControl>
                          <Textarea
                            id="category-desc-ar"
                            placeholder={m.descriptionArPlaceholder}
                            dir="rtl"
                            className="min-h-[120px] resize-none"
                            {...field}
                            value={field.value ?? ''}
                            disabled={isPending}
                          />
                        </FormControl>
                        <FormMessage />
                      </FormItem>
                    )}
                  />
                </CardContent>
              </Card>
            </div>

            {/* ── Sidebar column ────────────────────────────────────────── */}
            <div className="space-y-6">
              <Card className="shadow-xs">
                <CardHeader>
                  <CardTitle className="text-base">{m.sections.settings}</CardTitle>
                </CardHeader>
                <CardContent className="space-y-4">
                  <FormField
                    control={form.control}
                    name="parentId"
                    render={({ field }) => (
                      <FormItem>
                        <FormLabel>{m.parent}</FormLabel>
                        <FormControl>
                          <CategoryParentSelect
                            id="category-parent"
                            value={field.value}
                            options={availableParents}
                            disabled={isPending}
                            onChange={field.onChange}
                          />
                        </FormControl>
                        <FormDescription>{m.parentHelp}</FormDescription>
                        <FormMessage />
                      </FormItem>
                    )}
                  />

                  <FormField
                    control={form.control}
                    name="slug"
                    render={({ field }) => (
                      <FormItem>
                        <FormLabel>{m.slug}</FormLabel>
                        <FormControl>
                          <Input
                            id="category-slug"
                            placeholder={m.slugPlaceholder}
                            dir="ltr"
                            {...field}
                            value={field.value ?? ''}
                            disabled={isPending}
                          />
                        </FormControl>
                        <FormDescription>{m.slugHelp}</FormDescription>
                        {previewSlug && (
                          <p className="text-muted-foreground font-mono text-xs" dir="ltr">
                            {m.slugPreview(previewSlug)}
                          </p>
                        )}
                        <FormMessage />
                      </FormItem>
                    )}
                  />

                  <FormField
                    control={form.control}
                    name="status"
                    render={({ field }) => (
                      <FormItem>
                        <FormLabel>{m.status}</FormLabel>
                        <div className="mb-2">
                          <StatusIndicator
                            variant="dot"
                            tone={CATEGORY_STATUS_TONES[field.value]}
                            label={
                              field.value === 'active' ? m.statusActive : m.statusInactive
                            }
                          />
                        </div>
                        <Select
                          onValueChange={field.onChange}
                          value={field.value}
                          disabled={isPending}
                        >
                          <FormControl>
                            <SelectTrigger id="category-status">
                              <SelectValue />
                            </SelectTrigger>
                          </FormControl>
                          <SelectContent>
                            <SelectItem value="active">{m.statusActive}</SelectItem>
                            <SelectItem value="inactive">{m.statusInactive}</SelectItem>
                          </SelectContent>
                        </Select>
                        <FormMessage />
                      </FormItem>
                    )}
                  />
                </CardContent>
              </Card>
            </div>
          </div>

          <FormActionsBar>
            <Button
              id="category-submit-btn"
              type="submit"
              disabled={isPending}
              className="min-w-[130px]"
            >
              {isPending ? (
                <>
                  <Loader2 className="size-4 animate-spin" />
                  {m.saving}
                </>
              ) : (
                <>
                  <Save className="size-4" />
                  {isEditMode ? m.update : m.create}
                </>
              )}
            </Button>
            <Button
              id="category-cancel-btn"
              type="button"
              variant="outline"
              disabled={isPending}
              onClick={handleCancel}
            >
              <X className="size-4" />
              {m.cancel}
            </Button>
          </FormActionsBar>
        </form>
      </Form>

      <AlertDialog open={leaveOpen} onOpenChange={setLeaveOpen}>
        <AlertDialogContent>
          <AlertDialogHeader>
            <AlertDialogTitle>{m.unsavedTitle}</AlertDialogTitle>
            <AlertDialogDescription>{m.unsavedDescription}</AlertDialogDescription>
          </AlertDialogHeader>
          <AlertDialogFooter>
            <AlertDialogCancel>{m.unsavedStay}</AlertDialogCancel>
            <AlertDialogAction onClick={() => router.push('/categories')}>
              {m.unsavedLeave}
            </AlertDialogAction>
          </AlertDialogFooter>
        </AlertDialogContent>
      </AlertDialog>
    </>
  );
}
