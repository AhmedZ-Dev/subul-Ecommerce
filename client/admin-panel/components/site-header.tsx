import { Separator } from "@/components/ui/separator"
import { SidebarTrigger } from "@/components/ui/sidebar"
import { HeaderBreadcrumb } from "@/components/header/header-breadcrumb"
import { GlobalSearchTrigger } from "@/components/header/global-search-trigger"
import { MobileGlobalSearchTrigger } from "@/components/header/mobile-global-search-trigger"
import { NotificationBell } from "@/components/header/notification-bell"
import { ThemeSettingsMenu } from "./theme-settings-menu"

export function SiteHeader() {
  return (
    <header className="flex h-(--header-height) shrink-0 items-center gap-2 border-b transition-[width,height] ease-linear group-has-data-[collapsible=icon]/sidebar-wrapper:h-(--header-height)">
      <div className="flex w-full items-center gap-1 px-4 lg:gap-2 lg:px-6">
        <SidebarTrigger className="-ms-1" />
        <Separator
          orientation="vertical"
          className="mx-2 data-[orientation=vertical]:h-4"
        />
        <HeaderBreadcrumb />
        <div className="ms-auto flex items-center gap-1">
          <GlobalSearchTrigger className="hidden max-w-[220px] sm:flex" />
          <MobileGlobalSearchTrigger />
          <NotificationBell />
          <ThemeSettingsMenu />
        </div>
      </div>
    </header>
  )
}
