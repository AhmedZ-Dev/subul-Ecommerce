import { messages } from "@/lib/messages.ar"

const statusLabels: Record<string, string> = {
  Done: messages.table.statusDone,
  "In Process": messages.table.statusInProgress,
  "In Progress": messages.table.statusInProgress,
  "Not Started": messages.table.statusNotStarted,
}

const typeLabels: Record<string, string> = {
  "Table of Contents": messages.table.types.tableOfContents,
  "Executive Summary": messages.table.types.executiveSummary,
  "Technical Approach": messages.table.types.technicalApproach,
  Design: messages.table.types.design,
  Capabilities: messages.table.types.capabilities,
  "Focus Documents": messages.table.types.focusDocuments,
  Narrative: messages.table.types.narrative,
  "Cover Page": messages.table.types.coverPage,
  "Cover page": messages.table.types.coverPage,
  "Table of contents": messages.table.types.tableOfContents,
  "Executive summary": messages.table.types.executiveSummary,
}

export function translateStatus(status: string) {
  return statusLabels[status] ?? status
}

export function translateType(type: string) {
  return typeLabels[type] ?? type
}
