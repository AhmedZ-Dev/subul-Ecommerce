import { messages } from "@/lib/messages.ar"
import { SectionHeader } from "./section-header"

export function BrandStrip() {
  const brands = messages.brands.items

  return (
    <section className="mb-14 md:mb-16" aria-label={messages.brands.title}>
      <SectionHeader
        title={messages.brands.title}
        description={messages.brands.description}
      />

      <div className="grid grid-cols-2 gap-3 sm:grid-cols-4 lg:grid-cols-8">
        {brands.map((brand) => (
          <div
            key={brand}
            className="group bg-card flex h-16 items-center justify-center rounded-2xl ring-1 ring-foreground/8 transition-[transform,box-shadow,ring-color] duration-300 hover:-translate-y-0.5 hover:shadow-md hover:ring-primary/20"
          >
            <span className="text-muted-foreground group-hover:text-foreground text-sm font-semibold tracking-wide grayscale transition-[filter,color] duration-300 group-hover:grayscale-0">
              {brand}
            </span>
          </div>
        ))}
      </div>
    </section>
  )
}
