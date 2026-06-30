import { Suspense } from 'react';
import { PageContainer } from '@/components/layout/page-container';
import { CollectionListingPage } from '@/features/collection';

export const metadata = {
  title: 'المجموعات',
};

export default function CollectionsPage() {
  return (
    <PageContainer scrollable={false}>
      <Suspense>
        <CollectionListingPage />
      </Suspense>
    </PageContainer>
  );
}
