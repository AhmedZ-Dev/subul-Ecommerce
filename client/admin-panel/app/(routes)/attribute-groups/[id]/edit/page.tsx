import { notFound } from 'next/navigation';
import { PageContainer } from '@/components/layout/page-container';
import { PageHeader } from '@/components/layout/page-header';
import { AttributeGroupForm, getAttributeGroupById } from '@/features/attribute-group';
import { messages } from '@/lib/messages.ar';

const m = messages.attributeGroup;

export const metadata = {
  title: m.edit_title,
};

interface EditAttributeGroupPageProps {
  params: Promise<{ id: string }>;
}

export default async function EditAttributeGroupPage({ params }: EditAttributeGroupPageProps) {
  const { id } = await params;
  const groupId = parseInt(id, 10);

  if (isNaN(groupId) || groupId <= 0) {
    notFound();
  }

  const attributeGroup = await getAttributeGroupById(groupId).catch(() => null);

  if (!attributeGroup) {
    notFound();
  }

  return (
    <PageContainer>
      <div className="mx-auto flex w-full max-w-5xl flex-col gap-6">
        <PageHeader
          title={m.edit_title}
          description={attributeGroup.nameEn}
        />
        <AttributeGroupForm initialData={attributeGroup} />
      </div>
    </PageContainer>
  );
}
