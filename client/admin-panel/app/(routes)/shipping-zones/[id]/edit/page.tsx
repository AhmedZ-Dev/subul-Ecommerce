import { notFound } from 'next/navigation';
import { PageContainer } from '@/components/layout/page-container';
import { PageHeader } from '@/components/layout/page-header';
import { ShippingZoneForm, getShippingZoneById } from '@/features/shipping-zone';
import { messages } from '@/lib/messages.ar';

export const metadata = {
  title: messages.shippingZone.form.editTitle,
};

interface EditShippingZonePageProps {
  params: Promise<{ id: string }>;
}

export default async function EditShippingZonePage({ params }: EditShippingZonePageProps) {
  const { id } = await params;
  const zoneId = parseInt(id, 10);

  if (isNaN(zoneId) || zoneId <= 0) {
    notFound();
  }

  const zone = await getShippingZoneById(zoneId).catch(() => null);

  if (!zone) {
    notFound();
  }

  return (
    <PageContainer>
      <div className="mx-auto flex w-full max-w-5xl flex-col gap-6">
        <PageHeader
          title={messages.shippingZone.form.editTitle}
          description={zone.nameEn}
        />
        <ShippingZoneForm initialData={zone} />
      </div>
    </PageContainer>
  );
}
