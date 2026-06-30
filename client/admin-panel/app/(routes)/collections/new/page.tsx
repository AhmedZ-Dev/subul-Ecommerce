import { PageContainer } from '@/components/layout/page-container';
import { PageHeader } from '@/components/layout/page-header';
import { CollectionForm } from '@/features/collection';
import { messages } from '@/lib/messages.ar';

export const metadata = {
  title: messages.collection.form.newTitle,
};

export default function NewCollectionPage() {
  return (
    <PageContainer>
      <div className="mx-auto flex w-full max-w-5xl flex-col gap-6">
        <PageHeader
          title={messages.collection.form.newTitle}
          description={messages.collection.form.newDescription}
        />
        <CollectionForm />
      </div>
    </PageContainer>
  );
}
