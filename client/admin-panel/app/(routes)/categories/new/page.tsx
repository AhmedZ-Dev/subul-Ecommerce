import { PageContainer } from '@/components/layout/page-container';
import { PageHeader } from '@/components/layout/page-header';
import { CategoryForm, getCategories } from '@/features/category';
import { buildCategoryTree, flattenTree } from '@/features/category/utils';
import { messages } from '@/lib/messages.ar';

export const metadata = {
  title: messages.category.form.newTitle,
};

export default async function NewCategoryPage() {
  let parentOptions: Array<{ id: number; nameEn: string; nameAr: string | null; depth: number }> = [];

  try {
    const result = await getCategories({ limit: 1000, sortBy: 'nameEn', sortOrder: 'asc' });
    const tree = buildCategoryTree(result.items);
    parentOptions = flattenTree(tree);
  } catch {
    // If API is unavailable, form renders with an empty parent select
  }

  return (
    <PageContainer>
      <div className="mx-auto flex w-full max-w-5xl flex-col gap-6">
        <PageHeader
          title={messages.category.form.newTitle}
          description={messages.category.form.newDescription}
        />
        <CategoryForm parentOptions={parentOptions} />
      </div>
    </PageContainer>
  );
}
