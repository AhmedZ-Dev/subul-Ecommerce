import {
  LayoutDashboardIcon,
  FolderIcon,
  SearchIcon,
  DatabaseIcon,
  TagIcon,
  ShoppingBagIcon,
  ClipboardListIcon,
  TruckIcon,
  CreditCardIcon,
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
  {
    id: "shippingZones",
    title: messages.nav.shippingZones,
    url: "/shipping-zones",
    icon: TruckIcon,
  },
  {
    id: "orders",
    title: messages.nav.orders,
    url: "/orders",
    icon: ClipboardListIcon,
  },
  {
    id: "paymentMethods",
    title: messages.nav.paymentMethods,
    url: "/payment-methods",
    icon: CreditCardIcon,
  },
  {
    id: "products",
    title: messages.nav.products,
    url: "/products",
    icon: ShoppingBagIcon,
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
  {
    id: "add-shipping-zone",
    name: messages.kbar.addShippingZone,
    url: "/shipping-zones/new",
    section: messages.kbar.sectionActions,
    subtitle: messages.kbar.addShippingZoneSubtitle,
  },
  {
    id: "add-product",
    name: messages.kbar.addProduct,
    url: "/products/new",
    section: messages.kbar.sectionActions,
    subtitle: messages.kbar.addProductSubtitle,
  },
  {
    id: "add-payment-method",
    name: messages.kbar.addPaymentMethod,
    url: "/payment-methods/new",
    section: messages.kbar.sectionActions,
    subtitle: messages.kbar.addPaymentMethodSubtitle,
  },
] as const
