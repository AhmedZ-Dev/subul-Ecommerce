'use client';

import { useState } from 'react';
import { useForm, type Resolver } from 'react-hook-form';
import { zodResolver } from '@hookform/resolvers/zod';
import { Loader2, Pencil, Plus, Trash2, X } from 'lucide-react';
import { toast } from 'sonner';

import { Button } from '@/components/ui/button';
import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card';
import { Checkbox } from '@/components/ui/checkbox';
import {
  Form,
  FormControl,
  FormField,
  FormItem,
  FormLabel,
  FormMessage,
} from '@/components/ui/form';
import { Input } from '@/components/ui/input';
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
  Table,
  TableBody,
  TableCell,
  TableHead,
  TableHeader,
  TableRow,
} from '@/components/ui/table';
import { Skeleton } from '@/components/ui/skeleton';
import { formatCurrency, messages } from '@/lib/messages.ar';

import {
  createProductVariantSchema,
  type CreateProductVariantInput,
} from '../../schemas/product-variant.schema';
import { useProductVariants } from '../../hooks/useProductVariant';
import {
  useCreateProductVariant,
  useUpdateProductVariant,
  useDeleteProductVariant,
} from '../../hooks/useProductVariantMutations';
import type { ProductVariantInfo } from '../../types';

const m = messages.product.variant;

function getDefaultValues(variant?: ProductVariantInfo): CreateProductVariantInput {
  if (variant) {
    return {
      title: variant.title ?? '',
      sku: variant.sku ?? '',
      barcode: variant.barcode ?? '',
      price: variant.price,
      compareAtPrice: variant.compareAtPrice,
      costPrice: variant.costPrice,
      stockQuantity: variant.stockQuantity,
      weight: variant.weight,
      isActive: variant.isActive,
      sortOrder: variant.sortOrder,
    };
  }

  return {
    title: '',
    sku: '',
    barcode: '',
    price: null,
    compareAtPrice: null,
    costPrice: null,
    stockQuantity: 0,
    weight: null,
    isActive: true,
    sortOrder: 0,
  };
}

function buildCreatePayload(values: CreateProductVariantInput) {
  return {
    title: values.title || undefined,
    sku: values.sku || undefined,
    barcode: values.barcode || undefined,
    price: values.price ?? null,
    compareAtPrice: values.compareAtPrice ?? null,
    costPrice: values.costPrice ?? null,
    stockQuantity: values.stockQuantity,
    weight: values.weight ?? null,
    isActive: values.isActive,
    sortOrder: values.sortOrder,
  };
}

function buildUpdatePayload(values: CreateProductVariantInput) {
  return {
    title: values.title || null,
    sku: values.sku || null,
    barcode: values.barcode || null,
    price: values.price ?? null,
    compareAtPrice: values.compareAtPrice ?? null,
    costPrice: values.costPrice ?? null,
    stockQuantity: values.stockQuantity,
    weight: values.weight ?? null,
    isActive: values.isActive,
    sortOrder: values.sortOrder,
  };
}

interface VariantFormProps {
  productId: number;
  initialVariant?: ProductVariantInfo;
  onDone: () => void;
}

