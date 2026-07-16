'use client';

import { useState } from 'react';
import Link from 'next/link';
import { useRouter } from 'next/navigation';
import {
  ArrowRight,
  Copy,
  CreditCard,
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
import type { PaymentMethodDto } from '../../types';
import { usePaymentMethod } from '../../hooks/usePaymentMethod';
import { useDeletePaymentMethod } from '../../hooks/usePaymentMethodMutations';
import { PaymentMethodStatusBadge } from '../blocks/payment-method-status-badge';

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

function InstructionsBlock({
  label,
  value,
  dir,
}: {
  label: string;
  value: string | null;
  dir: 'rtl' | 'ltr';
}) {
  const m = messages.paymentMethod.view;

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
          {m.noInstructions}
        </div>
      )}
    </div>
  );
}

function getDisplayLabel(method: PaymentMethodDto): string {
  return method.labelAr ?? method.labelEn ?? method.name;
}

interface PaymentMethodViewProps {
  paymentMethod: PaymentMethodDto;
}

export function PaymentMethodView({ paymentMethod }: PaymentMethodViewProps) {
  const router = useRouter();
  const [deleteOpen, setDeleteOpen] = useState(false);
  const { data: livePaymentMethod } = usePaymentMethod(paymentMethod.id, {
    initialData: paymentMethod,
  });
  const current = livePaymentMethod ?? paymentMethod;
  const { mutate: deletePaymentMethod, isPending } = useDeletePaymentMethod();

  const m = messages.paymentMethod.view;
  const f = messages.paymentMethod.form;
  const typeLabels = messages.paymentMethod.type;
  const displayName = getDisplayLabel(current);

  function handleConfirmDelete() {
    deletePaymentMethod(current.id, {
      onSuccess: () => router.push('/payment-methods'),
      onSettled: () => setDeleteOpen(false),
    });
  }

  async function handleCopyName() {
    try {
      await navigator.clipboard.writeText(current.name);
      toast.success(m.nameCopied);
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
          <Link href="/payment-methods">
            <ArrowRight className="size-4" />
            {m.backToList}
          </Link>
        </Button>

        <Card className="overflow-hidden border-border/60 shadow-xs">
          <CardContent className="flex flex-col gap-5 p-5 sm:p-6">
            <div className="flex flex-col gap-5 lg:flex-row lg:items-start lg:justify-between">
              <div className="flex min-w-0 items-start gap-4">
                <div className="bg-primary/10 text-primary flex size-12 shrink-0 items-center justify-center rounded-xl">
                  <CreditCard className="size-6" />
                </div>

                <div className="min-w-0 space-y-2">
                  <div className="flex flex-wrap items-center gap-2">
                    <h1 className="text-2xl font-bold tracking-tight">{displayName}</h1>
                    <PaymentMethodStatusBadge status={current.status} />
                  </div>

                  <div className="flex flex-wrap items-center gap-2 pt-1">
                    <code className="bg-muted text-muted-foreground rounded-md px-2 py-1 font-mono text-xs" dir="ltr">
                      {current.name}
                    </code>
                    <Tooltip>
                      <TooltipTrigger asChild>
                        <Button
                          type="button"
                          variant="ghost"
                          size="icon-xs"
                          onClick={handleCopyName}
                          aria-label={messages.common.copy}
                        >
                          <Copy className="size-3.5" />
                        </Button>
                      </TooltipTrigger>
                      <TooltipContent side="top">{messages.common.copy}</TooltipContent>
                    </Tooltip>
                    <span className="text-muted-foreground text-xs">
                      {m.paymentMethodId}: {current.id}
                    </span>
                  </div>
                </div>
              </div>

              <div className="flex shrink-0 flex-wrap items-center gap-2">
                <Button asChild>
                  <Link href={`/payment-methods/${current.id}/edit`}>
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
                label={f.type}
                value={
                  current.type ? typeLabels[current.type] : m.empty
                }
              />
              <DetailField
                label={f.gateway}
                value={current.gateway ?? m.empty}
                dir="ltr"
              />
              <DetailField label={m.sortOrder} value={current.sortOrder} />
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
              <CardTitle>{f.sections.identity}</CardTitle>
            </CardHeader>
            <CardContent>
              <dl className="grid gap-3">
                <DetailField label={f.name} value={current.name} dir="ltr" />
                <DetailField
                  label={f.labelEn}
                  value={current.labelEn ?? m.empty}
                  dir="ltr"
                />
                <DetailField
                  label={f.labelAr}
                  value={current.labelAr ?? m.empty}
                  dir="rtl"
                />
                <DetailField
                  label={f.iconUrl}
                  value={
                    current.iconUrl ? (
                      <a
                        href={current.iconUrl}
                        target="_blank"
                        rel="noopener noreferrer"
                        className="text-primary hover:underline break-all"
                      >
                        {current.iconUrl}
                      </a>
                    ) : (
                      m.empty
                    )
                  }
                  dir="ltr"
                />
              </dl>
            </CardContent>
          </Card>

          <Card className="border-border/60 shadow-xs">
            <CardHeader>
              <CardTitle>{f.sections.gateway}</CardTitle>
            </CardHeader>
            <CardContent className="space-y-4">
              <DetailField
                label={f.gatewayConfig}
                value={
                  current.gatewayConfig ? (
                    <pre className="overflow-x-auto rounded-lg border bg-muted/30 p-3 font-mono text-xs whitespace-pre-wrap" dir="ltr">
                      {current.gatewayConfig}
                    </pre>
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
              <CardTitle>{f.sections.instructions}</CardTitle>
              <CardDescription>{f.instructionsHelp}</CardDescription>
            </CardHeader>
            <CardContent>
              <div className="grid gap-6 lg:grid-cols-2">
                <InstructionsBlock
                  label={f.instructionsEn}
                  value={current.instructionsEn}
                  dir="ltr"
                />
                <InstructionsBlock
                  label={f.instructionsAr}
                  value={current.instructionsAr}
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
              {messages.paymentMethod.form.cancel}
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
