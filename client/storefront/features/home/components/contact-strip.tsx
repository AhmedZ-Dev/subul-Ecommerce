import { Mail, MessageCircle, Phone } from "lucide-react"
import { Button } from "@/components/ui/button"
import { messages } from "@/lib/messages.ar"

export function ContactStrip() {
  const { title, description, primaryCta, whatsapp, phone, email, label } = messages.contact

  return (
    <section
      className="mb-6 overflow-hidden rounded-2xl border border-foreground/8 md:mb-8"
      aria-label={label}
    >
      <div className="grid lg:grid-cols-[1.4fr_1fr]">
        <div className="hero-gradient relative flex flex-col justify-center gap-4 px-5 py-8 sm:px-8 sm:py-10 md:px-10">
          <div className="relative z-10 max-w-lg">
            <h2 className="text-primary-foreground text-2xl font-bold md:text-3xl">
              {title}
            </h2>
            <p className="text-primary-foreground/85 mt-2 text-sm leading-relaxed md:text-base">
              {description}
            </p>
            <Button
              asChild
              size="lg"
              className="mt-5 h-11 rounded-full bg-white px-5 font-semibold text-neutral-950 hover:bg-white/95"
            >
              <a href={whatsapp.href} target="_blank" rel="noopener noreferrer">
                <MessageCircle className="size-4" aria-hidden />
                {primaryCta}
              </a>
            </Button>
          </div>
          <div
            className="pointer-events-none absolute -end-10 -top-10 size-40 rounded-full bg-white/10"
            aria-hidden
          />
        </div>

        <div className="bg-card flex flex-col justify-center gap-3 p-5 sm:p-6 md:p-8">
          <ContactLink
            href={whatsapp.href}
            icon={MessageCircle}
            label={whatsapp.label}
            value={whatsapp.value}
            external
          />
          <ContactLink
            href={phone.href}
            icon={Phone}
            label={phone.label}
            value={phone.value}
          />
          <ContactLink
            href={email.href}
            icon={Mail}
            label={email.label}
            value={email.value}
          />
        </div>
      </div>
    </section>
  )
}

function ContactLink({
  href,
  icon: Icon,
  label,
  value,
  external = false,
}: {
  href: string
  icon: typeof Phone
  label: string
  value: string
  external?: boolean
}) {
  return (
    <a
      href={href}
      {...(external ? { target: "_blank", rel: "noopener noreferrer" } : {})}
      className="hover:border-primary/30 hover:bg-muted/40 flex items-center gap-3 rounded-xl border border-transparent px-3 py-2.5 transition-colors"
    >
      <span className="bg-primary/10 text-primary flex size-10 shrink-0 items-center justify-center rounded-xl">
        <Icon className="size-4" aria-hidden />
      </span>
      <span className="min-w-0">
        <span className="block text-sm font-semibold">{label}</span>
        <span className="text-muted-foreground block truncate text-sm" dir="ltr">
          {value}
        </span>
      </span>
    </a>
  )
}
