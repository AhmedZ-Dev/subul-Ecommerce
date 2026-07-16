import { notFound } from 'next/navigation';
import { PageContainer } from '@/components/layout/page-container';
import { PageHeader } from '@/components/layout/page-header';
import { PaymentMethodForm, getPaymentMethodById } from '@/features/payment-method';
import { messages } from '@/lib/messages.ar';

export const metadata = {
  title: messages.paymentMethod.form.editTitle,
};

interface EditPaymentMethodPageProps {
  params: Promise<{ id: string }>;
}

export default async function EditPaymentMethodPage({ params }: EditPaymentMethodPageProps) {
  const { id } = await params;
  const paymentMethodId = parseInt(id, 10);

  if (isNaN(paymentMethodId) || paymentMethodId <= 0) {
    notFound();
  }

  const paymentMethod = await getPaymentMethodById(paymentMethodId).catch(() => null);

  if (!paymentMethod) {
    notFound();
  }

  const displayName =
    paymentMethod.labelAr ?? paymentMethod.labelEn ?? paymentMethod.name;

  return (
    <PageContainer>
      <div className="mx-auto flex w-full max-w-5xl flex-col gap-6">
        <PageHeader
          title={messages.paymentMethod.form.editTitle}
          description={displayName}
        />
        <PaymentMethodForm initialData={paymentMethod} />
      </div>
    </PageContainer>
  );
}
