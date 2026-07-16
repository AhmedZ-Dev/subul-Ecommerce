'use client';

import { useState } from 'react';
import Image from 'next/image';
import { Loader2, Star, Trash2 } from 'lucide-react';
import { toast } from 'sonner';

import { Badge } from '@/components/ui/badge';
import { Button } from '@/components/ui/button';
import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card';
import { Input } from '@/components/ui/input';
import { Checkbox } from '@/components/ui/checkbox';
import { Label } from '@/components/ui/label';
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
import { ImageUploadField } from '@/components/image-upload-field';
import { Skeleton } from '@/components/ui/skeleton';
import { messages } from '@/lib/messages.ar';
import { resolveAssetUrl } from '@/lib/asset-url';

import { useProductImages } from '../../hooks/useProductImage';
import {
  useCreateProductImage,
  useUpdateProductImage,
  useDeleteProductImage,
} from '../../hooks/useProductImageMutations';
import type { ProductImageInfo } from '../../types';

const m = messages.product.image;

interface ProductImageGalleryProps {
  productId: number;
}

export function ProductImageGallery({ productId }: ProductImageGalleryProps) {
  const [file, setFile] = useState<File | null>(null);
  const [altText, setAltText] = useState('');
  const [isPrimary, setIsPrimary] = useState(false);
  const [deleteTarget, setDeleteTarget] = useState<ProductImageInfo | null>(null);

  const { data, isLoading } = useProductImages(productId, { limit: 50, sortBy: 'sortOrder' });
  const { mutateAsync: uploadImage, isPending: isUploading } = useCreateProductImage(productId);
  const { mutate: updateImage, isPending: isUpdating } = useUpdateProductImage(productId);
  const { mutate: deleteImage, isPending: isDeleting } = useDeleteProductImage(productId);

  const images = data?.items ?? [];
  const isPending = isUploading || isUpdating || isDeleting;

  async function handleUpload() {
    if (!file) return;

    try {
      await uploadImage({
        file,
        altText: altText || undefined,
        sortOrder: images.length,
        isPrimary: isPrimary || images.length === 0,
      });
      setFile(null);
      setAltText('');
      setIsPrimary(false);
      toast.success(m.createSuccess);
    } catch {
      // Error surfaced via mutation onError
    }
  }

  function handleSetPrimary(image: ProductImageInfo) {
    updateImage(
      {
        imageId: image.id,
        payload: {
          variantId: image.variantId,
          altText: image.altText,
          sortOrder: image.sortOrder,
          isPrimary: true,
        },
      },
      { onSuccess: () => toast.success(m.updateSuccess) },
    );
  }

  function handleAltTextBlur(image: ProductImageInfo, newAltText: string) {
    if (newAltText === (image.altText ?? '')) return;
    updateImage({
      imageId: image.id,
      payload: {
        variantId: image.variantId,
        altText: newAltText || null,
        sortOrder: image.sortOrder,
        isPrimary: image.isPrimary,
      },
    });
  }

  function handleConfirmDelete() {
    if (!deleteTarget) return;
    deleteImage(deleteTarget.id, { onSettled: () => setDeleteTarget(null) });
  }

  return (
    <>
      <Card className="shadow-xs">
        <CardHeader>
          <CardTitle className="text-base">{m.sectionTitle}</CardTitle>
        </CardHeader>
        <CardContent className="space-y-6">
          <div className="grid gap-4 md:grid-cols-2">
            <ImageUploadField
              id="product-image-upload"
              label={m.upload}
              hint={m.uploadHint}
              chooseLabel={m.chooseLabel}
              replaceLabel={m.replaceLabel}
              removeLabel={m.removeLabel}
              emptyLabel={m.emptyLabel}
              invalidTypeError={m.invalidType}
              tooLargeError={m.tooLarge}
              file={file}
              disabled={isPending}
              onFileChange={setFile}
            />
            <div className="space-y-4">
              <div className="space-y-2">
                <Label htmlFor="product-image-alt">{m.altText}</Label>
                <Input
                  id="product-image-alt"
                  placeholder={m.altTextPlaceholder}
                  dir="ltr"
                  value={altText}
                  disabled={isPending}
                  onChange={(e) => setAltText(e.target.value)}
                />
              </div>
              <div className="flex items-center gap-2">
                <Checkbox
                  id="product-image-primary"
                  checked={isPrimary}
                  disabled={isPending}
                  onCheckedChange={(checked) => setIsPrimary(checked === true)}
                />
                <Label htmlFor="product-image-primary">{m.isPrimary}</Label>
              </div>
              <Button
                type="button"
                disabled={!file || isPending}
                onClick={() => void handleUpload()}
              >
                {isUploading ? (
                  <>
                    <Loader2 className="size-4 animate-spin" />
                    {messages.product.form.saving}
                  </>
                ) : (
                  m.upload
                )}
              </Button>
            </div>
          </div>

          {isLoading ? (
            <div className="grid grid-cols-2 gap-4 sm:grid-cols-3 md:grid-cols-4">
              {Array.from({ length: 4 }).map((_, i) => (
                <Skeleton key={i} className="aspect-square rounded-lg" />
              ))}
            </div>
          ) : images.length > 0 ? (
            <div className="grid grid-cols-2 gap-4 sm:grid-cols-3 md:grid-cols-4">
              {images.map((image) => (
                <div
                  key={image.id}
                  className="group relative flex flex-col gap-2 rounded-lg border border-border/60 p-2"
                >
                  <div className="relative aspect-square overflow-hidden rounded-md bg-muted">
                    <Image
                      src={resolveAssetUrl(image.imageUrl) ?? ''}
                      alt={image.altText ?? m.sectionTitle}
                      fill
                      unoptimized
                      className="object-contain"
                    />
                    {image.isPrimary && (
                      <Badge className="absolute start-2 top-2" variant="secondary">
                        <Star className="size-3 fill-current" />
                        {m.primary}
                      </Badge>
                    )}
                  </div>
                  <Input
                    defaultValue={image.altText ?? ''}
                    placeholder={m.altTextPlaceholder}
                    dir="ltr"
                    className="text-xs"
                    disabled={isPending}
                    onBlur={(e) => handleAltTextBlur(image, e.target.value)}
                  />
                  <div className="flex gap-1">
                    {!image.isPrimary && (
                      <Button
                        type="button"
                        variant="outline"
                        size="sm"
                        className="flex-1 text-xs"
                        disabled={isPending}
                        onClick={() => handleSetPrimary(image)}
                      >
                        {m.setPrimary}
                      </Button>
                    )}
                    <Button
                      type="button"
                      variant="ghost"
                      size="icon-sm"
                      className="text-destructive hover:bg-destructive/10 hover:text-destructive"
                      disabled={isPending}
                      onClick={() => setDeleteTarget(image)}
                    >
                      <Trash2 className="size-4" />
                    </Button>
                  </div>
                </div>
              ))}
            </div>
          ) : (
            <div className="text-muted-foreground flex min-h-24 items-center justify-center rounded-lg border border-dashed border-border/70 bg-muted/10 px-4 py-6 text-center text-sm">
              {m.noImages}
            </div>
          )}
        </CardContent>
      </Card>

      <AlertDialog open={!!deleteTarget} onOpenChange={(open) => !open && setDeleteTarget(null)}>
        <AlertDialogContent>
          <AlertDialogHeader>
            <AlertDialogTitle>{m.deleteTitle}</AlertDialogTitle>
            <AlertDialogDescription>{m.deleteDescription}</AlertDialogDescription>
          </AlertDialogHeader>
          <AlertDialogFooter>
            <AlertDialogCancel disabled={isDeleting}>{messages.product.form.cancel}</AlertDialogCancel>
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
