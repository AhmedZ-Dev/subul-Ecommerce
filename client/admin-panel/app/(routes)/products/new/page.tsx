import { PageContainer } from '@/components/layout/page-container';
import { PageHeader } from '@/components/layout/page-header';
import { ProductForm } from '@/features/product';
import { messages } from '@/lib/messages.ar';

export const metadata = {
  title: messages.product.form.newTitle,
};

export default function NewProductPage() {
  return (
    <PageContainer>
      <div className="mx-auto flex w-full max-w-5xl flex-col gap-6">
        <PageHeader
          title={messages.product.form.newTitle}
          description={messages.product.form.newDescription}
        />
        <ProductForm />
      </div>
    </PageContainer>
  );
}
