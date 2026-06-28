'use client';
// features/category/components/category-tree.tsx
// Recursive hierarchical tree view of categories with collapsible nodes.

import { useState } from 'react';
import Link from 'next/link';
import { Badge } from '@/components/ui/badge';
import { Button } from '@/components/ui/button';
import {
  DropdownMenu,
  DropdownMenuContent,
  DropdownMenuItem,
  DropdownMenuSeparator,
  DropdownMenuTrigger,
} from '@/components/ui/dropdown-menu';
import { Skeleton } from '@/components/ui/skeleton';
import {
  ChevronRight,
  ChevronDown,
  FolderOpen,
  FolderClosed,
  MoreHorizontal,
  Pencil,
  Trash2,
} from 'lucide-react';
import type { CategoryTreeNode } from '../types';
import { useDeleteCategory } from '../hooks/useCategoryMutations';
import { CATEGORY_STATUS_COLORS } from '../constants';

// ─── Tree Node ────────────────────────────────────────────────────────────────

interface TreeNodeProps {
  node: CategoryTreeNode;
  level?: number;
}

function TreeNode({ node, level = 0 }: TreeNodeProps) {
  const [isOpen, setIsOpen] = useState(level < 2); // auto-expand first 2 levels
  const { mutate: deleteCategory } = useDeleteCategory();
  const hasChildren = node.children.length > 0;

  return (
    <li className="select-none">
      <div
        className="group hover:bg-muted/50 flex items-center gap-2 rounded-md px-2 py-1.5 transition-colors"
        style={{ paddingLeft: `${level * 20 + 8}px` }}
      >
        {/* Expand / collapse toggle */}
        <button
          type="button"
          onClick={() => setIsOpen((o) => !o)}
          className="text-muted-foreground hover:text-foreground flex size-5 shrink-0 items-center justify-center rounded transition-colors"
          aria-label={isOpen ? 'Collapse' : 'Expand'}
          disabled={!hasChildren}
        >
          {hasChildren ? (
            isOpen ? (
              <ChevronDown className="size-4" />
            ) : (
              <ChevronRight className="size-4" />
            )
          ) : (
            <span className="size-4" />
          )}
        </button>

        {/* Folder icon */}
        {isOpen && hasChildren ? (
          <FolderOpen className="text-primary size-4 shrink-0" />
        ) : (
          <FolderClosed
            className={`size-4 shrink-0 ${hasChildren ? 'text-primary' : 'text-muted-foreground'}`}
          />
        )}

        {/* Name */}
        <div className="flex min-w-0 flex-1 items-center gap-2">
          <span className="truncate text-sm font-medium">{node.nameEn}</span>
          <span className="text-muted-foreground shrink-0 truncate text-xs" dir="rtl">
            {node.nameAr}
          </span>
        </div>

        {/* Slug */}
        <code className="bg-muted text-muted-foreground hidden rounded px-1.5 py-0.5 text-xs sm:block">
          {node.slug}
        </code>

        {/* Status badge */}
        <Badge
          variant="outline"
          className={`hidden shrink-0 text-xs md:flex ${CATEGORY_STATUS_COLORS[node.status]}`}
        >
          {node.status}
        </Badge>

        {/* Actions */}
        <DropdownMenu>
          <DropdownMenuTrigger asChild>
            <Button
              id={`tree-actions-${node.id}`}
              variant="ghost"
              size="icon-sm"
              className="opacity-0 transition-opacity group-hover:opacity-100"
              aria-label="Open actions"
            >
              <MoreHorizontal className="size-3.5" />
            </Button>
          </DropdownMenuTrigger>
          <DropdownMenuContent align="end">
            <DropdownMenuItem asChild>
              <Link href={`/categories/${node.id}/edit`}>
                <Pencil className="mr-2 size-4" />
                Edit
              </Link>
            </DropdownMenuItem>
            <DropdownMenuSeparator />
            <DropdownMenuItem
              id={`tree-delete-${node.id}`}
              className="text-destructive focus:text-destructive"
              onClick={() => {
                if (
                  confirm(
                    `Delete "${node.nameEn}"?\nThis may also affect sub-categories.`,
                  )
                ) {
                  deleteCategory(node.id);
                }
              }}
            >
              <Trash2 className="mr-2 size-4" />
              Delete
            </DropdownMenuItem>
          </DropdownMenuContent>
        </DropdownMenu>
      </div>

      {/* Children */}
      {hasChildren && isOpen && (
        <ul className="mt-0.5 space-y-0.5">
          {node.children.map((child) => (
            <TreeNode key={child.id} node={child} level={level + 1} />
          ))}
        </ul>
      )}
    </li>
  );
}

// ─── Skeleton ─────────────────────────────────────────────────────────────────

function TreeSkeleton() {
  return (
    <div className="space-y-2 p-4">
      {[0, 1, 2, 3, 4].map((i) => (
        <div
          key={i}
          className="flex items-center gap-2"
          style={{ paddingLeft: `${(i % 3) * 20}px` }}
        >
          <Skeleton className="size-4" />
          <Skeleton className="h-4 w-40" />
          <Skeleton className="h-4 w-20" />
        </div>
      ))}
    </div>
  );
}

// ─── Public component ─────────────────────────────────────────────────────────

interface CategoryTreeProps {
  nodes: CategoryTreeNode[];
  isLoading?: boolean;
}

export function CategoryTree({ nodes, isLoading }: CategoryTreeProps) {
  if (isLoading) return <TreeSkeleton />;

  if (nodes.length === 0) {
    return (
      <div className="text-muted-foreground py-10 text-center text-sm">
        No categories found.
      </div>
    );
  }

  return (
    <div className="rounded-md border">
      {/* Header row */}
      <div className="border-b bg-muted/50 px-4 py-2">
        <div className="text-muted-foreground flex items-center gap-4 text-xs font-medium uppercase tracking-wide">
          <span className="flex-1">Name</span>
          <span className="hidden sm:block">Slug</span>
          <span className="hidden md:block">Status</span>
          <span className="w-7" />
        </div>
      </div>
      <ul className="space-y-0.5 p-2">
        {nodes.map((node) => (
          <TreeNode key={node.id} node={node} level={0} />
        ))}
      </ul>
    </div>
  );
}
