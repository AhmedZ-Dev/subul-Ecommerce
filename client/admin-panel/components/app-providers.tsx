"use client"

import { ThemeProvider } from "@/components/theme-provider"
import { KBar } from "@/components/kbar"
import { DirectionProvider } from "@/components/ui/direction"
import { Toaster } from "@/components/ui/sonner"
import { TooltipProvider } from "@/components/ui/tooltip"
import { QueryClientProvider } from "@/providers/query-client-provider"

export function AppProviders({ children }: { children: React.ReactNode }) {
  return (
    <QueryClientProvider>
      <ThemeProvider attribute="class" defaultTheme="system" enableSystem disableTransitionOnChange>
        <DirectionProvider dir="rtl">
          <KBar>
            <TooltipProvider>{children}</TooltipProvider>
          </KBar>
          <Toaster richColors position="bottom-center" />
        </DirectionProvider>
      </ThemeProvider>
    </QueryClientProvider>
  )
}
