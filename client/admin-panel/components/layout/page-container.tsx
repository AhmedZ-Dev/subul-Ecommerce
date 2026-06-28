import { ScrollArea } from '@/components/ui/scroll-area';

interface PageContainerProps {
  children: React.ReactNode;
  /**
   * scrollable=true  → content inside ScrollArea (good for forms / long pages)
   * scrollable=false → flex-1 flex-col container, table scroll lives inside the component
   */
  scrollable?: boolean;
}

export function PageContainer({ children, scrollable = true }: PageContainerProps) {
  return scrollable ? (
    <ScrollArea className="h-[calc(100dvh-var(--header-height))]">
      <div className="flex flex-1 flex-col p-4 md:px-6">{children}</div>
    </ScrollArea>
  ) : (
    <div className="flex flex-1 flex-col p-4 md:px-6">{children}</div>
  );
}
