'use client';

import { useEffect, useState } from 'react';
import { useRouter } from 'next/navigation';
import { useForm, type Resolver } from 'react-hook-form';
import { zodResolver } from '@hookform/resolvers/zod';
import { useQueryClient } from '@tanstack/react-query';
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
import { Checkbox } from '@/components/ui/checkbox';
import { Button } from '@/components/ui/button';
import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card';
import { FormActionsBar } from '@/components/layout/form-actions-bar';
import { StatusIndicator } from '@/components/ui/status-indicator';
import { ImageUploadField } from '@/components/image-upload-field';
import { Loader2, Save, X } from 'lucide-react';
import { messages } from '@/lib/messages.ar';
import { createBrandSchema, type CreateBrandInput } from '../../schemas/brand.schema';
import {
  deleteBrandBanner,
  deleteBrandLogo,
  uploadBrandBanner,
  uploadBrandLogo,
} from '../../api/brand.api';
import { useCreateBrand, useUpdateBrand } from '../../hooks/useBrandMutations';
import { brandKeys } from '../../hooks/useBrand';
import type { BrandDto } from '../../types';
import { generateSlug } from '@/features/category/utils';
import { BRAND_STATUS_TONES } from '../../constants';

const m = messages.brand.form;
const u = m.upload;

function getDefaultValues(initialData?: BrandDto | null): CreateBrandInput {
  if (initialData) {
    return {
      name: initialData.name,
      slug: initialData.slug,
      descriptionEn: initialData.descriptionEn ?? '',
      descriptionAr: initialData.descriptionAr ?? '',
      websiteUrl: initialData.websiteUrl ?? '',
      isFeatured: initialData.isFeatured,
      status: initialData.status,
      sortOrder: initialData.sortOrder,
    };
  }

  return {
    name: '',
    slug: '',
    descriptionEn: '',
    descriptionAr: '',
    websiteUrl: '',
    isFeatured: false,
    status: 'active',
    sortOrder: 0,
  };
}

interface BrandFormProps {
  initialData?: BrandDto | null;
}

