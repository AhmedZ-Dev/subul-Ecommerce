'use client';

import { useMemo, useState } from 'react';
import { useForm, type Resolver } from 'react-hook-form';
import { zodResolver } from '@hookform/resolvers/zod';
import { useQueries } from '@tanstack/react-query';
import { Loader2, Plus, Trash2, X } from 'lucide-react';
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
import { Skeleton } from '@/components/ui/skeleton';
import {
  attributeGroupKeys,
  getAttributeGroupById,
  useAttributeGroups,
  type AttributeDto,
} from '@/features/attribute-group';
import { messages } from '@/lib/messages.ar';

import {
  createProductAttributeValueSchema,
  type CreateProductAttributeValueInput,
} from '../../schemas/product-attribute-value.schema';
import { useProductAttributeValues } from '../../hooks/useProductAttributeValue';
import {
  useCreateProductAttributeValue,
  useDeleteProductAttributeValue,
} from '../../hooks/useProductAttributeValueMutations';
import type { ProductAttributeValueInfo } from '../../types';

const m = messages.product.attributeValue;

function useAllAttributes() {
  const { data: groupsData, isLoading: isListLoading } = useAttributeGroups({
    limit: 200,
    sortBy: 'nameEn',
    sortOrder: 'asc',
  });

  const groupIds = groupsData?.items.map((g) => g.id) ?? [];

  const groupQueries = useQueries({
    queries: groupIds.map((id) => ({
      queryKey: attributeGroupKeys.detail(id),
      queryFn: () => getAttributeGroupById(id),
      staleTime: 60_000,
      enabled: id > 0,
    })),
  });

  const allAttributes = useMemo(() => {
    const attrs: AttributeDto[] = [];
    for (const query of groupQueries) {
      if (query.data?.attributes) {
        attrs.push(...query.data.attributes);
      }
    }
    return attrs.sort((a, b) => a.sortOrder - b.sortOrder);
  }, [groupQueries]);

  const isLoading =
    isListLoading || groupQueries.some((q) => q.isLoading);

  return { allAttributes, isLoading };
}

function formatDisplayValue(av: ProductAttributeValueInfo): string {
  const value =
    av.valueText ??
    (av.valueNumber != null ? String(av.valueNumber) : null) ??
    (av.valueBoolean != null
      ? av.valueBoolean
        ? messages.common.yes
        : messages.common.no
      : null) ??
    messages.product.view.empty;

  return av.attribute.unit ? `${value} ${av.attribute.unit}` : value;
}

interface AttributeValueFormProps {
  productId: number;
  allAttributes: AttributeDto[];
  usedAttributeIds: number[];
  onDone: () => void;
}

