import { PageContainer } from '@/components/layout/page-container';
import { GuidePage } from '@/features/guide';

export const metadata = {
  title: 'دليل الاستخدام',
};

export default function GuideRoutePage() {
  return (
    <PageContainer>
      <GuidePage />
    </PageContainer>
  );
}
