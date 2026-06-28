// features/category/utils/index.ts
// Pure functions for category domain logic — no React, no API calls

import type { CategoryDto, CategoryListItem, CategoryTreeNode } from '../types';

// ─── Tree building ────────────────────────────────────────────────────────────

/**
 * Converts a flat list of categories into a hierarchical tree.
 * Computes `depth` during construction (not returned by backend).
 * Assumes the backend returns all categories (up to limit).
 */
export function buildCategoryTree(items: CategoryListItem[]): CategoryTreeNode[] {
  const nodeMap = new Map<number, CategoryTreeNode>();

  // First pass: create nodes with empty children arrays and depth=0 (corrected below)
  for (const item of items) {
    nodeMap.set(item.id, { ...item, depth: 0, children: [] });
  }

  const roots: CategoryTreeNode[] = [];

  // Second pass: wire up parent-child relationships
  for (const node of nodeMap.values()) {
    if (node.parentId === null || node.parentId === undefined) {
      roots.push(node);
    } else {
      const parent = nodeMap.get(node.parentId);
      if (parent) {
        parent.children.push(node);
      } else {
        // Parent not in list (filtered out) — treat as root
        roots.push(node);
      }
    }
  }

  // Third pass: assign correct depth values via BFS
  function assignDepth(nodes: CategoryTreeNode[], depth: number) {
    for (const node of nodes) {
      node.depth = depth;
      assignDepth(node.children, depth + 1);
    }
  }
  assignDepth(roots, 0);

  return roots;
}

/**
 * Flattens a tree back to a depth-first array (children stripped, depth preserved).
 */
export function flattenTree(nodes: CategoryTreeNode[]): Omit<CategoryTreeNode, 'children'>[] {
  const result: Omit<CategoryTreeNode, 'children'>[] = [];

  function traverse(items: CategoryTreeNode[]) {
    for (const item of items) {
      const { children: _children, ...listItem } = item;
      result.push(listItem);
      if (item.children.length > 0) {
        traverse(item.children);
      }
    }
  }

  traverse(nodes);
  return result;
}

/**
 * Returns true if `potentialAncestorId` is an ancestor of `nodeId` in the tree.
 * Used to prevent circular parent assignments in the form.
 */
export function isAncestor(
  nodes: CategoryTreeNode[],
  nodeId: number,
  potentialAncestorId: number,
): boolean {
  function findNodeAndCheckAncestry(
    items: CategoryTreeNode[],
    targetId: number,
    searchId: number,
  ): boolean {
    for (const item of items) {
      if (item.id === targetId) {
        // Found the node; check if searchId is in its subtree
        return isInSubtree(item, searchId);
      }
      if (isAncestor(item.children, targetId, searchId)) return true;
    }
    return false;
  }

  function isInSubtree(node: CategoryTreeNode, searchId: number): boolean {
    if (node.id === searchId) return true;
    return node.children.some((child) => isInSubtree(child, searchId));
  }

  return findNodeAndCheckAncestry(nodes, nodeId, potentialAncestorId);
}

// ─── Display helpers ──────────────────────────────────────────────────────────

/**
 * Returns the localised category name.
 */
export function getCategoryName(
  category: Pick<CategoryListItem, 'nameEn' | 'nameAr'>,
  locale: 'en' | 'ar' = 'en',
): string {
  return locale === 'ar' ? (category.nameAr ?? category.nameEn) : category.nameEn;
}

export function getParentDisplayName(
  category: Pick<CategoryDto, 'parentId' | 'parentNameEn' | 'parentNameAr'>,
  parent?: Pick<CategoryDto, 'nameEn' | 'nameAr'> | null,
): string | null {
  if (!category.parentId) return null;
  return (
    category.parentNameAr ??
    category.parentNameEn ??
    parent?.nameAr ??
    parent?.nameEn ??
    null
  );
}

/**
 * Generates a URL-friendly slug from an English string.
 */
export function generateSlug(nameEn: string): string {
  return nameEn
    .toLowerCase()
    .trim()
    .replace(/[^\w\s-]/g, '')
    .replace(/[\s_-]+/g, '-')
    .replace(/^-+|-+$/g, '');
}
