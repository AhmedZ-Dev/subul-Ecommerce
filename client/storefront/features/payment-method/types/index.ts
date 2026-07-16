export interface PaymentMethodDto {
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

export type CheckoutPaymentMethod = "cod" | "bank_transfer" | "online"