export function BrandForm({ initialData }: BrandFormProps) {
  const router = useRouter();
  const queryClient = useQueryClient();
  const { mutateAsync: createBrandAsync, isPending: isCreating } = useCreateBrand();
  const { mutateAsync: updateBrandAsync, isPending: isUpdating } = useUpdateBrand();
  const [isSyncingImages, setIsSyncingImages] = useState(false);
  const isPending = isCreating || isUpdating || isSyncingImages;
  const isEditMode = !!initialData;
  const [leaveOpen, setLeaveOpen] = useState(false);
  const [logoFile, setLogoFile] = useState<File | null>(null);
  const [bannerFile, setBannerFile] = useState<File | null>(null);
  const [removeLogo, setRemoveLogo] = useState(false);
  const [removeBanner, setRemoveBanner] = useState(false);

  const form = useForm<CreateBrandInput>({
    resolver: zodResolver(createBrandSchema) as Resolver<CreateBrandInput>,
    defaultValues: getDefaultValues(initialData),
  });

  const { isDirty } = form.formState;
  const hasMediaChanges =
    logoFile !== null || bannerFile !== null || removeLogo || removeBanner;
  const hasUnsavedChanges = isDirty || hasMediaChanges;
  const watchedSlug = form.watch('slug');
  const previewSlug = (watchedSlug || generateSlug(form.watch('name') || '')).trim();

  useEffect(() => {
    if (initialData) {
      form.reset(getDefaultValues(initialData));
      setLogoFile(null);
      setBannerFile(null);
      setRemoveLogo(false);
      setRemoveBanner(false);
    }
  }, [initialData, form.reset]);

  const watchedName = form.watch('name');
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

  function buildPayload(values: CreateBrandInput) {
    return {
      name: values.name,
      slug: values.slug || undefined,
      descriptionEn: values.descriptionEn || undefined,
      descriptionAr: values.descriptionAr || undefined,
      websiteUrl: values.websiteUrl || undefined,
      isFeatured: values.isFeatured,
      status: values.status,
      sortOrder: values.sortOrder,
    };
  }

  async function syncBrandImages(brandId: number) {
    if (!logoFile && !bannerFile && !removeLogo && !removeBanner) return;

    setIsSyncingImages(true);
    try {
      if (removeLogo) await deleteBrandLogo(brandId);
      if (removeBanner) await deleteBrandBanner(brandId);
      if (logoFile) await uploadBrandLogo(brandId, logoFile);
      if (bannerFile) await uploadBrandBanner(brandId, bannerFile);

      await queryClient.invalidateQueries({ queryKey: brandKeys.lists() });
      await queryClient.invalidateQueries({ queryKey: brandKeys.detail(brandId) });
    } catch (error) {
      const message = error instanceof Error ? error.message : messages.common.error;
      toast.error(message);
      throw error;
    } finally {
      setIsSyncingImages(false);
    }
  }

  async function onSubmit(values: CreateBrandInput) {
    const payload = buildPayload(values);
    let savedBrandId: number | null = null;

    try {
      if (isEditMode && initialData) {
        await updateBrandAsync({ id: initialData.id, payload });
        savedBrandId = initialData.id;
        await syncBrandImages(initialData.id);
      } else {
        const created = await createBrandAsync(payload);
        savedBrandId = created.id;
        await syncBrandImages(created.id);
      }

      toast.success(isEditMode ? messages.brand.updateSuccess : messages.brand.createSuccess);
      router.push('/brands');
    } catch {
      if (!isEditMode && savedBrandId !== null) {
        router.replace(`/brands/${savedBrandId}/edit`);
      }
    }
  }

  function handleCancel() {
    if (hasUnsavedChanges) {
      setLeaveOpen(true);
      return;
    }
    router.push('/brands');
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
                    name="name"
                    render={({ field }) => (
                      <FormItem>
                        <FormLabel>{m.name} *</FormLabel>
                        <FormControl>
                          <Input
                            id="brand-name"
                            placeholder={m.namePlaceholder}
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
                            id="brand-slug"
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
                            id="brand-desc-en"
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
                            id="brand-desc-ar"
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

              <Card className="shadow-xs">
                <CardHeader>
                  <CardTitle className="text-base">{m.sections.media}</CardTitle>
                </CardHeader>
                <CardContent className="grid gap-6 md:grid-cols-2">
                  <ImageUploadField
                    id="brand-logo-upload"
                    label={m.logo}
                    hint={u.hint}
                    invalidTypeError={u.invalidType}
                    tooLargeError={u.tooLarge}
                    chooseLabel={u.chooseImage}
                    replaceLabel={u.replaceImage}
                    removeLabel={u.removeImage}
                    emptyLabel={u.noImage}
                    currentUrl={initialData?.logoUrl}
                    file={logoFile}
                    markedForRemoval={removeLogo}
                    disabled={isPending}
                    onFileChange={setLogoFile}
                    onMarkForRemoval={() => setRemoveLogo(true)}
                    onClearRemoval={() => setRemoveLogo(false)}
                  />
                  <ImageUploadField
                    id="brand-banner-upload"
                    label={m.banner}
                    hint={u.hint}
                    invalidTypeError={u.invalidType}
                    tooLargeError={u.tooLarge}
                    chooseLabel={u.chooseImage}
                    replaceLabel={u.replaceImage}
                    removeLabel={u.removeImage}
                    emptyLabel={u.noImage}
                    currentUrl={initialData?.bannerUrl}
                    file={bannerFile}
                    markedForRemoval={removeBanner}
                    disabled={isPending}
                    onFileChange={setBannerFile}
                    onMarkForRemoval={() => setRemoveBanner(true)}
                    onClearRemoval={() => setRemoveBanner(false)}
                    previewClassName="min-h-44"
                  />
                  <FormField
                    control={form.control}
                    name="websiteUrl"
                    render={({ field }) => (
                      <FormItem className="md:col-span-2">
                        <FormLabel>{m.websiteUrl}</FormLabel>
                        <FormControl>
                          <Input
                            id="brand-website-url"
                            dir="ltr"
                            placeholder="https://..."
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
                    name="status"
                    render={({ field }) => (
                      <FormItem>
                        <FormLabel>{m.status}</FormLabel>
                        <div className="mb-2">
                          <StatusIndicator
                            variant="dot"
                            tone={BRAND_STATUS_TONES[field.value]}
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
                            <SelectTrigger id="brand-status">
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
                    name="isFeatured"
                    render={({ field }) => (
                      <FormItem className="flex flex-row items-center justify-between rounded-lg border p-4">
                        <div className="space-y-0.5">
                          <FormLabel className="text-base">{m.isFeatured}</FormLabel>
                        </div>
                        <FormControl>
                          <Checkbox
                            checked={field.value}
                            onCheckedChange={field.onChange}
                            disabled={isPending}
                          />
                        </FormControl>
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
                            id="brand-sort-order"
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
              id="brand-submit-btn"
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
              id="brand-cancel-btn"
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
            <AlertDialogAction onClick={() => router.push('/brands')}>
              {m.unsavedLeave}
            </AlertDialogAction>
          </AlertDialogFooter>
        </AlertDialogContent>
      </AlertDialog>
    </>
  );
}
