"use client"

import { Settings2Icon } from "lucide-react"

import { ThemeSettingsPanel } from "@/components/theme-settings-panel"
import { Button } from "@/components/ui/button"
import { Sheet, SheetContent, SheetTrigger } from "@/components/ui/sheet"
import {
  Tooltip,
  TooltipContent,
  TooltipTrigger,
} from "@/components/ui/tooltip"
import { messages } from "@/lib/messages.ar"

export function ThemeSettingsMenu() {
  return (
    <Sheet>
      <Tooltip>
        <TooltipTrigger asChild>
          <SheetTrigger asChild>
            <Button
              variant="outline"
              size="icon"
              className="transition-colors hover:bg-primary/5"
            >
              <Settings2Icon className="size-[1.15rem]" />
              <span className="sr-only">{messages.theme.panelTitle}</span>
            </Button>
          </SheetTrigger>
        </TooltipTrigger>
        <TooltipContent side="bottom">{messages.theme.panelTitle}</TooltipContent>
      </Tooltip>
      <SheetContent
        side="left"
        className="flex w-full flex-col gap-0 border-e border-border/60 bg-popover p-0 shadow-2xl sm:max-w-[22rem]"
      >
        <ThemeSettingsPanel />
      </SheetContent>
    </Sheet>
  )
}
