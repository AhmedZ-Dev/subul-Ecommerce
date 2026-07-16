import { PageContainer } from '@/components/layout/page-container';
import { PageHeader } from '@/components/layout/page-header';
import { ShippingZoneForm } from '@/features/shipping-zone';
import { messages } from '@/lib/messages.ar';

export const metadata = {
  title: messages.shippingZone.form.newTitle,
};

export default function NewShippingZonePage() {
  return (
    <PageContainer>
      <div className="mx-auto flex w-full max-w-5xl flex-col gap-6">
        <PageHeader
          title={messages.shippingZone.form.newTitle}
          description={messages.shippingZone.form.newDescription}
        />
        <ShippingZoneForm />
      </div>
    </PageContainer>
  );
}
