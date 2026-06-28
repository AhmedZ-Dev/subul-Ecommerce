"use client"

import * as React from "react"
import {
  CheckIcon,
  MonitorIcon,
  MoonIcon,
  PaletteIcon,
  PanelRightCloseIcon,
  PanelRightOpenIcon,
  RotateCcwIcon,
  SunIcon,
} from "lucide-react"
import { useTheme } from "next-themes"
import { toast } from "sonner"

import { Badge } from "@/components/ui/badge"
import { Button } from "@/components/ui/button"
import { Input } from "@/components/ui/input"
import { ScrollArea } from "@/components/ui/scroll-area"
import {
  SheetDescription,
  SheetFooter,
  SheetHeader,
  SheetTitle,
} from "@/components/ui/sheet"
import { useSidebar } from "@/components/ui/sidebar"
import { useThemePreferences } from "@/hooks/use-theme-preferences"
import { messages } from "@/lib/messages.ar"
import {
  ACCENT_SWATCHES,
  RADIUS_PRESETS,
  type AccentPreset,
  type RadiusPreset,
} from "@/lib/theme-presets"
import { cn } from "@/lib/utils"

const radiusOptions: { value: RadiusPreset; label: string }[] = [
  { value: "sm", label: messages.theme.radiusSm },
  { value: "default", label: messages.theme.radiusDefault },
  { value: "lg", label: messages.theme.radiusLg },
]

const accentOptions: { value: AccentPreset; label: string }[] = [
  { value: "bronze", label: messages.theme.accentBronze },
  { value: "blue", label: messages.theme.accentBlue },
  { value: "green", label: messages.theme.accentGreen },
]

const modeOptions = [
  { value: "light", label: messages.theme.light, icon: SunIcon },
  { value: "dark", label: messages.theme.dark, icon: MoonIcon },
  { value: "system", label: messages.theme.system, icon: MonitorIcon },
] as const

const sidebarOptions = [
  {
    value: "expanded",
    label: messages.theme.sidebarExpanded,
    icon: PanelRightOpenIcon,
  },
  {
    value: "collapsed",
    label: messages.theme.sidebarCollapsed,
    icon: PanelRightCloseIcon,
  },
] as const

function SettingsSection({
  title,
  description,
  children,
  className,
}: {
  title: string
  description?: string
  children: React.ReactNode
  className?: string
}) {
  return (
    <section
      className={cn(
        "flex flex-col gap-3 rounded-xl border border-border/60 bg-card/50 p-4 shadow-xs",
        className
      )}
    >
      <div className="flex flex-col gap-0.5">
        <h3 className="text-sm font-semibold tracking-tight text-foreground">
          {title}
        </h3>
        {description ? (
          <p className="text-xs leading-relaxed text-muted-foreground">
            {description}
          </p>
        ) : null}
      </div>
      {children}
    </section>
  )
}

function SelectionIndicator({ selected }: { selected: boolean }) {
  if (!selected) {
    return null
  }

  return (
    <span className="absolute top-2 start-2 flex size-4 items-center justify-center rounded-full bg-primary text-primary-foreground shadow-sm">
      <CheckIcon className="size-2.5" strokeWidth={3} />
    </span>
  )
}

function ThemePreview() {
  return (
    <div className="overflow-hidden rounded-xl border border-border/70 bg-background shadow-sm ring-1 ring-foreground/5">
      <div className="border-b bg-muted/30 px-3 py-2">
        <p className="text-[0.65rem] font-medium text-muted-foreground">
          {messages.theme.preview}
        </p>
      </div>
      <div className="flex min-h-32">
        <div className="flex w-[38%] flex-col gap-2 border-e border-border/60 bg-sidebar p-2.5">
          <div className="h-2 w-10 rounded-full bg-sidebar-primary" />
          <div className="mt-1 space-y-1.5">
            <div className="h-1.5 w-full rounded-sm bg-sidebar-primary/70" />
            <div className="h-1.5 w-[80%] rounded-sm bg-sidebar-accent" />
            <div className="h-1.5 w-[60%] rounded-sm bg-sidebar-accent/70" />
          </div>
        </div>
        <div className="flex flex-1 flex-col justify-center gap-2.5 p-3">
          <div className="flex flex-wrap gap-1.5">
            <Button size="xs">{messages.common.button}</Button>
            <Button size="xs" variant="outline">
              {messages.theme.previewSecondary}
            </Button>
          </div>
          <Input
            className="h-7 text-xs"
            placeholder={messages.theme.previewInput}
            readOnly
          />
          <Badge className="w-fit">{messages.theme.previewBadge}</Badge>
        </div>
      </div>
    </div>
  )
}

