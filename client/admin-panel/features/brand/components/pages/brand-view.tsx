'use client';

import Image from 'next/image';
import { useState } from 'react';
import Link from 'next/link';
import { useRouter } from 'next/navigation';
import {
  ArrowRight,
  Copy,
  Pencil,
  Tag,
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
import { Separator } from '@/components/ui/separator';
import {
  Tooltip,
  TooltipContent,
  TooltipTrigger,
} from '@/components/ui/tooltip';
import { formatDate, messages } from '@/lib/messages.ar';
import { resolveAssetUrl } from '@/lib/asset-url';
import { cn } from '@/lib/utils';
import type { BrandDto } from '../../types';
import { useBrand } from '../../hooks/useBrand';
import { useDeleteBrand } from '../../hooks/useBrandMutations';
import { BrandStatusBadge } from '../blocks/brand-status-badge';

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
  const m = messages.brand.view;

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
          {m.noDescription}
        </div>
      )}
    </div>
  );
}

function MediaPreview({
  label,
  url,
  emptyLabel,
  wide = false,
}: {
  label: string;
  url: string | null;
  emptyLabel: string;
  wide?: boolean;
}) {
  const resolved = resolveAssetUrl(url);

  return (
    <div className="space-y-2">
      <h4 className="text-sm font-medium">{label}</h4>
      {resolved ? (
        <div
          className={
            wide
              ? 'relative aspect-[3/1] overflow-hidden rounded-lg border bg-muted/20'
              : 'relative mx-auto size-28 overflow-hidden rounded-lg border bg-muted/20'
          }
        >
          <Image
            src={resolved}
            alt={label}
            fill
            unoptimized
            className="object-contain"
          />
        </div>
      ) : (
        <div className="text-muted-foreground flex min-h-24 items-center justify-center rounded-lg border border-dashed border-border/70 bg-muted/10 px-4 py-6 text-center text-sm">
          {emptyLabel}
        </div>
      )}
    </div>
  );
}

interface BrandViewProps {
  brand: BrandDto;
}

