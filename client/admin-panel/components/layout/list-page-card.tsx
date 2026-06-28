import { Card, CardContent } from '@/components/ui/card';
import { cn } from '@/lib/utils';

interface ListPageCardProps {
  children: React.ReactNode;
  className?: string;
}

interface ListPageCardToolbarProps {
  children: React.ReactNode;
  className?: string;
}

interface ListPageCardContentProps {
  children: React.ReactNode;
  className?: string;
}

function ListPageCardRoot({ children, className }: ListPageCardProps) {
  return (
    <Card
      className={cn(
        'flex flex-1 flex-col shadow-xs',
        className,
      )}
    >
      {children}
    </Card>
  );
}

function ListPageCardToolbar({ children, className }: ListPageCardToolbarProps) {
  return (
    <div
      className={cn(
        'flex flex-col gap-3 border-b px-(--card-spacing) py-(--card-spacing) sm:flex-row sm:items-center',
        className,
      )}
    >
      {children}
    </div>
  );
}

function ListPageCardContent({ children, className }: ListPageCardContentProps) {
  return (
    <CardContent className={cn('flex flex-1 flex-col gap-3 pt-(--card-spacing)', className)}>
      {children}
    </CardContent>
  );
}

export const ListPageCard = Object.assign(ListPageCardRoot, {
  Toolbar: ListPageCardToolbar,
  Content: ListPageCardContent,
});
