import { Suspense } from 'react';
import { PageContainer } from '@/components/layout/page-container';
import { PaymentMethodListingPage } from '@/features/payment-method';
import { messages } from '@/lib/messages.ar';

export const metadata = {
  title: messages.paymentMethod.listing.title,
};

export default function PaymentMethodsPage() {
  return (
    <PageContainer scrollable={false}>
      <Suspense>
        <PaymentMethodListingPage />
      </Suspense>
    </PageContainer>
  );
}
