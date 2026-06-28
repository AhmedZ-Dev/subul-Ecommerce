"use client"

import * as React from "react"
import Link from "next/link"
import { CommandIcon } from "lucide-react"
import { useKBar } from "kbar"

import { NavMain } from "@/components/nav-main"
import { NavSecondary, type NavSecondaryItem } from "@/components/nav-secondary"
import { NavUser } from "@/components/nav-user"
import {
  Sidebar,
  SidebarContent,
  SidebarFooter,
  SidebarHeader,
  SidebarMenu,
  SidebarMenuButton,
  SidebarMenuItem,
} from "@/components/ui/sidebar"
import { navMain, navSecondary } from "@/config/navigation"
import { messages } from "@/lib/messages.ar"

// TODO: Replace with the authenticated user from your auth provider.
const placeholderUser = {
  name: "المستخدم",
  email: "user@example.com",
  avatar: "",
}

export function AppSidebar({ ...props }: React.ComponentProps<typeof Sidebar>) {
  const { query } = useKBar()

  const mainItems = navMain.map((item) => ({
    title: item.title,
    url: item.url,
    icon: <item.icon />,
  }))

  const secondaryItems: NavSecondaryItem[] = navSecondary.map((item) => ({
    title: item.title,
    icon: <item.icon />,
    onClick: () => query.toggle(),
  }))

  return (
    <Sidebar collapsible="offcanvas" side="right" {...props}>
      <SidebarHeader>
        <SidebarMenu>
          <SidebarMenuItem>
            <SidebarMenuButton
              asChild
              className="data-[slot=sidebar-menu-button]:p-1.5!"
            >
              <Link href="/dashboard">
                <CommandIcon className="size-5!" />
                <span className="text-base font-semibold">
                  {messages.common.companyName}
                </span>
              </Link>
            </SidebarMenuButton>
          </SidebarMenuItem>
        </SidebarMenu>
      </SidebarHeader>
      <SidebarContent>
        <NavMain items={mainItems} />
      </SidebarContent>
      <SidebarFooter>
        <NavSecondary items={secondaryItems} />
        <NavUser user={placeholderUser} />
      </SidebarFooter>
    </Sidebar>
  )
}
