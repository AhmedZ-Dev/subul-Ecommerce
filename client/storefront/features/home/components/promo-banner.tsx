import Link from "next/link"
import Image from "next/image"
import { ArrowLeft } from "lucide-react"
import { Button } from "@/components/ui/button"
import { messages } from "@/lib/messages.ar"

interface PromoBannerProps {
  title?: string
  subtitle?: string
  cta?: string
  href?: string
  image?: string
}

export function PromoBanner({
  title = messages.promo.title,
  subtitle = messages.promo.subtitle,
  cta = messages.promo.cta,
  href = messages.promo.href,
  image = messages.promo.image,
}: PromoBannerProps) {
  return (
    <section
      className="relative mb-14 overflow-hidden rounded-2xl md:mb-16 md:rounded-3xl"
      aria-label={title}
    >
      <div className="relative aspect-16/10 min-h-56 w-full sm:aspect-21/9 sm:min-h-64">
        <Image
          src={image}
          alt=""
          fill
          className="object-cover"
          sizes="100vw"
        />
        <div
          className="pointer-events-none absolute inset-0 bg-linear-to-t from-black/80 via-black/45 to-black/20 md:bg-linear-to-l md:from-black/75 md:via-black/40 md:to-black/15"
          aria-hidden
        />
        <div
          className="absolute inset-0 flex items-end md:items-center"
          dir="rtl"
        >
          <div className="flex max-w-xl flex-col gap-3 px-5 pb-8 pt-10 sm:px-8 md:px-10 lg:px-12">
            <h2 className="text-balance text-2xl font-bold text-white md:text-3xl">
              {title}
            </h2>
            <p className="max-w-md text-pretty text-sm text-white/85 md:text-base">
              {subtitle}
            </p>
            <div className="mt-1">
              <Button
                asChild
                size="lg"
                className="h-11 rounded-full bg-white px-5 font-semibold text-neutral-950 hover:bg-white/95"
              >
                <Link href={href}>
                  {cta}
                  <ArrowLeft className="ms-2 size-4" aria-hidden />
                </Link>
              </Button>
            </div>
          </div>
        </div>
      </div>
    </section>
  )
}