export function BrandView({ brand }: BrandViewProps) {
  const router = useRouter();
  const [deleteOpen, setDeleteOpen] = useState(false);
  const { data: liveBrand } = useBrand(brand.id, { initialData: brand });
  const current = liveBrand ?? brand;
  const { mutate: deleteBrand, isPending } = useDeleteBrand();

  const m = messages.brand.view;
  const f = messages.brand.form;
  const displayName = current.name;

  function handleConfirmDelete() {
    deleteBrand(current.id, {
      onSuccess: () => router.push('/brands'),
      onSettled: () => setDeleteOpen(false),
    });
  }

  async function handleCopySlug() {
    try {
      await navigator.clipboard.writeText(current.slug);
      toast.success(m.slugCopied);
    } catch {
      toast.error(messages.common.error);
    }
  }

  return (
    <>
      <div className="flex flex-col gap-6">
        <Button
          asChild
          variant="ghost"
          size="sm"
          className="text-muted-foreground hover:text-foreground -ms-2 w-fit gap-1.5"
        >
          <Link href="/brands">
            <ArrowRight className="size-4" />
            {m.backToList}
          </Link>
        </Button>

        <Card className="overflow-hidden border-border/60 shadow-xs">
          <CardContent className="flex flex-col gap-5 p-5 sm:p-6">
            <div className="flex flex-col gap-5 lg:flex-row lg:items-start lg:justify-between">
              <div className="flex min-w-0 items-start gap-4">
                <div className="bg-primary/10 text-primary flex size-12 shrink-0 items-center justify-center rounded-xl">
                  <Tag className="size-6" />
                </div>

                <div className="min-w-0 space-y-2">
                  <div className="flex flex-wrap items-center gap-2">
                    <h1 className="text-2xl font-bold tracking-tight">
                      {current.name}
                    </h1>
                    <BrandStatusBadge status={current.status} />
                  </div>

                  <div className="flex flex-wrap items-center gap-2 pt-1">
                    <code className="bg-muted text-muted-foreground rounded-md px-2 py-1 font-mono text-xs">
                      {current.slug}
                    </code>
                    <Tooltip>
                      <TooltipTrigger asChild>
                        <Button
                          type="button"
                          variant="ghost"
                          size="icon-xs"
                          onClick={handleCopySlug}
                          aria-label={messages.common.copy}
                        >
                          <Copy className="size-3.5" />
                        </Button>
                      </TooltipTrigger>
                      <TooltipContent side="top">
                        {messages.common.copy}
                      </TooltipContent>
                    </Tooltip>
                    <span className="text-muted-foreground text-xs">
                      {m.brandId}: {current.id}
                    </span>
                  </div>
                </div>
              </div>

              <div className="flex shrink-0 flex-wrap items-center gap-2">
                <Button asChild>
                  <Link href={`/brands/${current.id}/edit`}>
                    <Pencil />
                    {messages.common.edit}
                  </Link>
                </Button>
                <Button
                  variant="outline"
                  className="text-destructive hover:bg-destructive/10 hover:text-destructive"
                  onClick={() => setDeleteOpen(true)}
                  disabled={isPending}
                >
                  <Trash2 />
                  {messages.common.delete}
                </Button>
              </div>
            </div>

            <Separator />

            <div className="grid gap-3 sm:grid-cols-2 xl:grid-cols-4">
              <DetailField label={m.sortOrder} value={current.sortOrder} />
              <DetailField label={f.isFeatured} value={current.isFeatured ? messages.common.yes : messages.common.no} />
              <DetailField
                label={m.createdAt}
                value={formatDate(current.createdAt, {
                  dateStyle: 'medium',
                  timeStyle: 'short',
                })}
              />
              <DetailField
                label={m.updatedAt}
                value={
                  current.updatedAt
                    ? formatDate(current.updatedAt, {
                        dateStyle: 'medium',
                        timeStyle: 'short',
                      })
                    : m.notUpdated
                }
              />
            </div>
          </CardContent>
        </Card>

        <div className="grid gap-6 lg:grid-cols-2">
          <Card className="border-border/60 shadow-xs">
            <CardHeader>
               <CardTitle>{f.sections.names}</CardTitle>
            </CardHeader>
            <CardContent>
              <dl className="grid gap-3">
                <DetailField label={f.name} value={current.name} dir="ltr" />
                <DetailField
                  label={f.slug}
                  value={
                    <code className="bg-muted text-muted-foreground rounded px-1.5 py-0.5 font-mono text-xs">
                      {f.slugPreview(current.slug)}
                    </code>
                  }
                  dir="ltr"
                />
              </dl>
            </CardContent>
          </Card>

          <Card className="border-border/60 shadow-xs lg:col-span-2">
            <CardHeader>
              <CardTitle>{f.sections.media}</CardTitle>
            </CardHeader>
            <CardContent className="space-y-6">
              <div className="grid gap-6 md:grid-cols-2">
                <MediaPreview
                  label={f.logo}
                  url={current.logoUrl}
                  emptyLabel={m.noLogo}
                />
                <MediaPreview
                  label={f.banner}
                  url={current.bannerUrl}
                  emptyLabel={m.noBanner}
                  wide
                />
              </div>
              <DetailField
                label={f.websiteUrl}
                value={
                  current.websiteUrl ? (
                    <a
                      href={current.websiteUrl}
                      target="_blank"
                      rel="noopener noreferrer"
                      className="text-primary hover:underline"
                    >
                      {current.websiteUrl}
                    </a>
                  ) : (
                    m.empty
                  )
                }
                dir="ltr"
              />
            </CardContent>
          </Card>

          <Card className="border-border/60 shadow-xs lg:col-span-2">
            <CardHeader>
              <CardTitle>{f.sections.descriptions}</CardTitle>
              <CardDescription>{f.descriptionHelp}</CardDescription>
            </CardHeader>
            <CardContent>
              <div className="grid gap-6 lg:grid-cols-2">
                <DescriptionBlock
                  label={f.descriptionEn}
                  value={current.descriptionEn}
                  dir="ltr"
                />
                <DescriptionBlock
                  label={f.descriptionAr}
                  value={current.descriptionAr}
                  dir="rtl"
                />
              </div>
            </CardContent>
          </Card>
        </div>
      </div>

      <AlertDialog open={deleteOpen} onOpenChange={setDeleteOpen}>
        <AlertDialogContent>
          <AlertDialogHeader>
            <AlertDialogTitle>{m.deleteTitle}</AlertDialogTitle>
            <AlertDialogDescription>
              {m.deleteDescription(displayName)}
            </AlertDialogDescription>
          </AlertDialogHeader>
          <AlertDialogFooter>
            <AlertDialogCancel disabled={isPending}>
              {messages.brand.form.cancel}
            </AlertDialogCancel>
            <AlertDialogAction
              variant="destructive"
              onClick={handleConfirmDelete}
              disabled={isPending}
            >
              {messages.common.delete}
            </AlertDialogAction>
          </AlertDialogFooter>
        </AlertDialogContent>
      </AlertDialog>
    </>
  );
}
