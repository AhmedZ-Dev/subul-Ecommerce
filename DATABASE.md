# AL-NABAA E-Commerce — Database Schema & Documentation
# متجر النبع للإلكترونيات — هيكلية قاعدة البيانات

---

## 📌 نظرة عامة

متجر إلكترونيات عراقي متخصص في الحواسيب واللابتوبات والإلكترونيات.  
قاعدة البيانات مكوّنة من **51 جدول** مقسّمة إلى 16 مجموعة وظيفية.

---

## 🚀 خارطة طريق التطوير (MVP → نمو → توسع)

> **المبدأ:** ابنِ ما تحتاجه اليوم فقط، لكن صمّم هيكل قاعدة البيانات ليستوعب الباقي غداً.

---

### ✅ المرحلة الأولى — MVP (الإطلاق الأول)
> **الهدف:** متجر يعمل بالكامل — عرض منتجات، سلة، طلب، توصيل، تحصيل نقد.  
> **الوقت المقدّر للتطوير:** 6-10 أسابيع

| # | الجدول | الوظيفة |
|---|--------|---------|
| 1 | `users` | حسابات العملاء |
| 2 | `addresses` | عناوين التوصيل |
| 3 | `admin_users` | حسابات الإدارة |
| 4 | `categories` | تصنيفات المنتجات |
| 5 | `brands` | العلامات التجارية |
| 6 | `products` | المنتجات |
| 7 | `product_images` | صور المنتجات |
| 8 | `product_variants` | متغيرات المنتج |
| 9 | `attribute_groups` | مجموعات الخصائص التقنية |
| 10 | `attributes` | الخصائص التقنية (RAM, GPU...) |
| 11 | `product_attribute_values` | قيم الخصائص لكل منتج |
| 12 | `carts` | سلة المشتريات |
| 13 | `cart_items` | محتويات السلة |
| 14 | `orders` | الطلبات |
| 15 | `order_items` | تفاصيل الطلبات |
| 16 | `order_status_history` | تتبع تغييرات حالة الطلب |
| 17 | `shipping_zones` | مناطق الشحن (محافظات العراق) |
| 18 | `shipping_rates` | تسعيرة الشحن + حد الشحن المجاني |
| 19 | `payment_methods` | طرق الدفع (COD فقط الآن) |
| 20 | `payment_transactions` | سجل عمليات الدفع |
| 21 | `delivery_agents` | مندوبو التوصيل |
| 22 | `order_deliveries` | تعيين الطلبات للمندوبين |
| 23 | `cash_collections` | تحصيل النقد اليومي ومطابقته |
| 24 | `banners` | بنرات الصفحة الرئيسية |
| 25 | `pages` | الصفحات الثابتة (من نحن، سياسة الإرجاع...) |
| 26 | `notifications` | إشعارات العملاء |
| 27 | `activity_logs` | سجل نشاط المدراء |
| 28 | `settings` | إعدادات المتجر |

**المجموع: 28 جدول كافية للإطلاق ✅**

---

### 🔶 المرحلة الثانية — النمو (بعد الإطلاق بـ 1-3 أشهر)
> **الهدف:** تجربة مستخدم أفضل، تسويق، إدارة مخزون احترافية، دعم ما بعد البيع.

| # | الجدول | الوظيفة | الأولوية |
|---|--------|---------|---------|
| 29 | `collections` | مجموعات تسويقية (Top Picks, Deals...) | عالية |
| 30 | `collection_products` | ربط المنتجات بالمجموعات | عالية |
| 31 | `discounts` | الكوبونات والخصومات | عالية |
| 32 | `discount_conditions` | شروط تطبيق الخصم | عالية |
| 33 | `discount_usages` | منع إعادة استخدام الكوبون | عالية |
| 34 | `returns` | طلبات الإرجاع | عالية (ضمان سنة) |
| 35 | `return_items` | تفاصيل المرتجعات | عالية (ضمان سنة) |
| 36 | `warranty_claims` | مطالبات الضمان | عالية (ضمان سنة) |
| 37 | `reviews` | تقييمات المنتجات | متوسطة |
| 38 | `wishlists` | قائمة الأمنيات | متوسطة |
| 39 | `back_in_stock_requests` | "نبّهني عند التوفر" | متوسطة |
| 40 | `tags` | التاغات | متوسطة |
| 41 | `product_tags` | ربط المنتجات بالتاغات | متوسطة |
| 42 | `menus` | إدارة قوائم التنقل | متوسطة |
| 43 | `menu_items` | عناصر القوائم | متوسطة |
| 44 | `contact_messages` | نموذج التواصل | منخفضة |
| 45 | `product_compares` | مقارنة المنتجات | منخفضة |

---

### 🔷 المرحلة الثالثة — التوسع (بعد 3-6 أشهر)
> **الهدف:** إدارة سلسلة التوريد الكاملة، عروض خاصة، دفع إلكتروني.

| # | الجدول | الوظيفة |
|---|--------|---------|
| 46 | `suppliers` | إدارة الموردين |
| 47 | `purchase_orders` | أوامر الشراء من الموردين |
| 48 | `purchase_order_items` | تفاصيل أوامر الشراء |
| 49 | `inventory_movements` | تتبع كل حركة مخزون |
| 50 | `flash_sales` | عروض محدودة بالوقت |
| 51 | `flash_sale_products` | منتجات العرض الخاص |
| — | تفعيل الدفع الإلكتروني | إضافة بيانات في `payment_methods` فقط، الهيكل جاهز |

---

### 📊 ملخص المراحل

```
المرحلة الأولى  (MVP)    → 28 جدول → الإطلاق والبيع الفعلي
المرحلة الثانية (نمو)    → 17 جدول → تجربة أفضل + دعم ما بعد البيع
المرحلة الثالثة (توسع)   →  6 جداول → سلسلة توريد + عروض + دفع إلكتروني
─────────────────────────────────────────────────────────────
المجموع الكلي             → 51 جدول
```

> ⚠️ **ملاحظة مهمة:** حتى جداول المرحلتين الثانية والثالثة يجب تصميمها وإنشاؤها في قاعدة البيانات منذ البداية — فقط لا تبني الكود الخاص بها بعد. هذا يوفر عليك تعديل هيكل قاعدة البيانات لاحقاً.

---

## 🔄 دورة حياة المنتج الكاملة (من الشراء حتى البيع)

```
[1] المورد (Supplier)
        |
        v
[2] أمر الشراء (Purchase Order) --> تفاصيل الأمر (Purchase Order Items)
        |
        v عند الاستلام
[3] حركة مخزون - دخول (inventory_movements: type=IN)
        |
        v يُحدَّث تلقائياً
[4] مخزون المنتج (products.stock_quantity ++)
        |
        v العميل يتصفح
[5] السلة (Cart) --> محتويات السلة (Cart Items)
        |
        v إتمام الشراء
[6] الطلب (Order) --> تفاصيل الطلب (Order Items)
        |
        v تأكيد الطلب
[7] حركة مخزون - خروج (inventory_movements: type=OUT)
        |  products.stock_quantity --
        |
        |--> [8a] توصيل ناجح delivered ✅
        |
        `--> [8b] إرجاع (Return) --> تفاصيل الإرجاع (Return Items)
                     |
                     |--> حركة مخزون - إرجاع (type=RETURN_IN)
                     `--> مطالبة ضمان (Warranty Claim) إذا لزم
```

---

## 🗺️ الخريطة الهرمية للجداول

```
settings (إعدادات عامة)
    |
admin_users ---- activity_logs
    |
categories --+
brands       +--> products --> product_images
             |                --> product_variants
             |                --> product_attribute_values
attribute_groups --> attributes ---+
    |
collections --> collection_products <--- products
    |
[المخزون والموردون]
suppliers --> purchase_orders --> purchase_order_items --> products
                  |
                  v
          inventory_movements <---- order_items  (OUT عند البيع)
                              <---- purchase_order_items (IN عند الاستلام)
                              <---- return_items  (RETURN_IN عند الإرجاع)
    |
[العروض الخاصة]
flash_sales --> flash_sale_products <--- products
    |
[المستخدمون]
users --> addresses
      --> carts --> cart_items --> products
      --> orders --> order_items --> products
      --> returns --> return_items --> order_items
      --> wishlists --> products
      --> reviews --> products
      --> notifications
      --> contact_messages
    |
warranty_claims --> return_items + products
    |
shipping_zones --> shipping_rates
discounts --> discount_conditions
          --> discount_usages --> orders
    |
banners / pages (CMS)
product_compares (session-based)
```

---

## 👤 Group 1 — المستخدمون والمصادقة

### `users` — العملاء
> قلب نظام الحسابات. كل طلب وسلة ومراجعة مرتبطة بهذا الجدول.

| الحقل | النوع | الوصف |
|-------|-------|-------|
| id | bigint PK | المعرف الفريد |
| email | varchar(255) UNIQUE | البريد الإلكتروني |
| password_hash | varchar(255) | كلمة المرور مشفرة |
| first_name | varchar(100) | الاسم الأول |
| last_name | varchar(100) | الاسم الأخير |
| phone | varchar(20) | رقم الهاتف |
| accepts_marketing | boolean | قبول الإشعارات التسويقية |
| is_active | boolean | حالة الحساب |
| email_verified_at | timestamp | تاريخ تأكيد الإيميل |
| **store_credit** | **decimal(15,2)** | **رصيد المتجر (للاسترداد كرصيد بدل نقد)** |
| created_at | timestamp | تاريخ التسجيل |

---

### `addresses` — عناوين التوصيل
> العميل الواحد يمكنه حفظ أكثر من عنوان. علاقة **One-to-Many** مع `users`.

| الحقل | النوع | الوصف |
|-------|-------|-------|
| id | bigint PK | المعرف الفريد |
| user_id | bigint FK → users.id | مرجع للعميل |
| first_name | varchar(100) | اسم المستلم |
| phone | varchar(20) | هاتف المستلم |
| address1 | text | العنوان التفصيلي |
| address2 | text | تفاصيل إضافية |
| city | varchar(100) | المدينة |
| governorate | varchar(100) | المحافظة (بغداد، البصرة...) |
| country | varchar(100) | الدولة (Iraq افتراضي) |
| is_default | boolean | العنوان الافتراضي |

---

### `admin_users` — مستخدمو لوحة التحكم
> فصل صلاحيات الإدارة عن حسابات العملاء. يدعم أدوار متعددة.

| الحقل | النوع | الوصف |
|-------|-------|-------|
| id | bigint PK | المعرف الفريد |
| name | varchar(255) | الاسم الكامل |
| email | varchar(255) UNIQUE | البريد الإلكتروني |
| password_hash | varchar(255) | كلمة المرور مشفرة |
| role | varchar(50) | الدور: `super_admin / admin / staff` |
| is_active | boolean | حالة الحساب |
| last_login_at | timestamp | آخر تسجيل دخول |

---

## 📦 Group 2 — الكتالوج والمنتجات

### `categories` — التصنيفات الهرمية
> تنظيم المنتجات في شجرة تصنيفات متعددة المستويات باستخدام **Self-Join**.

```
شبكات (parent_id = NULL)
├── أجهزة الشبكات    (parent_id = 1)
├── الكابلات والتوصيلات (parent_id = 1)
├── الخوادم والتخزين  (parent_id = 1)
└── الكاميرات والمراقبة (parent_id = 1)
```

