'use client';

import { useState } from 'react';
import Link from 'next/link';
import { useRouter } from 'next/navigation';
import {
  ArrowRight,
  Copy,
  Layers,
  Pencil,
  Trash2,
} from 'lucide-react';
import { toast } from 'sonner';
import { Button } from '@/components/ui/button';
import {
  Card,
  CardContent,
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
import { Badge } from '@/components/ui/badge';
import { Separator } from '@/components/ui/separator';
import {
  Tooltip,
  TooltipContent,
  TooltipTrigger,
} from '@/components/ui/tooltip';
import { formatDate, messages } from '@/lib/messages.ar';
import { cn } from '@/lib/utils';
import type { AttributeGroupDto } from '../../types';
import { useAttributeGroup } from '../../hooks/useAttributeGroup';
import { useDeleteAttributeGroup } from '../../hooks/useAttributeGroupMutations';

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

interface AttributeGroupViewProps {
  data: AttributeGroupDto;
}

export function AttributeGroupView({ data }: AttributeGroupViewProps) {
  const router = useRouter();
  const [deleteOpen, setDeleteOpen] = useState(false);
  const { data: liveGroup } = useAttributeGroup(data.id, { initialData: data });
  const current = liveGroup ?? data;
  const { mutate: deleteGroup, isPending } = useDeleteAttributeGroup();

  const m = messages.attributeGroup.view;
  const f = messages.attributeGroup.fields;
  const form = messages.attributeGroup.form;
  const displayName = current.nameEn;

  function handleConfirmDelete() {
    deleteGroup(current.id, {
      onSuccess: () => router.push('/attribute-groups'),
      onSettled: () => setDeleteOpen(false),
    });
  }

  async function handleCopySlug() {
    if (!current.slug) return;
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
          <Link href="/attribute-groups">
            <ArrowRight className="size-4" />
            {m.backToList}
          </Link>
        </Button>

        <Card className="overflow-hidden border-border/60 shadow-xs">
          <CardContent className="flex flex-col gap-5 p-5 sm:p-6">
            <div className="flex flex-col gap-5 lg:flex-row lg:items-start lg:justify-between">
              <div className="flex min-w-0 items-start gap-4">
                <div className="bg-primary/10 text-primary flex size-12 shrink-0 items-center justify-center rounded-xl">
                  <Layers className="size-6" />
                </div>

                <div className="min-w-0 space-y-2">
                  <div className="flex flex-wrap items-center gap-2">
                    <h1 className="text-2xl font-bold tracking-tight">
                      {current.nameEn}
                    </h1>
                    {current.nameAr && (
                      <span className="text-muted-foreground text-lg" dir="rtl">
                        {current.nameAr}
                      </span>
                    )}
                  </div>

                  <div className="flex flex-wrap items-center gap-2 pt-1">
                    {current.slug && (
                      <>
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
                      </>
                    )}
                    <span className="text-muted-foreground text-xs">
                      {m.groupId}: {current.id}
                    </span>
                  </div>
                </div>
              </div>

              <div className="flex shrink-0 flex-wrap items-center gap-2">
                <Button asChild>
                  <Link href={`/attribute-groups/${current.id}/edit`}>
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

            <div className="grid gap-3 sm:grid-cols-2 xl:grid-cols-3">
              <DetailField label={f.sortOrder} value={current.sortOrder} />
              <DetailField
                label={f.isFilterable}
                value={current.isFilterable ? messages.common.yes : messages.common.no}
              />
              <DetailField
                label={f.attributeCount}
                value={current.attributes.length}
              />
              <DetailField
                label={m.createdAt}
                value={formatDate(current.createdAt, {
                  dateStyle: 'medium',
                  timeStyle: 'short',
                })}
              />
            </div>
          </CardContent>
        </Card>

        <Card className="border-border/60 shadow-xs">
          <CardHeader>
            <CardTitle>
              {form.attributes} ({current.attributes.length})
            </CardTitle>
          </CardHeader>
          <CardContent>
            {current.attributes.length === 0 ? (
              <div className="text-muted-foreground flex min-h-24 items-center justify-center rounded-lg border border-dashed border-border/70 bg-muted/10 px-4 py-6 text-center text-sm">
                {m.noAttributes}
              </div>
            ) : (
              <div className="space-y-3">
                {current.attributes.map((attr) => (
                  <div
                    key={attr.id}
                    className="rounded-lg border border-border/50 bg-muted/20 p-4"
                  >
                    <div className="flex flex-col gap-4 lg:flex-row lg:items-start lg:justify-between">
                      <div className="min-w-0 space-y-1">
                        <p className="font-medium">{attr.nameEn}</p>
                        {attr.nameAr && (
                          <p className="text-muted-foreground text-sm" dir="rtl">
                            {attr.nameAr}
                          </p>
                        )}
                      </div>
                      <div className="grid gap-3 sm:grid-cols-2 lg:grid-cols-4">
                        <DetailField
                          label={f.slug}
                          value={attr.slug || m.empty}
                          dir="ltr"
                        />
                        <DetailField
                          label={f.inputType}
                          value={<Badge variant="secondary">{attr.inputType}</Badge>}
                        />
                        <DetailField
                          label={f.unit}
                          value={attr.unit || m.empty}
                          dir="ltr"
                        />
                        <DetailField
                          label={f.isFilterable}
                          value={attr.isFilterable ? messages.common.yes : messages.common.no}
                        />
                      </div>
                    </div>
                  </div>
                ))}
              </div>
            )}
          </CardContent>
        </Card>
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
              {messages.attributeGroup.form.cancel}
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
