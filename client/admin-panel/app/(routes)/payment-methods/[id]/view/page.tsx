import { notFound } from 'next/navigation';
import { PageContainer } from '@/components/layout/page-container';
import { PaymentMethodView, getCachedPaymentMethodById } from '@/features/payment-method';
import { messages } from '@/lib/messages.ar';

export async function generateMetadata({
  params,
}: {
  params: Promise<{ id: string }>;
}) {
  const { id } = await params;
  const paymentMethodId = parseInt(id, 10);

  if (isNaN(paymentMethodId) || paymentMethodId <= 0) {
    return { title: messages.paymentMethod.view.title };
  }

  const paymentMethod = await getCachedPaymentMethodById(paymentMethodId).catch(() => null);
  const displayName =
    paymentMethod?.labelAr ??
    paymentMethod?.labelEn ??
    paymentMethod?.name ??
    '';

  return {
    title: paymentMethod
      ? messages.paymentMethod.view.pageTitle(displayName)
      : messages.paymentMethod.view.title,
  };
}

interface ViewPaymentMethodPageProps {
  params: Promise<{ id: string }>;
}

export default async function ViewPaymentMethodPage({ params }: ViewPaymentMethodPageProps) {
  const { id } = await params;
  const paymentMethodId = parseInt(id, 10);

  if (isNaN(paymentMethodId) || paymentMethodId <= 0) {
    notFound();
  }

  const paymentMethod = await getCachedPaymentMethodById(paymentMethodId).catch(() => null);

  if (!paymentMethod) {
    notFound();
  }

  return (
    <PageContainer>
      <div className="mx-auto flex w-full max-w-5xl flex-col gap-6">
        <PaymentMethodView paymentMethod={paymentMethod} />
      </div>
    </PageContainer>
  );
}
