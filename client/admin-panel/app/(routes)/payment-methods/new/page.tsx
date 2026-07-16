import { PageContainer } from '@/components/layout/page-container';
import { PageHeader } from '@/components/layout/page-header';
import { PaymentMethodForm } from '@/features/payment-method';
import { messages } from '@/lib/messages.ar';

export const metadata = {
  title: messages.paymentMethod.form.newTitle,
};

export default function NewPaymentMethodPage() {
  return (
    <PageContainer>
      <div className="mx-auto flex w-full max-w-5xl flex-col gap-6">
        <PageHeader
          title={messages.paymentMethod.form.newTitle}
          description={messages.paymentMethod.form.newDescription}
        />
        <PaymentMethodForm />
      </div>
    </PageContainer>
  );
}