| الحقل | النوع | الوصف |
|-------|-------|-------|
| id | bigint PK | المعرف الفريد |
| parent_id | bigint FK → categories.id | التصنيف الأب (NULL = رئيسي) |
| name_en | varchar(255) | الاسم بالإنجليزية |
| name_ar | varchar(255) | الاسم بالعربية |
| slug | varchar(255) UNIQUE | المعرف النصي في الرابط |
| description_en | text | الوصف بالإنجليزية |
| description_ar | text | الوصف بالعربية |
| image_url | text | صورة التصنيف |
| banner_url | text | بنر التصنيف |
| is_active | boolean | حالة الظهور |
| sort_order | int | ترتيب العرض |
| seo_title | varchar(255) | عنوان SEO |
| seo_description | text | وصف SEO |

---

### `brands` — العلامات التجارية
> أساس الفلترة في الموقع (ASUS 40، Lenovo 56، HP 33...).

| الحقل | النوع | الوصف |
|-------|-------|-------|
| id | bigint PK | المعرف الفريد |
| name | varchar(255) | اسم العلامة التجارية |
| slug | varchar(255) UNIQUE | المعرف النصي |
| logo_url | text | صورة الشعار |
| banner_url | text | صورة البنر |
| description_en | text | الوصف بالإنجليزية |
| description_ar | text | الوصف بالعربية |
| website_url | varchar(255) | الموقع الرسمي |
| is_featured | boolean | ظهور في قسم "العلامات المميزة" |
| is_active | boolean | حالة الظهور |
| sort_order | int | ترتيب العرض |

---

### `products` ⭐ — المنتجات (الجدول الأهم)
> قلب المتجر بالكامل. كل شيء يدور حوله.

| الحقل | النوع | الوصف |
|-------|-------|-------|
| id | bigint PK | المعرف الفريد |
| category_id | bigint FK → categories.id | التصنيف |
| brand_id | bigint FK → brands.id | العلامة التجارية |
| name_en | varchar(500) | الاسم بالإنجليزية |
| name_ar | varchar(500) | الاسم بالعربية |
| slug | varchar(500) UNIQUE | المعرف النصي في الرابط |
| sku | varchar(100) UNIQUE | رمز المنتج |
| barcode | varchar(100) | الباركود |
| description_en | text | الوصف التفصيلي بالإنجليزية |
| description_ar | text | الوصف التفصيلي بالعربية |
| short_description_en | text | الوصف المختصر بالإنجليزية |
| short_description_ar | text | الوصف المختصر بالعربية |
| price | decimal(15,2) | السعر الحالي |
| compare_at_price | decimal(15,2) | السعر قبل الخصم (الشطب) |
| cost_price | decimal(15,2) | سعر التكلفة (لحساب الربح) |
| currency | varchar(10) | العملة (IQD افتراضي) |
| stock_quantity | int | الكمية المتوفرة |
| low_stock_threshold | int | حد التنبيه بنقص المخزون |
| min_order_quantity | int | الحد الأدنى للطلب |
| weight | decimal(8,2) | الوزن |
| status | varchar(20) | `active / draft / archived` |
| is_featured | boolean | ظهور في قسم المميز |
| requires_shipping | boolean | يحتاج شحن |
| warranty_months | int | مدة الضمان بالشهور (12 افتراضي) |
| warranty_description | text | تفاصيل الضمان |
| total_sold | int | إجمالي الوحدات المباعة |
| views_count | int | عدد المشاهدات |
| meta_title | varchar(255) | عنوان SEO |
| meta_description | text | وصف SEO |
| ~~tags~~ | ~~text~~ | **محذوف — تم الانتقال إلى جدول `product_tags` (Many-to-Many)** |

---

### `product_images` — صور المنتجات
> كل منتج له صور متعددة. `is_primary` يحدد الصورة الرئيسية في القوائم.

| الحقل | النوع | الوصف |
|-------|-------|-------|
| id | bigint PK | المعرف الفريد |
| product_id | bigint FK → products.id | المنتج |
| variant_id | bigint FK → product_variants.id | المتغير (اختياري) |
| image_url | text | رابط الصورة |
| alt_text | varchar(255) | النص البديل للصورة |
| sort_order | int | ترتيب الصورة في المعرض |
| is_primary | boolean | الصورة الرئيسية |

---

### `product_variants` — متغيرات المنتج
> نفس المنتج بمواصفات مختلفة (ذاكرة، تخزين، لون) بأسعار مختلفة.

**مثال:** ASUS TUF Gaming F16
- متغير 1: 16GB RAM / 512GB SSD → 1,625,000 IQD
- متغير 2: 32GB RAM / 1TB SSD   → 2,100,000 IQD

| الحقل | النوع | الوصف |
|-------|-------|-------|
| id | bigint PK | المعرف الفريد |
| product_id | bigint FK → products.id | المنتج الأب |
| title | varchar(255) | وصف المتغير (16GB / 512GB / Gray) |
| sku | varchar(100) UNIQUE | رمز المتغير |
| barcode | varchar(100) | الباركود |
| price | decimal(15,2) | سعر هذا المتغير |
| compare_at_price | decimal(15,2) | السعر قبل الخصم |
| cost_price | decimal(15,2) | سعر التكلفة |
| stock_quantity | int | مخزون هذا المتغير |
| weight | decimal(8,2) | الوزن |
| is_active | boolean | حالة الظهور |
| sort_order | int | ترتيب العرض |

---

## 🔧 Group 3 — الخصائص التقنية (الفلاتر)

> نظام مرن لإدارة الخصائص التقنية دون تعديل هيكل قاعدة البيانات.  
> يُمكّن الفلاتر الديناميكية في صفحات الكتالوج.

### `attribute_groups` — مجموعات الخصائص

**أمثلة على المجموعات:**
- **المعالج** → (نوع المعالج، الجيل، السرعة، عدد الأنوية)
- **الشاشة** → (الحجم، الدقة، معدل التحديث، نوع اللوحة، شاشة لمس)
- **الذاكرة** → (RAM، التخزين، نوع التخزين)
- **الرسوميات** → (GPU، ذاكرة الرسوميات)

| الحقل | النوع | الوصف |
|-------|-------|-------|
| id | bigint PK | المعرف الفريد |
| name_en | varchar(100) | الاسم بالإنجليزية |
| name_ar | varchar(100) | الاسم بالعربية |
| slug | varchar(100) UNIQUE | المعرف النصي |
| sort_order | int | ترتيب الظهور |
| is_filterable | boolean | يظهر كفلتر في الكتالوج |

---

### `attributes` — الخصائص التقنية

**الفلاتر الموجودة في الموقع:**

| الخاصية | المجموعة | قابل للفلترة |
|---------|----------|-------------|
| RAM | الذاكرة | ✅ |
| Storage Capacity | التخزين | ✅ |
| Storage Type | التخزين | ✅ |
| Display Size | الشاشة | ✅ |
| Monitor Resolution | الشاشة | ✅ |
| GPU | الرسوميات | ✅ |
| Graphic Card Memory | الرسوميات | ✅ |
| Processor | المعالج | ✅ |
| Touch Screen | الشاشة | ✅ |

| الحقل | النوع | الوصف |
|-------|-------|-------|
| id | bigint PK | المعرف الفريد |
| group_id | bigint FK → attribute_groups.id | المجموعة |
| name_en | varchar(255) | الاسم بالإنجليزية |
| name_ar | varchar(255) | الاسم بالعربية |
| slug | varchar(255) UNIQUE | المعرف النصي |
| unit | varchar(50) | الوحدة (GB، inch، Hz) |
| input_type | varchar(50) | النوع: `text / select / boolean / number` |
| is_filterable | boolean | يظهر كفلتر |
| sort_order | int | ترتيب الظهور |

---

### `product_attribute_values` — قيم الخصائص لكل منتج
> ربط المنتج بقيم خصائصه التقنية الفعلية.

**مثال (ASUS TUF Gaming F16):**
```
product_id: 101
├── RAM              → value_text: "16GB"
├── Storage          → value_text: "512GB SSD"
├── Display Size     → value_text: "16 inch"
├── Resolution       → value_text: "1920x1200 WUXGA 165Hz"
├── GPU              → value_text: "NVIDIA RTX 5050"
├── GPU Memory       → value_text: "8GB"
├── Processor        → value_text: "Core i5-13450HX"
└── Touch Screen     → value_boolean: false
```

| الحقل | النوع | الوصف |
|-------|-------|-------|
| id | bigint PK | المعرف الفريد |
| product_id | bigint FK → products.id | المنتج |
| attribute_id | bigint FK → attributes.id | الخاصية |
| value_text | varchar(500) | قيمة نصية |
| value_number | decimal(10,2) | قيمة رقمية |
| value_boolean | boolean | قيمة منطقية (نعم/لا) |

---

## 🗂️ Group 4 — المجموعات التسويقية

### `collections` — المجموعات
> تختلف عن التصنيفات — هي مجموعات تسويقية مرنة يمكن إنشاؤها يدوياً أو تلقائياً.

| نوع | مثال |
|-----|------|
| `categories` | لابتوبات ← ألعاب (هيكل ثابت وهرمي) |
| `collections` | "Top Picks from ADATA"، "عروض حصرية"، "الأكثر مبيعاً" (مرن) |

| الحقل | النوع | الوصف |
|-------|-------|-------|
| id | bigint PK | المعرف الفريد |
| name_en | varchar(255) | الاسم بالإنجليزية |
| name_ar | varchar(255) | الاسم بالعربية |
| slug | varchar(255) UNIQUE | المعرف النصي في الرابط |
| description_en | text | الوصف بالإنجليزية |
| description_ar | text | الوصف بالعربية |
| image_url | text | صورة المجموعة |
| banner_url | text | بنر المجموعة |
| collection_type | varchar(50) | `manual / smart` |
| is_active | boolean | حالة الظهور |
| sort_order | int | ترتيب العرض |

---

### `collection_products` — ربط المنتجات بالمجموعات
> جدول علاقة **Many-to-Many**. المنتج الواحد يظهر في أكثر من مجموعة.

```
ASUS TUF Gaming F16 يظهر في:
├── مجموعة "لابتوبات الألعاب"
├── مجموعة "أفضل المبيعات"
└── مجموعة "مجموعة ASUS"
```

| الحقل | النوع | الوصف |
|-------|-------|-------|
| id | bigint PK | المعرف الفريد |
| collection_id | bigint FK → collections.id | المجموعة |
| product_id | bigint FK → products.id | المنتج |
| sort_order | int | ترتيب المنتج داخل المجموعة |

---

## 🛒 Group 5 — السلة والطلبات

### `carts` — سلة المشتريات
> تحفظ السلة حتى للزوار غير المسجلين (`user_id = NULL`).  
> عند تسجيل الدخول تُدمج سلة الزائر مع حسابه تلقائياً.

| الحقل | النوع | الوصف |
|-------|-------|-------|
| id | bigint PK | المعرف الفريد |
| user_id | bigint FK → users.id | العميل (NULL للزوار) |
| session_id | varchar(255) | معرف جلسة الزائر |
| coupon_code | varchar(100) | كوبون مطبّق |
| notes | text | ملاحظات الطلب |
| expires_at | timestamp | تاريخ انتهاء السلة المهجورة |

---

### `cart_items` — محتويات السلة
> `unit_price` يُحفظ وقت الإضافة لمعرفة إذا تغيّر السعر لاحقاً وتنبيه العميل.

| الحقل | النوع | الوصف |
|-------|-------|-------|
| id | bigint PK | المعرف الفريد |
| cart_id | bigint FK → carts.id | السلة |
| product_id | bigint FK → products.id | المنتج |
| variant_id | bigint FK → product_variants.id | المتغير (اختياري) |
| quantity | int | الكمية |
| unit_price | decimal(15,2) | السعر وقت الإضافة |

---

### `orders` ⭐ — الطلبات

**دورة حياة الطلب:**
```
pending → confirmed → processing → shipped → out_for_delivery → delivered
                                                   ↘ cancelled / refunded
```

