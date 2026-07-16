"use client"

import * as React from "react"
import { ThemeProvider as NextThemesProvider, useTheme } from "next-themes"

import {
  applyThemePreferences,
  readThemePreferences,
} from "@/lib/theme-preferences"

function ThemeProvider({
  children,
  ...props
}: React.ComponentProps<typeof NextThemesProvider>) {
  return (
    <NextThemesProvider
      attribute="class"
      defaultTheme="light"
      storageKey="subul-storefront-theme"
      disableTransitionOnChange
      {...props}
    >
      <ThemePreferencesApplier />
      {children}
    </NextThemesProvider>
  )
}

function ThemePreferencesApplier() {
  const { resolvedTheme } = useTheme()

  React.useEffect(() => {
    const prefs = readThemePreferences()
    applyThemePreferences(prefs, resolvedTheme === "dark" ? "dark" : "light")
  }, [resolvedTheme])

  return null
}

export { ThemeProvider }
