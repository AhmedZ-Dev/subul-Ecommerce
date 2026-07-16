export interface ShippingRate {
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

export interface ShippingZoneDto {
  id: number
  nameEn: string
  nameAr: string | null
  governorates: string[]
  isActive: boolean
  shippingRates: ShippingRate[]
}
