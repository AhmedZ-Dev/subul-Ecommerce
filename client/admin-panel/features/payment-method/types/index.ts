// features/payment-method/types/index.ts

export type PaymentMethodStatus = 'active' | 'inactive';

export type PaymentMethodType = 'offline' | 'online';

export interface PaymentMethodDto {
  id: number;
  name: string;
  labelEn: string | null;
  labelAr: string | null;
  type: PaymentMethodType | null;
  gateway: string | null;
  gatewayConfig: string | null;
  iconUrl: string | null;
  instructionsEn: string | null;
  instructionsAr: string | null;
  sortOrder: number;
  status: PaymentMethodStatus;
  createdAt: string;
  updatedAt: string | null;
}

export interface PaymentMethodListItem {
  id: number;
  name: string;
  labelEn: string | null;
  labelAr: string | null;
  type: PaymentMethodType | null;
  gateway: string | null;
  iconUrl: string | null;
  sortOrder: number;
  status: PaymentMethodStatus;
}

export interface PaymentMethodQueryParams {
  page?: number;
  limit?: number;
  search?: string;
  type?: PaymentMethodType;
  status?: PaymentMethodStatus;
  sortBy?: 'name' | 'createdAt' | 'sortOrder';
  sortOrder?: 'asc' | 'desc';
}
