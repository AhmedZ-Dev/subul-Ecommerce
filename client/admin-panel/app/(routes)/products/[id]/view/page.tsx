import { notFound } from 'next/navigation';
import { PageContainer } from '@/components/layout/page-container';
import { ProductView, getCachedProductById } from '@/features/product';
import { messages } from '@/lib/messages.ar';

export async function generateMetadata({
  params,
}: {
  params: Promise<{ id: string }>;
}) {
  const { id } = await params;
  const productId = parseInt(id, 10);

  if (isNaN(productId) || productId <= 0) {
    return { title: messages.product.view.title };
  }

  const product = await getCachedProductById(productId).catch(() => null);

  return {
    title: product
      ? messages.product.view.pageTitle(product.nameEn)
      : messages.product.view.title,
  };
}

interface ViewProductPageProps {
  params: Promise<{ id: string }>;
}

export default async function ViewProductPage({ params }: ViewProductPageProps) {
  const { id } = await params;
  const productId = parseInt(id, 10);

  if (isNaN(productId) || productId <= 0) {
    notFound();
  }

  const product = await getCachedProductById(productId).catch(() => null);

  if (!product) {
    notFound();
  }

  return (
    <PageContainer>
      <div className="mx-auto flex w-full max-w-5xl flex-col gap-6">
        <ProductView product={product} />
      </div>
    </PageContainer>
  );
}
