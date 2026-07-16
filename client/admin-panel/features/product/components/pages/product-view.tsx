'use client';

import { useState } from 'react';
import Image from 'next/image';
import Link from 'next/link';
import { useRouter } from 'next/navigation';
import {
  ArrowRight,
  Copy,
  Package,
  Pencil,
  Star,
  Trash2,
} from 'lucide-react';
import { toast } from 'sonner';
import { Button } from '@/components/ui/button';
import {
  Card,
  CardContent,
  CardDescription,
  CardHeader,
  CardTitle,
} from '@/components/ui/card';
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
import { Separator } from '@/components/ui/separator';
import {
  Tooltip,
  TooltipContent,
  TooltipTrigger,
} from '@/components/ui/tooltip';
import { Badge } from '@/components/ui/badge';
import { Skeleton } from '@/components/ui/skeleton';
import { formatCurrency, formatDate, messages } from '@/lib/messages.ar';
import { resolveAssetUrl } from '@/lib/asset-url';
import { cn } from '@/lib/utils';
import type { ProductDto } from '../../types';
import { useProduct } from '../../hooks/useProduct';
import { useProductImages } from '../../hooks/useProductImage';
import { useDeleteProduct } from '../../hooks/useProductMutations';
import { ProductStatusBadge } from '../blocks/product-status-badge';

function DetailField({
  label,
  value,
  dir,
  className,
}: {
  label: string;
  value: React.ReactNode;
  dir?: 'rtl' | 'ltr' | 'auto';
  className?: string;
}) {
  return (
    <div
      className={cn(
        'space-y-1.5 rounded-lg border border-border/50 bg-muted/20 px-3 py-2.5',
        className,
      )}
    >
      <dt className="text-muted-foreground text-xs font-medium">{label}</dt>
      <dd className="text-sm leading-relaxed" dir={dir}>
        {value}
      </dd>
    </div>
  );
}

function DescriptionBlock({
  label,
  value,
  dir,
}: {
  label: string;
  value: string | null;
  dir: 'rtl' | 'ltr';
}) {
  const v = messages.product.view;

  return (
    <div className="space-y-2">
      <h4 className="text-sm font-medium">{label}</h4>
      {value ? (
        <p
          className="text-muted-foreground rounded-lg border border-border/50 bg-muted/20 p-3 text-sm leading-relaxed whitespace-pre-wrap"
          dir={dir}
        >
          {value}
        </p>
      ) : (
        <div className="text-muted-foreground flex min-h-24 items-center justify-center rounded-lg border border-dashed border-border/70 bg-muted/10 px-4 py-6 text-center text-sm">
          {v.noDescription}
        </div>
      )}
    </div>
  );
}

interface ProductViewProps {
  product: ProductDto;
}

