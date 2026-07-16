'use client';

import { useEffect, useState } from 'react';
import { useRouter } from 'next/navigation';
import { useForm, type Resolver } from 'react-hook-form';
import { zodResolver } from '@hookform/resolvers/zod';
import { Loader2, Save, X } from 'lucide-react';
import { toast } from 'sonner';

import { Button } from '@/components/ui/button';
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
import { Checkbox } from '@/components/ui/checkbox';
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
import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card';
import { FormActionsBar } from '@/components/layout/form-actions-bar';
import { StatusIndicator } from '@/components/ui/status-indicator';
import { useCategories } from '@/features/category';
import { useBrands } from '@/features/brand';
import { generateSlug } from '@/features/category/utils';
import { messages } from '@/lib/messages.ar';

import {
  createProductSchema,
  type CreateProductInput,
} from '../../schemas/product.schema';
import {
  useCreateProduct,
  useUpdateProduct,
} from '../../hooks/useProductMutations';
import type { ProductDto } from '../../types';
import { PRODUCT_DEFAULT_CURRENCY, PRODUCT_STATUS_TONES } from '../../constants';
import type { CreateProductPayload, UpdateProductPayload } from '../../api/product.api';
import { ProductImageGallery } from '../blocks/product-image-gallery';
import { ProductVariantSection } from '../blocks/product-variant-section';
import { ProductAttributeValueSection } from '../blocks/product-attribute-value-section';

const m = messages.product.form;

function getDefaultValues(initialData?: ProductDto | null): CreateProductInput {
  if (initialData) {
    return {
      nameEn: initialData.nameEn,
      nameAr: initialData.nameAr ?? '',
      slug: initialData.slug,
      sku: initialData.sku ?? '',
      barcode: initialData.barcode ?? '',
      descriptionEn: initialData.descriptionEn ?? '',
      descriptionAr: initialData.descriptionAr ?? '',
      shortDescriptionEn: initialData.shortDescriptionEn ?? '',
      shortDescriptionAr: initialData.shortDescriptionAr ?? '',
      price: initialData.price,
      compareAtPrice: initialData.compareAtPrice,
      costPrice: initialData.costPrice,
      currency: initialData.currency,
      stockQuantity: initialData.stockQuantity,
      lowStockThreshold: initialData.lowStockThreshold,
      minOrderQuantity: initialData.minOrderQuantity,
      weight: initialData.weight,
      status: initialData.status,
      isFeatured: initialData.isFeatured,
      requiresShipping: initialData.requiresShipping,
      warrantyMonths: initialData.warrantyMonths,
      warrantyDescription: initialData.warrantyDescription ?? '',
      categoryId: initialData.categoryId,
      brandId: initialData.brandId,
      metaTitle: initialData.metaTitle ?? '',
      metaDescription: initialData.metaDescription ?? '',
    };
  }

  return {
    nameEn: '',
    nameAr: '',
    slug: '',
    sku: '',
    barcode: '',
    descriptionEn: '',
    descriptionAr: '',
    shortDescriptionEn: '',
    shortDescriptionAr: '',
    price: 0,
    compareAtPrice: null,
    costPrice: null,
    currency: PRODUCT_DEFAULT_CURRENCY,
    stockQuantity: 0,
    lowStockThreshold: 2,
    minOrderQuantity: 1,
    weight: null,
    status: 'active',
    isFeatured: false,
    requiresShipping: true,
    warrantyMonths: 12,
    warrantyDescription: '',
    categoryId: null,
    brandId: null,
    metaTitle: '',
    metaDescription: '',
  };
}

function buildCreatePayload(values: CreateProductInput): CreateProductPayload {
  return {
    nameEn: values.nameEn,
    nameAr: values.nameAr || undefined,
    slug: values.slug || undefined,
    sku: values.sku || undefined,
    barcode: values.barcode || undefined,
    descriptionEn: values.descriptionEn || undefined,
    descriptionAr: values.descriptionAr || undefined,
    shortDescriptionEn: values.shortDescriptionEn || undefined,
    shortDescriptionAr: values.shortDescriptionAr || undefined,
    price: values.price,
    compareAtPrice: values.compareAtPrice ?? null,
    costPrice: values.costPrice ?? null,
    currency: values.currency,
    stockQuantity: values.stockQuantity,
    lowStockThreshold: values.lowStockThreshold,
    minOrderQuantity: values.minOrderQuantity,
    weight: values.weight ?? null,
    status: values.status,
    isFeatured: values.isFeatured,
    requiresShipping: values.requiresShipping,
    warrantyMonths: values.warrantyMonths,
    warrantyDescription: values.warrantyDescription || undefined,
    categoryId: values.categoryId ?? null,
    brandId: values.brandId ?? null,
    metaTitle: values.metaTitle || undefined,
    metaDescription: values.metaDescription || undefined,
  };
}

