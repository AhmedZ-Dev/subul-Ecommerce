'use client';

import { useEffect, useRef, useState } from 'react';
import Image from 'next/image';
import { ImageIcon, Upload, X } from 'lucide-react';

import { Button } from '@/components/ui/button';
import { resolveAssetUrl } from '@/lib/asset-url';
import { cn } from '@/lib/utils';

const MAX_FILE_SIZE_BYTES = 5 * 1024 * 1024;
const ALLOWED_TYPES = ['image/jpeg', 'image/png', 'image/webp', 'image/gif'];

export interface ImageUploadFieldProps {
  id: string;
  label: string;
  hint?: string;
  invalidTypeError?: string;
  tooLargeError?: string;
  chooseLabel: string;
  replaceLabel: string;
  removeLabel: string;
  emptyLabel: string;
  currentUrl?: string | null;
  file: File | null;
  markedForRemoval?: boolean;
  disabled?: boolean;
  onFileChange: (file: File | null) => void;
  onMarkForRemoval?: () => void;
  onClearRemoval?: () => void;
  className?: string;
  previewClassName?: string;
}

export function ImageUploadField({
  id,
  label,
  hint,
  invalidTypeError = 'Invalid image file',
  tooLargeError = 'File too large',
  chooseLabel,
  replaceLabel,
  removeLabel,
  emptyLabel,
  currentUrl,
  file,
  markedForRemoval = false,
  disabled = false,
  onFileChange,
  onMarkForRemoval,
  onClearRemoval,
  className,
  previewClassName,
}: ImageUploadFieldProps) {
  const inputRef = useRef<HTMLInputElement>(null);
  const [previewUrl, setPreviewUrl] = useState<string | null>(null);
  const [error, setError] = useState<string | null>(null);

  const storedUrl = markedForRemoval ? null : resolveAssetUrl(currentUrl);
  const displayUrl = previewUrl ?? storedUrl;

  useEffect(() => {
    if (!file) {
      setPreviewUrl(null);
      return;
    }

    const objectUrl = URL.createObjectURL(file);
    setPreviewUrl(objectUrl);
    return () => URL.revokeObjectURL(objectUrl);
  }, [file]);

  function handleFileSelect(selected: File | null) {
    setError(null);
    if (!selected) {
      onFileChange(null);
      return;
    }

    if (!ALLOWED_TYPES.includes(selected.type)) {
      setError(invalidTypeError);
      return;
    }

    if (selected.size > MAX_FILE_SIZE_BYTES) {
      setError(tooLargeError);
      return;
    }

    onClearRemoval?.();
    onFileChange(selected);
  }

  function handleRemove() {
    onFileChange(null);
    if (inputRef.current) inputRef.current.value = '';
    if (currentUrl && onMarkForRemoval) {
      onMarkForRemoval();
      return;
    }
    setPreviewUrl(null);
  }

  return (
    <div className={cn('space-y-2', className)}>
      <p className="text-sm font-medium">{label}</p>

      <div
        className={cn(
          'relative flex min-h-36 flex-col items-center justify-center gap-3 overflow-hidden rounded-lg border border-dashed border-border/70 bg-muted/10 p-4',
          previewClassName,
        )}
      >
        {displayUrl ? (
          <div className="relative size-24 overflow-hidden rounded-md border bg-background">
            <Image
              src={displayUrl}
              alt={label}
              fill
              unoptimized
              className="object-contain"
            />
          </div>
        ) : (
          <div className="bg-muted text-muted-foreground flex size-16 items-center justify-center rounded-full">
            <ImageIcon className="size-7" />
          </div>
        )}

        <p className="text-muted-foreground text-center text-xs">
          {displayUrl ? '' : emptyLabel}
        </p>

        <div className="flex flex-wrap items-center justify-center gap-2">
          <Button
            type="button"
            variant="outline"
            size="sm"
            disabled={disabled}
            onClick={() => inputRef.current?.click()}
          >
            <Upload className="size-4" />
            {displayUrl ? replaceLabel : chooseLabel}
          </Button>

          {(displayUrl || file || currentUrl) && (
            <Button
              type="button"
              variant="ghost"
              size="sm"
              disabled={disabled}
              onClick={handleRemove}
            >
              <X className="size-4" />
              {removeLabel}
            </Button>
          )}
        </div>

        <input
          ref={inputRef}
          id={id}
          type="file"
          accept="image/png,image/jpeg,image/webp,image/gif"
          className="sr-only"
          disabled={disabled}
          onChange={(e) => handleFileSelect(e.target.files?.[0] ?? null)}
        />
      </div>

      {hint && !error && (
        <p className="text-muted-foreground text-xs">{hint}</p>
      )}
      {error && <p className="text-destructive text-xs">{error}</p>}
    </div>
  );
}
