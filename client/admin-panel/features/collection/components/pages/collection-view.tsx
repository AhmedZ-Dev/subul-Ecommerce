'use client';

import { useState } from 'react';
import Link from 'next/link';
import { useRouter } from 'next/navigation';
import {
  ArrowRight,
  Copy,
  FolderOpen,
  Pencil,
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
import { cn } from '@/lib/utils';
import type { CollectionDto } from '../../types';
import { useCollection } from '../../hooks/useCollection';
import { useDeleteCollection } from '../../hooks/useCollectionMutations';
import { CollectionStatusBadge } from '../blocks/collection-status-badge';
import { CollectionTypeBadge } from '../blocks/collection-type-badge';

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
  const m = messages.collection.view;

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

interface CollectionViewProps {
  collection: CollectionDto;
}

export function CollectionView({ collection }: CollectionViewProps) {
  const router = useRouter();
  const [deleteOpen, setDeleteOpen] = useState(false);
  const { data: liveCollection } = useCollection(collection.id, { initialData: collection });
  const current = liveCollection ?? collection;
  const { mutate: deleteCollection, isPending } = useDeleteCollection();

  const m = messages.collection.view;
  const f = messages.collection.form;
  const displayName = current.nameEn;

  function handleConfirmDelete() {
    deleteCollection(current.id, {
      onSuccess: () => router.push('/collections'),
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
          <Link href="/collections">
            <ArrowRight className="size-4" />
            {m.backToList}
          </Link>
        </Button>

        <Card className="overflow-hidden border-border/60 shadow-xs">
          <CardContent className="flex flex-col gap-5 p-5 sm:p-6">
            <div className="flex flex-col gap-5 lg:flex-row lg:items-start lg:justify-between">
              <div className="flex min-w-0 items-start gap-4">
                <div className="bg-primary/10 text-primary flex size-12 shrink-0 items-center justify-center rounded-xl">
                  <FolderOpen className="size-6" />
                </div>

                <div className="min-w-0 space-y-2">
                  <div className="flex flex-wrap items-center gap-2">
                    <h1 className="text-2xl font-bold tracking-tight">
                      {current.nameEn}
                    </h1>
                    <CollectionStatusBadge status={current.status} />
                    <CollectionTypeBadge type={current.collectionType} />
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
                      {m.collectionId}: {current.id}
                    </span>
                  </div>
                </div>
              </div>

              <div className="flex shrink-0 flex-wrap items-center gap-2">
                <Button asChild>
                  <Link href={`/collections/${current.id}/edit`}>
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
              <DetailField
                label={f.type}
                value={<CollectionTypeBadge type={current.collectionType} />}
              />
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
                <DetailField label={f.nameEn} value={current.nameEn} dir="ltr" />
                {current.nameAr && (
                  <DetailField label={f.nameAr} value={current.nameAr} dir="rtl" />
                )}
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
              {messages.collection.form.cancel}
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
