'use client';

import { useEffect, useState } from 'react';
import { useRouter } from 'next/navigation';
import { useForm, type Resolver } from 'react-hook-form';
import { zodResolver } from '@hookform/resolvers/zod';
import { toast } from 'sonner';
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
import { createCollectionSchema, type CreateCollectionInput } from '../../schemas/collection.schema';
import { useCreateCollection, useUpdateCollection } from '../../hooks/useCollectionMutations';
import type { CollectionDto } from '../../types';
import { generateSlug } from '@/features/category/utils';
import { COLLECTION_STATUS_TONES } from '../../constants';

const m = messages.collection.form;

function getDefaultValues(initialData?: CollectionDto | null): CreateCollectionInput {
  if (initialData) {
    return {
      nameEn: initialData.nameEn,
      nameAr: initialData.nameAr ?? '',
      slug: initialData.slug,
      descriptionEn: initialData.descriptionEn ?? '',
      descriptionAr: initialData.descriptionAr ?? '',
      collectionType: initialData.collectionType,
      status: initialData.status,
      sortOrder: initialData.sortOrder,
      metaTitle: initialData.metaTitle ?? '',
      metaDescription: initialData.metaDescription ?? '',
    };
  }

  return {
    nameEn: '',
    nameAr: '',
    slug: '',
    descriptionEn: '',
    descriptionAr: '',
    collectionType: 'manual',
    status: 'active',
    sortOrder: 0,
    metaTitle: '',
    metaDescription: '',
  };
}

interface CollectionFormProps {
  initialData?: CollectionDto | null;
}

export function CollectionForm({ initialData }: CollectionFormProps) {
  const router = useRouter();
  const { mutateAsync: createCollectionAsync, isPending: isCreating } = useCreateCollection();
  const { mutateAsync: updateCollectionAsync, isPending: isUpdating } = useUpdateCollection();
  const isPending = isCreating || isUpdating;
  const isEditMode = !!initialData;
  const [leaveOpen, setLeaveOpen] = useState(false);

  const form = useForm<CreateCollectionInput>({
    resolver: zodResolver(createCollectionSchema) as Resolver<CreateCollectionInput>,
    defaultValues: getDefaultValues(initialData),
  });

  const { isDirty } = form.formState;
  const hasUnsavedChanges = isDirty;
  const watchedSlug = form.watch('slug');
  const previewSlug = (watchedSlug || generateSlug(form.watch('nameEn') || '')).trim();

  useEffect(() => {
    if (initialData) {
      form.reset(getDefaultValues(initialData));
    }
  }, [initialData, form.reset]);

  const watchedName = form.watch('nameEn');
  useEffect(() => {
    if (!isEditMode && watchedName) {
      const currentSlug = form.getValues('slug');
      if (!currentSlug) {
        form.setValue('slug', generateSlug(watchedName), { shouldValidate: false });
      }
    }
  }, [watchedName, isEditMode, form.setValue, form.getValues]);

  useEffect(() => {
    if (!hasUnsavedChanges) return;
    const onBeforeUnload = (e: BeforeUnloadEvent) => {
      e.preventDefault();
    };
    window.addEventListener('beforeunload', onBeforeUnload);
    return () => window.removeEventListener('beforeunload', onBeforeUnload);
  }, [hasUnsavedChanges]);

  function buildPayload(values: CreateCollectionInput) {
    return {
      nameEn: values.nameEn,
      nameAr: values.nameAr || undefined,
      slug: values.slug || undefined,
      descriptionEn: values.descriptionEn || undefined,
      descriptionAr: values.descriptionAr || undefined,
      collectionType: values.collectionType,
      status: values.status,
      sortOrder: values.sortOrder,
      metaTitle: values.metaTitle || undefined,
      metaDescription: values.metaDescription || undefined,
    };
  }

  async function onSubmit(values: CreateCollectionInput) {
    const payload = buildPayload(values);
    let savedCollectionId: number | null = null;

    try {
      if (isEditMode && initialData) {
        await updateCollectionAsync({ id: initialData.id, payload });
        savedCollectionId = initialData.id;
      } else {
        const created = await createCollectionAsync(payload);
        savedCollectionId = created.id;
      }

      toast.success(isEditMode ? messages.collection.updateSuccess : messages.collection.createSuccess);
      router.push('/collections');
    } catch {
      if (!isEditMode && savedCollectionId !== null) {
        router.replace(`/collections/${savedCollectionId}/edit`);
      }
    }
  }

  function handleCancel() {
    if (hasUnsavedChanges) {
      setLeaveOpen(true);
      return;
    }
    router.push('/collections');
  }

  return (
    <>
      <Form {...form}>
        <form onSubmit={form.handleSubmit(onSubmit)} className="space-y-6">
          <div className="grid gap-6 lg:grid-cols-[1fr_300px]">
            <div className="space-y-6">
              <Card className="shadow-xs">
                <CardHeader>
                  <CardTitle className="text-base">{m.sections.names}</CardTitle>
                </CardHeader>
                <CardContent className="grid gap-4">
                  <FormField
                    control={form.control}
                    name="nameEn"
                    render={({ field }) => (
                      <FormItem>
                        <FormLabel>{m.nameEn} *</FormLabel>
                        <FormControl>
                          <Input
                            id="collection-name-en"
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
                        <FormLabel>{m.nameAr}</FormLabel>
                        <FormControl>
                          <Input
                            id="collection-name-ar"
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
                  <FormField
                    control={form.control}
                    name="slug"
                    render={({ field }) => (
                      <FormItem>
                        <FormLabel>{m.slug}</FormLabel>
                        <FormControl>
                          <Input
                            id="collection-slug"
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
                            id="collection-desc-en"
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
                            id="collection-desc-ar"
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

            <div className="space-y-6">
              <Card className="shadow-xs">
                <CardHeader>
                  <CardTitle className="text-base">{m.sections.settings}</CardTitle>
                </CardHeader>
                <CardContent className="space-y-4">
                  <FormField
                    control={form.control}
                    name="collectionType"
                    render={({ field }) => (
                      <FormItem>
                        <FormLabel>{m.type}</FormLabel>
                        <Select
                          onValueChange={field.onChange}
                          value={field.value}
                          disabled={isPending || isEditMode}
                        >
                          <FormControl>
                            <SelectTrigger id="collection-type">
                              <SelectValue />
                            </SelectTrigger>
                          </FormControl>
                          <SelectContent>
                            <SelectItem value="manual">{m.typeManual}</SelectItem>
                            <SelectItem value="smart">{m.typeSmart}</SelectItem>
                          </SelectContent>
                        </Select>
                        {field.value === 'smart' && (
                          <FormDescription className="text-primary">{m.typeSmartHelp}</FormDescription>
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
                            tone={COLLECTION_STATUS_TONES[field.value]}
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
                            <SelectTrigger id="collection-status">
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

                  <FormField
                    control={form.control}
                    name="sortOrder"
                    render={({ field }) => (
                      <FormItem>
                        <FormLabel>{m.sortOrder}</FormLabel>
                        <FormControl>
                          <Input
                            type="number"
                            id="collection-sort-order"
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
            </div>
          </div>

          <FormActionsBar>
            <Button
              id="collection-submit-btn"
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
              id="collection-cancel-btn"
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
            <AlertDialogAction onClick={() => router.push('/collections')}>
              {m.unsavedLeave}
            </AlertDialogAction>
          </AlertDialogFooter>
        </AlertDialogContent>
      </AlertDialog>
    </>
  );
}