export function ProductView({ product }: ProductViewProps) {
  const router = useRouter();
  const [deleteOpen, setDeleteOpen] = useState(false);
  const { mutate: deleteProduct, isPending: isDeleting } = useDeleteProduct();

  const { data: liveProduct } = useProduct(product.id, { initialData: product });
  const { data: imagesData, isLoading: isImagesLoading } = useProductImages(product.id, {
    limit: 50,
    sortBy: 'sortOrder',
    sortOrder: 'asc',
  });
  const p = liveProduct ?? product;
  const images = imagesData?.items ?? [];
  const primaryImage = images.find((img) => img.isPrimary) ?? images[0];
  const primaryImageUrl = resolveAssetUrl(primaryImage?.imageUrl);

  const m = messages.product.view;
  const f = messages.product.form;
  const img = messages.product.image;
  const v = messages.product.variant;
  const displayName = p.nameAr ?? p.nameEn;

  function handleCopySlug() {
    void navigator.clipboard.writeText(p.slug);
    toast.success(m.slugCopied);
  }

  function handleConfirmDelete() {
    deleteProduct(p.id, {
      onSuccess: () => router.push('/products'),
      onSettled: () => setDeleteOpen(false),
    });
  }

  return (
    <>
      <div className="flex flex-col gap-4 sm:flex-row sm:items-start sm:justify-between">
        <div className="space-y-1">
          <Button variant="ghost" size="sm" className="-ms-2 mb-1" asChild>
            <Link href="/products">
              <ArrowRight className="size-4" />
              {m.backToList}
            </Link>
          </Button>
          <div className="flex items-center gap-3">
            <div className="bg-muted relative flex size-10 shrink-0 items-center justify-center overflow-hidden rounded-lg">
              {primaryImageUrl ? (
                <Image
                  src={primaryImageUrl}
                  alt={primaryImage?.altText ?? p.nameEn}
                  fill
                  unoptimized
                  className="object-cover"
                />
              ) : (
                <Package className="text-primary size-5" />
              )}
            </div>
            <div>
              <h1 className="text-xl font-semibold tracking-tight">{p.nameEn}</h1>
              {p.nameAr && (
                <p className="text-muted-foreground text-sm" dir="rtl">
                  {p.nameAr}
                </p>
              )}
            </div>
          </div>
          <div className="flex flex-wrap items-center gap-2 pt-1">
            <ProductStatusBadge status={p.status} />
            {p.isFeatured && (
              <Badge variant="secondary">{f.isFeatured}</Badge>
            )}
          </div>
        </div>

        <div className="flex flex-wrap gap-2">
          <Tooltip>
            <TooltipTrigger asChild>
              <Button variant="outline" size="icon-sm" onClick={handleCopySlug}>
                <Copy className="size-4" />
                <span className="sr-only">{messages.common.copy}</span>
              </Button>
            </TooltipTrigger>
            <TooltipContent>{messages.common.copy}</TooltipContent>
          </Tooltip>
          <Button variant="outline" size="sm" asChild>
            <Link href={`/products/${p.id}/edit`}>
              <Pencil className="size-4" />
              {messages.common.edit}
            </Link>
          </Button>
          <Button
            variant="outline"
            size="sm"
            className="text-destructive hover:bg-destructive/10 hover:text-destructive"
            onClick={() => setDeleteOpen(true)}
            disabled={isDeleting}
          >
            <Trash2 className="size-4" />
            {messages.common.delete}
          </Button>
        </div>
      </div>

      <div className="grid gap-6 lg:grid-cols-[1fr_300px]">
        <div className="space-y-6">
          <Card className="shadow-xs">
            <CardHeader>
              <CardTitle className="text-base">{img.sectionTitle}</CardTitle>
            </CardHeader>
            <CardContent>
              {isImagesLoading ? (
                <div className="grid grid-cols-2 gap-4 sm:grid-cols-3 md:grid-cols-4">
                  {Array.from({ length: 4 }).map((_, i) => (
                    <Skeleton key={i} className="aspect-square rounded-lg" />
                  ))}
                </div>
              ) : images.length > 0 ? (
                <div className="grid grid-cols-2 gap-4 sm:grid-cols-3 md:grid-cols-4">
                  {images.map((image) => {
                    const url = resolveAssetUrl(image.imageUrl);
                    return (
                      <div
                        key={image.id}
                        className="space-y-2 rounded-lg border border-border/60 p-2"
                      >
                        <div className="relative aspect-square overflow-hidden rounded-md bg-muted">
                          {url && (
                            <Image
                              src={url}
                              alt={image.altText ?? p.nameEn}
                              fill
                              unoptimized
                              className="object-contain"
                            />
                          )}
                          {image.isPrimary && (
                            <Badge className="absolute start-2 top-2" variant="secondary">
                              <Star className="size-3 fill-current" />
                              {img.primary}
                            </Badge>
                          )}
                        </div>
                        {image.altText && (
                          <p className="text-muted-foreground truncate text-xs" dir="ltr">
                            {image.altText}
                          </p>
                        )}
                      </div>
                    );
                  })}
                </div>
              ) : (
                <div className="text-muted-foreground flex min-h-24 items-center justify-center rounded-lg border border-dashed border-border/70 bg-muted/10 px-4 py-6 text-center text-sm">
                  {img.noImages}
                </div>
              )}
            </CardContent>
          </Card>

          <Card className="shadow-xs">
            <CardHeader>
              <CardTitle className="text-base">{f.sections.basic}</CardTitle>
              <CardDescription dir="ltr">
                <code className="bg-muted rounded px-1.5 py-0.5 text-xs">{p.slug}</code>
              </CardDescription>
            </CardHeader>
            <CardContent>
              <dl className="grid grid-cols-1 gap-3 sm:grid-cols-2">
                <DetailField label={f.sku} value={p.sku ?? m.empty} dir="ltr" />
                <DetailField label={f.barcode} value={p.barcode ?? m.empty} dir="ltr" />
                <DetailField
                  label={f.category}
                  value={p.category?.nameAr ?? p.category?.nameEn ?? m.empty}
                />
                <DetailField label={f.brand} value={p.brand?.name ?? m.empty} />
              </dl>
            </CardContent>
          </Card>

          <Card className="shadow-xs">
            <CardHeader>
              <CardTitle className="text-base">{f.sections.descriptions}</CardTitle>
            </CardHeader>
            <CardContent className="space-y-4">
              <DescriptionBlock label={f.shortDescriptionEn} value={p.shortDescriptionEn} dir="ltr" />
              <DescriptionBlock label={f.shortDescriptionAr} value={p.shortDescriptionAr} dir="rtl" />
              <Separator />
              <DescriptionBlock label={f.descriptionEn} value={p.descriptionEn} dir="ltr" />
              <DescriptionBlock label={f.descriptionAr} value={p.descriptionAr} dir="rtl" />
            </CardContent>
          </Card>

          <Card className="shadow-xs">
            <CardHeader>
              <CardTitle className="text-base">{m.variants}</CardTitle>
            </CardHeader>
            <CardContent>
              {p.variants.length > 0 ? (
                <div className="overflow-x-auto rounded-lg border">
                  <Table>
                    <TableHeader>
                      <TableRow>
                        <TableHead>{m.variantColumns.title}</TableHead>
                        <TableHead>{m.variantColumns.sku}</TableHead>
                        <TableHead>{f.barcode}</TableHead>
                        <TableHead>{m.variantColumns.price}</TableHead>
                        <TableHead>{v.compareAtPrice}</TableHead>
                        <TableHead>{m.variantColumns.stock}</TableHead>
                        <TableHead>{v.weight}</TableHead>
                        <TableHead>{m.variantColumns.active}</TableHead>
                      </TableRow>
                    </TableHeader>
                    <TableBody>
                      {p.variants.map((variant) => (
                        <TableRow key={variant.id}>
                          <TableCell>{variant.title ?? m.empty}</TableCell>
                          <TableCell dir="ltr">{variant.sku ?? m.empty}</TableCell>
                          <TableCell dir="ltr">{variant.barcode ?? m.empty}</TableCell>
                          <TableCell dir="ltr">
                            {variant.price != null
                              ? formatCurrency(variant.price, p.currency)
                              : m.empty}
                          </TableCell>
                          <TableCell dir="ltr">
                            {variant.compareAtPrice != null
                              ? formatCurrency(variant.compareAtPrice, p.currency)
                              : m.empty}
                          </TableCell>
                          <TableCell dir="ltr">{variant.stockQuantity}</TableCell>
                          <TableCell dir="ltr">
                            {variant.weight != null ? variant.weight : m.empty}
                          </TableCell>
                          <TableCell>
                            {variant.isActive ? messages.common.yes : messages.common.no}
                          </TableCell>
                        </TableRow>
                      ))}
                    </TableBody>
                  </Table>
                </div>
              ) : (
                <div className="text-muted-foreground flex min-h-24 items-center justify-center rounded-lg border border-dashed border-border/70 bg-muted/10 px-4 py-6 text-center text-sm">
                  {m.noVariants}
                </div>
              )}
            </CardContent>
          </Card>

          <Card className="shadow-xs">
            <CardHeader>
              <CardTitle className="text-base">{m.attributeValues}</CardTitle>
            </CardHeader>
            <CardContent>
              {p.attributeValues.length > 0 ? (
                <dl className="grid grid-cols-1 gap-3 sm:grid-cols-2">
                  {p.attributeValues.map((av) => {
                    const attrName = av.attribute.nameAr ?? av.attribute.nameEn;
                    const value =
                      av.valueText ??
                      (av.valueNumber != null ? String(av.valueNumber) : null) ??
                      (av.valueBoolean != null
                        ? av.valueBoolean
                          ? messages.common.yes
                          : messages.common.no
                        : null) ??
                      m.empty;
                    return (
                      <DetailField
                        key={av.id}
                        label={attrName}
                        value={
                          av.attribute.unit ? `${value} ${av.attribute.unit}` : value
                        }
                      />
                    );
                  })}
                </dl>
              ) : (
                <div className="text-muted-foreground flex min-h-24 items-center justify-center rounded-lg border border-dashed border-border/70 bg-muted/10 px-4 py-6 text-center text-sm">
                  {m.noAttributeValues}
                </div>
              )}
            </CardContent>
          </Card>
        </div>

        <div className="space-y-6">
          <Card className="shadow-xs">
            <CardHeader>
              <CardTitle className="text-base">{f.sections.pricing}</CardTitle>
            </CardHeader>
            <CardContent>
              <dl className="space-y-3">
                <DetailField label={f.currency} value={p.currency} dir="ltr" />
                <DetailField
                  label={f.price}
                  value={formatCurrency(p.price, p.currency)}
                  dir="ltr"
                />
                <DetailField
                  label={f.compareAtPrice}
                  value={
                    p.compareAtPrice != null
                      ? formatCurrency(p.compareAtPrice, p.currency)
                      : m.empty
                  }
                  dir="ltr"
                />
                <DetailField
                  label={f.costPrice}
                  value={
                    p.costPrice != null
                      ? formatCurrency(p.costPrice, p.currency)
                      : m.empty
                  }
                  dir="ltr"
                />
              </dl>
            </CardContent>
          </Card>

          <Card className="shadow-xs">
            <CardHeader>
              <CardTitle className="text-base">{f.sections.inventory}</CardTitle>
            </CardHeader>
            <CardContent>
              <dl className="space-y-3">
                <DetailField label={f.stockQuantity} value={p.stockQuantity} dir="ltr" />
                <DetailField label={f.lowStockThreshold} value={p.lowStockThreshold} dir="ltr" />
                <DetailField label={f.minOrderQuantity} value={p.minOrderQuantity} dir="ltr" />
                <DetailField
                  label={f.weight}
                  value={p.weight != null ? p.weight : m.empty}
                  dir="ltr"
                />
              </dl>
            </CardContent>
          </Card>

          <Card className="shadow-xs">
            <CardHeader>
              <CardTitle className="text-base">{f.sections.settings}</CardTitle>
            </CardHeader>
            <CardContent>
              <dl className="space-y-3">
                <DetailField label={f.status} value={<ProductStatusBadge status={p.status} />} />
                <DetailField
                  label={f.requiresShipping}
                  value={p.requiresShipping ? messages.common.yes : messages.common.no}
                />
                <DetailField
                  label={f.isFeatured}
                  value={p.isFeatured ? messages.common.yes : messages.common.no}
                />
                <DetailField label={m.totalSold} value={p.totalSold} dir="ltr" />
                <DetailField label={m.viewsCount} value={p.viewsCount} dir="ltr" />
              </dl>
            </CardContent>
          </Card>

          <Card className="shadow-xs">
            <CardHeader>
              <CardTitle className="text-base">{f.sections.warranty}</CardTitle>
            </CardHeader>
            <CardContent>
              <dl className="space-y-3">
                <DetailField label={f.warrantyMonths} value={p.warrantyMonths} dir="ltr" />
                <DetailField
                  label={f.warrantyDescription}
                  value={p.warrantyDescription ?? m.empty}
                  dir="rtl"
                />
              </dl>
            </CardContent>
          </Card>

          <Card className="shadow-xs">
            <CardHeader>
              <CardTitle className="text-base">{f.sections.seo}</CardTitle>
            </CardHeader>
            <CardContent>
              <dl className="space-y-3">
                <DetailField label={f.metaTitle} value={p.metaTitle ?? m.empty} dir="ltr" />
                <DetailField
                  label={f.metaDescription}
                  value={p.metaDescription ?? m.empty}
                  dir="ltr"
                />
              </dl>
            </CardContent>
          </Card>

          <Card className="shadow-xs">
            <CardHeader>
              <CardTitle className="text-base">{messages.common.more}</CardTitle>
            </CardHeader>
            <CardContent>
              <dl className="space-y-3">
                <DetailField label={m.productId} value={p.id} dir="ltr" />
                <Separator />
                <DetailField
                  label={m.createdAt}
                  value={formatDate(p.createdAt, {
                    dateStyle: 'medium',
                    timeStyle: 'short',
                  })}
                />
                <DetailField
                  label={m.updatedAt}
                  value={
                    p.updatedAt
                      ? formatDate(p.updatedAt, { dateStyle: 'medium', timeStyle: 'short' })
                      : m.notUpdated
                  }
                />
              </dl>
            </CardContent>
          </Card>
        </div>
      </div>

      <AlertDialog open={deleteOpen} onOpenChange={setDeleteOpen}>
        <AlertDialogContent>
          <AlertDialogHeader>
            <AlertDialogTitle>{m.deleteTitle}</AlertDialogTitle>
            <AlertDialogDescription>{m.deleteDescription(displayName)}</AlertDialogDescription>
          </AlertDialogHeader>
          <AlertDialogFooter>
            <AlertDialogCancel disabled={isDeleting}>{f.cancel}</AlertDialogCancel>
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
