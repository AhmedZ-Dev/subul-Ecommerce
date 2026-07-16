import apiClient from "@/lib/api-client"
import type { ApiResponse, PaginatedResponse } from "@/types/api"
import type { ShippingRate, ShippingZoneDto } from "../types"

interface BackendShippingRate {
  id: number
  shippingZoneId: number
  nameEn: string | null
  nameAr: string | null
  rateType: string
  price: number
  minOrderValue: number | null
  maxOrderValue: number | null
  freeShippingThreshold: number | null
  estimatedDaysMin: number | null
  estimatedDaysMax: number | null
}

interface BackendShippingZone {
  id: number
  nameEn: string
  nameAr: string | null
  governorates: string[]
  isActive: boolean
  shippingRates: BackendShippingRate[]
}

function toRate(raw: BackendShippingRate): ShippingRate {
  return {
    id: raw.id,
    shippingZoneId: raw.shippingZoneId,
    nameEn: raw.nameEn,
    nameAr: raw.nameAr,
    rateType: raw.rateType,
    price: raw.price,
    minOrderValue: raw.minOrderValue,
    maxOrderValue: raw.maxOrderValue,
    freeShippingThreshold: raw.freeShippingThreshold,
    estimatedDaysMin: raw.estimatedDaysMin,
    estimatedDaysMax: raw.estimatedDaysMax,
  }
}

function toDto(raw: BackendShippingZone): ShippingZoneDto {
  return {
    id: raw.id,
    nameEn: raw.nameEn,
    nameAr: raw.nameAr,
    governorates: raw.governorates,
    isActive: raw.isActive,
    shippingRates: (raw.shippingRates ?? []).map(toRate),
  }
}

export async function getActiveShippingZones(): Promise<ShippingZoneDto[]> {
  const { data } = await apiClient.get<
    ApiResponse<{
      items: BackendShippingZone[]
      total: number
      page: number
      limit: number
      totalPages: number
    }>
  >("/shipping-zones", {
    params: { isActive: true, limit: 100 },
  })

  if (!data.success) throw new Error(data.message ?? "Failed to fetch shipping zones")
  return (data.data?.items ?? []).map(toDto)
}
