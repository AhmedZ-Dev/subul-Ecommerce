import { cn } from "@/lib/utils"

interface PageContainerProps {
  children: React.ReactNode
  className?: string
}

export function PageContainer({ children, className }: PageContainerProps) {
  return (
    <div className={cn("container mx-auto flex flex-1 flex-col px-4 py-6 md:px-6", className)}>
      {children}
    </div>
  )
}