function buildUpdatePayload(values: CreateProductInput): UpdateProductPayload {
  return {
    nameEn: values.nameEn,
    nameAr: values.nameAr || null,
    slug: values.slug || null,
    sku: values.sku || null,
    barcode: values.barcode || null,
    descriptionEn: values.descriptionEn || null,
    descriptionAr: values.descriptionAr || null,
    shortDescriptionEn: values.shortDescriptionEn || null,
    shortDescriptionAr: values.shortDescriptionAr || null,
    price: values.price,
    compareAtPrice: values.compareAtPrice ?? null,
    costPrice: values.costPrice ?? null,
    currency: values.currency,
    stockQuantity: values.stockQuantity,
    lowStockThreshold: values.lowStockThreshold,
    minOrderQuantity: values.minOrderQuantity,
    weight: values.weight ?? null,
    status: values.status,
    isFeatured: values.isFeatured,
    requiresShipping: values.requiresShipping,
    warrantyMonths: values.warrantyMonths,
    warrantyDescription: values.warrantyDescription || null,
    categoryId: values.categoryId ?? null,
    brandId: values.brandId ?? null,
    metaTitle: values.metaTitle || null,
    metaDescription: values.metaDescription || null,
  };
}

interface ProductFormProps {
  initialData?: ProductDto | null;
}

