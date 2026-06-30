import { Suspense } from 'react';
import { PageContainer } from '@/components/layout/page-container';
import { AttributeGroupListingPage } from '@/features/attribute-group';

export const metadata = {
  title: 'مجموعات السمات',
};

export default function AttributeGroupsPage() {
  return (
    <PageContainer scrollable={false}>
      <Suspense>
        <AttributeGroupListingPage />
      </Suspense>
    </PageContainer>
  );
}
