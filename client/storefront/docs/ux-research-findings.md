# UX Research Findings — Subul Storefront

> Generated: 2026-07-09 via `ui-ux-pro-max` skill queries

## Queries Run

| Query | Domain/Flag | Key Finding |
|-------|-------------|-------------|
| RTL arabic ecommerce product listing | `--design-system` | Marketplace pattern: search-focused hero, category grid, featured listings |
| checkout form step indicator | `--domain ux` | Multi-step processes need visible progress indicators |
| product card arabic mobile-first | `--domain style` | Bento grid cards: hover scale 1.02, soft shadows, 16–24px radius |
| shadcn nextjs ecommerce | `--stack shadcn` | Use shadcn CLI/blocks; rely on built-in ARIA |
| ecommerce cart sticky summary | `--domain product` | E-commerce: vibrant block layout, brand primary + success green |

---

## Applied Recommendations

### Homepage (`features/home/`)

- **Pattern:** Hero with prominent CTA + category grid + featured products
- **Style:** Warm bronze brand palette (not generic green from search — brand guidelines override)
- **Effects:** 200–300ms hover transitions, section gaps ≥48px

### Product Cards (`features/product/`)

- **Layout:** Consistent aspect-square images, rounded-xl cards
- **Interaction:** Hover lift + shadow (`product-card-hover` utility)
- **Badges:** Sale/discount badge using accent green token
- **Mobile:** 2-column grid on small screens, thumb-friendly add-to-cart

### Cart (`features/cart/`)

- **Layout:** Sticky order summary on desktop (`lg:sticky lg:top-20`)
- **Items:** Quantity stepper with icon buttons (already present — enhance thumbnails)
- **UX:** Clear subtotal + prominent checkout CTA

### Checkout (`features/checkout/`)

- **Progress:** 3-step indicator (Shipping → Payment → Review)
- **Forms:** Keep labeled fields (no placeholder-only inputs)
- **Submit:** Loading state on place-order button (already present)

### Header / Footer (`components/storefront/`)

- **Header:** Sticky with backdrop blur (already present), add mobile Sheet nav
- **Search:** Prominent in header (already present)
- **Footer:** Structured quick links + brand description

### RTL / Accessibility Checklist

- [x] Use `start`/`end` not `ml`/`mr`
- [x] Lucide SVG icons (not emoji)
- [x] `cursor-pointer` on interactive elements
- [x] 4.5:1 contrast minimum (brand bronze verified in guidelines)
- [x] Focus states via shadcn components
- [x] Responsive breakpoints: 375, 768, 1024, 1440

---

## Typography Note

Search recommended Noto Naskh Arabic; storefront uses **Cairo** (already loaded locally) — aligned with brand guidelines. No font change needed.

## Color Note

Search suggested green marketplace palette; **brand guidelines bronze (#B8956A) takes precedence** via design token sync.
