'use client';

import { useState } from 'react';
import Link from 'next/link';
import { useRouter } from 'next/navigation';
import { ArrowRight, MapPin, Pencil, Trash2 } from 'lucide-react';
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
import { Separator } from '@/components/ui/separator';
import { Badge } from '@/components/ui/badge';
import {
  Table,
  TableBody,
  TableCell,
  TableHead,
  TableHeader,
  TableRow,
} from '@/components/ui/table';
import { formatDate, messages } from '@/lib/messages.ar';
import { cn } from '@/lib/utils';
import type { ShippingZoneDto } from '../../types';
import { useShippingZone } from '../../hooks/useShippingZone';
import { useDeleteShippingZone } from '../../hooks/useShippingZoneMutations';
import { ShippingZoneStatusBadge } from '../blocks/shipping-zone-status-badge';
import { IRAQI_GOVERNORATES, SHIPPING_RATE_TYPE_LABELS } from '../../constants';

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

function getGovernorateLabel(value: string): string {
  const match = IRAQI_GOVERNORATES.find((g) => g.value === value);
  return match ? match.labelAr : value;
}

interface ShippingZoneViewProps {
  zone: ShippingZoneDto;
}

export function ShippingZoneView({ zone }: ShippingZoneViewProps) {
  const router = useRouter();
  const [deleteOpen, setDeleteOpen] = useState(false);
  const { data: liveZone } = useShippingZone(zone.id, { initialData: zone });
  const current = liveZone ?? zone;
  const { mutate: deleteZone, isPending } = useDeleteShippingZone();

  const m = messages.shippingZone.view;
  const f = messages.shippingZone.form;
  const r = messages.shippingZone.rates;
  const displayName = current.nameEn;

  function handleConfirmDelete() {
    deleteZone(current.id, {
      onSuccess: () => router.push('/shipping-zones'),
      onSettled: () => setDeleteOpen(false),
    });
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
          <Link href="/shipping-zones">
            <ArrowRight className="size-4" />
            {m.backToList}
          </Link>
        </Button>

        <Card className="overflow-hidden border-border/60 shadow-xs">
          <CardContent className="flex flex-col gap-5 p-5 sm:p-6">
            <div className="flex flex-col gap-5 lg:flex-row lg:items-start lg:justify-between">
              <div className="flex min-w-0 items-start gap-4">
                <div className="bg-primary/10 text-primary flex size-12 shrink-0 items-center justify-center rounded-xl">
                  <MapPin className="size-6" />
                </div>

                <div className="min-w-0 space-y-2">
                  <div className="flex flex-wrap items-center gap-2">
                    <h1 className="text-2xl font-bold tracking-tight">{current.nameEn}</h1>
                    <ShippingZoneStatusBadge status={current.status} />
                  </div>

                  {current.nameAr && (
                    <p className="text-muted-foreground text-sm" dir="rtl">
                      {current.nameAr}
                    </p>
                  )}

                  <span className="text-muted-foreground text-xs">
                    {m.zoneId}: {current.id}
                  </span>
                </div>
              </div>

              <div className="flex shrink-0 flex-wrap items-center gap-2">
                <Button asChild>
                  <Link href={`/shipping-zones/${current.id}/edit`}>
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
              <DetailField
                label={m.governorateCount}
                value={current.governorates.length}
              />
              <DetailField label={m.rateCount} value={current.shippingRates.length} />
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
              </dl>
            </CardContent>
          </Card>

          <Card className="border-border/60 shadow-xs">
            <CardHeader>
              <CardTitle>{f.sections.governorates}</CardTitle>
            </CardHeader>
            <CardContent>
              {current.governorates.length > 0 ? (
                <div className="flex flex-wrap gap-2">
                  {current.governorates.map((gov) => (
                    <Badge key={gov} variant="secondary">
                      {getGovernorateLabel(gov)}
                    </Badge>
                  ))}
                </div>
              ) : (
                <p className="text-muted-foreground text-sm">{m.noGovernorates}</p>
              )}
            </CardContent>
          </Card>
        </div>

        <Card className="border-border/60 shadow-xs">
          <CardHeader>
            <CardTitle>{f.sections.rates}</CardTitle>
          </CardHeader>
          <CardContent>
            {current.shippingRates.length > 0 ? (
              <div className="overflow-x-auto rounded-lg border border-border/60">
                <Table>
                  <TableHeader>
                    <TableRow>
                      <TableHead>{r.nameEn}</TableHead>
                      <TableHead>{r.rateType}</TableHead>
                      <TableHead>{r.price}</TableHead>
                      <TableHead>{r.estimatedDays}</TableHead>
                      <TableHead>{f.status}</TableHead>
                    </TableRow>
                  </TableHeader>
                  <TableBody>
                    {current.shippingRates.map((rate) => (
                      <TableRow key={rate.id}>
                        <TableCell>
                          <div className="space-y-0.5">
                            <p dir="ltr">{rate.nameEn || '—'}</p>
                            {rate.nameAr && (
                              <p className="text-muted-foreground text-xs" dir="rtl">
                                {rate.nameAr}
                              </p>
                            )}
                          </div>
                        </TableCell>
                        <TableCell>{SHIPPING_RATE_TYPE_LABELS[rate.rateType]}</TableCell>
                        <TableCell dir="ltr">{rate.price}</TableCell>
                        <TableCell dir="ltr">
                          {rate.estimatedDaysMin != null || rate.estimatedDaysMax != null
                            ? `${rate.estimatedDaysMin ?? '?'} – ${rate.estimatedDaysMax ?? '?'}`
                            : m.empty}
                        </TableCell>
                        <TableCell>
                          <Badge variant={rate.isActive ? 'default' : 'secondary'}>
                            {rate.isActive ? f.statusActive : f.statusInactive}
                          </Badge>
                        </TableCell>
                      </TableRow>
                    ))}
                  </TableBody>
                </Table>
              </div>
            ) : (
              <p className="text-muted-foreground text-sm">{r.noRates}</p>
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
              {messages.shippingZone.form.cancel}
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
