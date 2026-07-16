import Link from "next/link"
import { Button } from "@/components/ui/button"

export default function NotFound() {
  return (
    <div
      dir="rtl"
      className="flex min-h-svh flex-col items-center justify-center gap-6 text-center"
    >
      <p className="text-muted-foreground text-sm font-medium tracking-widest uppercase">
        404
      </p>
      <h1 className="text-3xl font-bold tracking-tight">
        الصفحة غير موجودة
      </h1>
      <p className="text-muted-foreground max-w-sm text-sm">
        عذرًا، لم نتمكن من العثور على الصفحة التي تبحث عنها. ربما تم نقلها أو
        حذفها.
      </p>
      <Button asChild>
        <Link href="/">العودة إلى الرئيسية</Link>
      </Button>
    </div>
  )
}
