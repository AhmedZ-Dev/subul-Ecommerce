import Link from "next/link"
import { ArrowLeft } from "lucide-react"
import { Button } from "@/components/ui/button"
import { messages } from "@/lib/messages.ar"

export function HeroBanner() {
  return (
    <section
      className="hero-gradient relative mb-12 overflow-hidden rounded-2xl px-6 py-16 text-center md:px-12 md:py-20"
      aria-label={messages.storefront.heroTitle}
    >
      <div className="relative z-10 mx-auto max-w-2xl">
        <p className="text-primary-foreground/80 mb-3 text-sm font-medium tracking-wide">
          {messages.common.companyName}
        </p>
        <h1 className="text-primary-foreground text-3xl font-bold md:text-5xl">
          {messages.storefront.heroTitle}
        </h1>
        <p className="text-primary-foreground/90 mx-auto mt-4 max-w-lg text-base md:text-lg">
          {messages.storefront.heroDescription}
        </p>
        <Button
          asChild
          size="lg"
          className="bg-background text-foreground hover:bg-background/90 mt-8 shadow-md"
        >
          <Link href="/products">
            {messages.storefront.viewAll}
            <ArrowLeft className="ms-2 size-4" aria-hidden />
          </Link>
        </Button>
      </div>
      <div
        className="pointer-events-none absolute -end-16 -top-16 size-64 rounded-full bg-white/10"
        aria-hidden
      />
      <div
        className="pointer-events-none absolute -bottom-20 -start-10 size-48 rounded-full bg-black/10"
        aria-hidden
      />
    </section>
  )
}
