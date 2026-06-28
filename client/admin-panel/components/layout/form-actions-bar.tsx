import { cn } from '@/lib/utils';

interface FormActionsBarProps {
  children: React.ReactNode;
  className?: string;
}

export function FormActionsBar({ children, className }: FormActionsBarProps) {
  return (
    <div
      className={cn(
        '-mx-4 mt-6 flex flex-wrap items-center gap-3 bg-background/95 px-4 backdrop-blur-sm supports-backdrop-filter:bg-background/80 md:-mx-6 md:px-6',
        className,
      )}
    >
      {children}
    </div>
  );
}
