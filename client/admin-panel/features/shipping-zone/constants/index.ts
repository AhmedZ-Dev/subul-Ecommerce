import type { StatusTone } from '@/components/ui/status-indicator';
import type { ShippingRateType, ShippingZoneStatus } from '../types';

export const SHIPPING_ZONE_QUERY_KEYS = {
  ALL: ['shipping-zones'] as const,
};

export const SHIPPING_ZONE_DEFAULT_PAGE_SIZE = 10;
export const SHIPPING_ZONE_MAX_PAGE_SIZE = 100;

export const SHIPPING_ZONE_STATUS_TONES: Record<ShippingZoneStatus, StatusTone> = {
  active: 'success',
  inactive: 'neutral',
};

export const SHIPPING_ZONE_STATUS_COLORS: Record<ShippingZoneStatus, string> = {
  active:
    'bg-green-50 text-green-700 border-green-200 dark:bg-green-950/40 dark:text-green-400 dark:border-green-900',
  inactive:
    'bg-gray-50 text-gray-700 border-gray-200 dark:bg-gray-900/40 dark:text-gray-400 dark:border-gray-800',
};

export const SHIPPING_ZONE_STATUS_DOT_COLORS: Record<ShippingZoneStatus, string> = {
  active: 'bg-green-500',
  inactive: 'bg-muted-foreground/60',
};

export const SHIPPING_RATE_TYPE_LABELS: Record<ShippingRateType, string> = {
  flat: 'ثابت',
  weight_based: 'حسب الوزن',
  price_based: 'حسب السعر',
};

export const IRAQI_GOVERNORATES = [
  { value: 'Anbar', labelAr: 'الأنبار' },
  { value: 'Babylon', labelAr: 'بابل' },
  { value: 'Baghdad', labelAr: 'بغداد' },
  { value: 'Basra', labelAr: 'البصرة' },
  { value: 'Dhi Qar', labelAr: 'ذي قار' },
  { value: 'Diyala', labelAr: 'ديالى' },
  { value: 'Duhok', labelAr: 'دهوك' },
  { value: 'Erbil', labelAr: 'أربيل' },
  { value: 'Karbala', labelAr: 'كربلاء' },
  { value: 'Kirkuk', labelAr: 'كركوك' },
  { value: 'Maysan', labelAr: 'ميسان' },
  { value: 'Muthanna', labelAr: 'المثنى' },
  { value: 'Najaf', labelAr: 'النجف' },
  { value: 'Nineveh', labelAr: 'نينوى' },
  { value: 'Qadisiyyah', labelAr: 'القادسية' },
  { value: 'Saladin', labelAr: 'صلاح الدين' },
  { value: 'Sulaymaniyah', labelAr: 'السليمانية' },
  { value: 'Wasit', labelAr: 'واسط' },
] as const;