function AttributeValueForm({
  productId,
  allAttributes,
  usedAttributeIds,
  onDone,
}: AttributeValueFormProps) {
  const { mutateAsync: createValue, isPending } = useCreateProductAttributeValue(productId);

  const form = useForm<CreateProductAttributeValueInput>({
    resolver: zodResolver(createProductAttributeValueSchema) as Resolver<CreateProductAttributeValueInput>,
    defaultValues: {
      attributeId: 0,
      valueText: '',
      valueNumber: null,
      valueBoolean: null,
    },
  });

  const selectedAttributeId = form.watch('attributeId');
  const selectedAttribute = allAttributes.find((a) => a.id === selectedAttributeId);
  const inputType = selectedAttribute?.inputType?.toLowerCase() ?? 'text';

  async function onSubmit(values: CreateProductAttributeValueInput) {
    if (usedAttributeIds.includes(values.attributeId)) {
      toast.error(m.alreadyUsed);
      return;
    }

    try {
      await createValue({
        attributeId: values.attributeId,
        valueText: values.valueText || undefined,
        valueNumber: values.valueNumber ?? null,
        valueBoolean: values.valueBoolean ?? null,
      });
      toast.success(m.createSuccess);
      onDone();
    } catch {
      // Error surfaced via mutation onError
    }
  }

  return (
    <Form {...form}>
      <form onSubmit={form.handleSubmit(onSubmit)} className="space-y-4 rounded-lg border p-4">
        <FormField
          control={form.control}
          name="attributeId"
          render={({ field }) => (
            <FormItem>
              <FormLabel>{m.attribute}</FormLabel>
              <Select
                onValueChange={(v) => field.onChange(Number(v))}
                value={field.value > 0 ? String(field.value) : undefined}
                disabled={isPending}
              >
                <FormControl>
                  <SelectTrigger>
                    <SelectValue placeholder={m.attributePlaceholder} />
                  </SelectTrigger>
                </FormControl>
                <SelectContent>
                  {allAttributes.map((attr) => (
                    <SelectItem
                      key={attr.id}
                      value={String(attr.id)}
                      disabled={usedAttributeIds.includes(attr.id)}
                    >
                      {attr.nameAr ?? attr.nameEn}
                      {usedAttributeIds.includes(attr.id) ? ` (${m.alreadyUsed})` : ''}
                    </SelectItem>
                  ))}
                </SelectContent>
              </Select>
              <FormMessage />
            </FormItem>
          )}
        />

        {(inputType === 'text' || inputType === 'select') && selectedAttributeId > 0 && (
          <FormField
            control={form.control}
            name="valueText"
            render={({ field }) => (
              <FormItem>
                <FormLabel>{m.value}</FormLabel>
                <FormControl>
                  <Input
                    placeholder={m.valuePlaceholder}
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
        )}

        {inputType === 'number' && selectedAttributeId > 0 && (
          <FormField
            control={form.control}
            name="valueNumber"
            render={({ field }) => (
              <FormItem>
                <FormLabel>{m.value}</FormLabel>
                <FormControl>
                  <Input
                    type="number"
                    step="any"
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
        )}

        {inputType === 'boolean' && selectedAttributeId > 0 && (
          <FormField
            control={form.control}
            name="valueBoolean"
            render={({ field }) => (
              <FormItem className="flex flex-row items-center gap-2">
                <FormControl>
                  <Checkbox
                    checked={field.value === true}
                    disabled={isPending}
                    onCheckedChange={(checked) => field.onChange(checked === true)}
                  />
                </FormControl>
                <FormLabel className="!mt-0">{m.value}</FormLabel>
              </FormItem>
            )}
          />
        )}

        <div className="flex gap-2">
          <Button type="submit" size="sm" disabled={isPending || selectedAttributeId <= 0}>
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

interface ProductAttributeValueSectionProps {
  productId: number;
}

export function ProductAttributeValueSection({ productId }: ProductAttributeValueSectionProps) {
  const [showForm, setShowForm] = useState(false);
  const [deleteTarget, setDeleteTarget] = useState<ProductAttributeValueInfo | null>(null);

  const { allAttributes, isLoading: isAttributesLoading } = useAllAttributes();
  const { data, isLoading: isValuesLoading } = useProductAttributeValues(productId, { limit: 50 });
  const { mutate: deleteValue, isPending: isDeleting } =
    useDeleteProductAttributeValue(productId);

  const values = data?.items ?? [];
  const usedAttributeIds = values.map((v) => v.attributeId);
  const isLoading = isValuesLoading || isAttributesLoading;

  function handleConfirmDelete() {
    if (!deleteTarget) return;
    deleteValue(deleteTarget.id, { onSettled: () => setDeleteTarget(null) });
  }

  return (
    <>
      <Card className="shadow-xs">
        <CardHeader className="flex flex-row items-center justify-between">
          <CardTitle className="text-base">{m.sectionTitle}</CardTitle>
          {!showForm && (
            <Button type="button" variant="outline" size="sm" onClick={() => setShowForm(true)}>
              <Plus className="size-4" />
              {m.addAttribute}
            </Button>
          )}
        </CardHeader>
        <CardContent className="space-y-4">
          {showForm && (
            <AttributeValueForm
              productId={productId}
              allAttributes={allAttributes}
              usedAttributeIds={usedAttributeIds}
              onDone={() => setShowForm(false)}
            />
          )}

          {isLoading ? (
            <div className="space-y-2">
              {Array.from({ length: 3 }).map((_, i) => (
                <Skeleton key={i} className="h-10 w-full" />
              ))}
            </div>
          ) : values.length > 0 ? (
            <dl className="grid grid-cols-1 gap-3 sm:grid-cols-2">
              {values.map((av) => {
                const attrName = av.attribute.nameAr ?? av.attribute.nameEn;
                return (
                  <div
                    key={av.id}
                    className="flex items-start justify-between gap-2 rounded-lg border border-border/50 bg-muted/20 px-3 py-2.5"
                  >
                    <div className="space-y-1">
                      <dt className="text-muted-foreground text-xs font-medium">{attrName}</dt>
                      <dd className="text-sm">{formatDisplayValue(av)}</dd>
                    </div>
                    <Button
                      type="button"
                      variant="ghost"
                      size="icon-sm"
                      className="text-destructive hover:bg-destructive/10 hover:text-destructive shrink-0"
                      disabled={isDeleting}
                      onClick={() => setDeleteTarget(av)}
                    >
                      <Trash2 className="size-4" />
                    </Button>
                  </div>
                );
              })}
            </dl>
          ) : !showForm ? (
            <div className="text-muted-foreground flex min-h-24 items-center justify-center rounded-lg border border-dashed border-border/70 bg-muted/10 px-4 py-6 text-center text-sm">
              {m.noAttributeValues}
            </div>
          ) : null}
        </CardContent>
      </Card>

      <AlertDialog open={!!deleteTarget} onOpenChange={(open) => !open && setDeleteTarget(null)}>
        <AlertDialogContent>
          <AlertDialogHeader>
            <AlertDialogTitle>{m.deleteTitle}</AlertDialogTitle>
            <AlertDialogDescription>
              {m.deleteDescription(
                deleteTarget?.attribute.nameAr ??
                  deleteTarget?.attribute.nameEn ??
                  '—',
              )}
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
