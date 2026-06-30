import { PageContainer } from '@/components/layout/page-container';
import { PageHeader } from '@/components/layout/page-header';
import { BrandForm } from '@/features/brand';
import { messages } from '@/lib/messages.ar';

export const metadata = {
  title: messages.brand?.form?.newTitle || 'إضافة ماركة جديدة',
};

export default function NewBrandPage() {
  return (
    <PageContainer>
      <div className="mx-auto flex w-full max-w-5xl flex-col gap-6">
        <PageHeader
          title={messages.brand?.form?.newTitle || 'إضافة ماركة جديدة'}
          description={messages.brand?.form?.newDescription || 'أدخل تفاصيل الماركة الجديدة هنا.'}
        />
        <BrandForm />
      </div>
    </PageContainer>
  );
}
