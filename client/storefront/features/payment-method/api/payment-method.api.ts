import apiClient from "@/lib/api-client"
import type { ApiResponse } from "@/types/api"
import type { PaymentMethodDto } from "../types"

interface BackendPublicPaymentMethod {
  id: number
  name: string
  labelEn: string | null
  labelAr: string | null
  type: string | null
  iconUrl: string | null
  instructionsEn: string | null
  instructionsAr: string | null
  sortOrder: number
}

function toDto(raw: BackendPublicPaymentMethod): PaymentMethodDto {
  return {
    id: raw.id,
    name: raw.name,
    labelEn: raw.labelEn,
    labelAr: raw.labelAr,
    type: raw.type,
    iconUrl: raw.iconUrl,
    instructionsEn: raw.instructionsEn,
    instructionsAr: raw.instructionsAr,
    sortOrder: raw.sortOrder,
  }
}

/**
 * Reads the public projection, not the admin list: `/payment-methods` returns
 * `gatewayConfig` (gateway API keys) and is authenticated. This route filters
 * to active methods server-side and omits every gateway field.
 */
export async function getActivePaymentMethods(): Promise<PaymentMethodDto[]> {
  const { data } = await apiClient.get<
    ApiResponse<{ items: BackendPublicPaymentMethod[] }>
  >("/payment-methods/public")

  if (!data.success) throw new Error(data.message ?? "Failed to fetch payment methods")
  return (data.data?.items ?? []).map(toDto)
}
