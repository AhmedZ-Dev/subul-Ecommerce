import { Badge } from '@/components/ui/badge';
import { cn } from '@/lib/utils';

export type StatusTone = 'success' | 'warning' | 'danger' | 'neutral' | 'info';

const dotToneClasses: Record<StatusTone, string> = {
  success: 'bg-green-500',
  warning: 'bg-amber-500',
  danger: 'bg-red-500',
  neutral: 'bg-muted-foreground/60',
  info: 'bg-blue-500',
};

const badgeToneClasses: Record<StatusTone, string> = {
  success: 'bg-green-50 text-green-700 border-green-200 dark:bg-green-950/40 dark:text-green-400 dark:border-green-900',
  warning: 'bg-amber-50 text-amber-700 border-amber-200 dark:bg-amber-950/40 dark:text-amber-400 dark:border-amber-900',
  danger: 'bg-red-50 text-red-700 border-red-200 dark:bg-red-950/40 dark:text-red-400 dark:border-red-900',
  neutral: 'bg-gray-50 text-gray-700 border-gray-200 dark:bg-gray-900/40 dark:text-gray-400 dark:border-gray-800',
  info: 'bg-blue-50 text-blue-700 border-blue-200 dark:bg-blue-950/40 dark:text-blue-400 dark:border-blue-900',
};

interface StatusIndicatorProps {
  label: string;
  tone?: StatusTone;
  variant?: 'dot' | 'badge';
  className?: string;
}

export function StatusIndicator({
  label,
  tone = 'neutral',
  variant = 'dot',
  className,
}: StatusIndicatorProps) {
  if (variant === 'badge') {
    return (
      <Badge variant="outline" className={cn(badgeToneClasses[tone], className)}>
        {label}
      </Badge>
    );
  }

  return (
    <span className={cn('inline-flex items-center gap-2 text-sm', className)}>
      <span
        className={cn('size-2 shrink-0 rounded-full', dotToneClasses[tone])}
        aria-hidden="true"
      />
      <span>{label}</span>
    </span>
  );
}
