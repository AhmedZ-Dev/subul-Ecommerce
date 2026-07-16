import { notFound } from 'next/navigation';
import { PageContainer } from '@/components/layout/page-container';
import { OrderView, getCachedOrderById } from '@/features/order';
import { messages } from '@/lib/messages.ar';

export async function generateMetadata({
  params,
}: {
  params: Promise<{ id: string }>;
}) {
  const { id } = await params;
  const orderId = parseInt(id, 10);

  if (isNaN(orderId) || orderId <= 0) {
    return { title: messages.order.view.title };
  }

  const order = await getCachedOrderById(orderId).catch(() => null);

  return {
    title: order
      ? messages.order.view.pageTitle(order.orderNumber)
      : messages.order.view.title,
  };
}

interface ViewOrderPageProps {
  params: Promise<{ id: string }>;
}

export default async function ViewOrderPage({ params }: ViewOrderPageProps) {
  const { id } = await params;
  const orderId = parseInt(id, 10);

  if (isNaN(orderId) || orderId <= 0) {
    notFound();
  }

  const order = await getCachedOrderById(orderId).catch(() => null);

  if (!order) {
    notFound();
  }

  return (
    <PageContainer>
      <div className="mx-auto flex w-full max-w-5xl flex-col gap-6">
        <OrderView order={order} />
      </div>
    </PageContainer>
  );
}
