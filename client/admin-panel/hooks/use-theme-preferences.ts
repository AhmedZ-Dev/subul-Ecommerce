"use client"

import * as React from "react"
import { useTheme } from "next-themes"

import {
  applyThemePreferences,
  readThemePreferences,
  resetThemePreferences,
  writeThemePreferences,
  type ThemeMode,
} from "@/lib/theme-preferences"
import {
  DEFAULT_THEME_PREFERENCES,
  type AccentPreset,
  type RadiusPreset,
  type ThemePreferences,
} from "@/lib/theme-presets"

function resolveThemeMode(resolvedTheme: string | undefined): ThemeMode {
  return resolvedTheme === "dark" ? "dark" : "light"
}

export function useThemePreferences() {
  const { resolvedTheme } = useTheme()
  const [prefs, setPrefs] = React.useState<ThemePreferences>(
    DEFAULT_THEME_PREFERENCES
  )
  const [mounted, setMounted] = React.useState(false)

  React.useEffect(() => {
    setPrefs(readThemePreferences())
    setMounted(true)
  }, [])

  React.useEffect(() => {
    if (!mounted) {
      return
    }

    applyThemePreferences(prefs, resolveThemeMode(resolvedTheme))
  }, [prefs, resolvedTheme, mounted])

  const setRadius = React.useCallback((radius: RadiusPreset) => {
    setPrefs((current) => {
      const next = { ...current, radius }
      writeThemePreferences(next)
      return next
    })
  }, [])

  const setAccent = React.useCallback((accent: AccentPreset) => {
    setPrefs((current) => {
      const next = { ...current, accent }
      writeThemePreferences(next)
      return next
    })
  }, [])

  const reset = React.useCallback(() => {
    const defaults = resetThemePreferences()
    setPrefs(defaults)
  }, [])

  return {
    prefs,
    mounted,
    setRadius,
    setAccent,
    reset,
  }
}
