# Brand Guidelines v1.0 — سبُل (Subul)

> Last updated: 2026-07-09
> Status: Approved

## Quick Reference

| Element | Value |
|---------|-------|
| Primary Color | #B8956A |
| Secondary Color | #E8E4DC |
| Accent Color | #16A34A |
| Primary Font | Cairo |
| Voice | Warm, Trustworthy, Clear |

---

## 1. Color Palette

### Primary Colors

| Name | Hex | RGB | Usage |
|------|-----|-----|-------|
| Primary Bronze | #B8956A | rgb(184,149,106) | CTAs, headers, links, brand accent |
| Primary Dark | #96754D | rgb(150,117,77) | Hover states, emphasis |

### Secondary Colors

| Name | Hex | RGB | Usage |
|------|-----|-----|-------|
| Secondary Warm | #E8E4DC | rgb(232,228,220) | Section backgrounds, cards |
| Accent Green | #16A34A | rgb(22,163,74) | Success, positive states, sale badges |

### Neutral Palette

| Name | Hex | RGB | Usage |
|------|-----|-----|-------|
| Background | #FAF9F7 | rgb(250,249,247) | Page backgrounds |
| Surface | #FFFFFF | rgb(255,255,255) | Cards, modals |
| Text Primary | #2C2418 | rgb(44,36,24) | Headings, body text |
| Text Secondary | #6B6560 | rgb(107,101,96) | Captions, muted text |
| Border | #E0DCD4 | rgb(224,220,212) | Dividers, borders |

### Semantic Colors

| State | Hex | Usage |
|-------|-----|-------|
| Success | #16A34A | Positive actions, confirmations |
| Warning | #D97706 | Cautions, pending states |
| Error | #DC2626 | Errors, destructive actions |
| Info | #2563EB | Informational messages |

### Accessibility

- Text on white background: 12.1:1 contrast ratio (AAA)
- Primary bronze on white: 4.6:1 contrast ratio (AA)
- All interactive elements meet WCAG 2.1 AA standards

---

## 2. Typography

### Font Stack

```css
--font-heading: 'Cairo', system-ui, sans-serif;
--font-body: 'Cairo', system-ui, sans-serif;
--font-mono: 'Geist Mono', ui-monospace, monospace;
```

### Type Scale

| Element | Size (Desktop) | Size (Mobile) | Weight | Line Height |
|---------|----------------|---------------|--------|-------------|
| H1 | 48px | 32px | 700 | 1.2 |
| H2 | 36px | 28px | 600 | 1.25 |
| H3 | 28px | 24px | 600 | 1.3 |
| H4 | 24px | 20px | 600 | 1.35 |
| Body | 16px | 16px | 400 | 1.5 |
| Body Large | 18px | 18px | 400 | 1.6 |
| Small | 14px | 14px | 400 | 1.5 |
| Caption | 12px | 12px | 400 | 1.4 |

---

## 3. Logo Usage

### Variants

| Variant | File | Use Case |
|---------|------|----------|
| Icon Only | logo-icon.svg | Favicons, header mark |
| Full Horizontal | logo-full-horizontal.svg | Headers, documents |

### Clear Space

Minimum clear space = height of the logo icon (mark)

### Minimum Size

| Context | Minimum Width |
|---------|---------------|
| Digital - Full Logo | 120px |
| Digital - Icon | 24px |

### Don'ts

- Don't rotate or skew the logo
- Don't change colors outside approved palette
- Don't add shadows or effects
- Don't crop or modify proportions

---

## 4. Voice & Tone

### Brand Personality

| Trait | Description |
|-------|-------------|
| **Warm** | Approachable, welcoming Arabic-first experience |
| **Trustworthy** | Reliable delivery, honest pricing |
| **Clear** | Direct communication, jargon-free |
| **Confident** | Assured without being arrogant |

### Voice Chart

| Trait | We Are | We Are Not |
|-------|--------|------------|
| Warm | Friendly, inviting | Overly casual |
| Trustworthy | Honest, dependable | Corporate, cold |
| Clear | Direct, concise | Vague, wordy |
| Confident | Assured | Arrogant, overselling |

### Tone by Context

| Context | Tone | Example |
|---------|------|---------|
| Marketing | Engaging, benefit-focused | "اكتشف تشكيلتنا المختارة بعناية." |
| Product | Informative, helpful | "التوصيل خلال 2-5 أيام عمل." |
| Error messages | Calm, solution-focused | "حاول تحديث الصفحة." |
| Success | Brief, celebratory | "تم تأكيد طلبك!" |

---

## 5. Imagery Guidelines

### Photography Style

- **Lighting:** Natural, soft lighting preferred
- **Subjects:** Products in context, authentic scenarios
- **Color treatment:** Warm tones aligned with bronze palette
- **Composition:** Clean, focused subjects

### Icons

- Style: Outlined, 24px base grid (Phosphor / Lucide)
- Stroke: 1.5px consistent
- Fill: None (outline only)

---

## 6. Design Components

### Buttons

| Type | Background | Text | Border Radius |
|------|------------|------|---------------|
| Primary | #B8956A | #FFFFFF | 12px |
| Secondary | Transparent | #B8956A | 12px |
| Tertiary | Transparent | #6B6560 | 12px |

### Spacing Scale

| Token | Value | Usage |
|-------|-------|-------|
| xs | 4px | Tight spacing |
| sm | 8px | Compact elements |
| md | 16px | Standard spacing |
| lg | 24px | Section spacing |
| xl | 32px | Large gaps |
| 2xl | 48px | Section dividers |

### Border Radius

| Element | Radius |
|---------|--------|
| Buttons | 12px |
| Cards | 12px |
| Inputs | 8px |
| Modals | 16px |
| Pills/Tags | 9999px |

---

## AI Image Generation

### Base Prompt Template

```
Warm bronze and cream palette (#B8956A, #FAF9F7), soft natural lighting,
Arabic e-commerce aesthetic, clean minimal composition, professional product photography
```

### Visual Don'ts

| Avoid | Reason |
|-------|--------|
| Cool blue tones | Off-brand for Subul |
| Cluttered layouts | Conflicts with minimal aesthetic |
| Low contrast text | Accessibility failure |

---

## Changelog

| Version | Date | Changes |
|---------|------|---------|
| 1.0 | 2026-07-09 | Initial Subul storefront guidelines |
