import Link from "next/link"
import { Card, CardContent } from "@/components/ui/card"
import { getCategoryName, messages } from "@/lib/messages.ar"
import type { CategoryListItem } from "../../types"

interface CategoryCardProps {
  category: CategoryListItem
}

export function CategoryCard({ category }: CategoryCardProps) {
  const name = getCategoryName(category.nameAr, category.nameEn)

  return (
    <Link href={`/categories/${category.slug}`}>
      <Card className="hover:border-primary transition-colors">
        <CardContent className="flex flex-col items-center gap-2 p-6 text-center">
          <div className="bg-muted flex size-16 items-center justify-center rounded-full text-2xl">
            {name.charAt(0)}
          </div>
          <p className="font-medium">{name}</p>
        </CardContent>
      </Card>
    </Link>
  )
}