export function ThemeSettingsPanel() {
  const { theme, setTheme } = useTheme()
  const { prefs, mounted, setRadius, setAccent, reset } = useThemePreferences()
  const {
    open,
    openMobile,
    isMobile,
    setOpen,
    setOpenMobile,
  } = useSidebar()

  const isSidebarOpen = isMobile ? openMobile : open

  const setSidebarOpen = React.useCallback(
    (value: boolean) => {
      if (isMobile) {
        setOpenMobile(value)
      } else {
        setOpen(value)
      }
    },
    [isMobile, setOpen, setOpenMobile]
  )

  const handleReset = () => {
    reset()
    toast.success(messages.theme.resetSuccess)
  }

  return (
    <>
      <SheetHeader className="relative gap-3 border-b border-border/60 bg-linear-to-b from-primary/8 via-primary/3 to-transparent px-5 pb-5 pt-6">
        <div className="flex size-11 items-center justify-center rounded-2xl bg-primary/12 text-primary shadow-sm ring-1 ring-primary/15">
          <PaletteIcon className="size-5" />
        </div>
        <div className="flex flex-col gap-1 pe-8">
          <SheetTitle className="text-lg font-semibold tracking-tight">
            {messages.theme.panelTitle}
          </SheetTitle>
          <SheetDescription className="text-xs leading-relaxed">
            {messages.theme.panelDescription}
          </SheetDescription>
        </div>
      </SheetHeader>

      <ScrollArea className="min-h-0 flex-1">
        <div
          className={cn(
            "flex flex-col gap-4 p-4",
            !mounted && "pointer-events-none opacity-60"
          )}
        >
          <SettingsSection title={messages.theme.appearance}>
            <div className="grid grid-cols-3 gap-2">
              {modeOptions.map((option) => {
                const selected = mounted && theme === option.value
                const Icon = option.icon

                return (
                  <button
                    key={option.value}
                    type="button"
                    aria-label={option.label}
                    aria-pressed={selected}
                    onClick={() => setTheme(option.value)}
                    className={cn(
                      "relative flex flex-col items-center gap-2.5 rounded-xl border-2 px-2 py-3.5 transition-all duration-200",
                      selected
                        ? "border-primary bg-primary/6 shadow-sm"
                        : "border-transparent bg-muted/40 hover:border-border/80 hover:bg-muted/70"
                    )}
                  >
                    <SelectionIndicator selected={selected} />
                    <span
                      className={cn(
                        "flex size-9 items-center justify-center rounded-full transition-colors",
                        selected
                          ? "bg-primary text-primary-foreground shadow-sm"
                          : "bg-background text-muted-foreground ring-1 ring-border/60"
                      )}
                    >
                      <Icon className="size-4" />
                    </span>
                    <span
                      className={cn(
                        "text-xs font-medium",
                        selected ? "text-foreground" : "text-muted-foreground"
                      )}
                    >
                      {option.label}
                    </span>
                  </button>
                )
              })}
            </div>
          </SettingsSection>

          <SettingsSection
            title={messages.theme.sidebar}
            description={messages.theme.sidebarDescription}
          >
            <div className="grid grid-cols-2 gap-2">
              {sidebarOptions.map((option) => {
                const selected =
                  mounted &&
                  (option.value === "expanded"
                    ? isSidebarOpen
                    : !isSidebarOpen)
                const Icon = option.icon

                return (
                  <button
                    key={option.value}
                    type="button"
                    aria-label={option.label}
                    aria-pressed={selected}
                    onClick={() =>
                      setSidebarOpen(option.value === "expanded")
                    }
                    className={cn(
                      "relative flex flex-col items-center gap-2.5 rounded-xl border-2 px-2 py-3.5 transition-all duration-200",
                      selected
                        ? "border-primary bg-primary/6 shadow-sm"
                        : "border-transparent bg-muted/40 hover:border-border/80 hover:bg-muted/70"
                    )}
                  >
                    <SelectionIndicator selected={selected} />
                    <span
                      className={cn(
                        "flex size-9 items-center justify-center rounded-full transition-colors",
                        selected
                          ? "bg-primary text-primary-foreground shadow-sm"
                          : "bg-background text-muted-foreground ring-1 ring-border/60"
                      )}
                    >
                      <Icon className="size-4" />
                    </span>
                    <span
                      className={cn(
                        "text-xs font-medium",
                        selected ? "text-foreground" : "text-muted-foreground"
                      )}
                    >
                      {option.label}
                    </span>
                  </button>
                )
              })}
            </div>
          </SettingsSection>

          <SettingsSection
            title={messages.theme.radius}
            description={messages.theme.radiusDescription}
          >
            <div className="grid grid-cols-3 gap-2">
              {radiusOptions.map((option) => {
                const selected = mounted && prefs.radius === option.value

                return (
                  <button
                    key={option.value}
                    type="button"
                    aria-label={option.label}
                    aria-pressed={selected}
                    onClick={() => setRadius(option.value)}
                    className={cn(
                      "relative flex flex-col items-center gap-2.5 rounded-xl border-2 px-2 py-3 transition-all duration-200",
                      selected
                        ? "border-primary bg-primary/6 shadow-sm"
                        : "border-transparent bg-muted/40 hover:border-border/80 hover:bg-muted/70"
                    )}
                  >
                    <SelectionIndicator selected={selected} />
                    <div className="flex size-12 items-center justify-center rounded-lg bg-background ring-1 ring-border/50">
                      <div
                        className="size-8 border-2 border-muted-foreground/20 bg-linear-to-br from-muted to-muted/50 shadow-inner"
                        style={{
                          borderRadius: RADIUS_PRESETS[option.value],
                        }}
                      />
                    </div>
                    <span
                      className={cn(
                        "text-xs font-medium",
                        selected ? "text-foreground" : "text-muted-foreground"
                      )}
                    >
                      {option.label}
                    </span>
                  </button>
                )
              })}
            </div>
          </SettingsSection>

          <SettingsSection
            title={messages.theme.accent}
            description={messages.theme.accentDescription}
          >
            <div className="grid grid-cols-3 gap-2">
              {accentOptions.map((option) => {
                const selected = mounted && prefs.accent === option.value

                return (
                  <button
                    key={option.value}
                    type="button"
                    aria-label={option.label}
                    aria-pressed={selected}
                    onClick={() => setAccent(option.value)}
                    className={cn(
                      "relative flex flex-col items-center gap-2 rounded-xl border-2 px-2 py-3 transition-all duration-200",
                      selected
                        ? "border-primary bg-primary/6 shadow-sm"
                        : "border-transparent bg-muted/40 hover:border-border/80 hover:bg-muted/70"
                    )}
                  >
                    <SelectionIndicator selected={selected} />
                    <div className="relative size-11 overflow-hidden rounded-full ring-1 ring-border/60">
                      <div className="absolute inset-0 grid grid-cols-2">
                        <div className="bg-[oklch(0.97_0.01_85)]" />
                        <div className="bg-[oklch(0.17_0.01_260)]" />
                      </div>
                      <div
                        className="absolute inset-2 rounded-full shadow-md"
                        style={{
                          backgroundColor: ACCENT_SWATCHES[option.value],
                        }}
                      />
                    </div>
                    <span
                      className={cn(
                        "text-xs font-medium",
                        selected ? "text-foreground" : "text-muted-foreground"
                      )}
                    >
                      {option.label}
                    </span>
                  </button>
                )
              })}
            </div>
          </SettingsSection>

          <SettingsSection
            title={messages.theme.preview}
            description={messages.theme.previewDescription}
            className="gap-2.5"
          >
            <ThemePreview />
          </SettingsSection>
        </div>
      </ScrollArea>

      <SheetFooter className="relative border-t border-border/60 bg-linear-to-t from-muted/30 to-transparent px-4 pb-5 pt-4">
        <Button
          type="button"
          variant="outline"
          className="h-10 w-full gap-2 border-border/80 bg-background/80 shadow-xs hover:bg-muted/50"
          onClick={handleReset}
        >
          <RotateCcwIcon className="size-3.5" />
          {messages.theme.reset}
        </Button>
      </SheetFooter>
    </>
  )
}
