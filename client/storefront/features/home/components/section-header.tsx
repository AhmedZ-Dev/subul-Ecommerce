import Link from "next/link"
import { ArrowLeft } from "lucide-react"
import { messages } from "@/lib/messages.ar"

interface SectionHeaderProps {
  title: string
  description?: string
  viewAllHref?: string
}

export function SectionHeader({ title, description, viewAllHref }: SectionHeaderProps) {
  return (
    <div className="mb-6 flex items-end justify-between gap-4 md:mb-8">
      <div className="min-w-0 max-w-2xl">
        <h2 className="text-xl font-bold tracking-tight md:text-2xl lg:text-[1.75rem]">
          {title}
        </h2>
        {description && (
          <p className="text-muted-foreground mt-1.5 text-sm leading-relaxed md:text-[0.9375rem]">
            {description}
          </p>
        )}
      </div>
      {viewAllHref && (
        <Link
          href={viewAllHref}
          className="text-primary hover:text-primary/80 group inline-flex shrink-0 items-center gap-1 text-sm font-semibold transition-colors"
        >
          {messages.storefront.viewAll}
          <ArrowLeft
            className="size-4 transition-transform group-hover:-translate-x-0.5"
            aria-hidden
          />
        </Link>
      )}
    </div>
  )
}
