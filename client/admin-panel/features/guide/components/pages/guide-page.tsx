'use client';

import { useCallback, useEffect, useMemo, useRef, useState } from 'react';
import { ListTreeIcon } from 'lucide-react';

import { Button } from '@/components/ui/button';
import { Card, CardContent } from '@/components/ui/card';
import {
  Sheet,
  SheetContent,
  SheetHeader,
  SheetTitle,
  SheetTrigger,
} from '@/components/ui/sheet';
import { PageHeader } from '@/components/layout/page-header';
import { guideSections } from '../../constants/guide-content';
import { GuideBlockRenderer } from '../blocks/guide-blocks';
import { GuideToc } from '../blocks/guide-toc';
import type { GuideSection } from '../../types';

/** Matches a section when its own text or any of its subsections match. */
function filterSections(sections: GuideSection[], query: string): GuideSection[] {
  const needle = query.trim().toLowerCase();
  if (!needle) return sections;

  return sections
    .map((section) => {
      const sectionMatches =
        section.title.toLowerCase().includes(needle) ||
        section.summary.toLowerCase().includes(needle);

      const matchingSubsections = section.subsections.filter((sub) =>
        sub.title.toLowerCase().includes(needle),
      );

      if (sectionMatches) return section;
      if (matchingSubsections.length > 0) {
        return { ...section, subsections: matchingSubsections };
      }
      return null;
    })
    .filter((section): section is GuideSection => section !== null);
}

export function GuidePage() {
  const [query, setQuery] = useState('');
  const [activeId, setActiveId] = useState<string | null>(null);
  const [mobileOpen, setMobileOpen] = useState(false);

  // Set while a click-driven scroll is in flight, so the observer does not
  // fight the animation and land the reader on a section they did not pick.
  const isNavigatingRef = useRef(false);

  const filteredSections = useMemo(
    () => filterSections(guideSections, query),
    [query],
  );

  const headingIds = useMemo(
    () =>
      guideSections.flatMap((section) => [
        section.id,
        ...section.subsections.map((sub) => sub.id),
      ]),
    [],
  );

  useEffect(() => {
    const elements = headingIds
      .map((id) => document.getElementById(id))
      .filter((element): element is HTMLElement => element !== null);

    if (elements.length === 0) return;

    const observer = new IntersectionObserver(
      (entries) => {
        if (isNavigatingRef.current) return;

        const visible = entries
          .filter((entry) => entry.isIntersecting)
          .sort((a, b) => a.boundingClientRect.top - b.boundingClientRect.top);

        if (visible[0]) setActiveId(visible[0].target.id);
      },
      // Only the band just under the header counts as "current", so the active
      // item tracks what the reader is actually looking at.
      { rootMargin: '-80px 0px -70% 0px', threshold: 0 },
    );

    elements.forEach((element) => observer.observe(element));
    return () => observer.disconnect();
  }, [headingIds]);

  const handleNavigate = useCallback((id: string) => {
    const element = document.getElementById(id);
    if (!element) return;

    isNavigatingRef.current = true;
    setActiveId(id);
    setMobileOpen(false);
    element.scrollIntoView({ behavior: 'smooth', block: 'start' });

    window.setTimeout(() => {
      isNavigatingRef.current = false;
    }, 700);
  }, []);

  return (
    <div className="flex flex-col gap-6">
      <PageHeader
        title="دليل استخدام النظام"
        description="شرح تفصيلي لكل قسم في لوحة التحكم، وكيف ينعكس على المتجر."
        action={
          <Sheet open={mobileOpen} onOpenChange={setMobileOpen}>
            <SheetTrigger asChild>
              <Button variant="outline" className="gap-2 lg:hidden">
                <ListTreeIcon className="size-4" aria-hidden />
                المحتويات
              </Button>
            </SheetTrigger>
            <SheetContent side="right" className="w-80 overflow-y-auto">
              <SheetHeader>
                <SheetTitle>محتويات الدليل</SheetTitle>
              </SheetHeader>
              <div className="px-4 pb-6">
                <GuideToc
                  sections={filteredSections}
                  activeId={activeId}
                  query={query}
                  onQueryChange={setQuery}
                  onNavigate={handleNavigate}
                />
              </div>
            </SheetContent>
          </Sheet>
        }
      />

      <div className="flex items-start gap-8">
        <div className="flex min-w-0 flex-1 flex-col gap-10">
          {filteredSections.length === 0 ? (
            <Card>
              <CardContent className="text-muted-foreground py-16 text-center text-sm">
                لا يوجد موضوع مطابق لبحثك.
              </CardContent>
            </Card>
          ) : (
            filteredSections.map((section) => (
              <section
                key={section.id}
                id={section.id}
                className="scroll-mt-24 flex flex-col gap-4"
              >
                <div className="flex items-start gap-3">
                  <span className="bg-primary/10 text-primary flex size-9 shrink-0 items-center justify-center rounded-lg">
                    <section.icon className="size-5" aria-hidden />
                  </span>
                  <div className="flex min-w-0 flex-col gap-1">
                    <h2 className="text-xl font-bold tracking-tight">
                      {section.title}
                    </h2>
                    <p className="text-muted-foreground text-sm">
                      {section.summary}
                    </p>
                  </div>
                </div>

                <div className="flex flex-col gap-4">
                  {section.subsections.map((subsection) => (
                    <Card key={subsection.id} id={subsection.id} className="scroll-mt-24">
                      <CardContent className="flex flex-col gap-4">
                        <h3 className="text-base font-semibold">
                          {subsection.title}
                        </h3>
                        {subsection.blocks.map((block, index) => (
                          <GuideBlockRenderer key={index} block={block} />
                        ))}
                      </CardContent>
                    </Card>
                  ))}
                </div>
              </section>
            ))
          )}
        </div>

        <aside className="sticky top-6 hidden w-72 shrink-0 lg:block">
          <GuideToc
            sections={filteredSections}
            activeId={activeId}
            query={query}
            onQueryChange={setQuery}
            onNavigate={handleNavigate}
            className="max-h-[calc(100dvh-6rem)] overflow-y-auto pe-1"
          />
        </aside>
      </div>
    </div>
  );
}
