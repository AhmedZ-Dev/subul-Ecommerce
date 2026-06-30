import { notFound } from 'next/navigation';
import { PageContainer } from '@/components/layout/page-container';
import { AttributeGroupView, getCachedAttributeGroupById } from '@/features/attribute-group';
import { messages } from '@/lib/messages.ar';

const m = messages.attributeGroup;

export async function generateMetadata({
  params,
}: {
  params: Promise<{ id: string }>;
}) {
  const { id } = await params;
  const groupId = parseInt(id, 10);

  if (isNaN(groupId) || groupId <= 0) {
    return { title: m.view.title };
  }

  const group = await getCachedAttributeGroupById(groupId).catch(() => null);

  return {
    title: group ? m.view.pageTitle(group.nameEn) : m.view.title,
  };
}

interface ViewAttributeGroupPageProps {
  params: Promise<{ id: string }>;
}

export default async function ViewAttributeGroupPage({ params }: ViewAttributeGroupPageProps) {
  const { id } = await params;
  const groupId = parseInt(id, 10);

  if (isNaN(groupId) || groupId <= 0) {
    notFound();
  }

  const attributeGroup = await getCachedAttributeGroupById(groupId).catch(() => null);

  if (!attributeGroup) {
    notFound();
  }

  return (
    <PageContainer>
      <div className="mx-auto flex w-full max-w-5xl flex-col gap-6">
        <AttributeGroupView data={attributeGroup} />
      </div>
    </PageContainer>
  );
}
