import { cache } from "react"
import { getTopLevelCategories } from "./category.api"

/**
 * Deduped for the length of one server render: the store layout and the home
 * page both need the top-level categories, and this keeps that to one request.
 */
export const getCachedTopLevelCategories = cache(getTopLevelCategories)