| الحقل | النوع | الوصف |
|-------|-------|-------|
| id | bigint PK | المعرف الفريد |
| user_id | bigint FK → users.id | العميل (NULL للضيوف) |
| order_number | varchar(50) UNIQUE | رقم الطلب المعروض للعميل |
| status | varchar(50) | حالة الطلب |
| payment_status | varchar(50) | حالة الدفع: `pending / paid / refunded` |
| fulfillment_status | varchar(50) | حالة التوصيل: `unfulfilled / partial / fulfilled` |
| subtotal | decimal(15,2) | المجموع قبل الخصم والشحن |
| discount_amount | decimal(15,2) | مبلغ الخصم |
| shipping_amount | decimal(15,2) | تكلفة الشحن |
| tax_amount | decimal(15,2) | الضريبة |
| total | decimal(15,2) | المجموع النهائي |
| currency | varchar(10) | العملة (IQD) |
| discount_id | bigint FK → discounts.id | الخصم المطبّق |
| coupon_code | varchar(100) | كود الكوبون |
| shipping_first_name | varchar(100) | اسم المستلم |
| shipping_last_name | varchar(100) | لقب المستلم |
| shipping_phone | varchar(20) | هاتف المستلم |
| shipping_address1 | text | العنوان |
| shipping_city | varchar(100) | المدينة |
| shipping_governorate | varchar(100) | المحافظة |
| shipping_country | varchar(100) | الدولة |
| shipping_zone_id | bigint FK → shipping_zones.id | منطقة الشحن |
| payment_method | varchar(100) | طريقة الدفع: `cod / bank_transfer / online` |
| tracking_number | varchar(255) | رقم تتبع الشحنة |
| notes | text | ملاحظات داخلية للإدارة |
| customer_notes | text | ملاحظة العميل على الطلب |
| cancelled_at | timestamp | تاريخ الإلغاء |
| cancel_reason | text | سبب الإلغاء |
| shipped_at | timestamp | تاريخ الشحن |
| delivered_at | timestamp | تاريخ الاستلام |

> ⚠️ **ملاحظة مهمة:** عنوان الشحن يُحفظ مباشرة في جدول `orders` وليس كمرجع فقط،  
> لأن العميل قد يغير عنوانه لاحقاً ونريد الحفاظ على دقة السجل التاريخي.

---

### `order_items` — تفاصيل الطلب
> `product_name` يُحفظ كنص ثابت لحماية السجل التاريخي حتى لو حُذف المنتج مستقبلاً.

| الحقل | النوع | الوصف |
|-------|-------|-------|
| id | bigint PK | المعرف الفريد |
| order_id | bigint FK → orders.id | الطلب |
| product_id | bigint FK → products.id | المنتج |
| variant_id | bigint FK → product_variants.id | المتغير |
| product_name | varchar(500) | اسم المنتج (snapshot) |
| sku | varchar(100) | رمز المنتج (snapshot) |
| quantity | int | الكمية |
| unit_price | decimal(15,2) | سعر الوحدة |
| compare_at_price | decimal(15,2) | السعر قبل الخصم |
| discount_amount | decimal(15,2) | الخصم على هذا البند |
| total_price | decimal(15,2) | الإجمالي |
| warranty_months | int | مدة الضمان (12 افتراضي) |

---

## 🚚 Group 6 — الشحن والتوصيل

### `shipping_zones` — مناطق الشحن
> تقسيم العراق إلى مناطق بتكاليف وأوقات توصيل مختلفة.

**مثال:**
| المنطقة | المحافظات | وقت التوصيل |
|---------|-----------|------------|
| المنطقة 1 | بغداد | يوم واحد |
| المنطقة 2 | البصرة، أربيل، الموصل، النجف | 2-3 أيام |
| المنطقة 3 | المحافظات النائية | 3-5 أيام |

| الحقل | النوع | الوصف |
|-------|-------|-------|
| id | bigint PK | المعرف الفريد |
| name_en | varchar(100) | اسم المنطقة بالإنجليزية |
| name_ar | varchar(100) | اسم المنطقة بالعربية |
| governorates | text | JSON array بأسماء المحافظات |
| is_active | boolean | حالة المنطقة |

---

### `shipping_rates` — أسعار الشحن
> يتضمن `free_shipping_threshold = 100,000 IQD` وهو حد الشحن المجاني الموجود في الموقع.

| الحقل | النوع | الوصف |
|-------|-------|-------|
| id | bigint PK | المعرف الفريد |
| shipping_zone_id | bigint FK → shipping_zones.id | منطقة الشحن |
| name_en | varchar(100) | اسم التعريفة |
| name_ar | varchar(100) | اسم التعريفة بالعربية |
| rate_type | varchar(20) | `flat / weight_based / price_based` |
| price | decimal(15,2) | سعر الشحن |
| min_order_value | decimal(15,2) | الحد الأدنى للطلب |
| max_order_value | decimal(15,2) | الحد الأقصى للطلب |
| free_shipping_threshold | decimal(15,2) | حد الشحن المجاني |
| estimated_days_min | int | أقل وقت للتوصيل |
| estimated_days_max | int | أقصى وقت للتوصيل |
| is_active | boolean | حالة التعريفة |

---

## 🎁 Group 7 — الخصومات والكوبونات

### `discounts` — الخصومات

| نوع الخصم | وصف | مثال |
|-----------|-----|------|
| `percentage` | نسبة مئوية | خصم 20% |
| `fixed_amount` | مبلغ ثابت | خصم 50,000 IQD |
| `free_shipping` | شحن مجاني | — |
| `buy_x_get_y` | اشترِ X احصل على Y | اشترِ 2 واحصل على 1 |

| الحقل | النوع | الوصف |
|-------|-------|-------|
| id | bigint PK | المعرف الفريد |
| code | varchar(100) UNIQUE | كود الكوبون |
| name | varchar(255) | الاسم الداخلي |
| type | varchar(50) | نوع الخصم |
| value | decimal(15,2) | قيمة الخصم |
| min_order_value | decimal(15,2) | الحد الأدنى للطلب |
| usage_limit | int | حد الاستخدام الكلي (NULL = لا حد) |
| usage_count | int | عدد مرات الاستخدام الحالية |
| per_customer_limit | int | حد الاستخدام لكل عميل |
| starts_at | timestamp | تاريخ بداية الصلاحية |
| ends_at | timestamp | تاريخ انتهاء الصلاحية |
| is_active | boolean | حالة الكوبون |

---

### `discount_conditions` — شروط تطبيق الخصم
> تحدد على ماذا يُطبَّق الكوبون تحديداً.

**مثال:** كوبون يعمل فقط على:
- منتجات ASUS
- مجموعة "لابتوبات الألعاب"
- عملاء VIP محددين

| الحقل | النوع | الوصف |
|-------|-------|-------|
| id | bigint PK | المعرف الفريد |
| discount_id | bigint FK → discounts.id | الخصم |
| entity_type | varchar(50) | `product / collection / brand / category` |
| entity_id | bigint | معرف الكيان المحدد |

---

### `discount_usages` — سجل استخدام الخصومات
> يمنع إعادة استخدام الكوبون ويُتيح تحليل نجاح الحملات التسويقية.

| الحقل | النوع | الوصف |
|-------|-------|-------|
| id | bigint PK | المعرف الفريد |
| discount_id | bigint FK → discounts.id | الخصم |
| order_id | bigint FK → orders.id | الطلب |
| user_id | bigint FK → users.id | العميل |
| amount_saved | decimal(15,2) | المبلغ الموفَّر |
| used_at | timestamp | وقت الاستخدام |

---

## ⭐ Group 8 — تفاعل العميل

### `reviews` — التقييمات والمراجعات
> `is_verified_purchase` يميز مراجعات المشترين الفعليين عن غيرهم.

| الحقل | النوع | الوصف |
|-------|-------|-------|
| id | bigint PK | المعرف الفريد |
| product_id | bigint FK → products.id | المنتج |
| user_id | bigint FK → users.id | العميل |
| order_item_id | bigint FK → order_items.id | بند الطلب (للتحقق من الشراء) |
| rating | tinyint | التقييم من 1 إلى 5 نجوم |
| title | varchar(255) | عنوان المراجعة |
| body | text | نص المراجعة |
| status | varchar(20) | `pending / approved / rejected` |
| is_verified_purchase | boolean | هل العميل اشترى المنتج فعلاً؟ |
| helpful_count | int | عدد من وجد المراجعة مفيدة |

---

### `wishlists` — قائمة الأمنيات
> مصدر بيانات ذهبي للتسويق: إذا انخفض سعر منتج في قائمة أمنيات عميل → إشعار فوري.

| الحقل | النوع | الوصف |
|-------|-------|-------|
| id | bigint PK | المعرف الفريد |
| user_id | bigint FK → users.id | العميل |
| product_id | bigint FK → products.id | المنتج |
| created_at | timestamp | تاريخ الإضافة |

---

### `product_compares` — مقارنة المنتجات
> يدعم مقارنة حتى 4 منتجات جنباً لجنب (كما في الموقع).  
> يعمل مع الزوار غير المسجلين عبر `session_id`.

| الحقل | النوع | الوصف |
|-------|-------|-------|
| id | bigint PK | المعرف الفريد |
| session_id | varchar(255) | معرف جلسة الزائر |
| user_id | bigint FK → users.id | العميل (اختياري) |
| product_id | bigint FK → products.id | المنتج المضاف للمقارنة |
| created_at | timestamp | تاريخ الإضافة |

---

## 🎨 Group 9 — إدارة المحتوى (CMS)

### `banners` — البنرات الترويجية
> `starts_at` و `ends_at` يجعل البنر يظهر ويختفي **تلقائياً** حسب توقيت العرض.

| الحقل | النوع | الوصف |
|-------|-------|-------|
| id | bigint PK | المعرف الفريد |
| title_en | varchar(255) | العنوان بالإنجليزية |
| title_ar | varchar(255) | العنوان بالعربية |
| subtitle_en | varchar(500) | العنوان الفرعي بالإنجليزية |
| subtitle_ar | varchar(500) | العنوان الفرعي بالعربية |
| image_url | text | صورة سطح المكتب |
| mobile_image_url | text | صورة الجوال |
| link_url | text | الرابط عند الضغط |
| button_text_en | varchar(100) | نص الزر بالإنجليزية |
| button_text_ar | varchar(100) | نص الزر بالعربية |
| position | varchar(50) | الموضع: `hero / collection_top / sidebar / popup` |
| sort_order | int | ترتيب العرض |
| starts_at | timestamp | بداية العرض |
| ends_at | timestamp | نهاية العرض |
| is_active | boolean | حالة الظهور |

---

### `pages` — الصفحات الثابتة
> صفحات "من نحن"، "سياسة الإرجاع"، "الشروط والأحكام" — قابلة للتعديل من لوحة التحكم.

| الحقل | النوع | الوصف |
|-------|-------|-------|
| id | bigint PK | المعرف الفريد |
| title_en | varchar(255) | العنوان بالإنجليزية |
| title_ar | varchar(255) | العنوان بالعربية |
| slug | varchar(255) UNIQUE | المعرف النصي في الرابط |
| content_en | text | المحتوى بالإنجليزية (HTML) |
| content_ar | text | المحتوى بالعربية (HTML) |
| is_published | boolean | حالة النشر |
| meta_title | varchar(255) | عنوان SEO |
| meta_description | text | وصف SEO |

---

## 📊 Group 10 — المراقبة والإعدادات

### `notifications` — إشعارات العملاء

