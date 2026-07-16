import { Suspense } from 'react';
import { PageContainer } from '@/components/layout/page-container';
import { ProductListingPage } from '@/features/product';

export const metadata = {
  title: 'المنتجات',
};

export default function ProductsPage() {
  return (
    <PageContainer scrollable={false}>
      <Suspense>
        <ProductListingPage />
      </Suspense>
    </PageContainer>
  );
}
