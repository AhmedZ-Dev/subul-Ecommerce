import { Suspense } from 'react';
import { PageContainer } from '@/components/layout/page-container';
import { OrderListingPage } from '@/features/order';

export const metadata = {
  title: 'الطلبات',
};

export default function OrdersPage() {
  return (
    <PageContainer scrollable={false}>
      <Suspense>
        <OrderListingPage />
      </Suspense>
    </PageContainer>
  );
}
