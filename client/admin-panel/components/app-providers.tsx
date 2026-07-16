"use client"

import { SessionProvider } from 'next-auth/react';
import { ThemeProvider } from "@/components/theme-provider"
import { KBar } from "@/components/kbar"
import { DirectionProvider } from "@/components/ui/direction"
import { Toaster } from "@/components/ui/sonner"
import { TooltipProvider } from "@/components/ui/tooltip"
import { QueryClientProvider } from "@/providers/query-client-provider"
import { TokenSync } from "@/features/auth"

export function AppProviders({ children }: { children: React.ReactNode }) {
  return (
    <SessionProvider>
      <QueryClientProvider>
        <ThemeProvider attribute="class" defaultTheme="system" enableSystem disableTransitionOnChange>
          <DirectionProvider dir="rtl">
            <TokenSync />
            <KBar>
              <TooltipProvider>{children}</TooltipProvider>
            </KBar>
            <Toaster richColors position="bottom-center" />
          </DirectionProvider>
        </ThemeProvider>
      </QueryClientProvider>
    </SessionProvider>
  )
}