| نوع الإشعار | الحدث المُشغِّل |
|------------|----------------|
| `order_confirmed` | تأكيد الطلب |
| `order_shipped` | شحن الطلب |
| `order_delivered` | استلام الطلب |
| `price_drop` | انخفاض سعر منتج في قائمة الأمنيات |
| `back_in_stock` | عودة منتج للمخزون |
| `order_cancelled` | إلغاء الطلب |

| الحقل | النوع | الوصف |
|-------|-------|-------|
| id | bigint PK | المعرف الفريد |
| user_id | bigint FK → users.id | العميل |
| type | varchar(100) | نوع الإشعار |
| title | varchar(255) | عنوان الإشعار |
| body | text | نص الإشعار |
| data | text | بيانات إضافية (JSON) |
| read_at | timestamp | وقت القراءة (NULL = غير مقروء) |

---

### `activity_logs` — سجل نشاط المدراء
> يسجل كل إجراء يقوم به المدير مع `old_values` و `new_values` بصيغة JSON.  
> **ضرورة أمنية:** إذا حُذف منتج بالخطأ، نعرف من فعل ذلك ومتى وما كانت قيمه.

| الحقل | النوع | الوصف |
|-------|-------|-------|
| id | bigint PK | المعرف الفريد |
| admin_user_id | bigint FK → admin_users.id | المدير المنفّذ |
| action | varchar(100) | الإجراء: `create / update / delete` |
| entity_type | varchar(100) | نوع الكيان: `product / order / user` |
| entity_id | bigint | معرف الكيان |
| old_values | text | القيم قبل التغيير (JSON) |
| new_values | text | القيم بعد التغيير (JSON) |
| ip_address | varchar(50) | عنوان IP |

---

### `settings` — الإعدادات العامة
> كل إعدادات المتجر قابلة للتغيير من لوحة التحكم بدون تعديل الكود.

**أمثلة على الإعدادات:**
```
free_shipping_threshold = 100000
whatsapp_number         = +964 774 802 5119
support_phone           = 6543
store_name_ar           = النبع
store_name_en           = AL-NABAA
default_currency        = IQD
warranty_default_months = 12
```

| الحقل | النوع | الوصف |
|-------|-------|-------|
| id | bigint PK | المعرف الفريد |
| key | varchar(255) UNIQUE | مفتاح الإعداد |
| value | text | قيمة الإعداد |
| type | varchar(50) | `string / integer / boolean / json` |
| group | varchar(100) | `general / shipping / payment / seo / social` |

---

## 🗄️ هيكلية dbdiagram.io الكاملة

