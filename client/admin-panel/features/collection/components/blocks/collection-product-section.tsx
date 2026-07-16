'use client';

import { useMemo, useState } from 'react';
import { useForm, type Resolver } from 'react-hook-form';
import { zodResolver } from '@hookform/resolvers/zod';
import { Loader2, Pencil, Plus, Trash2, X } from 'lucide-react';

import { Button } from '@/components/ui/button';
import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card';
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
import { useProducts } from '@/features/product';

import {
  addCollectionProductSchema,
  updateCollectionProductSchema,
  type AddCollectionProductInput,
  type UpdateCollectionProductInput,
} from '../../schemas/collection-product.schema';
import { useCollectionProducts } from '../../hooks/useCollectionProduct';
import {
  useAddCollectionProduct,
  useUpdateCollectionProduct,
  useDeleteCollectionProduct,
} from '../../hooks/useCollectionProductMutations';
import type { CollectionProductInfo } from '../../types';

const m = messages.collection.product;

function getAddDefaultValues(): AddCollectionProductInput {
  return {
    productId: 0,
    sortOrder: 0,
  };
}

function getUpdateDefaultValues(item: CollectionProductInfo): UpdateCollectionProductInput {
  return {
    sortOrder: item.sortOrder,
  };
}

interface AddProductFormProps {
  collectionId: number;
  usedProductIds: number[];
  onDone: () => void;
}

