"use client"

import { useEffect } from "react"
import { Button } from "@/components/ui/button"

export default function GlobalError({
  error,
  reset,
}: {
  error: Error & { digest?: string }
  reset: () => void
}) {
  useEffect(() => {
    console.error(error)
  }, [error])

  return (
    <div
      dir="rtl"
      className="flex min-h-svh flex-col items-center justify-center gap-6 text-center"
    >
      <p className="text-destructive text-sm font-medium tracking-widest uppercase">
        خطأ
      </p>
      <h1 className="text-3xl font-bold tracking-tight">حدث خطأ غير متوقع</h1>
      <p className="text-muted-foreground max-w-sm text-sm">
        {error.message ?? "حدث خطأ أثناء تحميل الصفحة. يرجى المحاولة مرة أخرى."}
      </p>
      {error.digest && (
        <p className="text-muted-foreground font-mono text-xs">
          معرّف الخطأ: {error.digest}
        </p>
      )}
      <div className="flex gap-3">
        <Button onClick={reset}>إعادة المحاولة</Button>
        <Button variant="outline" asChild>
          <a href="/dashboard">العودة إلى لوحة التحكم</a>
        </Button>
      </div>
    </div>
  )
}
