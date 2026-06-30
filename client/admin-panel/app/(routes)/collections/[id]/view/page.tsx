import { notFound } from 'next/navigation';
import { PageContainer } from '@/components/layout/page-container';
import { CollectionView, getCachedCollectionById } from '@/features/collection';
import { messages } from '@/lib/messages.ar';

export async function generateMetadata({ params }: { params: Promise<{ id: string }> }) {
  const { id } = await params;
  const collectionId = parseInt(id, 10);

  if (isNaN(collectionId) || collectionId <= 0) {
    return { title: messages.collection.view.title };
  }

  const collection = await getCachedCollectionById(collectionId).catch(() => null);

  return {
    title: collection
      ? messages.collection.view.pageTitle(collection.nameEn)
      : messages.collection.view.title,
  };
}

interface ViewCollectionPageProps {
  params: Promise<{ id: string }>;
}

export default async function ViewCollectionPage({ params }: ViewCollectionPageProps) {
  const { id } = await params;
  const collectionId = parseInt(id, 10);

  if (isNaN(collectionId) || collectionId <= 0) {
    notFound();
  }

  const collection = await getCachedCollectionById(collectionId).catch(() => null);

  if (!collection) {
    notFound();
  }

  return (
    <PageContainer scrollable>
      <div className="mx-auto flex w-full max-w-5xl flex-col gap-6">
        <CollectionView collection={collection} />
      </div>
    </PageContainer>
  );
}
