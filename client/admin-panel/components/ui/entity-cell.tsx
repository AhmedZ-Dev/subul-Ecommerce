import { useEffect, useState } from 'react';
import Image from 'next/image';

import { cn } from '@/lib/utils';

interface EntityCellProps {
  title: string;
  subtitle?: string;
  thumbnailUrl?: string | null;
  thumbnailAlt?: string;
  fallback?: React.ReactNode;
  className?: string;
  subtitleDir?: 'rtl' | 'ltr' | 'auto';
  /** Use for API-hosted assets (bypasses Next image optimizer). */
  unoptimizedThumbnail?: boolean;
}

export function EntityCell({
  title,
  subtitle,
  thumbnailUrl,
  thumbnailAlt,
  fallback,
  className,
  subtitleDir,
  unoptimizedThumbnail = true,
}: EntityCellProps) {
  const [imageError, setImageError] = useState(false);
  const showThumbnail = thumbnailUrl && !imageError;

  useEffect(() => {
    setImageError(false);
  }, [thumbnailUrl]);

  return (
    <div className={cn('flex items-center gap-3', className)}>
      <div className="bg-muted flex size-10 shrink-0 items-center justify-center overflow-hidden rounded-full">
        {showThumbnail ? (
          <Image
            src={thumbnailUrl}
            alt={thumbnailAlt ?? title}
            width={40}
            height={40}
            unoptimized={unoptimizedThumbnail}
            className="size-full object-contain p-1"
            onError={() => setImageError(true)}
          />
        ) : (
          fallback
        )}
      </div>
      <div className="min-w-0">
        <p className="truncate font-medium">{title}</p>
        {subtitle && (
          <p
            className="text-muted-foreground truncate text-xs"
            dir={subtitleDir}
          >
            {subtitle}
          </p>
        )}
      </div>
    </div>
  );
}
