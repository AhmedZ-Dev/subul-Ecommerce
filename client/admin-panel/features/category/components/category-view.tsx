'use client';

import { useEffect, useState } from 'react';
import Link from 'next/link';
import { useRouter } from 'next/navigation';
import {
  ArrowRight,
  Copy,
  FolderOpen,
  FolderTree,
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
import type { CategoryDto, CategoryStatus } from '../types';
import { useDeleteCategory } from '../hooks/useCategoryMutations';
import { CategoryStatusToggle } from './category-status-toggle';

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
  const m = messages.category.view;

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

interface CategoryViewProps {
  category: CategoryDto;
}

export function CategoryView({ category }: CategoryViewProps) {
  const router = useRouter();
  const [deleteOpen, setDeleteOpen] = useState(false);
  const [status, setStatus] = useState<CategoryStatus>(category.status);
  const [updatedAt, setUpdatedAt] = useState(category.updatedAt);
  const { mutate: deleteCategory, isPending } = useDeleteCategory();

  const m = messages.category.view;
  const f = messages.category.form;
  const displayName = category.nameAr ?? category.nameEn;
  const isRoot = category.parentId === null;

  useEffect(() => {
    setStatus(category.status);
    setUpdatedAt(category.updatedAt);
  }, [category.status, category.updatedAt]);

  function handleConfirmDelete() {
    deleteCategory(category.id, {
      onSuccess: () => router.push('/categories'),
      onSettled: () => setDeleteOpen(false),
    });
  }

  async function handleCopySlug() {
    try {
      await navigator.clipboard.writeText(category.slug);
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
          <Link href="/categories">
            <ArrowRight className="size-4" />
            {m.backToList}
          </Link>
        </Button>

        <Card className="overflow-hidden border-border/60 shadow-xs">
          <CardContent className="flex flex-col gap-5 p-5 sm:p-6">
            <div className="flex flex-col gap-5 lg:flex-row lg:items-start lg:justify-between">
              <div className="flex min-w-0 items-start gap-4">
                <div className="bg-primary/10 text-primary flex size-12 shrink-0 items-center justify-center rounded-xl">
                  {isRoot ? (
                    <FolderOpen className="size-6" />
                  ) : (
                    <FolderTree className="size-6" />
                  )}
                </div>

                <div className="min-w-0 space-y-2">
                  <div className="flex flex-wrap items-center gap-2">
                    <h1 className="text-2xl font-bold tracking-tight">
                      {category.nameAr ?? category.nameEn}
                    </h1>
                    <CategoryStatusToggle
                      categoryId={category.id}
                      status={status}
                      onStatusChange={(result) => {
                        setStatus(result.status);
                        setUpdatedAt(result.updatedAt);
                      }}
                    />
                  </div>

                  {category.nameAr && (
                    <p className="text-muted-foreground text-sm" dir="ltr">
                      {category.nameEn}
                    </p>
                  )}

                  <div className="flex flex-wrap items-center gap-2 pt-1">
                    <code className="bg-muted text-muted-foreground rounded-md px-2 py-1 font-mono text-xs">
                      {category.slug}
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
                      {m.categoryId}: {category.id}
                    </span>
                  </div>
                </div>
              </div>

              <div className="flex shrink-0 flex-wrap items-center gap-2">
                <Button asChild>
                  <Link href={`/categories/${category.id}/edit`}>
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
              <DetailField
                label={f.parent}
                value={
                  category.parentId ? (
                    <Link
                      href={`/categories/${category.parentId}/view`}
                      className="text-primary font-medium hover:underline"
                    >
                      {category.parentNameAr ?? category.parentNameEn}
                    </Link>
                  ) : (
                    <span className="text-muted-foreground">{m.rootCategory}</span>
                  )
                }
                dir={category.parentNameAr ? 'rtl' : 'ltr'}
              />
              <DetailField label={m.sortOrder} value={category.sortOrder} />
              <DetailField
                label={m.createdAt}
                value={formatDate(category.createdAt, {
                  dateStyle: 'medium',
                  timeStyle: 'short',
                })}
              />
              <DetailField
                label={m.updatedAt}
                value={
                  updatedAt
                    ? formatDate(updatedAt, {
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
              <CardDescription>{m.namesDescription}</CardDescription>
            </CardHeader>
            <CardContent>
              <dl className="grid gap-3">
                <DetailField label={f.nameEn} value={category.nameEn} dir="ltr" />
                <DetailField
                  label={f.nameAr}
                  value={category.nameAr ?? m.empty}
                  dir="rtl"
                />
                <DetailField
                  label={f.slug}
                  value={
                    <code className="bg-muted text-muted-foreground rounded px-1.5 py-0.5 font-mono text-xs">
                      {f.slugPreview(category.slug)}
                    </code>
                  }
                  dir="ltr"
                />
              </dl>
            </CardContent>
          </Card>

          <Card className="border-border/60 shadow-xs">
            <CardHeader>
              <CardTitle>{f.sections.settings}</CardTitle>
              <CardDescription>{m.settingsDescription}</CardDescription>
            </CardHeader>
            <CardContent>
              <dl className="grid gap-3">
                <DetailField
                  label={f.status}
                  value={
                    <CategoryStatusToggle
                      categoryId={category.id}
                      status={status}
                      onStatusChange={(result) => {
                        setStatus(result.status);
                        setUpdatedAt(result.updatedAt);
                      }}
                    />
                  }
                />
                <DetailField
                  label={f.parent}
                  value={
                    category.parentId ? (
                      <Link
                        href={`/categories/${category.parentId}/view`}
                        className="text-primary font-medium hover:underline"
                      >
                        {category.parentNameAr ?? category.parentNameEn}
                      </Link>
                    ) : (
                      m.rootCategory
                    )
                  }
                  dir={category.parentNameAr ? 'rtl' : 'ltr'}
                />
                <DetailField label={m.sortOrder} value={category.sortOrder} />
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
                  value={category.descriptionEn}
                  dir="ltr"
                />
                <DescriptionBlock
                  label={f.descriptionAr}
                  value={category.descriptionAr}
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
              {messages.category.form.cancel}
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
