"use client"

import { ThemeProvider } from "@/components/theme-provider"
import { DirectionProvider } from "@/components/ui/direction"
import { Toaster } from "@/components/ui/sonner"
import { TooltipProvider } from "@/components/ui/tooltip"
import { QueryClientProvider } from "@/providers/query-client-provider"

export function AppProviders({ children }: { children: React.ReactNode }) {
  return (
    <QueryClientProvider>
      <ThemeProvider>
        <DirectionProvider dir="rtl">
          <TooltipProvider>{children}</TooltipProvider>
          <Toaster richColors position="bottom-center" />
        </DirectionProvider>
      </ThemeProvider>
    </QueryClientProvider>
  )
}
