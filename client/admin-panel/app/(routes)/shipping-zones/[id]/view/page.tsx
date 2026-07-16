import { notFound } from 'next/navigation';
import { PageContainer } from '@/components/layout/page-container';
import { ShippingZoneView, getCachedShippingZoneById } from '@/features/shipping-zone';
import { messages } from '@/lib/messages.ar';

export async function generateMetadata({ params }: { params: Promise<{ id: string }> }) {
  const { id } = await params;
  const zoneId = parseInt(id, 10);

  if (isNaN(zoneId) || zoneId <= 0) {
    return { title: messages.shippingZone.view.title };
  }

  const zone = await getCachedShippingZoneById(zoneId).catch(() => null);

  return {
    title: zone
      ? messages.shippingZone.view.pageTitle(zone.nameEn)
      : messages.shippingZone.view.title,
  };
}

interface ViewShippingZonePageProps {
  params: Promise<{ id: string }>;
}

export default async function ViewShippingZonePage({ params }: ViewShippingZonePageProps) {
  const { id } = await params;
  const zoneId = parseInt(id, 10);

  if (isNaN(zoneId) || zoneId <= 0) {
    notFound();
  }

  const zone = await getCachedShippingZoneById(zoneId).catch(() => null);

  if (!zone) {
    notFound();
  }

  return (
    <PageContainer scrollable>
      <div className="mx-auto flex w-full max-w-5xl flex-col gap-6">
        <ShippingZoneView zone={zone} />
      </div>
    </PageContainer>
  );
}