function AddProductForm({ collectionId, usedProductIds, onDone }: AddProductFormProps) {
  const { data: productsData, isLoading: isLoadingProducts } = useProducts(
    { limit: 200, sortBy: 'nameEn', sortOrder: 'asc' },
    true,
  );
  const { mutateAsync: addProduct, isPending } = useAddCollectionProduct(collectionId);

  const form = useForm<AddCollectionProductInput>({
    resolver: zodResolver(addCollectionProductSchema) as Resolver<AddCollectionProductInput>,
    defaultValues: getAddDefaultValues(),
  });

  const availableProducts = useMemo(
    () => (productsData?.items ?? []).filter((product) => !usedProductIds.includes(product.id)),
    [productsData?.items, usedProductIds],
  );

  async function onSubmit(values: AddCollectionProductInput) {
    try {
      await addProduct({
        productId: values.productId,
        sortOrder: values.sortOrder,
      });
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
            name="productId"
            render={({ field }) => (
              <FormItem>
                <FormLabel>{m.product}</FormLabel>
                <Select
                  onValueChange={(v) => field.onChange(Number(v))}
                  value={field.value > 0 ? String(field.value) : undefined}
                  disabled={isPending || isLoadingProducts}
                >
                  <FormControl>
                    <SelectTrigger>
                      <SelectValue placeholder={m.productPlaceholder} />
                    </SelectTrigger>
                  </FormControl>
                  <SelectContent>
                    {availableProducts.map((product) => (
                      <SelectItem key={product.id} value={String(product.id)}>
                        {product.nameAr ?? product.nameEn}
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
        </div>
        <div className="flex gap-2">
          <Button type="submit" size="sm" disabled={isPending || availableProducts.length === 0}>
            {isPending ? (
              <>
                <Loader2 className="size-4 animate-spin" />
                {messages.collection.form.saving}
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

interface EditProductFormProps {
  collectionId: number;
  item: CollectionProductInfo;
  onDone: () => void;
}

function EditProductForm({ collectionId, item, onDone }: EditProductFormProps) {
  const { mutateAsync: updateProduct, isPending } = useUpdateCollectionProduct(collectionId);

  const form = useForm<UpdateCollectionProductInput>({
    resolver: zodResolver(updateCollectionProductSchema) as Resolver<UpdateCollectionProductInput>,
    defaultValues: getUpdateDefaultValues(item),
  });

  async function onSubmit(values: UpdateCollectionProductInput) {
    try {
      await updateProduct({
        id: item.id,
        payload: { sortOrder: values.sortOrder },
      });
      onDone();
    } catch {
      // Error surfaced via mutation onError
    }
  }

  return (
    <Form {...form}>
      <form onSubmit={form.handleSubmit(onSubmit)} className="space-y-4 rounded-lg border p-4">
        <div className="grid grid-cols-1 gap-4 md:grid-cols-2">
          <FormItem>
            <FormLabel>{m.product}</FormLabel>
            <FormControl>
              <Input
                value={item.product.nameAr ?? item.product.nameEn}
                disabled
                dir={item.product.nameAr ? 'rtl' : 'ltr'}
              />
            </FormControl>
          </FormItem>
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
        </div>
        <div className="flex gap-2">
          <Button type="submit" size="sm" disabled={isPending}>
            {isPending ? (
              <>
                <Loader2 className="size-4 animate-spin" />
                {messages.collection.form.saving}
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

interface CollectionProductSectionProps {
  collectionId: number;
}

export function CollectionProductSection({ collectionId }: CollectionProductSectionProps) {
  const [showForm, setShowForm] = useState(false);
  const [editingItem, setEditingItem] = useState<CollectionProductInfo | null>(null);
  const [deleteTarget, setDeleteTarget] = useState<CollectionProductInfo | null>(null);

  const { data, isLoading } = useCollectionProducts(collectionId, {
    limit: 50,
    sortBy: 'sortOrder',
    sortOrder: 'asc',
  });
  const { mutate: deleteProduct, isPending: isDeleting } =
    useDeleteCollectionProduct(collectionId);

  const items = data?.items ?? [];
  const usedProductIds = items.map((item) => item.productId);

  function handleFormDone() {
    setShowForm(false);
    setEditingItem(null);
  }

  function handleConfirmDelete() {
    if (!deleteTarget) return;
    deleteProduct(deleteTarget.id, { onSettled: () => setDeleteTarget(null) });
  }

  return (
    <>
      <Card className="shadow-xs">
        <CardHeader className="flex flex-row items-center justify-between">
          <CardTitle className="text-base">{m.sectionTitle}</CardTitle>
          {!showForm && !editingItem && (
            <Button type="button" variant="outline" size="sm" onClick={() => setShowForm(true)}>
              <Plus className="size-4" />
              {m.addProduct}
            </Button>
          )}
        </CardHeader>
        <CardContent className="space-y-4">
          {showForm && (
            <AddProductForm
              collectionId={collectionId}
              usedProductIds={usedProductIds}
              onDone={handleFormDone}
            />
          )}

          {editingItem && (
            <EditProductForm
              collectionId={collectionId}
              item={editingItem}
              onDone={handleFormDone}
            />
          )}

          {isLoading ? (
            <div className="space-y-2">
              {Array.from({ length: 3 }).map((_, i) => (
                <Skeleton key={i} className="h-10 w-full" />
              ))}
            </div>
          ) : items.length > 0 ? (
            <div className="overflow-x-auto rounded-lg border">
              <Table>
                <TableHeader>
                  <TableRow>
                    <TableHead>{m.nameEn}</TableHead>
                    <TableHead>{m.nameAr}</TableHead>
                    <TableHead>{m.slug}</TableHead>
                    <TableHead>{m.price}</TableHead>
                    <TableHead>{m.sortOrder}</TableHead>
                    <TableHead>{messages.common.more}</TableHead>
                  </TableRow>
                </TableHeader>
                <TableBody>
                  {items.map((item) => (
                    <TableRow key={item.id}>
                      <TableCell dir="ltr">{item.product.nameEn}</TableCell>
                      <TableCell dir="rtl">
                        {item.product.nameAr ?? messages.collection.view.empty}
                      </TableCell>
                      <TableCell dir="ltr">{item.product.slug}</TableCell>
                      <TableCell dir="ltr">{formatCurrency(item.product.price)}</TableCell>
                      <TableCell dir="ltr">{item.sortOrder}</TableCell>
                      <TableCell>
                        <div className="flex gap-1">
                          <Button
                            type="button"
                            variant="ghost"
                            size="icon-sm"
                            disabled={!!editingItem || showForm}
                            onClick={() => setEditingItem(item)}
                          >
                            <Pencil className="size-4" />
                          </Button>
                          <Button
                            type="button"
                            variant="ghost"
                            size="icon-sm"
                            className="text-destructive hover:bg-destructive/10 hover:text-destructive"
                            disabled={isDeleting}
                            onClick={() => setDeleteTarget(item)}
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
              {m.noProducts}
            </div>
          ) : null}
        </CardContent>
      </Card>

      <AlertDialog open={!!deleteTarget} onOpenChange={(open) => !open && setDeleteTarget(null)}>
        <AlertDialogContent>
          <AlertDialogHeader>
            <AlertDialogTitle>{m.deleteTitle}</AlertDialogTitle>
            <AlertDialogDescription>
              {m.deleteDescription(deleteTarget?.product.nameEn ?? '—')}
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
