"use client"

import Link from "next/link"
import Image from "next/image"
import { useCallback, useEffect, useRef, useState } from "react"
import { ArrowLeft, ChevronLeft, ChevronRight } from "lucide-react"
import { Button } from "@/components/ui/button"
import { cn } from "@/lib/utils"
import { messages } from "@/lib/messages.ar"

const SLIDE_INTERVAL_MS = 6000

export function HeroCarousel() {
  const slides = messages.hero.slides
  const [activeIndex, setActiveIndex] = useState(0)
  const [isPaused, setIsPaused] = useState(false)
  const [progress, setProgress] = useState(0)
  const indexRef = useRef(0)

  const goTo = useCallback(
    (index: number) => {
      const next = (index + slides.length) % slides.length
      indexRef.current = next
      setActiveIndex(next)
      setProgress(0)
    },
    [slides.length],
  )

  const goNext = useCallback(() => goTo(indexRef.current + 1), [goTo])
  const goPrev = useCallback(() => goTo(indexRef.current - 1), [goTo])

  useEffect(() => {
    indexRef.current = activeIndex
  }, [activeIndex])

  useEffect(() => {
    if (isPaused || slides.length <= 1) return

    let raf = 0
    const start = performance.now() - (progress / 100) * SLIDE_INTERVAL_MS

    const tick = (now: number) => {
      const elapsed = now - start
      const nextProgress = Math.min(100, (elapsed / SLIDE_INTERVAL_MS) * 100)
      setProgress(nextProgress)

      if (nextProgress >= 100) {
        goNext()
        return
      }
      raf = window.requestAnimationFrame(tick)
    }

    raf = window.requestAnimationFrame(tick)
    return () => window.cancelAnimationFrame(raf)
    // progress intentionally omitted — pause resumes from current value
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [activeIndex, goNext, isPaused, slides.length])

  return (
    <section
      className="group/hero relative mb-10 overflow-hidden rounded-2xl md:mb-14 md:rounded-3xl"
      aria-label={messages.hero.label}
      aria-roledescription="carousel"
      onMouseEnter={() => setIsPaused(true)}
      onMouseLeave={() => setIsPaused(false)}
      onFocusCapture={() => setIsPaused(true)}
      onBlurCapture={(e) => {
        if (!e.currentTarget.contains(e.relatedTarget as Node | null)) {
          setIsPaused(false)
        }
      }}
    >
      <div className="relative aspect-4/5 min-h-88 w-full sm:aspect-16/10 sm:min-h-104 lg:aspect-21/9 lg:min-h-112">
        {slides.map((slide, index) => {
          const isActive = index === activeIndex
          return (
            <div
              key={slide.title}
              className={cn(
                "absolute inset-0 transition-opacity duration-700 ease-out motion-reduce:transition-none",
                isActive ? "z-10 opacity-100" : "pointer-events-none z-0 opacity-0",
              )}
              aria-hidden={!isActive}
            >
              <Image
                src={slide.image}
                alt=""
                fill
                priority={index === 0}
                className={cn(
                  "object-cover transition-transform duration-6000 ease-out motion-reduce:scale-100 motion-reduce:transition-none",
                  isActive ? "scale-105" : "scale-100",
                )}
                sizes="100vw"
              />

              <div
                className="pointer-events-none absolute inset-0 bg-linear-to-t from-black/80 via-black/45 to-black/20 md:bg-linear-to-l md:from-black/75 md:via-black/40 md:to-black/15"
                aria-hidden
              />

              <div
                className="absolute inset-0 flex items-end md:items-center"
                dir="rtl"
              >
                <div className="flex w-full max-w-xl flex-col gap-4 px-5 pb-16 pt-10 sm:px-8 sm:pb-14 md:px-10 lg:px-14">
                  <p className="text-xs font-medium tracking-[0.18em] text-white/70 uppercase sm:text-sm">
                    {messages.common.companyName}
                  </p>

                  <h1 className="text-balance text-3xl leading-tight font-bold text-white sm:text-4xl md:text-5xl lg:text-[3.25rem]">
                    {slide.title}
                  </h1>

                  <p className="max-w-md text-pretty text-sm leading-relaxed text-white/85 sm:text-base md:text-lg">
                    {slide.subtitle}
                  </p>

                  <div className="mt-2">
                    <Button
                      asChild
                      size="lg"
                      className="h-12 rounded-full bg-white px-6 text-base font-semibold text-neutral-950 shadow-lg transition-transform hover:scale-[1.02] hover:bg-white/95 active:scale-[0.98]"
                    >
                      <Link href={slide.href}>
                        {slide.cta}
                        <ArrowLeft className="ms-2 size-4" aria-hidden />
                      </Link>
                    </Button>
                  </div>
                </div>
              </div>
            </div>
          )
        })}
      </div>

      <div className="pointer-events-none absolute inset-0 z-20">
        <Button
          type="button"
          variant="ghost"
          size="icon"
          className="pointer-events-auto absolute inset-s-3 top-1/2 size-11 -translate-y-1/2 cursor-pointer rounded-full border border-white/20 bg-black/25 text-white backdrop-blur-md transition-colors hover:bg-black/40 hover:text-white sm:inset-s-4 md:size-12"
          onClick={goPrev}
          aria-label={messages.hero.prevSlide}
        >
          <ChevronRight className="size-5" aria-hidden />
        </Button>
        <Button
          type="button"
          variant="ghost"
          size="icon"
          className="pointer-events-auto absolute inset-e-3 top-1/2 size-11 -translate-y-1/2 cursor-pointer rounded-full border border-white/20 bg-black/25 text-white backdrop-blur-md transition-colors hover:bg-black/40 hover:text-white sm:inset-e-4 md:size-12"
          onClick={goNext}
          aria-label={messages.hero.nextSlide}
        >
          <ChevronLeft className="size-5" aria-hidden />
        </Button>

        <div className="pointer-events-auto absolute inset-x-0 bottom-4 flex items-center justify-center gap-2 sm:bottom-5">
          {slides.map((slide, index) => {
            const isActive = index === activeIndex
            return (
              <button
                key={slide.title}
                type="button"
                onClick={() => goTo(index)}
                className="group/dot flex h-11 cursor-pointer items-center justify-center px-0.5"
                aria-label={`${messages.hero.goToSlide} ${index + 1}`}
                aria-current={isActive ? "true" : undefined}
              >
                <span
                  className={cn(
                    "relative h-1 overflow-hidden rounded-full bg-white/35 transition-all duration-300",
                    isActive ? "w-8 sm:w-10" : "w-2.5 group-hover/dot:bg-white/55",
                  )}
                >
                  {isActive && (
                    <span
                      className="absolute inset-y-0 inset-s-0 rounded-full bg-white motion-reduce:w-full"
                      style={{ width: `${progress}%` }}
                      aria-hidden
                    />
                  )}
                </span>
              </button>
            )
          })}
        </div>
      </div>
    </section>
  )
}
