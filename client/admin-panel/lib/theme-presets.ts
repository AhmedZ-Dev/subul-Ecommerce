export const RADIUS_PRESETS = {
  sm: "0.5rem",
  default: "0.75rem",
  lg: "1rem",
} as const

export type RadiusPreset = keyof typeof RADIUS_PRESETS

export const ACCENT_PRESETS = {
  bronze: {
    light: {
      "--primary": "oklch(0.67 0.08 60)",
      "--primary-foreground": "oklch(0.99 0 0)",
      "--ring": "oklch(0.67 0.08 60)",
      "--sidebar-primary": "oklch(0.67 0.08 60)",
      "--sidebar-primary-foreground": "oklch(0.99 0 0)",
      "--sidebar-ring": "oklch(0.67 0.08 60)",
      "--chart-1": "oklch(0.67 0.08 60)",
    },
    dark: {
      "--primary": "oklch(0.72 0.09 60)",
      "--primary-foreground": "oklch(0.15 0.01 260)",
      "--ring": "oklch(0.72 0.09 60)",
      "--sidebar-primary": "oklch(0.72 0.09 60)",
      "--sidebar-primary-foreground": "oklch(0.15 0.01 260)",
      "--sidebar-ring": "oklch(0.72 0.09 60)",
      "--chart-1": "oklch(0.72 0.09 60)",
    },
  },
  blue: {
    light: {
      "--primary": "oklch(0.55 0.18 250)",
      "--primary-foreground": "oklch(0.99 0 0)",
      "--ring": "oklch(0.55 0.18 250)",
      "--sidebar-primary": "oklch(0.55 0.18 250)",
      "--sidebar-primary-foreground": "oklch(0.99 0 0)",
      "--sidebar-ring": "oklch(0.55 0.18 250)",
      "--chart-1": "oklch(0.55 0.18 250)",
    },
    dark: {
      "--primary": "oklch(0.65 0.15 250)",
      "--primary-foreground": "oklch(0.15 0.01 260)",
      "--ring": "oklch(0.65 0.15 250)",
      "--sidebar-primary": "oklch(0.65 0.15 250)",
      "--sidebar-primary-foreground": "oklch(0.15 0.01 260)",
      "--sidebar-ring": "oklch(0.65 0.15 250)",
      "--chart-1": "oklch(0.65 0.15 250)",
    },
  },
  green: {
    light: {
      "--primary": "oklch(0.52 0.14 145)",
      "--primary-foreground": "oklch(0.99 0 0)",
      "--ring": "oklch(0.52 0.14 145)",
      "--sidebar-primary": "oklch(0.52 0.14 145)",
      "--sidebar-primary-foreground": "oklch(0.99 0 0)",
      "--sidebar-ring": "oklch(0.52 0.14 145)",
      "--chart-1": "oklch(0.52 0.14 145)",
    },
    dark: {
      "--primary": "oklch(0.62 0.12 145)",
      "--primary-foreground": "oklch(0.15 0.01 260)",
      "--ring": "oklch(0.62 0.12 145)",
      "--sidebar-primary": "oklch(0.62 0.12 145)",
      "--sidebar-primary-foreground": "oklch(0.15 0.01 260)",
      "--sidebar-ring": "oklch(0.62 0.12 145)",
      "--chart-1": "oklch(0.62 0.12 145)",
    },
  },
} as const

export type AccentPreset = keyof typeof ACCENT_PRESETS

export const ACCENT_SWATCHES: Record<AccentPreset, string> = {
  bronze: "oklch(0.67 0.08 60)",
  blue: "oklch(0.55 0.18 250)",
  green: "oklch(0.52 0.14 145)",
}

export type ThemePreferences = {
  radius: RadiusPreset
  accent: AccentPreset
}

export const DEFAULT_THEME_PREFERENCES = {
  radius: "default",
  accent: "bronze",
} as const satisfies ThemePreferences

export const THEME_PREFERENCES_STORAGE_KEY = "subul-theme-prefs"

export const OVERRIDDEN_CSS_VARS = [
  "--radius",
  "--primary",
  "--primary-foreground",
  "--ring",
  "--sidebar-primary",
  "--sidebar-primary-foreground",
  "--sidebar-ring",
  "--chart-1",
] as const
