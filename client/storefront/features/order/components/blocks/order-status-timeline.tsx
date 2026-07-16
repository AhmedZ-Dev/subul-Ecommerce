import { Badge } from "@/components/ui/badge"
import { cn } from "@/lib/utils"
import { messages } from "@/lib/messages.ar"

const STEPS = [
  { key: "pending", label: messages.order.timeline.placed },
  { key: "confirmed", label: messages.order.timeline.confirmed },
  { key: "processing", label: messages.order.timeline.processing },
  { key: "shipped", label: messages.order.timeline.shipped },
  { key: "delivered", label: messages.order.timeline.delivered },
] as const

const STATUS_ORDER = ["pending", "confirmed", "processing", "shipped", "out_for_delivery", "delivered"]

interface OrderStatusTimelineProps {
  status: string
}

export function OrderStatusTimeline({ status }: OrderStatusTimelineProps) {
  const normalized = status.toLowerCase()
  const currentIndex = STATUS_ORDER.indexOf(normalized)

  return (
    <div className="flex flex-col gap-4">
      <h3 className="font-semibold">{messages.order.timeline.title}</h3>
      <div className="flex flex-col gap-3">
        {STEPS.map((step, i) => {
          const stepIndex = STATUS_ORDER.indexOf(step.key)
          const isActive = currentIndex >= stepIndex
          const isCurrent = normalized === step.key

          return (
            <div key={step.key} className="flex items-center gap-3">
              <div
                className={cn(
                  "size-3 rounded-full",
                  isActive ? "bg-primary" : "bg-muted",
                  isCurrent && "ring-primary ring-2 ring-offset-2",
                )}
              />
              <span className={cn("text-sm", isActive ? "font-medium" : "text-muted-foreground")}>
                {step.label}
              </span>
              {isCurrent && (
                <Badge variant="secondary">
                  {(messages.order.status as Record<string, string>)[normalized] ?? status}
                </Badge>
              )}
            </div>
          )
        })}
      </div>
    </div>
  )
}
