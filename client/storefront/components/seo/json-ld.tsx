interface JsonLdProps {
  data: Record<string, unknown>
}

/**
 * Emits a schema.org block for crawlers. Rendered from a server component so it
 * ships in the initial HTML, where Google reads it.
 */
export function JsonLd({ data }: JsonLdProps) {
  return (
    <script
      type="application/ld+json"
      // Escaping "<" keeps a product description containing markup from
      // breaking out of the script tag.
      dangerouslySetInnerHTML={{
        __html: JSON.stringify(data).replace(/</g, "\u003c"),
      }}
    />
  )
}
