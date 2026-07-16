import { Suspense } from 'react';
import { PageContainer } from '@/components/layout/page-container';
import { ShippingZoneListingPage } from '@/features/shipping-zone';

export const metadata = {
  title: 'مناطق الشحن',
};

export default function ShippingZonesPage() {
  return (
    <PageContainer scrollable={false}>
      <Suspense>
        <ShippingZoneListingPage />
      </Suspense>
    </PageContainer>
  );
}
