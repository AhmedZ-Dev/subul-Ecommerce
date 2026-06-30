import {
  LayoutDashboardIcon,
  FolderIcon,
  SearchIcon,
  DatabaseIcon,
  TagIcon,
} from "lucide-react"
import { messages } from "@/lib/messages.ar"

// ─── Main navigation ──────────────────────────────────────────────────────────
// Add new features here. Each entry is picked up by KBar too.

export const navMain = [
  {
    id: "dashboard",
    title: messages.nav.dashboard,
    url: "/dashboard",
    icon: LayoutDashboardIcon,
  },
  {
    id: "categories",
    title: messages.nav.categories,
    url: "/categories",
    icon: FolderIcon,
  },
  {
    id: "brands",
    title: messages.nav.brands,
    url: "/brands",
    icon: TagIcon,
  },
  {
    id: "attributeGroups",
    title: messages.nav.attributeGroups,
    url: "/attribute-groups",
    icon: DatabaseIcon,
  },
  {
    id: "collections",
    title: messages.nav.collections,
    url: "/collections",
    icon: FolderIcon, // We can use FolderIcon or another suitable one
  },
] as const

// ─── Secondary navigation (sidebar footer) ────────────────────────────────────
// Use `action: "kbar"` for items that open the command palette instead of routing.

export const navSecondary = [
  {
    id: "search",
    title: messages.nav.search,
    action: "kbar",
    icon: SearchIcon,
  },
] as const

// ─── Documents section (hidden until routes exist) ────────────────────────────

export const navDocuments = [
  {
    id: "data-library",
    name: messages.nav.dataLibrary,
    url: "#",
    icon: DatabaseIcon,
  },
] as const

// ─── KBar command-palette items (derived from navMain) ────────────────────────

export const kbarNavItems = navMain.map((item) => ({
  id: item.id,
  name: item.title,
  url: item.url,
  section: messages.kbar.sectionNav,
  subtitle: item.title,
}))

export const kbarQuickActions = [
  {
    id: "add-category",
    name: messages.kbar.addCategory,
    url: "/categories/new",
    section: messages.kbar.sectionActions,
    subtitle: messages.kbar.addCategorySubtitle,
  },
  {
    id: "add-brand",
    name: messages.kbar.addBrand,
    url: "/brands/new",
    section: messages.kbar.sectionActions,
    subtitle: messages.kbar.addBrandSubtitle,
  },
  {
    id: "add-attribute-group",
    name: messages.kbar.addAttributeGroup,
    url: "/attribute-groups/new",
    section: messages.kbar.sectionActions,
    subtitle: messages.kbar.addAttributeGroupSubtitle,
  },
  {
    id: "add-collection",
    name: messages.kbar.addCollection,
    url: "/collections/new",
    section: messages.kbar.sectionActions,
    subtitle: messages.kbar.addCollectionSubtitle,
  },
] as const
