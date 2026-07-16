import { notFound } from 'next/navigation';
import { PageContainer } from '@/components/layout/page-container';
import { PageHeader } from '@/components/layout/page-header';
import { ProductForm, getProductById } from '@/features/product';
import { messages } from '@/lib/messages.ar';

export const metadata = {
  title: messages.product.form.editTitle,
};

interface EditProductPageProps {
  params: Promise<{ id: string }>;
}

export default async function EditProductPage({ params }: EditProductPageProps) {
  const { id } = await params;
  const productId = parseInt(id, 10);

  if (isNaN(productId) || productId <= 0) {
    notFound();
  }

  const product = await getProductById(productId).catch(() => null);

  if (!product) {
    notFound();
  }

  return (
    <PageContainer>
      <div className="mx-auto flex w-full max-w-5xl flex-col gap-6">
        <PageHeader
          title={messages.product.form.editTitle}
          description={product.nameEn}
        />
        <ProductForm initialData={product} />
      </div>
    </PageContainer>
  );
}
