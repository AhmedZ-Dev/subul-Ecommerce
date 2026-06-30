import { PageContainer } from '@/components/layout/page-container';
import { PageHeader } from '@/components/layout/page-header';
import { AttributeGroupForm } from '@/features/attribute-group';
import { messages } from '@/lib/messages.ar';

const m = messages.attributeGroup;

export const metadata = {
  title: m.create_title,
};

export default function NewAttributeGroupPage() {
  return (
    <PageContainer>
      <div className="mx-auto flex w-full max-w-5xl flex-col gap-6">
        <PageHeader
          title={m.create_title}
          description={m.create_description}
        />
        <AttributeGroupForm />
      </div>
    </PageContainer>
  );
}
