/**
 * Iraq's governorates with Arabic labels for display and the English spelling
 * used by `ShippingZone.governorates`, so picking one governorate can resolve
 * its shipping zone without asking the customer a second time.
 */
export const IRAQ_GOVERNORATES = [
  { en: "Baghdad", ar: "بغداد" },
  { en: "Basra", ar: "البصرة" },
  { en: "Nineveh", ar: "نينوى" },
  { en: "Erbil", ar: "أربيل" },
  { en: "Sulaymaniyah", ar: "السليمانية" },
  { en: "Duhok", ar: "دهوك" },
  { en: "Kirkuk", ar: "كركوك" },
  { en: "Najaf", ar: "النجف" },
  { en: "Karbala", ar: "كربلاء" },
  { en: "Anbar", ar: "الأنبار" },
  { en: "Babil", ar: "بابل" },
  { en: "Diyala", ar: "ديالى" },
  { en: "Dhi Qar", ar: "ذي قار" },
  { en: "Maysan", ar: "ميسان" },
  { en: "Muthanna", ar: "المثنى" },
  { en: "Al-Qadisiyyah", ar: "القادسية" },
  { en: "Salah ad Din", ar: "صلاح الدين" },
  { en: "Wasit", ar: "واسط" },
] as const

export type IraqGovernorate = (typeof IRAQ_GOVERNORATES)[number]
