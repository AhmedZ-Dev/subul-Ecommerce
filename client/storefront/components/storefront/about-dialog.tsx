"use client"

import { Button } from "@/components/ui/button"
import {
  Dialog,
  DialogClose,
  DialogContent,
  DialogDescription,
  DialogFooter,
  DialogHeader,
  DialogTitle,
  DialogTrigger,
} from "@/components/ui/dialog"
import { messages } from "@/lib/messages.ar"

/** Footer "من نحن" entry — opens the company blurb in place instead of a page. */
export function AboutDialog() {
  return (
    <Dialog>
      <DialogTrigger asChild>
        <button
          type="button"
          className="text-muted-foreground hover:text-primary flex min-h-11 items-center text-start transition-colors"
        >
          {messages.footer.about}
        </button>
      </DialogTrigger>
      <DialogContent className="max-h-[85svh] overflow-y-auto sm:max-w-lg">
        <DialogHeader>
          <DialogTitle>{messages.about.title}</DialogTitle>
          <DialogDescription>{messages.about.subtitle}</DialogDescription>
        </DialogHeader>

        <div className="flex flex-col gap-4">
          <div className="text-muted-foreground flex flex-col gap-3 text-sm leading-relaxed">
            {messages.about.body.map((paragraph) => (
              <p key={paragraph}>{paragraph}</p>
            ))}
          </div>

          <dl className="grid gap-2 sm:grid-cols-2">
            {messages.about.facts.map((fact) => (
              <div
                key={fact.label}
                className="bg-muted/40 rounded-xl border border-foreground/8 px-3 py-2.5"
              >
                <dt className="text-muted-foreground text-xs">{fact.label}</dt>
                <dd className="mt-0.5 text-sm font-semibold">{fact.value}</dd>
              </div>
            ))}
          </dl>
        </div>

        <DialogFooter>
          <DialogClose asChild>
            <Button variant="outline" className="h-11">
              {messages.about.close}
            </Button>
          </DialogClose>
        </DialogFooter>
      </DialogContent>
    </Dialog>
  )
}
