import {
  createSearchParamsCache,
  parseAsArrayOf,
  parseAsBoolean,
  parseAsFloat,
  parseAsInteger,
  parseAsJson,
  parseAsString,
  parseAsStringEnum,
} from "nuqs/server"
import { PRODUCT_DEFAULT_PAGE_SIZE } from "./constants"

export const productListingParsers = {
  page: parseAsInteger.withDefault(1),
  limit: parseAsInteger.withDefault(PRODUCT_DEFAULT_PAGE_SIZE).withOptions({
    clearOnDefault: true,
  }),
  search: parseAsString.withDefault(""),
  categoryId: parseAsInteger,
  brandId: parseAsInteger,
  brandIds: parseAsArrayOf(parseAsInteger).withDefault([]),
  minPrice: parseAsFloat,
  maxPrice: parseAsFloat,
  inStock: parseAsBoolean,
  attrs: parseAsJson<Record<string, string[]>>((value) => {
    if (typeof value !== "object" || value === null || Array.isArray(value)) {
      return null
    }
    const record = value as Record<string, unknown>
    for (const entry of Object.values(record)) {
      if (!Array.isArray(entry) || entry.some((item) => typeof item !== "string")) {
        return null
      }
    }
    return record as Record<string, string[]>
  }).withDefault({}),
  sortBy: parseAsStringEnum(["price", "nameEn", "createdAt", "totalSold"]).withDefault(
    "createdAt",
  ),
  sortOrder: parseAsStringEnum(["asc", "desc"]).withDefault("desc"),
}

export const productListingSearchParamsCache =
  createSearchParamsCache(productListingParsers)