> انسخ الكود التالي وألصقه في [dbdiagram.io](https://dbdiagram.io) للحصول على المخطط البصري.

```sql
Table users {
  id                bigint        [pk, increment]
  email             varchar(255)  [unique, not null]
  password_hash     varchar(255)
  first_name        varchar(100)
  last_name         varchar(100)
  phone             varchar(20)
  accepts_marketing boolean       [default: false]
  is_active         boolean       [default: true]
  email_verified_at timestamp
  store_credit      decimal(15,2) [default: 0, note: 'رصيد المتجر للاسترداد']
  notes             text
  created_at        timestamp     [default: `now()`]
  updated_at        timestamp
}

Table addresses {
  id          bigint       [pk, increment]
  user_id     bigint       [ref: > users.id, not null]
  first_name  varchar(100)
  last_name   varchar(100)
  phone       varchar(20)
  address1    text         [not null]
  address2    text
  city        varchar(100)
  governorate varchar(100)
  country     varchar(100) [default: 'Iraq']
  is_default  boolean      [default: false]
  created_at  timestamp    [default: `now()`]
  updated_at  timestamp
}

Table admin_users {
  id            bigint       [pk, increment]
  name          varchar(255) [not null]
  email         varchar(255) [unique, not null]
  password_hash varchar(255) [not null]
  role          varchar(50)  [default: 'staff']
  is_active     boolean      [default: true]
  last_login_at timestamp
  created_at    timestamp    [default: `now()`]
  updated_at    timestamp
}

Table categories {
  id              bigint       [pk, increment]
  parent_id       bigint       [ref: > categories.id]
  name_en         varchar(255) [not null]
  name_ar         varchar(255)
  slug            varchar(255) [unique, not null]
  description_en  text
  description_ar  text
  image_url       text
  banner_url      text
  sort_order      int          [default: 0]
  is_active       boolean      [default: true]
  seo_title       varchar(255)
  seo_description text
  created_at      timestamp    [default: `now()`]
  updated_at      timestamp
}

Table brands {
  id             bigint       [pk, increment]
  name           varchar(255) [not null]
  slug           varchar(255) [unique, not null]
  logo_url       text
  banner_url     text
  description_en text
  description_ar text
  website_url    varchar(255)
  is_featured    boolean      [default: false]
  is_active      boolean      [default: true]
  sort_order     int          [default: 0]
  created_at     timestamp    [default: `now()`]
  updated_at     timestamp
}

Table products {
  id                   bigint        [pk, increment]
  category_id          bigint        [ref: > categories.id]
  brand_id             bigint        [ref: > brands.id]
  name_en              varchar(500)  [not null]
  name_ar              varchar(500)
  slug                 varchar(500)  [unique, not null]
  sku                  varchar(100)  [unique]
  barcode              varchar(100)
  description_en       text
  description_ar       text
  short_description_en text
  short_description_ar text
  price                decimal(15,2) [not null]
  compare_at_price     decimal(15,2)
  cost_price           decimal(15,2)
  currency             varchar(10)   [default: 'IQD']
  stock_quantity       int           [default: 0]
  low_stock_threshold  int           [default: 5]
  min_order_quantity   int           [default: 1]
  weight               decimal(8,2)
  status               varchar(20)   [default: 'active']
  is_featured          boolean       [default: false]
  requires_shipping    boolean       [default: true]
  warranty_months      int           [default: 12]
  warranty_description text
  total_sold           int           [default: 0]
  views_count          int           [default: 0]
  meta_title           varchar(255)
  meta_description     text
  tags                 text
  created_at           timestamp     [default: `now()`]
  updated_at           timestamp
}

Table product_images {
  id         bigint    [pk, increment]
  product_id bigint    [ref: > products.id, not null]
  variant_id bigint    [ref: > product_variants.id]
  image_url  text      [not null]
  alt_text   varchar(255)
  sort_order int       [default: 0]
  is_primary boolean   [default: false]
  created_at timestamp [default: `now()`]
}

Table product_variants {
  id               bigint        [pk, increment]
  product_id       bigint        [ref: > products.id, not null]
  title            varchar(255)
  sku              varchar(100)  [unique]
  barcode          varchar(100)
  price            decimal(15,2)
  compare_at_price decimal(15,2)
  cost_price       decimal(15,2)
  stock_quantity   int           [default: 0]
  weight           decimal(8,2)
  is_active        boolean       [default: true]
  sort_order       int           [default: 0]
  created_at       timestamp     [default: `now()`]
  updated_at       timestamp
}

Table attribute_groups {
  id            bigint       [pk, increment]
  name_en       varchar(100) [not null]
  name_ar       varchar(100)
  slug          varchar(100) [unique]
  sort_order    int          [default: 0]
  is_filterable boolean      [default: true]
  created_at    timestamp    [default: `now()`]
}

Table attributes {
  id            bigint       [pk, increment]
  group_id      bigint       [ref: > attribute_groups.id]
  name_en       varchar(255) [not null]
  name_ar       varchar(255)
  slug          varchar(255) [unique]
  unit          varchar(50)
  input_type    varchar(50)  [default: 'text']
  is_filterable boolean      [default: true]
  sort_order    int          [default: 0]
  created_at    timestamp    [default: `now()`]
}

Table product_attribute_values {
  id            bigint        [pk, increment]
  product_id    bigint        [ref: > products.id, not null]
  attribute_id  bigint        [ref: > attributes.id, not null]
  value_text    varchar(500)
  value_number  decimal(10,2)
  value_boolean boolean
  created_at    timestamp     [default: `now()`]
}

Table collections {
  id               bigint       [pk, increment]
  name_en          varchar(255) [not null]
  name_ar          varchar(255)
  slug             varchar(255) [unique, not null]
  description_en   text
  description_ar   text
  image_url        text
  banner_url       text
  collection_type  varchar(50)  [default: 'manual']
  is_active        boolean      [default: true]
  sort_order       int          [default: 0]
  meta_title       varchar(255)
  meta_description text
  created_at       timestamp    [default: `now()`]
  updated_at       timestamp
}

Table collection_products {
  id            bigint    [pk, increment]
  collection_id bigint    [ref: > collections.id, not null]
  product_id    bigint    [ref: > products.id, not null]
  sort_order    int       [default: 0]
  created_at    timestamp [default: `now()`]
}

Table carts {
  id          bigint        [pk, increment]
  user_id     bigint        [ref: > users.id]
  session_id  varchar(255)
  coupon_code varchar(100)
  notes       text
  expires_at  timestamp
  created_at  timestamp     [default: `now()`]
  updated_at  timestamp
}

Table cart_items {
  id         bigint        [pk, increment]
  cart_id    bigint        [ref: > carts.id, not null]
  product_id bigint        [ref: > products.id, not null]
  variant_id bigint        [ref: > product_variants.id]
  quantity   int           [not null, default: 1]
  unit_price decimal(15,2)
  created_at timestamp     [default: `now()`]
  updated_at timestamp
}

Table orders {
  id                   bigint        [pk, increment]
  user_id              bigint        [ref: > users.id]
  order_number         varchar(50)   [unique, not null]
  status               varchar(50)   [default: 'pending']
  payment_status       varchar(50)   [default: 'pending']
  fulfillment_status   varchar(50)   [default: 'unfulfilled']
  subtotal             decimal(15,2) [not null]
  discount_amount      decimal(15,2) [default: 0]
  shipping_amount      decimal(15,2) [default: 0]
  tax_amount           decimal(15,2) [default: 0]
  total                decimal(15,2) [not null]
  currency             varchar(10)   [default: 'IQD']
  discount_id          bigint        [ref: > discounts.id]
  coupon_code          varchar(100)
  shipping_first_name  varchar(100)
  shipping_last_name   varchar(100)
  shipping_phone       varchar(20)
  shipping_address1    text
  shipping_address2    text
  shipping_city        varchar(100)
  shipping_governorate varchar(100)
  shipping_country     varchar(100)  [default: 'Iraq']
  shipping_zone_id     bigint        [ref: > shipping_zones.id]
  payment_method       varchar(100)
  tracking_number      varchar(255)
  notes                text
  customer_notes       text
  cancelled_at         timestamp
  cancel_reason        text
  shipped_at           timestamp
  delivered_at         timestamp
  ip_address           varchar(50)
  created_at           timestamp     [default: `now()`]
  updated_at           timestamp
}

Table order_items {
  id               bigint        [pk, increment]
  order_id         bigint        [ref: > orders.id, not null]
  product_id       bigint        [ref: > products.id]
  variant_id       bigint        [ref: > product_variants.id]
  product_name     varchar(500)  [not null]
  sku              varchar(100)
  quantity         int           [not null]
  unit_price       decimal(15,2) [not null]
  compare_at_price decimal(15,2)
  discount_amount  decimal(15,2) [default: 0]
  total_price      decimal(15,2) [not null]
  warranty_months  int           [default: 12]
  requires_shipping boolean      [default: true]
  created_at       timestamp     [default: `now()`]
}

Table shipping_zones {
  id           bigint       [pk, increment]
  name_en      varchar(100) [not null]
  name_ar      varchar(100)
  governorates text
  is_active    boolean      [default: true]
  created_at   timestamp    [default: `now()`]
}

Table shipping_rates {
  id                      bigint        [pk, increment]
  shipping_zone_id        bigint        [ref: > shipping_zones.id, not null]
  name_en                 varchar(100)
  name_ar                 varchar(100)
  rate_type               varchar(20)   [default: 'flat']
  price                   decimal(15,2) [not null]
  min_order_value         decimal(15,2)
  max_order_value         decimal(15,2)
  free_shipping_threshold decimal(15,2)
  estimated_days_min      int
  estimated_days_max      int
  is_active               boolean       [default: true]
  created_at              timestamp     [default: `now()`]
}

Table discounts {
  id                 bigint        [pk, increment]
  code               varchar(100)  [unique]
  name               varchar(255)
  type               varchar(50)
  value              decimal(15,2)
  currency           varchar(10)   [default: 'IQD']
  min_order_value    decimal(15,2)
  min_quantity       int
  usage_limit        int
  usage_count        int           [default: 0]
  per_customer_limit int           [default: 1]
  applies_to         varchar(50)
  starts_at          timestamp
  ends_at            timestamp
  is_active          boolean       [default: true]
  created_at         timestamp     [default: `now()`]
  updated_at         timestamp
}

Table discount_conditions {
  id          bigint    [pk, increment]
  discount_id bigint    [ref: > discounts.id, not null]
  entity_type varchar(50)
  entity_id   bigint
  created_at  timestamp [default: `now()`]
}

Table discount_usages {
  id          bigint        [pk, increment]
  discount_id bigint        [ref: > discounts.id, not null]
  order_id    bigint        [ref: > orders.id, not null]
  user_id     bigint        [ref: > users.id]
  amount_saved decimal(15,2)
  used_at     timestamp     [default: `now()`]
}

Table reviews {
  id                   bigint    [pk, increment]
  product_id           bigint    [ref: > products.id, not null]
  user_id              bigint    [ref: > users.id, not null]
  order_item_id        bigint    [ref: > order_items.id]
  rating               tinyint   [not null]
  title                varchar(255)
  body                 text
  status               varchar(20) [default: 'pending']
  is_verified_purchase boolean   [default: false]
  helpful_count        int       [default: 0]
  created_at           timestamp [default: `now()`]
  updated_at           timestamp
}

Table wishlists {
  id         bigint    [pk, increment]
  user_id    bigint    [ref: > users.id, not null]
  product_id bigint    [ref: > products.id, not null]
  created_at timestamp [default: `now()`]
}

Table product_compares {
  id         bigint       [pk, increment]
  session_id varchar(255)
  user_id    bigint       [ref: > users.id]
  product_id bigint       [ref: > products.id, not null]
  created_at timestamp    [default: `now()`]
}

Table banners {
  id               bigint       [pk, increment]
  title_en         varchar(255)
  title_ar         varchar(255)
  subtitle_en      varchar(500)
  subtitle_ar      varchar(500)
  image_url        text         [not null]
  mobile_image_url text
  link_url         text
  button_text_en   varchar(100)
  button_text_ar   varchar(100)
  position         varchar(50)
  sort_order       int          [default: 0]
  starts_at        timestamp
  ends_at          timestamp
  is_active        boolean      [default: true]
  created_at       timestamp    [default: `now()`]
}

Table pages {
  id               bigint       [pk, increment]
  title_en         varchar(255) [not null]
  title_ar         varchar(255)
  slug             varchar(255) [unique, not null]
  content_en       text
  content_ar       text
  is_published     boolean      [default: true]
  meta_title       varchar(255)
  meta_description text
  created_at       timestamp    [default: `now()`]
  updated_at       timestamp
}

Table notifications {
  id         bigint       [pk, increment]
  user_id    bigint       [ref: > users.id, not null]
  type       varchar(100) [not null]
  title      varchar(255)
  body       text
  data       text
  read_at    timestamp
  created_at timestamp    [default: `now()`]
}

Table activity_logs {
  id            bigint       [pk, increment]
  admin_user_id bigint       [ref: > admin_users.id]
  action        varchar(100) [not null]
  entity_type   varchar(100)
  entity_id     bigint
  old_values    text
  new_values    text
  ip_address    varchar(50)
  created_at    timestamp    [default: `now()`]
}

Table settings {
  id         bigint       [pk, increment]
  key        varchar(255) [unique, not null]
  value      text
  type       varchar(50)  [default: 'string']
  group      varchar(100)
  created_at timestamp    [default: `now()`]
  updated_at timestamp
}

// ─────────────────────────────────────────────
// GROUP 11 — SUPPLIERS & INVENTORY
// ─────────────────────────────────────────────

Table suppliers {
  id            bigint       [pk, increment]
  name          varchar(255) [not null]
  company_name  varchar(255)
  email         varchar(255)
  phone         varchar(20)
  whatsapp      varchar(20)
  country       varchar(100)
  city          varchar(100)
  address       text
  payment_terms varchar(255)
  currency      varchar(10)  [default: 'USD']
  notes         text
  is_active     boolean      [default: true]
  created_at    timestamp    [default: `now()`]
  updated_at    timestamp
}

Table purchase_orders {
  id              bigint        [pk, increment]
  supplier_id     bigint        [ref: > suppliers.id, not null]
  po_number       varchar(50)   [unique, not null]
  status          varchar(50)   [default: 'draft']
  order_date      date
  expected_date   date
  received_date   date
  subtotal        decimal(15,2)
  tax_amount      decimal(15,2) [default: 0]
  shipping_cost   decimal(15,2) [default: 0]
  total           decimal(15,2)
  currency        varchar(10)   [default: 'USD']
  exchange_rate   decimal(10,4) [default: 1]
  payment_status  varchar(50)   [default: 'unpaid']
  paid_amount     decimal(15,2) [default: 0]
  notes           text
  admin_user_id   bigint        [ref: > admin_users.id]
  created_at      timestamp     [default: `now()`]
  updated_at      timestamp
}

Table purchase_order_items {
  id                 bigint        [pk, increment]
  purchase_order_id  bigint        [ref: > purchase_orders.id, not null]
  product_id         bigint        [ref: > products.id]
  variant_id         bigint        [ref: > product_variants.id]
  sku                varchar(100)
  product_name       varchar(500)
  quantity_ordered   int           [not null]
  quantity_received  int           [default: 0]
  unit_cost          decimal(15,2) [not null]
  total_cost         decimal(15,2)
  notes              text
  created_at         timestamp     [default: `now()`]
}

Table inventory_movements {
  id              bigint        [pk, increment]
  product_id      bigint        [ref: > products.id, not null]
  variant_id      bigint        [ref: > product_variants.id]
  movement_type   varchar(50)   [not null, note: 'PURCHASE_IN, SALE_OUT, RETURN_IN, ADJUSTMENT_IN, ADJUSTMENT_OUT, DAMAGE_OUT']
  quantity        int           [not null]
  quantity_before int           [not null]
  quantity_after  int           [not null]
  reference_type  varchar(50)   [note: 'purchase_order, order, return, manual']
  reference_id    bigint
  unit_cost       decimal(15,2)
  notes           text
  admin_user_id   bigint        [ref: > admin_users.id]
  created_at      timestamp     [default: `now()`]
}

// ─────────────────────────────────────────────
// GROUP 12 — RETURNS & WARRANTY
// ─────────────────────────────────────────────

Table returns {
  id             bigint        [pk, increment]
  return_number  varchar(50)   [unique, not null]
  order_id       bigint        [ref: > orders.id, not null]
  user_id        bigint        [ref: > users.id, not null]
  return_type    varchar(50)   [note: 'return, exchange, warranty_repair']
  status         varchar(50)   [default: 'requested']
  reason         varchar(255)
  reason_details text
  refund_amount  decimal(15,2)
  refund_method  varchar(50)   [note: 'original_payment, store_credit, bank_transfer']
  refund_status  varchar(50)   [default: 'pending']
  admin_notes    text
  reviewed_by    bigint        [ref: > admin_users.id]
  reviewed_at    timestamp
  received_at    timestamp
  created_at     timestamp     [default: `now()`]
  updated_at     timestamp
}

Table return_items {
  id            bigint        [pk, increment]
  return_id     bigint        [ref: > returns.id, not null]
  order_item_id bigint        [ref: > order_items.id, not null]
  product_id    bigint        [ref: > products.id]
  variant_id    bigint        [ref: > product_variants.id]
  quantity      int           [not null]
  unit_price    decimal(15,2)
  refund_amount decimal(15,2)
  condition     varchar(50)   [note: 'new, good, damaged, defective']
  return_to_stock boolean     [default: false]
  notes         text
  created_at    timestamp     [default: `now()`]
}

Table warranty_claims {
  id                  bigint       [pk, increment]
  claim_number        varchar(50)  [unique, not null]
  order_item_id       bigint       [ref: > order_items.id, not null]
  product_id          bigint       [ref: > products.id]
  user_id             bigint       [ref: > users.id, not null]
  return_id           bigint       [ref: > returns.id]
  issue_description   text         [not null]
  status              varchar(50)  [default: 'submitted']
  resolution          varchar(50)  [note: 'repair, replacement, refund']
  warranty_expires_at date
  received_at         timestamp
  resolved_at         timestamp
  admin_notes         text
  handled_by          bigint       [ref: > admin_users.id]
  created_at          timestamp    [default: `now()`]
  updated_at          timestamp
}

// ─────────────────────────────────────────────
// GROUP 13 — FLASH SALES
// ─────────────────────────────────────────────

Table flash_sales {
  id              bigint        [pk, increment]
  name_en         varchar(255)  [not null]
  name_ar         varchar(255)
  description_en  text
  description_ar  text
  banner_url      text
  discount_type   varchar(50)   [note: 'percentage, fixed_amount']
  discount_value  decimal(10,2)
  starts_at       timestamp     [not null]
  ends_at         timestamp     [not null]
  is_active       boolean       [default: true]
  max_uses        int
  uses_count      int           [default: 0]
  sort_order      int           [default: 0]
  created_by      bigint        [ref: > admin_users.id]
  created_at      timestamp     [default: `now()`]
  updated_at      timestamp
}

Table flash_sale_products {
  id                    bigint        [pk, increment]
  flash_sale_id         bigint        [ref: > flash_sales.id, not null]
  product_id            bigint        [ref: > products.id, not null]
  variant_id            bigint        [ref: > product_variants.id]
  sale_price            decimal(15,2) [not null]
  original_price        decimal(15,2)
  max_quantity_per_order int
  quantity_limit        int
  quantity_sold         int           [default: 0]
  sort_order            int           [default: 0]
  created_at            timestamp     [default: `now()`]
}

// ─────────────────────────────────────────────
// GROUP 14 — CONTACT MESSAGES
// ─────────────────────────────────────────────

Table contact_messages {
  id              bigint       [pk, increment]
  user_id         bigint       [ref: > users.id]
  name            varchar(255) [not null]
  email           varchar(255) [not null]
  phone           varchar(20)
  subject         varchar(255)
  message         text         [not null]
  status          varchar(50)  [default: 'new']
  replied_at      timestamp
  replied_by      bigint       [ref: > admin_users.id]
  reply_message   text
  ip_address      varchar(50)
  created_at      timestamp    [default: `now()`]
  updated_at      timestamp
}

// ─────────────────────────────────────────────
// GROUP 15 — PAYMENT
// ─────────────────────────────────────────────

Table payment_methods {
  id              bigint       [pk, increment]
  name            varchar(100) [unique, not null]
  label_en        varchar(100)
  label_ar        varchar(100)
  type            varchar(50)  [note: 'offline, online']
  gateway         varchar(100)
  gateway_config  text
  icon_url        text
  instructions_en text
  instructions_ar text
  is_active       boolean      [default: true]
  sort_order      int          [default: 0]
  created_at      timestamp    [default: `now()`]
  updated_at      timestamp
}

Table payment_transactions {
  id                     bigint        [pk, increment]
  order_id               bigint        [ref: > orders.id, not null]
  payment_method_id      bigint        [ref: > payment_methods.id]
  transaction_number     varchar(100)  [unique]
  amount                 decimal(15,2) [not null]
  currency               varchar(10)   [default: 'IQD']
  status                 varchar(50)   [default: 'pending']
  type                   varchar(50)   [default: 'charge', note: 'charge, refund']
  collected_by           bigint        [ref: > delivery_agents.id]
  collected_at           timestamp
  gateway_name           varchar(100)
  gateway_transaction_id varchar(255)
  gateway_status         varchar(100)
  gateway_response       text
  failure_reason         text
  paid_at                timestamp
  notes                  text
  created_at             timestamp     [default: `now()`]
  updated_at             timestamp
}

// ─────────────────────────────────────────────
// GROUP 16 — DELIVERY & CASH COLLECTION
// ─────────────────────────────────────────────

Table delivery_agents {
  id          bigint       [pk, increment]
  name        varchar(255) [not null]
  phone       varchar(20)
  whatsapp    varchar(20)
  national_id varchar(50)
  area        varchar(100)
  is_active   boolean      [default: true]
  notes       text
  created_at  timestamp    [default: `now()`]
  updated_at  timestamp
}

Table order_deliveries {
  id                bigint    [pk, increment]
  order_id          bigint    [ref: > orders.id, not null]
  delivery_agent_id bigint    [ref: > delivery_agents.id, not null]
  assigned_at       timestamp
  assigned_by       bigint    [ref: > admin_users.id]
  status            varchar(50) [default: 'assigned']
  picked_up_at      timestamp
  attempted_at      timestamp
  delivered_at      timestamp
  failure_reason    text
  notes             text
  created_at        timestamp [default: `now()`]
}

Table cash_collections {
  id                bigint        [pk, increment]
  delivery_agent_id bigint        [ref: > delivery_agents.id, not null]
  collection_date   date          [not null]
  expected_amount   decimal(15,2)
  collected_amount  decimal(15,2)
  difference        decimal(15,2)
  status            varchar(50)   [default: 'pending']
  received_by       bigint        [ref: > admin_users.id]
  received_at       timestamp
  notes             text
  created_at        timestamp     [default: `now()`]
  updated_at        timestamp
}

// ─────────────────────────────────────────────
// GROUP 17 — ORDER STATUS HISTORY
// ─────────────────────────────────────────────

Table order_status_history {
  id               bigint       [pk, increment]
  order_id         bigint       [ref: > orders.id, not null]
  from_status      varchar(50)
  to_status        varchar(50)  [not null]
  changed_by_type  varchar(20)  [note: 'admin, system, customer']
  admin_user_id    bigint       [ref: > admin_users.id]
  note             text
  created_at       timestamp    [default: `now()`]
}

// ─────────────────────────────────────────────
// GROUP 18 — BACK IN STOCK REQUESTS
// ─────────────────────────────────────────────

Table back_in_stock_requests {
  id          bigint       [pk, increment]
  user_id     bigint       [ref: > users.id]
  email       varchar(255)
  product_id  bigint       [ref: > products.id, not null]
  variant_id  bigint       [ref: > product_variants.id]
  notified_at timestamp
  is_active   boolean      [default: true]
  created_at  timestamp    [default: `now()`]
}

// ─────────────────────────────────────────────
// GROUP 19 — TAGS
// ─────────────────────────────────────────────

Table tags {
  id         bigint       [pk, increment]
  name       varchar(100) [unique, not null]
  slug       varchar(100) [unique, not null]
  created_at timestamp    [default: `now()`]
}

Table product_tags {
  id         bigint    [pk, increment]
  product_id bigint    [ref: > products.id, not null]
  tag_id     bigint    [ref: > tags.id, not null]
  created_at timestamp [default: `now()`]
}

// ─────────────────────────────────────────────
// GROUP 20 — NAVIGATION MENUS
// ─────────────────────────────────────────────

Table menus {
  id         bigint       [pk, increment]
  name       varchar(100) [unique, not null]
  label_en   varchar(100)
  label_ar   varchar(100)
  is_active  boolean      [default: true]
  created_at timestamp    [default: `now()`]
}

Table menu_items {
  id         bigint       [pk, increment]
  menu_id    bigint       [ref: > menus.id, not null]
  parent_id  bigint       [ref: > menu_items.id]
  label_en   varchar(100)
  label_ar   varchar(100)
  url        varchar(500)
  link_type  varchar(50)  [note: 'custom, category, collection, page, product']
  link_id    bigint
  icon       varchar(100)
  target     varchar(20)  [default: '_self']
  sort_order int          [default: 0]
  is_active  boolean      [default: true]
  created_at timestamp    [default: `now()`]
}
```

---

## 🏭 Group 11 — الموردون وإدارة المخزون

> هذه المجموعة تغطي الدورة الكاملة من استلام البضاعة من المورد حتى تحديث المخزون تلقائياً.

### `suppliers` — الموردون
> من أين تأتي البضاعة؟ موزعو ASUS، Lenovo، Dell في العراق أو من الخارج.

| الحقل | النوع | الوصف |
|-------|-------|-------|
| id | bigint PK | المعرف الفريد |
| name | varchar(255) | اسم المورد |
| company_name | varchar(255) | اسم الشركة |
| email | varchar(255) | البريد الإلكتروني |
| phone | varchar(20) | رقم الهاتف |
| whatsapp | varchar(20) | واتساب للتواصل السريع |
| country | varchar(100) | دولة المورد |
| city | varchar(100) | المدينة |
| address | text | العنوان التفصيلي |
| payment_terms | varchar(255) | شروط الدفع (مثال: 30 يوم، نقداً) |
| currency | varchar(10) | عملة التعامل (USD / IQD) |
| notes | text | ملاحظات داخلية |
| is_active | boolean | حالة المورد |
| created_at | timestamp | تاريخ الإضافة |
| updated_at | timestamp | تاريخ التعديل |

---

### `purchase_orders` — أوامر الشراء من الموردين
> كل مرة تطلب بضاعة من مورد يُسجَّل هنا. يُتيح تتبع ما طُلب وما وصل وما دُفع.

**دورة حياة أمر الشراء:**
```
draft --> sent --> confirmed --> partially_received --> received --> closed
                                                    --> cancelled
```

| الحقل | النوع | الوصف |
|-------|-------|-------|
| id | bigint PK | المعرف الفريد |
| supplier_id | bigint FK → suppliers.id | المورد |
| po_number | varchar(50) UNIQUE | رقم أمر الشراء |
| status | varchar(50) | `draft / sent / confirmed / partially_received / received / cancelled` |
| order_date | date | تاريخ الطلب |
| expected_date | date | التاريخ المتوقع للوصول |
| received_date | date | تاريخ الاستلام الفعلي |
| subtotal | decimal(15,2) | المجموع قبل الضريبة |
| tax_amount | decimal(15,2) | الضريبة والرسوم الجمركية |
| shipping_cost | decimal(15,2) | تكلفة الشحن من المورد |
| total | decimal(15,2) | الإجمالي الكلي |
| currency | varchar(10) | العملة |
| exchange_rate | decimal(10,4) | سعر الصرف (للتحويل إلى IQD) |
| payment_status | varchar(50) | `unpaid / partial / paid` |
| paid_amount | decimal(15,2) | المبلغ المدفوع |
| notes | text | ملاحظات |
| admin_user_id | bigint FK → admin_users.id | المسؤول عن الطلب |
| created_at | timestamp | تاريخ الإنشاء |
| updated_at | timestamp | تاريخ التعديل |

---

### `purchase_order_items` — تفاصيل أمر الشراء
> كل منتج في أمر الشراء مع الكمية المطلوبة والمستلمة والسعر.

| الحقل | النوع | الوصف |
|-------|-------|-------|
| id | bigint PK | المعرف الفريد |
| purchase_order_id | bigint FK → purchase_orders.id | أمر الشراء |
| product_id | bigint FK → products.id | المنتج |
| variant_id | bigint FK → product_variants.id | المتغير (اختياري) |
| sku | varchar(100) | رمز المنتج |
| product_name | varchar(500) | اسم المنتج (snapshot) |
| quantity_ordered | int | الكمية المطلوبة |
| quantity_received | int | الكمية المستلمة فعلياً |
| unit_cost | decimal(15,2) | سعر الوحدة من المورد |
| total_cost | decimal(15,2) | الإجمالي لهذا البند |
| notes | text | ملاحظات (تلف، نقص...) |
| created_at | timestamp | تاريخ الإضافة |

---

### `inventory_movements` — حركات المخزون ⭐ (الأهم)
> يُسجَّل هنا كل زيادة أو نقصان في المخزون مع السبب. يوفر سجلاً تاريخياً كاملاً.

**أنواع الحركات:**

| النوع | الحدث المُشغِّل | التأثير على المخزون |
|-------|----------------|---------------------|
| `PURCHASE_IN` | استلام بضاعة من مورد | ++ زيادة |
| `SALE_OUT` | بيع منتج لعميل | -- نقصان |
| `RETURN_IN` | إرجاع منتج من عميل | ++ زيادة |
| `ADJUSTMENT_IN` | تعديل يدوي بالزيادة | ++ زيادة |
| `ADJUSTMENT_OUT` | تعديل يدوي بالنقصان | -- نقصان |
| `DAMAGE_OUT` | إتلاف أو فقدان | -- نقصان |
| `TRANSFER_IN` | نقل من مستودع لآخر | ++ زيادة في الوجهة |
| `TRANSFER_OUT` | نقل من مستودع لآخر | -- نقصان في المصدر |

| الحقل | النوع | الوصف |
|-------|-------|-------|
| id | bigint PK | المعرف الفريد |
| product_id | bigint FK → products.id | المنتج |
| variant_id | bigint FK → product_variants.id | المتغير (اختياري) |
| movement_type | varchar(50) | نوع الحركة (من الجدول أعلاه) |
| quantity | int | الكمية (موجبة دائماً، النوع يحدد الاتجاه) |
| quantity_before | int | المخزون قبل الحركة |
| quantity_after | int | المخزون بعد الحركة |
| reference_type | varchar(50) | `purchase_order / order / return / manual` |
| reference_id | bigint | معرف المرجع (رقم الطلب أو أمر الشراء) |
| unit_cost | decimal(15,2) | تكلفة الوحدة (للشراء والإتلاف) |
| notes | text | سبب الحركة |
| admin_user_id | bigint FK → admin_users.id | المسؤول |
| created_at | timestamp | وقت الحركة |

---

## 🔁 Group 12 — المرتجعات والضمان

> متجر النبع يُعلن ضمان سنة على جميع المنتجات. هذه الجداول تُدير عملية الإرجاع والضمان.

### `returns` — طلبات الإرجاع
> عندما يريد عميل إرجاع منتج أو المطالبة بالضمان.

**دورة حياة الإرجاع:**
```
requested --> under_review --> approved --> received --> refunded / exchanged / repaired
                           --> rejected
```

| الحقل | النوع | الوصف |
|-------|-------|-------|
| id | bigint PK | المعرف الفريد |
| return_number | varchar(50) UNIQUE | رقم طلب الإرجاع |
| order_id | bigint FK → orders.id | الطلب الأصلي |
| user_id | bigint FK → users.id | العميل |
| return_type | varchar(50) | `return / exchange / warranty_repair` |
| status | varchar(50) | `requested / under_review / approved / received / refunded / rejected` |
| reason | varchar(255) | سبب الإرجاع |
| reason_details | text | تفاصيل إضافية |
| refund_amount | decimal(15,2) | مبلغ الاسترداد |
| refund_method | varchar(50) | `original_payment / store_credit / bank_transfer` |
| refund_status | varchar(50) | `pending / processed` |
| admin_notes | text | ملاحظات الإدارة |
| reviewed_by | bigint FK → admin_users.id | المراجع |
| reviewed_at | timestamp | تاريخ المراجعة |
| received_at | timestamp | تاريخ استلام المنتج المُرجَع |
| created_at | timestamp | تاريخ طلب الإرجاع |
| updated_at | timestamp | تاريخ التعديل |

---

### `return_items` — تفاصيل الإرجاع
> المنتجات المرتجعة مع سببها وحالتها عند الاستلام.

| الحقل | النوع | الوصف |
|-------|-------|-------|
| id | bigint PK | المعرف الفريد |
| return_id | bigint FK → returns.id | طلب الإرجاع |
| order_item_id | bigint FK → order_items.id | البند الأصلي في الطلب |
| product_id | bigint FK → products.id | المنتج |
| variant_id | bigint FK → product_variants.id | المتغير |
| quantity | int | الكمية المرتجعة |
| unit_price | decimal(15,2) | سعر الوحدة الأصلي |
| refund_amount | decimal(15,2) | مبلغ استرداد هذا البند |
| condition | varchar(50) | حالة المنتج المُرجَع: `new / good / damaged / defective` |
| return_to_stock | boolean | هل يُعاد للمخزون؟ |
| notes | text | ملاحظات |
| created_at | timestamp | تاريخ الإضافة |

---

### `warranty_claims` — مطالبات الضمان
> تتبع طلبات الصيانة والضمان بشكل مستقل (قد لا تنتهي بإرجاع بل بإصلاح).

| الحقل | النوع | الوصف |
|-------|-------|-------|
| id | bigint PK | المعرف الفريد |
| claim_number | varchar(50) UNIQUE | رقم مطالبة الضمان |
| order_item_id | bigint FK → order_items.id | البند المعني بالضمان |
| product_id | bigint FK → products.id | المنتج |
| user_id | bigint FK → users.id | العميل |
| return_id | bigint FK → returns.id | طلب الإرجاع المرتبط (اختياري) |
| issue_description | text | وصف المشكلة |
| status | varchar(50) | `submitted / under_review / approved / in_repair / repaired / replaced / rejected` |
| resolution | varchar(50) | `repair / replacement / refund` |
| warranty_expires_at | date | تاريخ انتهاء الضمان |
| received_at | timestamp | تاريخ استلام الجهاز للصيانة |
| resolved_at | timestamp | تاريخ الحل |
| admin_notes | text | ملاحظات الفني |
| handled_by | bigint FK → admin_users.id | المسؤول |
| created_at | timestamp | تاريخ تقديم المطالبة |
| updated_at | timestamp | تاريخ التعديل |

---

## ⚡ Group 13 — العروض الخاصة المحدودة بالوقت

> مستوحى مباشرة من "Peripheral Deals — In-Store Exclusive" الموجودة في متجر النبع.  
> عروض تبدأ وتنتهي تلقائياً بتوقيت محدد مع سعر خاص للعرض.

### `flash_sales` — العروض المحدودة
> يحدد توقيت العرض والخصم المطبّق تلقائياً على المنتجات المُدرجة فيه.

| الحقل | النوع | الوصف |
|-------|-------|-------|
| id | bigint PK | المعرف الفريد |
| name_en | varchar(255) | اسم العرض بالإنجليزية |
| name_ar | varchar(255) | اسم العرض بالعربية |
| description_en | text | وصف العرض بالإنجليزية |
| description_ar | text | وصف العرض بالعربية |
| banner_url | text | صورة بنر العرض |
| discount_type | varchar(50) | `percentage / fixed_amount` |
| discount_value | decimal(10,2) | قيمة الخصم |
| starts_at | timestamp | بداية العرض (تلقائي) |
| ends_at | timestamp | نهاية العرض (تلقائي) |
| is_active | boolean | تفعيل يدوي |
| max_uses | int | الحد الأقصى للاستخدام الكلي |
| uses_count | int | عدد مرات الاستخدام |
| sort_order | int | ترتيب العرض في الصفحة |
| created_by | bigint FK → admin_users.id | المشرف المنشئ |
| created_at | timestamp | تاريخ الإنشاء |
| updated_at | timestamp | تاريخ التعديل |

---

### `flash_sale_products` — منتجات العرض الخاص
> ربط المنتجات بالعروض مع إمكانية تحديد سعر خاص لكل منتج في العرض.

| الحقل | النوع | الوصف |
|-------|-------|-------|
| id | bigint PK | المعرف الفريد |
| flash_sale_id | bigint FK → flash_sales.id | العرض |
| product_id | bigint FK → products.id | المنتج |
| variant_id | bigint FK → product_variants.id | المتغير (اختياري) |
| sale_price | decimal(15,2) | سعر العرض المخصص |
| original_price | decimal(15,2) | السعر الأصلي قبل العرض |
| max_quantity_per_order | int | الحد الأقصى للطلب الواحد |
| quantity_limit | int | الكمية المتاحة في العرض (NULL = بلا حد) |
| quantity_sold | int | الكمية المباعة في العرض |
| sort_order | int | ترتيب المنتج في العرض |
| created_at | timestamp | تاريخ الإضافة |

---

## 💬 Group 14 — التواصل مع العملاء

### `contact_messages` — رسائل التواصل
> العملاء الذين يتواصلون عبر نموذج الموقع بدل WhatsApp أو الهاتف.

| الحقل | النوع | الوصف |
|-------|-------|-------|
| id | bigint PK | المعرف الفريد |
| user_id | bigint FK → users.id | العميل (اختياري، إذا كان مسجلاً) |
| name | varchar(255) | اسم المرسل |
| email | varchar(255) | البريد الإلكتروني |
| phone | varchar(20) | رقم الهاتف |
| subject | varchar(255) | موضوع الرسالة |
| message | text | نص الرسالة |
| status | varchar(50) | `new / in_progress / resolved` |
| replied_at | timestamp | وقت الرد |
| replied_by | bigint FK → admin_users.id | المسؤول الذي ردّ |
| reply_message | text | نص الرد |
| ip_address | varchar(50) | عنوان IP |
| created_at | timestamp | تاريخ الإرسال |
| updated_at | timestamp | تاريخ التعديل |

---

## 💳 Group 15 — الدفع (COD الآن + إلكتروني مستقبلاً)

> مُصمَّم للتوسع: يعمل الآن بـ COD فقط، ويستوعب الدفع الإلكتروني مستقبلاً بإضافة بيانات فقط — لا تعديل في الهيكل.

### `payment_methods` — طرق الدفع المتاحة
> يُدار من لوحة التحكم. تُفعَّل أو تُعطَّل طرق الدفع بدون تعديل الكود.

| طريقة الدفع | الحالة الآن | المستقبل |
|-------------|------------|---------|
| COD (نقداً عند الاستلام) | ✅ مفعّل | — |
| Bank Transfer (حوالة بنكية) | ✅ مفعّل | — |
| Zain Cash | ❌ معطّل | ✅ يُفعَّل |
| FIB / آسیا حوالة | ❌ معطّل | ✅ يُفعَّل |
| Visa / Mastercard | ❌ معطّل | ✅ يُفعَّل |

| الحقل | النوع | الوصف |
|-------|-------|-------|
| id | bigint PK | المعرف الفريد |
| name | varchar(100) | الاسم الداخلي: `cod, bank_transfer, zain_cash, visa` |
| label_en | varchar(100) | الاسم المعروض بالإنجليزية |
| label_ar | varchar(100) | الاسم المعروض بالعربية |
| type | varchar(50) | `offline / online` |
| gateway | varchar(100) | اسم بوابة الدفع (NULL للـ offline) |
| gateway_config | text | إعدادات البوابة (JSON مشفّر) |
| icon_url | text | أيقونة طريقة الدفع |
| instructions_en | text | تعليمات الدفع للعميل |
| instructions_ar | text | تعليمات الدفع بالعربية |
| is_active | boolean | مفعّل أم معطّل |
| sort_order | int | ترتيب العرض |
| created_at | timestamp | تاريخ الإضافة |
| updated_at | timestamp | تاريخ التعديل |

---

### `payment_transactions` — سجل معاملات الدفع
> يُسجَّل هنا كل عملية دفع أو محاولة — سواء COD أو إلكتروني مستقبلاً.

**كيف يعمل الآن (COD):**
```
العميل يطلب → payment_transaction (status=pending, method=cod)
المندوب يسلّم ويقبض → payment_transaction (status=collected)
المندوب يسلّم النقد للشركة → cash_collection (status=reconciled)
```

**كيف سيعمل مستقبلاً (إلكتروني) — نفس الجدول:**
```
العميل يدفع ببطاقة → payment_transaction (status=pending, method=visa)
البنك يرد → payment_transaction (status=paid, gateway_transaction_id=TXN123...)
```

| الحقل | النوع | الوصف |
|-------|-------|-------|
| id | bigint PK | المعرف الفريد |
| order_id | bigint FK → orders.id | الطلب |
| payment_method_id | bigint FK → payment_methods.id | طريقة الدفع |
| transaction_number | varchar(100) UNIQUE | رقم المعاملة الداخلي |
| amount | decimal(15,2) | المبلغ |
| currency | varchar(10) | العملة |
| status | varchar(50) | `pending / collected / paid / failed / refunded` |
| type | varchar(50) | `charge / refund` |
| — حقول COD — | | |
| collected_by | bigint FK → delivery_agents.id | المندوب الذي قبض (COD) |
| collected_at | timestamp | وقت القبض النقدي |
| — حقول الدفع الإلكتروني (تُملأ مستقبلاً) — | | |
| gateway_name | varchar(100) | اسم البوابة (Stripe, ZainCash...) |
| gateway_transaction_id | varchar(255) | رقم العملية من البوابة |
| gateway_status | varchar(100) | استجابة البوابة الأصلية |
| gateway_response | text | JSON كامل من البوابة |
| failure_reason | text | سبب الفشل |
| paid_at | timestamp | وقت الدفع الإلكتروني |
| notes | text | ملاحظات |
| created_at | timestamp | تاريخ الإنشاء |
| updated_at | timestamp | تاريخ التعديل |

---

## 🛵 Group 16 — التوصيل وتحصيل النقد

> خاص بنموذج COD. يتتبع المندوبين ومسار النقد من العميل وحتى الشركة.

### `delivery_agents` — مندوبو التوصيل
> الأشخاص الذين يوصّلون الطلبات ويقبضون النقد من العملاء.

| الحقل | النوع | الوصف |
|-------|-------|-------|
| id | bigint PK | المعرف الفريد |
| name | varchar(255) | الاسم الكامل |
| phone | varchar(20) | رقم الهاتف |
| whatsapp | varchar(20) | واتساب للتواصل |
| national_id | varchar(50) | رقم الهوية الوطنية |
| area | varchar(100) | المنطقة المسؤول عنها (بغداد، البصرة...) |
| is_active | boolean | حالة المندوب |
| notes | text | ملاحظات |
| created_at | timestamp | تاريخ الإضافة |
| updated_at | timestamp | تاريخ التعديل |

---

### `order_deliveries` — تعيين الطلبات للمندوبين
> أي مندوب مسؤول عن أي طلب؟

| الحقل | النوع | الوصف |
|-------|-------|-------|
| id | bigint PK | المعرف الفريد |
| order_id | bigint FK → orders.id | الطلب |
| delivery_agent_id | bigint FK → delivery_agents.id | المندوب |
| assigned_at | timestamp | وقت التعيين |
| assigned_by | bigint FK → admin_users.id | من عيّنه |
| status | varchar(50) | `assigned / picked_up / delivered / failed` |
| picked_up_at | timestamp | وقت استلام المندوب للطلب |
| attempted_at | timestamp | وقت محاولة التوصيل |
| delivered_at | timestamp | وقت التسليم الفعلي |
| failure_reason | text | سبب فشل التوصيل (إذا حدث) |
| notes | text | ملاحظات المندوب |
| created_at | timestamp | تاريخ الإضافة |

---

### `cash_collections` — تحصيل النقد اليومي
> تتبع مسار النقد: من العميل → المندوب → الشركة. يُمكّن المطابقة المالية اليومية.

```
نهاية كل يوم:
مجموع cash_collections للمندوب = مجموع payment_transactions المقبوضة
```

| الحقل | النوع | الوصف |
|-------|-------|-------|
| id | bigint PK | المعرف الفريد |
| delivery_agent_id | bigint FK → delivery_agents.id | المندوب |
| collection_date | date | تاريخ التحصيل |
| expected_amount | decimal(15,2) | المبلغ المتوقع (مجموع طلباته اليوم) |
| collected_amount | decimal(15,2) | المبلغ الذي سلّمه فعلياً |
| difference | decimal(15,2) | الفرق (يجب أن يكون صفراً) |
| status | varchar(50) | `pending / submitted / reconciled / discrepancy` |
| received_by | bigint FK → admin_users.id | المدير الذي استلم النقد |
| received_at | timestamp | وقت استلام النقد |
| notes | text | ملاحظات (عجز، زيادة، سبب الفرق) |
| created_at | timestamp | تاريخ السجل |
| updated_at | timestamp | تاريخ التعديل |

---

## 📋 Group 17 — تتبع الطلبات

### `order_status_history` — سجل تغييرات حالة الطلب
> يُحفظ هنا كل تغيير في حالة الطلب مع التوقيت والسبب والمسؤول.

**لماذا ضروري؟**
- العميل يسأل "متى تحوّل طلبي لـ shipped؟" → لديك الجواب
- النزاع: "البضاعة وصلت قبل 13 شهر أم 11؟" → السجل موجود
- التحليل: كم يستغرق متوسط الوصول من confirmed إلى delivered؟

**مثال على البيانات:**
```
order_id: 501
├── pending      → confirmed    (2026-06-10 09:15) بواسطة: نظام تلقائي
├── confirmed    → processing   (2026-06-10 10:30) بواسطة: أحمد (admin)
├── processing   → shipped      (2026-06-11 14:00) بواسطة: أحمد (admin)
└── shipped      → delivered    (2026-06-12 16:45) بواسطة: نظام (عند تأكيد المندوب)
```

| الحقل | النوع | الوصف |
|-------|-------|-------|
| id | bigint PK | المعرف الفريد |
| order_id | bigint FK → orders.id | الطلب |
| from_status | varchar(50) | الحالة السابقة |
| to_status | varchar(50) | الحالة الجديدة |
| changed_by_type | varchar(20) | `admin / system / customer` |
| admin_user_id | bigint FK → admin_users.id | المسؤول (إذا كان admin) |
| note | text | سبب التغيير |
| created_at | timestamp | وقت التغيير |

---

## 🔔 Group 18 — طلبات الإشعار عند التوفر

### `back_in_stock_requests` — "نبّهني عند التوفر"
> عندما ينفد مخزون منتج يضغط العميل "نبّهني عند التوفر". عند عودة المخزون يُرسَل إشعار تلقائي.

**الاستخدام:**
```
1. ASUS TUF F16 → نفد المخزون (stock_quantity = 0)
2. عميل يضغط "نبّهني" → يُسجَّل في back_in_stock_requests
3. يصل شحنة جديدة → inventory_movements (PURCHASE_IN)
4. نظام تلقائي يبحث → هل هناك طلبات إشعار لهذا المنتج؟
5. يُرسَل إشعار لكل عميل طلب → notifications
6. يُحدَّث السجل: notified_at = now()
```

| الحقل | النوع | الوصف |
|-------|-------|-------|
| id | bigint PK | المعرف الفريد |
| user_id | bigint FK → users.id | العميل المسجّل (اختياري) |
| email | varchar(255) | إيميل الزائر (للزوار غير المسجلين) |
| product_id | bigint FK → products.id | المنتج المطلوب |
| variant_id | bigint FK → product_variants.id | المتغير المحدد (اختياري) |
| notified_at | timestamp | وقت إرسال الإشعار (NULL = لم يُرسَل بعد) |
| is_active | boolean | لا يزال مهتماً (false بعد الشراء) |
| created_at | timestamp | تاريخ الطلب |

---

## 🏷️ Group 19 — التاغات (إصلاح التصميم)

> **سبب الإصلاح:** تخزين التاغات كنص مفصول بفواصل في `products.tags` يجعل الفلترة والبحث بطيئاً ومعقداً. الحل الصحيح: جدولان منفصلان.

### `tags` — التاغات
| الحقل | النوع | الوصف |
|-------|-------|-------|
| id | bigint PK | المعرف الفريد |
| name | varchar(100) UNIQUE | اسم التاغ (مثال: gaming, ultrabook, rtx) |
| slug | varchar(100) UNIQUE | المعرف النصي |
| created_at | timestamp | تاريخ الإنشاء |

### `product_tags` — ربط المنتجات بالتاغات (Many-to-Many)
| الحقل | النوع | الوصف |
|-------|-------|-------|
| id | bigint PK | المعرف الفريد |
| product_id | bigint FK → products.id | المنتج |
| tag_id | bigint FK → tags.id | التاغ |
| created_at | timestamp | تاريخ الربط |

---

## 🗺️ Group 20 — قوائم التنقل (Navigation)

> إدارة قوائم الموقع من لوحة التحكم بدون تعديل الكود.

### `menus` — القوائم الرئيسية
| الحقل | النوع | الوصف |
|-------|-------|-------|
| id | bigint PK | المعرف الفريد |
| name | varchar(100) | الاسم الداخلي: `main_nav, footer, mobile` |
| label_en | varchar(100) | التسمية بالإنجليزية |
| label_ar | varchar(100) | التسمية بالعربية |
| is_active | boolean | حالة الظهور |
| created_at | timestamp | تاريخ الإنشاء |

### `menu_items` — عناصر القوائم
| الحقل | النوع | الوصف |
|-------|-------|-------|
| id | bigint PK | المعرف الفريد |
| menu_id | bigint FK → menus.id | القائمة |
| parent_id | bigint FK → menu_items.id | العنصر الأب (NULL = مستوى أول) |
| label_en | varchar(100) | النص بالإنجليزية |
| label_ar | varchar(100) | النص بالعربية |
| url | varchar(500) | الرابط (خارجي) |
| link_type | varchar(50) | `custom / category / collection / page / product` |
| link_id | bigint | معرف الكيان المرتبط |
| icon | varchar(100) | أيقونة اختيارية |
| target | varchar(20) | `_self / _blank` |
| sort_order | int | ترتيب العنصر |
| is_active | boolean | حالة الظهور |
| created_at | timestamp | تاريخ الإضافة |

---

## 📊 إحصائيات قاعدة البيانات

| # | المجموعة | الجداول | عدد الجداول |
|---|----------|---------|-------------|
| 1 | المستخدمون والمصادقة | users, addresses, admin_users | 3 |
| 2 | الكتالوج والمنتجات | categories, brands, products, product_images, product_variants | 5 |
| 3 | الخصائص التقنية | attribute_groups, attributes, product_attribute_values | 3 |
| 4 | المجموعات التسويقية | collections, collection_products | 2 |
| 5 | السلة والطلبات | carts, cart_items, orders, order_items | 4 |
| 6 | الشحن | shipping_zones, shipping_rates | 2 |
| 7 | الخصومات | discounts, discount_conditions, discount_usages | 3 |
| 8 | تفاعل العميل | reviews, wishlists, product_compares | 3 |
| 9 | إدارة المحتوى | banners, pages | 2 |
| 10 | المراقبة والإعدادات | notifications, activity_logs, settings | 3 |
| 11 | الموردون والمخزون | suppliers, purchase_orders, purchase_order_items, inventory_movements | 4 |
| 12 | المرتجعات والضمان | returns, return_items, warranty_claims | 3 |
| 13 | العروض المحدودة | flash_sales, flash_sale_products | 2 |
| 14 | التواصل مع العملاء | contact_messages | 1 |
| 15 | **الدفع** ✨ | **payment_methods, payment_transactions** | **2** |
| 16 | **التوصيل وتحصيل النقد** ✨ | **delivery_agents, order_deliveries, cash_collections** | **3** |
| 17 | **تتبع الطلبات** ✨ | **order_status_history** | **1** |
| 18 | **إشعارات التوفر** ✨ | **back_in_stock_requests** | **1** |
| 19 | **التاغات** ✨ | **tags, product_tags** | **2** |
| 20 | **قوائم التنقل** ✨ | **menus, menu_items** | **2** |
| | **المجموع** | | **51 جدول** |

---

## 🔗 ملخص العلاقات

| من | العلاقة | إلى |
|----|---------|-----|
| addresses | Many → One | users |
| products | Many → One | categories |
| products | Many → One | brands |
| categories | Self-Join | categories (parent) |
| product_images | Many → One | products |
| product_variants | Many → One | products |
| product_attribute_values | Many → One | products + attributes |
| attributes | Many → One | attribute_groups |
| collection_products | Many-to-Many | collections ↔ products |
| cart_items | Many → One | carts + products |
| order_items | Many → One | orders + products |
| shipping_rates | Many → One | shipping_zones |
| discount_usages | Many → One | discounts + orders |
| reviews | Many → One | products + users |
| wishlists | Many → One | users + products |
| notifications | Many → One | users |
| activity_logs | Many → One | admin_users |
| purchase_orders | Many → One | suppliers |
| purchase_order_items | Many → One | purchase_orders + products |
| inventory_movements | Many → One | products (trigger: PURCHASE_IN / SALE_OUT / RETURN_IN) |
| returns | Many → One | orders + users |
| return_items | Many → One | returns + order_items |
| warranty_claims | Many → One | order_items + users |
| flash_sale_products | Many-to-Many | flash_sales ↔ products |
| contact_messages | Many → One | users (اختياري) |
| **payment_transactions** | Many → One | **orders + payment_methods** |
| **order_deliveries** | Many → One | **orders + delivery_agents** |
| **cash_collections** | Many → One | **delivery_agents** |
| **order_status_history** | Many → One | **orders** |
| **back_in_stock_requests** | Many → One | **products + users** |
| **product_tags** | Many-to-Many | **products ↔ tags** |
| **menu_items** | Many → One + Self-Join | **menus + menu_items (parent)** |

---

## 🔄 ملخص دورة حياة المخزون (inventory_movements)

| الحدث | نوع الحركة | التأثير |
|-------|-----------|---------|
| استلام بضاعة من مورد | `PURCHASE_IN` | stock_quantity ++ |
| بيع منتج لعميل | `SALE_OUT` | stock_quantity -- |
| إرجاع منتج من عميل (صالح) | `RETURN_IN` | stock_quantity ++ |
| تعديل يدوي بالزيادة | `ADJUSTMENT_IN` | stock_quantity ++ |
| تعديل يدوي بالنقصان | `ADJUSTMENT_OUT` | stock_quantity -- |
| إتلاف أو تلف | `DAMAGE_OUT` | stock_quantity -- |

---

*تم إنشاء هذا المخطط بناءً على تحليل متجر النبع — AL-NABAA (store.alnabaa.com)*  
*Iraq's #1 Online Store for Computers, Laptops & Electronics*  
*آخر تحديث: يشمل دورة كاملة من إدارة الموردين والمخزون حتى البيع والإرجاع والضمان*
