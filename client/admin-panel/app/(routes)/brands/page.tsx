import { Suspense } from 'react';
import { PageContainer } from '@/components/layout/page-container';
import { BrandListingPage } from '@/features/brand';

export const metadata = {
  title: 'الماركات',
};

export default function BrandsPage() {
  return (
    <PageContainer scrollable={false}>
      <Suspense>
        <BrandListingPage />
      </Suspense>
    </PageContainer>
  );
}
