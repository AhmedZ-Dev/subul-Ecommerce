import { notFound } from 'next/navigation';
import { PageContainer } from '@/components/layout/page-container';
import { PageHeader } from '@/components/layout/page-header';
import { BrandForm, getBrandById } from '@/features/brand';
import { messages } from '@/lib/messages.ar';

export const metadata = {
  title: messages.brand?.form?.editTitle || 'تعديل الماركة',
};

interface EditBrandPageProps {
  params: Promise<{ id: string }>;
}

export default async function EditBrandPage({ params }: EditBrandPageProps) {
  const { id } = await params;
  const brandId = parseInt(id, 10);

  if (isNaN(brandId) || brandId <= 0) {
    notFound();
  }

  const brand = await getBrandById(brandId).catch(() => null);

  if (!brand) {
    notFound();
  }

  return (
    <PageContainer>
      <div className="mx-auto flex w-full max-w-5xl flex-col gap-6">
        <PageHeader
          title={messages.brand?.form?.editTitle || 'تعديل الماركة'}
          description={brand.name}
        />
        <BrandForm initialData={brand} />
      </div>
    </PageContainer>
  );
}
