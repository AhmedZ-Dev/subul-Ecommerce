import { notFound } from 'next/navigation';
import { PageContainer } from '@/components/layout/page-container';
import { BrandView, getCachedBrandById } from '@/features/brand';
import { messages } from '@/lib/messages.ar';

export async function generateMetadata({
  params,
}: {
  params: Promise<{ id: string }>;
}) {
  const { id } = await params;
  const brandId = parseInt(id, 10);

  if (isNaN(brandId) || brandId <= 0) {
    return { title: messages.brand?.view?.title || 'تفاصيل الماركة' };
  }

  const brand = await getCachedBrandById(brandId).catch(() => null);

  return {
    title: brand
      ? (messages.brand?.view?.pageTitle ? messages.brand.view.pageTitle(brand.name) : `الماركة: ${brand.name}`)
      : (messages.brand?.view?.title || 'تفاصيل الماركة'),
  };
}

interface ViewBrandPageProps {
  params: Promise<{ id: string }>;
}

export default async function ViewBrandPage({ params }: ViewBrandPageProps) {
  const { id } = await params;
  const brandId = parseInt(id, 10);

  if (isNaN(brandId) || brandId <= 0) {
    notFound();
  }

  const brand = await getCachedBrandById(brandId).catch(() => null);

  if (!brand) {
    notFound();
  }

  return (
    <PageContainer>
      <div className="mx-auto flex w-full max-w-5xl flex-col gap-6">
        <BrandView brand={brand} />
      </div>
    </PageContainer>
  );
}
