import {
  ACCENT_PRESETS,
  DEFAULT_THEME_PREFERENCES,
  OVERRIDDEN_CSS_VARS,
  RADIUS_PRESETS,
  THEME_PREFERENCES_STORAGE_KEY,
  type AccentPreset,
  type RadiusPreset,
  type ThemePreferences,
} from "@/lib/theme-presets"

export type ThemeMode = "light" | "dark"

function isRadiusPreset(value: unknown): value is RadiusPreset {
  return value === "sm" || value === "default" || value === "lg"
}

function isAccentPreset(value: unknown): value is AccentPreset {
  return value === "bronze" || value === "blue" || value === "green"
}

export function readThemePreferences(): ThemePreferences {
  if (typeof window === "undefined") {
    return { ...DEFAULT_THEME_PREFERENCES }
  }

  try {
    const raw = localStorage.getItem(THEME_PREFERENCES_STORAGE_KEY)
    if (!raw) {
      return { ...DEFAULT_THEME_PREFERENCES }
    }

    const parsed = JSON.parse(raw) as Partial<ThemePreferences>

    return {
      radius: isRadiusPreset(parsed.radius)
        ? parsed.radius
        : DEFAULT_THEME_PREFERENCES.radius,
      accent: isAccentPreset(parsed.accent)
        ? parsed.accent
        : DEFAULT_THEME_PREFERENCES.accent,
    }
  } catch {
    return { ...DEFAULT_THEME_PREFERENCES }
  }
}

export function writeThemePreferences(prefs: ThemePreferences) {
  if (typeof window === "undefined") {
    return
  }

  localStorage.setItem(THEME_PREFERENCES_STORAGE_KEY, JSON.stringify(prefs))
}

export function clearThemePreferenceOverrides() {
  if (typeof document === "undefined") {
    return
  }

  for (const cssVar of OVERRIDDEN_CSS_VARS) {
    document.documentElement.style.removeProperty(cssVar)
  }
}

export function applyThemePreferences(
  prefs: ThemePreferences,
  mode: ThemeMode
) {
  if (typeof document === "undefined") {
    return
  }

  if (prefs.radius === DEFAULT_THEME_PREFERENCES.radius) {
    document.documentElement.style.removeProperty("--radius")
  } else {
    document.documentElement.style.setProperty(
      "--radius",
      RADIUS_PRESETS[prefs.radius]
    )
  }

  const accentTokens = ACCENT_PRESETS[prefs.accent][mode]

  if (prefs.accent === DEFAULT_THEME_PREFERENCES.accent) {
    for (const cssVar of OVERRIDDEN_CSS_VARS) {
      if (cssVar === "--radius") {
        continue
      }
      document.documentElement.style.removeProperty(cssVar)
    }
    return
  }

  for (const [cssVar, value] of Object.entries(accentTokens)) {
    document.documentElement.style.setProperty(cssVar, value)
  }
}

export function resetThemePreferences() {
  clearThemePreferenceOverrides()
  writeThemePreferences({ ...DEFAULT_THEME_PREFERENCES })
  return { ...DEFAULT_THEME_PREFERENCES }
}
