interface CategoryBannerProps {
  title: string
  description?: string
}

export function CategoryBanner({ title, description }: CategoryBannerProps) {
  return (
    <section
      className="hero-gradient relative mb-8 overflow-hidden rounded-xl px-6 py-10 md:px-10"
      aria-label={title}
    >
      <div className="relative z-10">
        <h1 className="text-primary-foreground text-2xl font-bold md:text-3xl">
          {title}
        </h1>
        {description && (
          <p className="text-primary-foreground/90 mt-2 text-sm md:text-base">
            {description}
          </p>
        )}
      </div>
      <div
        className="pointer-events-none absolute -end-8 -top-8 size-32 rounded-full bg-white/10"
        aria-hidden
      />
    </section>
  )
}
