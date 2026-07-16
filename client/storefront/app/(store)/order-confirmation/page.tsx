"use client"

import Link from "next/link"
import { useSearchParams } from "next/navigation"
import { Suspense } from "react"
import { CheckCircle2, Home, PackageSearch } from "lucide-react"
import { Button } from "@/components/ui/button"
import { PageContainer } from "@/components/layout/page-container"
import { messages } from "@/lib/messages.ar"

function OrderConfirmationContent() {
  const searchParams = useSearchParams()
  const orderNumber = searchParams.get("orderNumber")

  return (
    <PageContainer>
      <div className="mx-auto flex max-w-lg flex-col items-center gap-6 py-14 text-center md:py-20">
        <div className="bg-primary/10 text-primary flex size-20 items-center justify-center rounded-full">
          <CheckCircle2 className="size-11" aria-hidden />
        </div>

        <div className="space-y-2">
          <h1 className="text-2xl font-bold tracking-tight md:text-3xl">
            {messages.checkout.success}
          </h1>
          <p className="text-muted-foreground text-sm leading-relaxed md:text-base">
            {messages.checkout.successDescription}
          </p>
        </div>

        {orderNumber && (
          <div className="bg-card w-full rounded-2xl border border-foreground/8 p-5 shadow-xs">
            <p className="text-muted-foreground text-sm">{messages.checkout.orderNumber}</p>
            <p className="mt-1 font-mono text-xl font-bold tracking-wide" dir="ltr">
              {orderNumber}
            </p>
            <p className="text-muted-foreground mt-3 text-xs leading-relaxed">
              {messages.checkout.successHint}
            </p>
          </div>
        )}

        <div className="bg-muted/50 w-full rounded-2xl p-4 text-start text-sm leading-relaxed">
          <p className="font-semibold">{messages.checkout.paymentCod}</p>
          <p className="text-muted-foreground mt-1">{messages.checkout.paymentCodDescription}</p>
        </div>

        <div className="flex w-full flex-col gap-3 sm:flex-row sm:justify-center">
          <Button asChild size="lg" className="h-11 rounded-full">
            <Link href="/orders/track">
              <PackageSearch className="size-4" aria-hidden />
              {messages.checkout.trackYourOrder}
            </Link>
          </Button>
          <Button variant="outline" asChild size="lg" className="h-11 rounded-full">
            <Link href="/">
              <Home className="size-4" aria-hidden />
              {messages.checkout.backToHome}
            </Link>
          </Button>
        </div>
      </div>
    </PageContainer>
  )
}

export default function OrderConfirmationPage() {
  return (
    <Suspense fallback={<div className="p-8 text-center">{messages.common.loading}</div>}>
      <OrderConfirmationContent />
    </Suspense>
  )
}
