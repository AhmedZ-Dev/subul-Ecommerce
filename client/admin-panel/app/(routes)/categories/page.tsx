import { Suspense } from 'react';
import { PageContainer } from '@/components/layout/page-container';
import { CategoryListingPage } from '@/features/category';

export const metadata = {
  title: 'الفئات',
};

export default function CategoriesPage() {
  return (
    <PageContainer scrollable={false}>
      <Suspense>
        <CategoryListingPage />
      </Suspense>
    </PageContainer>
  );
}
