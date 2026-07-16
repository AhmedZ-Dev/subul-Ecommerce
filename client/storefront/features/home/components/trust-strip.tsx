import { Headphones, PackageCheck, ShieldCheck, Truck } from "lucide-react"
import { messages } from "@/lib/messages.ar"

const ICONS = [Truck, ShieldCheck, PackageCheck, Headphones] as const

export function TrustStrip() {
  const items = messages.trust.items

  return (
    <section
      className="mb-14 md:mb-16"
      aria-label={messages.trust.label}
    >
      <div className="grid grid-cols-2 gap-3 rounded-2xl border border-foreground/8 bg-card p-4 shadow-xs sm:gap-4 sm:p-5 lg:grid-cols-4 lg:p-6">
        {items.map((item, index) => {
          const Icon = ICONS[index] ?? Truck
          return (
            <div
              key={item.title}
              className="flex flex-col gap-2 rounded-xl p-2 sm:flex-row sm:items-start sm:gap-3 sm:p-3"
            >
              <span className="bg-primary/10 text-primary flex size-10 shrink-0 items-center justify-center rounded-xl">
                <Icon className="size-5" aria-hidden />
              </span>
              <div className="min-w-0">
                <p className="text-sm font-semibold sm:text-[0.9375rem]">{item.title}</p>
                <p className="text-muted-foreground mt-0.5 text-xs leading-relaxed sm:text-sm">
                  {item.description}
                </p>
              </div>
            </div>
          )
        })}
      </div>
    </section>
  )
}