function VariantForm({ productId, initialVariant, onDone }: VariantFormProps) {
  const isEdit = !!initialVariant;
  const { mutateAsync: createVariant, isPending: isCreating } =
    useCreateProductVariant(productId);
  const { mutateAsync: updateVariant, isPending: isUpdating } =
    useUpdateProductVariant(productId);
  const isPending = isCreating || isUpdating;

  const form = useForm<CreateProductVariantInput>({
    resolver: zodResolver(createProductVariantSchema) as Resolver<CreateProductVariantInput>,
    defaultValues: getDefaultValues(initialVariant),
  });

  async function onSubmit(values: CreateProductVariantInput) {
    try {
      if (isEdit && initialVariant) {
        await updateVariant({
          variantId: initialVariant.id,
          payload: buildUpdatePayload(values),
        });
        toast.success(m.updateSuccess);
      } else {
        await createVariant(buildCreatePayload(values));
        toast.success(m.createSuccess);
      }
      onDone();
    } catch {
      // Error surfaced via mutation onError
    }
  }

  return (
    <Form {...form}>
      <form onSubmit={form.handleSubmit(onSubmit)} className="space-y-4 rounded-lg border p-4">
        <div className="grid grid-cols-1 gap-4 md:grid-cols-2">
          <FormField
            control={form.control}
            name="title"
            render={({ field }) => (
              <FormItem>
                <FormLabel>{m.title}</FormLabel>
                <FormControl>
                  <Input
                    placeholder={m.titlePlaceholder}
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
            name="sku"
            render={({ field }) => (
              <FormItem>
                <FormLabel>{m.sku}</FormLabel>
                <FormControl>
                  <Input
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
            name="price"
            render={({ field }) => (
              <FormItem>
                <FormLabel>{m.price}</FormLabel>
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
            name="stockQuantity"
            render={({ field }) => (
              <FormItem>
                <FormLabel>{m.stock}</FormLabel>
                <FormControl>
                  <Input type="number" dir="ltr" disabled={isPending} {...field} />
                </FormControl>
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
                  <Input type="number" dir="ltr" disabled={isPending} {...field} />
                </FormControl>
                <FormMessage />
              </FormItem>
            )}
          />
          <FormField
            control={form.control}
            name="isActive"
            render={({ field }) => (
              <FormItem className="flex flex-row items-center gap-2 pt-6">
                <FormControl>
                  <Checkbox
                    checked={field.value}
                    disabled={isPending}
                    onCheckedChange={field.onChange}
                  />
                </FormControl>
                <FormLabel className="!mt-0">{m.active}</FormLabel>
              </FormItem>
            )}
          />
        </div>
        <div className="flex gap-2">
          <Button type="submit" size="sm" disabled={isPending}>
            {isPending ? (
              <>
                <Loader2 className="size-4 animate-spin" />
                {messages.product.form.saving}
              </>
            ) : (
              m.save
            )}
          </Button>
          <Button type="button" variant="outline" size="sm" disabled={isPending} onClick={onDone}>
            <X className="size-4" />
            {m.cancel}
          </Button>
        </div>
      </form>
    </Form>
  );
}

interface ProductVariantSectionProps {
  productId: number;
}

export function ProductVariantSection({ productId }: ProductVariantSectionProps) {
  const [showForm, setShowForm] = useState(false);
  const [editingVariant, setEditingVariant] = useState<ProductVariantInfo | null>(null);
  const [deleteTarget, setDeleteTarget] = useState<ProductVariantInfo | null>(null);

  const { data, isLoading } = useProductVariants(productId, {
    limit: 50,
    sortBy: 'sortOrder',
    sortOrder: 'asc',
  });
  const { mutate: deleteVariant, isPending: isDeleting } = useDeleteProductVariant(productId);

  const variants = data?.items ?? [];

  function handleFormDone() {
    setShowForm(false);
    setEditingVariant(null);
  }

  function handleConfirmDelete() {
    if (!deleteTarget) return;
    deleteVariant(deleteTarget.id, { onSettled: () => setDeleteTarget(null) });
  }

  return (
    <>
      <Card className="shadow-xs">
        <CardHeader className="flex flex-row items-center justify-between">
          <CardTitle className="text-base">{m.sectionTitle}</CardTitle>
          {!showForm && !editingVariant && (
            <Button
              type="button"
              variant="outline"
              size="sm"
              onClick={() => setShowForm(true)}
            >
              <Plus className="size-4" />
              {m.addVariant}
            </Button>
          )}
        </CardHeader>
        <CardContent className="space-y-4">
          {showForm && (
            <VariantForm productId={productId} onDone={handleFormDone} />
          )}

          {editingVariant && (
            <VariantForm
              productId={productId}
              initialVariant={editingVariant}
              onDone={handleFormDone}
            />
          )}

          {isLoading ? (
            <div className="space-y-2">
              {Array.from({ length: 3 }).map((_, i) => (
                <Skeleton key={i} className="h-10 w-full" />
              ))}
            </div>
          ) : variants.length > 0 ? (
            <div className="overflow-x-auto rounded-lg border">
              <Table>
                <TableHeader>
                  <TableRow>
                    <TableHead>{m.title}</TableHead>
                    <TableHead>{m.sku}</TableHead>
                    <TableHead>{m.price}</TableHead>
                    <TableHead>{m.stock}</TableHead>
                    <TableHead>{m.active}</TableHead>
                    <TableHead>{messages.common.more}</TableHead>
                  </TableRow>
                </TableHeader>
                <TableBody>
                  {variants.map((variant) => (
                    <TableRow key={variant.id}>
                      <TableCell>{variant.title ?? messages.product.view.empty}</TableCell>
                      <TableCell dir="ltr">{variant.sku ?? messages.product.view.empty}</TableCell>
                      <TableCell dir="ltr">
                        {variant.price != null
                          ? formatCurrency(variant.price)
                          : messages.product.view.empty}
                      </TableCell>
                      <TableCell dir="ltr">{variant.stockQuantity}</TableCell>
                      <TableCell>
                        {variant.isActive ? messages.common.yes : messages.common.no}
                      </TableCell>
                      <TableCell>
                        <div className="flex gap-1">
                          <Button
                            type="button"
                            variant="ghost"
                            size="icon-sm"
                            disabled={!!editingVariant || showForm}
                            onClick={() => setEditingVariant(variant)}
                          >
                            <Pencil className="size-4" />
                          </Button>
                          <Button
                            type="button"
                            variant="ghost"
                            size="icon-sm"
                            className="text-destructive hover:bg-destructive/10 hover:text-destructive"
                            disabled={isDeleting}
                            onClick={() => setDeleteTarget(variant)}
                          >
                            <Trash2 className="size-4" />
                          </Button>
                        </div>
                      </TableCell>
                    </TableRow>
                  ))}
                </TableBody>
              </Table>
            </div>
          ) : !showForm ? (
            <div className="text-muted-foreground flex min-h-24 items-center justify-center rounded-lg border border-dashed border-border/70 bg-muted/10 px-4 py-6 text-center text-sm">
              {m.noVariants}
            </div>
          ) : null}
        </CardContent>
      </Card>

      <AlertDialog open={!!deleteTarget} onOpenChange={(open) => !open && setDeleteTarget(null)}>
        <AlertDialogContent>
          <AlertDialogHeader>
            <AlertDialogTitle>{m.deleteTitle}</AlertDialogTitle>
            <AlertDialogDescription>
              {m.deleteDescription(deleteTarget?.title ?? deleteTarget?.sku ?? '—')}
            </AlertDialogDescription>
          </AlertDialogHeader>
          <AlertDialogFooter>
            <AlertDialogCancel disabled={isDeleting}>{m.cancel}</AlertDialogCancel>
            <AlertDialogAction
              variant="destructive"
              onClick={handleConfirmDelete}
              disabled={isDeleting}
            >
              {messages.common.delete}
            </AlertDialogAction>
          </AlertDialogFooter>
        </AlertDialogContent>
      </AlertDialog>
    </>
  );
}
