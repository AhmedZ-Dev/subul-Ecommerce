'use client';

import { useMemo, useState } from 'react';
import { FolderOpen, FolderTree } from 'lucide-react';

import { Input } from '@/components/ui/input';
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from '@/components/ui/select';
import { messages } from '@/lib/messages.ar';
import { cn } from '@/lib/utils';

const PARENT_SEARCH_THRESHOLD = 15;

interface ParentOption {
  id: number;
  nameEn: string;
  nameAr: string | null;
  depth: number;
}

interface CategoryParentSelectProps {
  value: number | null | undefined;
  options: ParentOption[];
  disabled?: boolean;
  onChange: (value: number | null) => void;
  id?: string;
}

function formatParentLabel(cat: ParentOption) {
  const ar = cat.nameAr?.trim();
  if (ar && ar !== cat.nameEn) {
    return `${cat.nameEn} · ${ar}`;
  }
  return cat.nameEn;
}

export function CategoryParentSelect({
  value,
  options,
  disabled,
  onChange,
  id,
}: CategoryParentSelectProps) {
  const [filter, setFilter] = useState('');
  const showSearch = options.length >= PARENT_SEARCH_THRESHOLD;
  const normalizedValue =
    value !== null && value !== undefined ? Number(value) : null;
  const selected = options.find((o) => o.id === normalizedValue);

  const filtered = useMemo(() => {
    const q = filter.trim().toLowerCase();
    if (!q) return options;
    return options.filter(
      (cat) =>
        cat.nameEn.toLowerCase().includes(q) ||
        (cat.nameAr?.toLowerCase().includes(q) ?? false),
    );
  }, [filter, options]);

  return (
    <Select
      onValueChange={(v) => onChange(v === 'none' ? null : parseInt(v, 10))}
      value={normalizedValue !== null ? String(normalizedValue) : 'none'}
      disabled={disabled}
    >
      <SelectTrigger id={id}>
        <SelectValue placeholder={messages.category.form.parentPlaceholder}>
          {normalizedValue === null
            ? messages.category.form.parentNone
            : selected
              ? formatParentLabel(selected)
              : undefined}
        </SelectValue>
      </SelectTrigger>
      <SelectContent>
        {showSearch && (
          <div className="p-2 pb-0" onPointerDown={(e) => e.stopPropagation()}>
            <Input
              value={filter}
              onChange={(e) => setFilter(e.target.value)}
              placeholder={messages.category.form.parentSearch}
              className="h-8"
            />
          </div>
        )}
        <SelectItem value="none">{messages.category.form.parentNone}</SelectItem>
        {filtered.map((cat) => (
          <SelectItem key={cat.id} value={String(cat.id)}>
            <span
              className="flex items-center gap-2"
              style={{ paddingInlineStart: `${cat.depth * 0.75}rem` }}
            >
              {cat.depth === 0 ? (
                <FolderOpen className="text-primary size-3.5 shrink-0" />
              ) : (
                <FolderTree className="text-muted-foreground size-3.5 shrink-0" />
              )}
              <span className={cn('truncate', cat.depth > 0 && 'text-muted-foreground')}>
                {formatParentLabel(cat)}
              </span>
            </span>
          </SelectItem>
        ))}
      </SelectContent>
    </Select>
  );
}
