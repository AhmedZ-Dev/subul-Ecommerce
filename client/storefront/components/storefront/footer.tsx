import Link from "next/link"
import { Package, Truck } from "lucide-react"
import { messages } from "@/lib/messages.ar"

export function StorefrontFooter() {
  const year = new Date().getFullYear()

  return (
    <footer
      className="mt-auto border-t"
      style={{ backgroundColor: "var(--footer-bg)" }}
    >
      <div className="container mx-auto px-4 py-10 md:px-6">
        <div className="grid gap-8 md:grid-cols-3">
          <div>
            <p className="text-primary text-lg font-bold">{messages.common.companyName}</p>
            <p className="text-muted-foreground mt-2 text-sm leading-relaxed">
              {messages.storefront.description}
            </p>
            <div className="text-muted-foreground mt-4 flex flex-col gap-2 text-sm">
              <span className="flex items-center gap-2">
                <Truck className="text-primary size-4 shrink-0" aria-hidden />
                توصيل سريع لجميع المناطق
              </span>
              <span className="flex items-center gap-2">
                <Package className="text-primary size-4 shrink-0" aria-hidden />
                منتجات أصلية مضمونة
              </span>
            </div>
          </div>

          <nav className="flex flex-col gap-1 text-sm">
            <p className="mb-1 font-semibold">{messages.footer.quickLinks}</p>
            <Link
              href="/products"
              className="text-muted-foreground hover:text-primary flex min-h-11 items-center transition-colors"
            >
              {messages.footer.allProducts}
            </Link>
            <Link
              href="/orders/track"
              className="text-muted-foreground hover:text-primary flex min-h-11 items-center transition-colors"
            >
              {messages.footer.trackOrder}
            </Link>
          </nav>

          <div className="flex flex-col justify-end">
            <p className="text-muted-foreground text-sm">
              {messages.footer.copyright(year)}
            </p>
          </div>
        </div>
      </div>
    </footer>
  )
}
