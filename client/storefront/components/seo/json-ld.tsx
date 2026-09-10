interface JsonLdProps {
  data: Record<string, unknown>
}

/**
 * Escapes the characters that let JSON break out of a <script> block.
 *
 * The replacement string needs TWO backslashes. Written with one, the escape
 * is resolved by the TypeScript parser itself and the literal is already the
 * character it was meant to encode -- so the replace becomes a no-op and the
 * XSS stays open. That was the original bug here. With two, the six-character
 * sequence reaches the HTML, and the JSON parser inside the tag decodes it.
 *
 * U+2028/U+2029 are legal inside a JSON string but terminate a line in a
 * script body, so they are escaped too.
 */
function escapeForScriptTag(json: string): string {
  return json
    .replace(/</g, "\\u003c")
    .replace(/>/g, "\\u003e")
    .replace(/\u2028/g, "\\u2028")
    .replace(/\u2029/g, "\\u2029")
}

/**
 * Emits a schema.org block for crawlers. Rendered from a server component so it
 * ships in the initial HTML, where Google reads it.
 */
export function JsonLd({ data }: JsonLdProps) {
  return (
    <script
      type="application/ld+json"
      dangerouslySetInnerHTML={{
        __html: escapeForScriptTag(JSON.stringify(data)),
      }}
    />
  )
}