export function ProductForm({ initialData }: ProductFormProps) {
  const router = useRouter();
  const { mutateAsync: createProductAsync, isPending: isCreating } = useCreateProduct();
  const { mutateAsync: updateProductAsync, isPending: isUpdating } = useUpdateProduct();
  const isEditMode = !!initialData;
  const isPending = isCreating || isUpdating;
  const [leaveOpen, setLeaveOpen] = useState(false);

  const { data: categoriesData } = useCategories({ limit: 200, sortBy: 'nameEn', sortOrder: 'asc' });
  const { data: brandsData } = useBrands({ limit: 200, sortBy: 'name', sortOrder: 'asc' });

  const form = useForm<CreateProductInput>({
    resolver: zodResolver(createProductSchema) as Resolver<CreateProductInput>,
    defaultValues: getDefaultValues(initialData),
  });

  const { isDirty } = form.formState;
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
    if (!isDirty) return;
    const onBeforeUnload = (e: BeforeUnloadEvent) => {
      e.preventDefault();
    };
    window.addEventListener('beforeunload', onBeforeUnload);
    return () => window.removeEventListener('beforeunload', onBeforeUnload);
  }, [isDirty]);

  async function onSubmit(values: CreateProductInput) {
    try {
      if (isEditMode && initialData) {
        await updateProductAsync({ id: initialData.id, payload: buildUpdatePayload(values) });
      } else {
        await createProductAsync(buildCreatePayload(values));
      }

      toast.success(
        isEditMode ? messages.product.updateSuccess : messages.product.createSuccess,
      );
      router.push('/products');
    } catch {
      // Errors surfaced via mutation onError toasts.
    }
  }

  function handleCancel() {
    if (isDirty) {
      setLeaveOpen(true);
      return;
    }
    router.push('/products');
  }

  const statusLabel =
    form.watch('status') === 'active'
      ? m.statusActive
      : form.watch('status') === 'draft'
        ? m.statusDraft
        : m.statusArchived;

  const productFormId = 'product-form';

  const actionsBar = (
    <FormActionsBar>
      <Button
        id="product-submit-btn"
        type="submit"
        form={isEditMode ? productFormId : undefined}
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
        id="product-cancel-btn"
        type="button"
        variant="outline"
        disabled={isPending}
        onClick={handleCancel}
      >
        <X className="size-4" />
        {m.cancel}
      </Button>
    </FormActionsBar>
  );

  return (
    <>
      <Form {...form}>
        <form id={productFormId} onSubmit={form.handleSubmit(onSubmit)} className="space-y-6">
          <div className="grid gap-6 lg:grid-cols-[1fr_300px]">
            <div className="space-y-6">
              <Card className="shadow-xs">
                <CardHeader>
                  <CardTitle className="text-base">{m.sections.basic}</CardTitle>
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
                            id="product-name-en"
                            placeholder={m.nameEnPlaceholder}
                            dir="ltr"
                            disabled={isPending}
                            {...field}
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
                            id="product-name-ar"
                            placeholder={m.nameArPlaceholder}
                            dir="rtl"
                            disabled={isPending}
                            {...field}
                            value={field.value ?? ''}
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
                            id="product-slug"
                            placeholder={m.slugPlaceholder}
                            dir="ltr"
                            disabled={isPending}
                            {...field}
                            value={field.value ?? ''}
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
                  <div className="grid grid-cols-1 gap-4 md:grid-cols-2">
                    <FormField
                      control={form.control}
                      name="sku"
                      render={({ field }) => (
                        <FormItem>
                          <FormLabel>{m.sku}</FormLabel>
                          <FormControl>
                            <Input
                              id="product-sku"
                              placeholder={m.skuPlaceholder}
                              dir="ltr"
                              disabled={isPending}
                              {...field}
                              value={field.value ?? ''}
                            />
                          </FormControl>
                          <FormMessage />
                        </FormItem>
                      )}
                    />
                    <FormField
                      control={form.control}
                      name="barcode"
                      render={({ field }) => (
                        <FormItem>
                          <FormLabel>{m.barcode}</FormLabel>
                          <FormControl>
                            <Input
                              id="product-barcode"
                              placeholder={m.barcodePlaceholder}
                              dir="ltr"
                              disabled={isPending}
                              {...field}
                              value={field.value ?? ''}
                            />
                          </FormControl>
                          <FormMessage />
                        </FormItem>
                      )}
                    />
                  </div>
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
                            id="product-desc-en"
                            placeholder={m.descriptionEnPlaceholder}
                            dir="ltr"
                            className="min-h-[120px] resize-none"
                            disabled={isPending}
                            {...field}
                            value={field.value ?? ''}
                          />
                        </FormControl>
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
                            id="product-desc-ar"
                            placeholder={m.descriptionArPlaceholder}
                            dir="rtl"
                            className="min-h-[120px] resize-none"
                            disabled={isPending}
                            {...field}
                            value={field.value ?? ''}
                          />
                        </FormControl>
                        <FormMessage />
                      </FormItem>
                    )}
                  />
                  <FormField
                    control={form.control}
                    name="shortDescriptionEn"
                    render={({ field }) => (
                      <FormItem>
                        <FormLabel>{m.shortDescriptionEn}</FormLabel>
                        <FormControl>
                          <Textarea
                            dir="ltr"
                            className="min-h-[80px] resize-none"
                            disabled={isPending}
                            {...field}
                            value={field.value ?? ''}
                          />
                        </FormControl>
                        <FormMessage />
                      </FormItem>
                    )}
                  />
                  <FormField
                    control={form.control}
                    name="shortDescriptionAr"
                    render={({ field }) => (
                      <FormItem>
                        <FormLabel>{m.shortDescriptionAr}</FormLabel>
                        <FormControl>
                          <Textarea
                            dir="rtl"
                            className="min-h-[80px] resize-none"
                            disabled={isPending}
                            {...field}
                            value={field.value ?? ''}
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
                  <CardTitle className="text-base">{m.sections.pricing}</CardTitle>
                </CardHeader>
                <CardContent className="grid grid-cols-1 gap-4 md:grid-cols-2">
                  <FormField
                    control={form.control}
                    name="price"
                    render={({ field }) => (
                      <FormItem>
                        <FormLabel>{m.price} *</FormLabel>
                        <FormControl>
                          <Input type="number" step="0.01" dir="ltr" disabled={isPending} {...field} />
                        </FormControl>
                        <FormMessage />
                      </FormItem>
                    )}
                  />
                  <FormField
                    control={form.control}
                    name="currency"
                    render={({ field }) => (
                      <FormItem>
                        <FormLabel>{m.currency} *</FormLabel>
                        <FormControl>
                          <Input dir="ltr" disabled={isPending} {...field} />
                        </FormControl>
                        <FormMessage />
                      </FormItem>
                    )}
                  />
                  <FormField
                    control={form.control}
                    name="compareAtPrice"
                    render={({ field }) => (
                      <FormItem>
                        <FormLabel>{m.compareAtPrice}</FormLabel>
                        <FormControl>
                          <Input
                            type="number"
                            step="0.01"
                            dir="ltr"
                            disabled={isPending}
                            {...field}
                            value={field.value ?? ''}
                            onChange={(e) =>
                              field.onChange(e.target.value === '' ? null : Number(e.target.value))
                            }
                          />
                        </FormControl>
                        <FormMessage />
                      </FormItem>
                    )}
                  />
                  <FormField
                    control={form.control}
                    name="costPrice"
                    render={({ field }) => (
                      <FormItem>
                        <FormLabel>{m.costPrice}</FormLabel>
                        <FormControl>
                          <Input
                            type="number"
                            step="0.01"
                            dir="ltr"
                            disabled={isPending}
                            {...field}
                            value={field.value ?? ''}
                            onChange={(e) =>
                              field.onChange(e.target.value === '' ? null : Number(e.target.value))
                            }
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
                  <CardTitle className="text-base">{m.sections.inventory}</CardTitle>
                </CardHeader>
                <CardContent className="grid grid-cols-1 gap-4 md:grid-cols-2">
                  <FormField
                    control={form.control}
                    name="stockQuantity"
                    render={({ field }) => (
                      <FormItem>
                        <FormLabel>{m.stockQuantity}</FormLabel>
                        <FormControl>
                          <Input type="number" dir="ltr" disabled={isPending} {...field} />
                        </FormControl>
                        <FormMessage />
                      </FormItem>
                    )}
                  />
                  <FormField
                    control={form.control}
                    name="lowStockThreshold"
                    render={({ field }) => (
                      <FormItem>
                        <FormLabel>{m.lowStockThreshold}</FormLabel>
                        <FormControl>
                          <Input type="number" dir="ltr" disabled={isPending} {...field} />
                        </FormControl>
                        <FormMessage />
                      </FormItem>
                    )}
                  />
                  <FormField
                    control={form.control}
                    name="minOrderQuantity"
                    render={({ field }) => (
                      <FormItem>
                        <FormLabel>{m.minOrderQuantity}</FormLabel>
                        <FormControl>
                          <Input type="number" dir="ltr" disabled={isPending} {...field} />
                        </FormControl>
                        <FormMessage />
                      </FormItem>
                    )}
                  />
                  <FormField
                    control={form.control}
                    name="weight"
                    render={({ field }) => (
                      <FormItem>
                        <FormLabel>{m.weight}</FormLabel>
                        <FormControl>
                          <Input
                            type="number"
                            step="0.01"
                            dir="ltr"
                            disabled={isPending}
                            {...field}
                            value={field.value ?? ''}
                            onChange={(e) =>
                              field.onChange(e.target.value === '' ? null : Number(e.target.value))
                            }
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
                  <CardTitle className="text-base">{m.sections.seo}</CardTitle>
                </CardHeader>
                <CardContent className="grid gap-4">
                  <FormField
                    control={form.control}
                    name="metaTitle"
                    render={({ field }) => (
                      <FormItem>
                        <FormLabel>{m.metaTitle}</FormLabel>
                        <FormControl>
                          <Input dir="ltr" disabled={isPending} {...field} value={field.value ?? ''} />
                        </FormControl>
                        <FormMessage />
                      </FormItem>
                    )}
                  />
                  <FormField
                    control={form.control}
                    name="metaDescription"
                    render={({ field }) => (
                      <FormItem>
                        <FormLabel>{m.metaDescription}</FormLabel>
                        <FormControl>
                          <Textarea
                            dir="ltr"
                            className="min-h-[80px] resize-none"
                            disabled={isPending}
                            {...field}
                            value={field.value ?? ''}
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
                            tone={PRODUCT_STATUS_TONES[field.value]}
                            label={statusLabel}
                          />
                        </div>
                        <Select
                          onValueChange={field.onChange}
                          value={field.value}
                          disabled={isPending}
                        >
                          <FormControl>
                            <SelectTrigger id="product-status">
                              <SelectValue />
                            </SelectTrigger>
                          </FormControl>
                          <SelectContent>
                            <SelectItem value="active">{m.statusActive}</SelectItem>
                            <SelectItem value="draft">{m.statusDraft}</SelectItem>
                            <SelectItem value="archived">{m.statusArchived}</SelectItem>
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
                        <FormLabel className="text-base">{m.isFeatured}</FormLabel>
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
                    name="requiresShipping"
                    render={({ field }) => (
                      <FormItem className="flex flex-row items-center justify-between rounded-lg border p-4">
                        <FormLabel className="text-base">{m.requiresShipping}</FormLabel>
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
                </CardContent>
              </Card>

              <Card className="shadow-xs">
                <CardHeader>
                  <CardTitle className="text-base">{m.sections.organization}</CardTitle>
                </CardHeader>
                <CardContent className="space-y-4">
                  <FormField
                    control={form.control}
                    name="categoryId"
                    render={({ field }) => (
                      <FormItem>
                        <FormLabel>{m.category}</FormLabel>
                        <Select
                          onValueChange={(v) =>
                            field.onChange(v === 'none' ? null : Number(v))
                          }
                          value={field.value != null ? String(field.value) : 'none'}
                          disabled={isPending}
                        >
                          <FormControl>
                            <SelectTrigger id="product-category">
                              <SelectValue placeholder={m.categoryPlaceholder} />
                            </SelectTrigger>
                          </FormControl>
                          <SelectContent>
                            <SelectItem value="none">{m.categoryNone}</SelectItem>
                            {(categoriesData?.items ?? []).map((cat) => (
                              <SelectItem key={cat.id} value={String(cat.id)}>
                                {cat.nameAr ?? cat.nameEn}
                              </SelectItem>
                            ))}
                          </SelectContent>
                        </Select>
                        <FormMessage />
                      </FormItem>
                    )}
                  />
                  <FormField
                    control={form.control}
                    name="brandId"
                    render={({ field }) => (
                      <FormItem>
                        <FormLabel>{m.brand}</FormLabel>
                        <Select
                          onValueChange={(v) =>
                            field.onChange(v === 'none' ? null : Number(v))
                          }
                          value={field.value != null ? String(field.value) : 'none'}
                          disabled={isPending}
                        >
                          <FormControl>
                            <SelectTrigger id="product-brand">
                              <SelectValue placeholder={m.brandPlaceholder} />
                            </SelectTrigger>
                          </FormControl>
                          <SelectContent>
                            <SelectItem value="none">{m.brandNone}</SelectItem>
                            {(brandsData?.items ?? []).map((brand) => (
                              <SelectItem key={brand.id} value={String(brand.id)}>
                                {brand.name}
                              </SelectItem>
                            ))}
                          </SelectContent>
                        </Select>
                        <FormMessage />
                      </FormItem>
                    )}
                  />
                </CardContent>
              </Card>

              <Card className="shadow-xs">
                <CardHeader>
                  <CardTitle className="text-base">{m.sections.warranty}</CardTitle>
                </CardHeader>
                <CardContent className="space-y-4">
                  <FormField
                    control={form.control}
                    name="warrantyMonths"
                    render={({ field }) => (
                      <FormItem>
                        <FormLabel>{m.warrantyMonths}</FormLabel>
                        <FormControl>
                          <Input type="number" dir="ltr" disabled={isPending} {...field} />
                        </FormControl>
                        <FormMessage />
                      </FormItem>
                    )}
                  />
                  <FormField
                    control={form.control}
                    name="warrantyDescription"
                    render={({ field }) => (
                      <FormItem>
                        <FormLabel>{m.warrantyDescription}</FormLabel>
                        <FormControl>
                          <Textarea
                            dir="rtl"
                            className="min-h-[80px] resize-none"
                            disabled={isPending}
                            {...field}
                            value={field.value ?? ''}
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

          {!isEditMode && actionsBar}
        </form>
      </Form>

      {isEditMode && initialData && (
        <>
          <div className="space-y-6 border-t pt-6">
            <ProductImageGallery productId={initialData.id} />
            <ProductVariantSection productId={initialData.id} />
            <ProductAttributeValueSection productId={initialData.id} />
          </div>
          {actionsBar}
        </>
      )}

      <AlertDialog open={leaveOpen} onOpenChange={setLeaveOpen}>
        <AlertDialogContent>
          <AlertDialogHeader>
            <AlertDialogTitle>{m.unsavedTitle}</AlertDialogTitle>
            <AlertDialogDescription>{m.unsavedDescription}</AlertDialogDescription>
          </AlertDialogHeader>
          <AlertDialogFooter>
            <AlertDialogCancel>{m.unsavedStay}</AlertDialogCancel>
            <AlertDialogAction onClick={() => router.push('/products')}>
              {m.unsavedLeave}
            </AlertDialogAction>
          </AlertDialogFooter>
        </AlertDialogContent>
      </AlertDialog>
    </>
  );
}
