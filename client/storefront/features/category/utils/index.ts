import type { CategoryListItem, CategoryTreeNode } from "../types"

export function buildCategoryTree(items: CategoryListItem[]): CategoryTreeNode[] {
  const map = new Map<number, CategoryTreeNode>()
  const roots: CategoryTreeNode[] = []

  for (const item of items) {
    map.set(item.id, { ...item, depth: 0, children: [] })
  }

  for (const item of items) {
    const node = map.get(item.id)!
    if (item.parentId != null && map.has(item.parentId)) {
      const parent = map.get(item.parentId)!
      node.depth = parent.depth + 1
      parent.children.push(node)
    } else {
      roots.push(node)
    }
  }

  function sortNodes(nodes: CategoryTreeNode[]) {
    nodes.sort((a, b) => a.sortOrder - b.sortOrder)
    for (const node of nodes) sortNodes(node.children)
  }
  sortNodes(roots)

  return roots
}
