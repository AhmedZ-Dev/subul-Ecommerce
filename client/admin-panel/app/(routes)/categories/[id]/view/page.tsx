import { notFound } from 'next/navigation';
import { PageContainer } from '@/components/layout/page-container';
import { CategoryView, getCachedCategoryById } from '@/features/category';
import { messages } from '@/lib/messages.ar';

export async function generateMetadata({
  params,
}: {
  params: Promise<{ id: string }>;
}) {
  const { id } = await params;
  const categoryId = parseInt(id, 10);

  if (isNaN(categoryId) || categoryId <= 0) {
    return { title: messages.category.view.title };
  }

  const category = await getCachedCategoryById(categoryId).catch(() => null);

  return {
    title: category
      ? messages.category.view.pageTitle(category.nameAr ?? category.nameEn)
      : messages.category.view.title,
  };
}

interface ViewCategoryPageProps {
  params: Promise<{ id: string }>;
}

export default async function ViewCategoryPage({ params }: ViewCategoryPageProps) {
  const { id } = await params;
  const categoryId = parseInt(id, 10);

  if (isNaN(categoryId) || categoryId <= 0) {
    notFound();
  }

  const category = await getCachedCategoryById(categoryId).catch(() => null);

  if (!category) {
    notFound();
  }

  return (
    <PageContainer>
      <div className="mx-auto flex w-full max-w-5xl flex-col gap-6">
        <CategoryView category={category} />
      </div>
    </PageContainer>
  );
}
