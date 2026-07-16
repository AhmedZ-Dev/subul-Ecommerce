export const LOCALE = "ar-SA"

export const messages = {
  common: {
    companyName: "سبُل المستقبل",
    button: "زر",
    more: "المزيد",
    open: "فتح",
    share: "مشاركة",
    delete: "حذف",
    edit: "تعديل",
    view: "عرض",
    copy: "نسخ نسخة",
    favorite: "تفضيل",
    submit: "إرسال",
    cancel: "إلغاء",
    done: "تم",
    error: "خطأ",
    noResults: "لا توجد نتائج.",
    yes: "نعم",
    no: "لا",
    loading: "جاري التحميل…",
    back: "رجوع",
    home: "الرئيسية",
  },
  announcement: {
    label: "عروض وخدمات المتجر",
    items: [
      "شحن مجاني للطلبات فوق 100,000 د.ع",
      "توصيل لجميع مناطق العراق",
      "ضمان لا يقل عن سنة على جميع المنتجات",
    ],
  },
  hero: {
    label: "عروض المتجر",
    prevSlide: "الشريحة السابقة",
    nextSlide: "الشريحة التالية",
    goToSlide: "انتقل إلى الشريحة",
    slides: [
      {
        title: "أفضل أجهزة الكمبيوتر والإلكترونيات",
        subtitle: "اكتشف أحدث المنتجات بأسعار تنافسية وضمان رسمي",
        cta: "تسوق الآن",
        href: "/products",
        image: "/img/seed/1587202372775-e229f172b9d7.jpg",
      },
      {
        title: "عروض حصرية على اللابتوبات",
        subtitle: "تشكيلة واسعة من أشهر العلامات التجارية العالمية",
        cta: "تسوق الآن",
        href: "/products?sortBy=price&sortOrder=asc",
        image: "/img/seed/1496181133206-80ce9b88a853.jpg",
      },
      {
        title: "ملحقات وإكسسوارات بأسعار مميزة",
        subtitle: "لوحات مفاتيح، فأرات، سماعات وأكثر — توصيل سريع",
        cta: "تسوق الآن",
        href: "/products",
        image: "/img/seed/1541140532154-b024d705b90a.jpg",
      },
    ],
  },
  promo: {
    title: "عروض حصرية داخل المتجر",
    subtitle: "اكتشف أفضل العروض على الملحقات والإكسسوارات — لفترة محدودة",
    cta: "تسوق الآن",
    href: "/products",
    image: "/img/seed/1593640408182-31c70c8268f5.jpg",
  },
  brands: {
    title: "العلامات التجارية الشائعة",
    description: "نوفر منتجات من أشهر العلامات العالمية",
    items: [
      "ASUS",
      "Dell",
      "Lenovo",
      "HP",
      "Acer",
      "MSI",
      "Samsung",
      "Sunell",
    ],
  },
  contact: {
    label: "تواصل معنا",
    title: "هل تحتاج مساعدة؟",
    description: "فريق الدعم جاهز لمساعدتك في اختيار المنتج أو تتبع الطلب",
    primaryCta: "تحدث عبر واتساب",
    whatsapp: {
      label: "واتساب",
      value: "+964 774 802 5119",
      href: "https://wa.me/9647748025119",
    },
    phone: {
      label: "اتصل بنا",
      value: "6543",
      href: "tel:6543",
    },
    email: {
      label: "راسلنا",
      value: "store@subul.com",
      href: "mailto:store@subul.com",
    },
  },
  trust: {
    label: "مزايا التسوق معنا",
    items: [
      {
        title: "توصيل لجميع العراق",
        description: "شحن سريع لمحافظات العراق",
      },
      {
        title: "ضمان رسمي",
        description: "ضمان لا يقل عن سنة على المنتجات",
      },
      {
        title: "شحن مجاني",
        description: "للطلبات فوق 100,000 د.ع",
      },
      {
        title: "دعم مباشر",
        description: "مساعدة عبر واتساب والهاتف",
      },
    ],
  },
  storefront: {
    pageTitle: "المتجر",
    description: "تسوق أونلاين من سبُل المستقبل",
    heroTitle: "مرحباً بك في متجرنا",
    heroDescription: "اكتشف أفضل المنتجات بأسعار تنافسية",
    featuredProducts: "منتجات مميزة",
    featuredProductsDescription: "اختيارات منتقاة لأفضل الأجهزة والعروض الحالية",
    topCategories: "تسوق حسب التصنيف",
    topCategoriesDescription: "تصفح الأقسام الأكثر طلباً وابدأ من التصنيف المناسب لك",
    collections: "مجموعات مختارة",
    collectionsDescription: "مجموعات جاهزة توفر عليك وقت البحث",
    viewAll: "عرض الكل",
    exploreCollection: "استكشف المجموعة",
    productCount: (count: number) => (count === 1 ? "منتج واحد" : `${count} منتجات`),
    trackOrder: "تتبع طلب",
  },
  header: {
    searchPlaceholder: "ابحث عن منتج…",
    cart: "السلة",
    products: "المنتجات",
    categories: "التصنيفات",
    collections: "المجموعات",
    menu: "القائمة",
  },
  footer: {
    quickLinks: "روابط سريعة",
    trackOrder: "تتبع الطلب",
    allProducts: "جميع المنتجات",
    copyright: (year: number) => `© ${year} سبُل المستقبل. جميع الحقوق محفوظة.`,
  },
  product: {
    addToCart: "أضف إلى السلة",
    addedToCart: "تمت الإضافة إلى السلة",
    addToCartError: "فشل إضافة المنتج إلى السلة",
    outOfStock: "غير متوفر",
    featured: "مميز",
    inStock: "متوفر",
    viewDetails: "عرض التفاصيل",
    noProducts: "لا توجد منتجات",
    noProductsDescription: "لم نعثر على منتجات تطابق بحثك.",
    filters: {
      title: "الفلاتر",
      search: "بحث",
      searchPlaceholder: "ابحث عن منتج…",
      category: "التصنيف",
      allCategories: "جميع التصنيفات",
      brand: "الماركة",
      priceRange: "نطاق السعر",
      priceMin: "الحد الأدنى",
      priceMax: "الحد الأعلى",
      inStockOnly: "متوفر فقط",
      clearAll: "مسح الكل",
      noFilters: "لا توجد فلاتر متاحة",
      showResults: "عرض النتائج",
      sort: "الترتيب",
      sortBy: "ترتيب حسب",
      sortNewest: "الأحدث",
      sortPriceAsc: "السعر: من الأقل",
      sortPriceDesc: "السعر: من الأعلى",
      sortName: "الاسم",
      sortBestSelling: "الأكثر مبيعاً",
    },
    detail: {
      description: "الوصف",
      attributes: "المواصفات",
      variants: "الخيارات",
      selectVariant: "اختر خياراً",
      quantity: "الكمية",
      minOrder: (qty: number) => `الحد الأدنى للطلب: ${qty}`,
      warranty: (months: number) => `ضمان ${months} شهر`,
      compareAtPrice: "السعر قبل الخصم",
      sku: "رمز المنتج",
      category: "التصنيف",
      brand: "الماركة",
      noImage: "لا توجد صورة",
    },
    listing: {
      title: "المنتجات",
      description: "تصفح جميع المنتجات المتوفرة",
      paginationTotal: (total: number) => `${total} منتج`,
      paginationPage: (page: number, totalPages: number) =>
        `صفحة ${page} من ${totalPages || 1}`,
    },
  },
  category: {
    listing: {
      title: "التصنيف",
      productsInCategory: "المنتجات في هذا التصنيف",
      emptyTitle: "لا توجد منتجات",
      emptyDescription: "لا توجد منتجات في هذا التصنيف حالياً.",
    },
    nav: {
      all: "جميع التصنيفات",
    },
  },
  cart: {
    title: "سلة التسوق",
    empty: "سلتك فارغة",
    emptyDescription: "أضف منتجات إلى سلتك للمتابعة.",
    browseProducts: "تصفح المنتجات",
    subtotal: "المجموع الفرعي",
    itemCount: (count: number) => `${count} منتج`,
    checkout: "إتمام الشراء",
    remove: "إزالة",
    quantity: "الكمية",
    clearCart: "تفريغ السلة",
    addSuccess: "تمت الإضافة إلى السلة",
    addError: "فشل إضافة المنتج",
    updateError: "فشل تحديث الكمية",
    removeSuccess: "تمت إزالة المنتج",
    removeError: "فشل إزالة المنتج",
    lineTotal: "المجموع",
    unitPrice: "سعر الوحدة",
    continueShopping: "متابعة التسوق",
  },
  checkout: {
    title: "إتمام الشراء",
    description: "أدخل بيانات التوصيل وسنؤكد طلبك فوراً — الدفع عند الاستلام",
    steps: {
      shipping: "الشحن",
      payment: "الدفع",
      review: "المراجعة",
    },
    placeOrder: "تأكيد الطلب",
    orderSummary: "ملخص الطلب",
    shippingInfo: "معلومات التوصيل",
    paymentMethod: "طريقة الدفع",
    customerNotes: "ملاحظات",
    customerNotesPlaceholder: "ملاحظات إضافية (اختياري)",
    success: "تم استلام طلبك بنجاح",
    successDescription:
      "شكراً لك. طلبك قيد المعالجة وسنتواصل معك قريباً لتأكيد التفاصيل والتوصيل.",
    successHint: "احفظ رقم الطلب لاستخدامه في التتبع لاحقاً.",
    orderNumber: "رقم الطلب",
    trackYourOrder: "تتبع طلبك",
    continueShopping: "متابعة التسوق",
    backToHome: "العودة للرئيسية",
    emptyCart: "سلتك فارغة. أضف منتجات قبل إتمام الشراء.",
    paymentCod: "الدفع عند الاستلام",
    paymentCodDescription: "ادفع نقداً لمندوب التوصيل عند استلام طلبك. لا حاجة لبطاقة أو تحويل الآن.",
    paymentBank: "تحويل بنكي",
    paymentOnline: "دفع إلكتروني",
    shipping: "الشحن",
    total: "الإجمالي",
    subtotal: "المجموع الفرعي",
    shippingAmount: "تكلفة الشحن",
    freeShipping: "شحن مجاني",
    estimatedDays: (min: number, max?: number) =>
      max ? `التسليم خلال ${min}–${max} يوم` : `التسليم خلال ${min} يوم`,
    fields: {
      firstName: "الاسم الأول",
      lastName: "اسم العائلة",
      phone: "رقم الهاتف",
      address1: "العنوان",
      address2: "تفاصيل إضافية (اختياري)",
      city: "المدينة",
      governorate: "المحافظة",
      shippingZone: "منطقة الشحن",
    },
    validation: {
      firstName: "الاسم الأول مطلوب (حرفان على الأقل)",
      lastName: "اسم العائلة مطلوب (حرفان على الأقل)",
      phone: "رقم الهاتف مطلوب (7 أرقام على الأقل)",
      address: "العنوان مطلوب (5 أحرف على الأقل)",
      city: "المدينة مطلوبة",
      governorate: "المحافظة مطلوبة",
      paymentMethod: "اختر طريقة الدفع",
    },
    createError: "فشل إنشاء الطلب",
  },
  order: {
    track: "تتبع الطلب",
    trackTitle: "تتبع طلبك",
    trackDescription: "أدخل رقم الطلب ورقم الهاتف لتتبع حالة طلبك.",
    orderNumber: "رقم الطلب",
    orderNumberPlaceholder: "مثال: ORD-20240101-001",
    phone: "رقم الهاتف",
    phonePlaceholder: "07XXXXXXXX",
    notFound: "لم يتم العثور على الطلب. تحقق من رقم الطلب والهاتف.",
    trackError: "فشل تتبع الطلب",
    status: {
      pending: "قيد الانتظار",
      confirmed: "مؤكد",
      processing: "قيد المعالجة",
      shipped: "تم الشحن",
      out_for_delivery: "في الطريق",
      delivered: "تم التسليم",
      cancelled: "ملغي",
      refunded: "مسترد",
    },
    paymentStatus: {
      pending: "في انتظار الدفع",
      paid: "مدفوع",
      refunded: "مسترد",
    },
    fulfillmentStatus: {
      unfulfilled: "لم يُنفَّذ",
      partial: "تنفيذ جزئي",
      fulfilled: "مُنفَّذ",
    },
    timeline: {
      title: "حالة الطلب",
      placed: "تم الطلب",
      confirmed: "تم التأكيد",
      processing: "قيد المعالجة",
      shipped: "تم الشحن",
      delivered: "تم التسليم",
    },
    details: {
      title: "تفاصيل الطلب",
      total: "الإجمالي",
      items: "المنتجات",
      trackingNumber: "رقم التتبع",
      shippingTo: "الشحن إلى",
      createdAt: "تاريخ الطلب",
    },
  },
  collection: {
    title: "المجموعات",
    products: "المنتجات في المجموعة",
    empty: "لا توجد منتجات في هذه المجموعة",
  },
} as const

export function formatCurrency(value: number, currency = "IQD") {
  return new Intl.NumberFormat(LOCALE, {
    style: "currency",
    currency,
    maximumFractionDigits: 0,
  }).format(value)
}

export function formatNumber(value: number) {
  return new Intl.NumberFormat(LOCALE).format(value)
}

export function formatPercent(value: number) {
  return new Intl.NumberFormat(LOCALE, {
    style: "percent",
    minimumFractionDigits: 1,
    maximumFractionDigits: 1,
  }).format(value / 100)
}

export function formatDate(
  value: string | Date,
  options?: Intl.DateTimeFormatOptions,
) {
  const date = typeof value === "string" ? new Date(value) : value
  return new Intl.DateTimeFormat(LOCALE, options).format(date)
}

export function getProductName(nameAr: string | null, nameEn: string): string {
  return nameAr?.trim() || nameEn
}

export function getCategoryName(nameAr: string | null, nameEn: string): string {
  return nameAr?.trim() || nameEn
}
