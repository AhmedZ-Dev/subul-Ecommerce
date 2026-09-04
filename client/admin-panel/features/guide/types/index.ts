import type { LucideIcon } from 'lucide-react';

export type GuideTone = 'info' | 'tip' | 'warning' | 'danger';

/** A single field explained in a field-reference block. */
export interface GuideField {
  name: string;
  description: string;
  required?: boolean;
}

/**
 * Content is data rather than JSX so the table of contents can be derived
 * automatically and new sections stay a data-only edit.
 */
export type GuideBlock =
  | { kind: 'text'; text: string }
  | { kind: 'steps'; title?: string; items: string[] }
  | { kind: 'list'; title?: string; items: string[] }
  | { kind: 'fields'; title?: string; items: GuideField[] }
  | { kind: 'callout'; tone: GuideTone; title: string; text: string }
  | { kind: 'path'; label: string; segments: string[] };

export interface GuideSubsection {
  id: string;
  title: string;
  blocks: GuideBlock[];
}

export interface GuideSection {
  id: string;
  title: string;
  summary: string;
  icon: LucideIcon;
  subsections: GuideSubsection[];
}
