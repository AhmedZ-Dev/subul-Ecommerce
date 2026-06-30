import { notFound } from 'next/navigation';
import { PageContainer } from '@/components/layout/page-container';
import { PageHeader } from '@/components/layout/page-header';
import { CollectionForm, getCollectionById } from '@/features/collection';
import { messages } from '@/lib/messages.ar';

export const metadata = {
  title: messages.collection.form.editTitle,
};

interface EditCollectionPageProps {
  params: Promise<{ id: string }>;
}

export default async function EditCollectionPage({ params }: EditCollectionPageProps) {
  const { id } = await params;
  const collectionId = parseInt(id, 10);

  if (isNaN(collectionId) || collectionId <= 0) {
    notFound();
  }

  const collection = await getCollectionById(collectionId).catch(() => null);

  if (!collection) {
    notFound();
  }

  return (
    <PageContainer>
      <div className="mx-auto flex w-full max-w-5xl flex-col gap-6">
        <PageHeader
          title={messages.collection.form.editTitle}
          description={collection.nameEn}
        />
        <CollectionForm initialData={collection} />
      </div>
    </PageContainer>
  );
}
