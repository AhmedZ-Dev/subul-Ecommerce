import Link from "next/link"
import { Button } from "@/components/ui/button"
import { messages } from "@/lib/messages.ar"

export function CartEmptyState() {
  return (
    <div className="flex flex-col items-center gap-4 py-16 text-center">
      <p className="text-xl font-semibold">{messages.cart.empty}</p>
      <p className="text-muted-foreground text-sm">{messages.cart.emptyDescription}</p>
      <Button asChild>
        <Link href="/products">{messages.cart.browseProducts}</Link>
      </Button>
    </div>
  )
}
