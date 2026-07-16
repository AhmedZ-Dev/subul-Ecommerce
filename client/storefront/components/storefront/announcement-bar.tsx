import { messages } from "@/lib/messages.ar"

export function AnnouncementBar() {
  const items = messages.announcement.items

  return (
    <div
      className="bg-primary text-primary-foreground overflow-hidden py-2 text-xs md:text-sm"
      role="region"
      aria-label={messages.announcement.label}
    >
      <div className="container mx-auto px-4 md:px-6">
        <div className="hidden items-center justify-center gap-3 md:flex">
          {items.map((item, index) => (
            <span key={item} className="flex items-center gap-3">
              {index > 0 && (
                <span className="text-primary-foreground/60" aria-hidden>
                  •
                </span>
              )}
              <span className="font-medium">{item}</span>
            </span>
          ))}
        </div>

        <div className="md:hidden">
          <div className="announcement-marquee flex w-max gap-8">
            {[...items, ...items].map((item, index) => (
              <span key={`${item}-${index}`} className="whitespace-nowrap font-medium">
                {item}
              </span>
            ))}
          </div>
        </div>
      </div>
    </div>
  )
}
