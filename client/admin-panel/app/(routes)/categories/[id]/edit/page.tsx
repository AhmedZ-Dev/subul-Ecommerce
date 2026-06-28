import { notFound } from 'next/navigation';
import { PageContainer } from '@/components/layout/page-container';
import { PageHeader } from '@/components/layout/page-header';
import { CategoryForm, getCategoryById, getCategories } from '@/features/category';
import { buildCategoryTree, flattenTree } from '@/features/category/utils';
import { messages } from '@/lib/messages.ar';

export const metadata = {
  title: messages.category.form.editTitle,
};

interface EditCategoryPageProps {
  params: Promise<{ id: string }>;
}

export default async function EditCategoryPage({ params }: EditCategoryPageProps) {
  const { id } = await params;
  const categoryId = parseInt(id, 10);

  if (isNaN(categoryId) || categoryId <= 0) {
    notFound();
  }

  const [category, allCategories] = await Promise.all([
    getCategoryById(categoryId).catch(() => null),
    getCategories({ limit: 1000, sortBy: 'nameEn', sortOrder: 'asc' }).catch(() => null),
  ]);

  if (!category) {
    notFound();
  }

  let parentOptions: Array<{ id: number; nameEn: string; nameAr: string | null; depth: number }> = [];
  if (allCategories) {
    const tree = buildCategoryTree(allCategories.items);
    parentOptions = flattenTree(tree);
  }

  return (
    <PageContainer>
      <div className="mx-auto flex w-full max-w-5xl flex-col gap-6">
        <PageHeader
          title={messages.category.form.editTitle}
          description={category.nameEn}
        />
        <CategoryForm initialData={category} parentOptions={parentOptions} />
      </div>
    </PageContainer>
  );
}
