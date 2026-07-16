import apiClient from "@/lib/api-client"
import type { ApiResponse } from "@/types/api"
import type { PaymentMethodDto } from "../types"

interface BackendPaymentMethod {
  id: number
  name: string
  labelEn: string | null
  labelAr: string | null
  type: string | null
  iconUrl: string | null
  instructionsEn: string | null
  instructionsAr: string | null
  isActive: boolean
  sortOrder: number
}

function toDto(raw: BackendPaymentMethod): PaymentMethodDto {
  return {
    id: raw.id,
    name: raw.name,
    labelEn: raw.labelEn,
    labelAr: raw.labelAr,
    type: raw.type,
    iconUrl: raw.iconUrl,
    instructionsEn: raw.instructionsEn,
    instructionsAr: raw.instructionsAr,
    isActive: raw.isActive,
    sortOrder: raw.sortOrder,
  }
}

export async function getActivePaymentMethods(): Promise<PaymentMethodDto[]> {
  const { data } = await apiClient.get<
    ApiResponse<{
      items: BackendPaymentMethod[]
      total: number
      page: number
      limit: number
      totalPages: number
    }>
  >("/payment-methods", {
    params: { isActive: true, limit: 50, sortBy: "sortOrder", sortOrder: "asc" },
  })

  if (!data.success) throw new Error(data.message ?? "Failed to fetch payment methods")
  return (data.data?.items ?? []).map(toDto)
}
