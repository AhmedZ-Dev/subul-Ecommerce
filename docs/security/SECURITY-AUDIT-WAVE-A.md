# التقرير الأمني المرحلي — الموجة أ

**النطاق:** التحكم بالوصول ومنطق الأعمال في الباك إند (المرحلتان 1 و2)
**التاريخ:** 2026-09-05
**الفرع:** main @ 0adfa90
**عدد الـ endpoints المفحوصة:** 92 عبر 18 feature

---

## الملخص التنفيذي

| الخطورة | العدد |
|---|---|
| حرِج (Critical) | 6 |
| عالٍ (High) | 5 |
| متوسط (Medium) | 6 |

**الخلاصة:** `OrderFeature` بأكمله مكشوف دون مصادقة — قراءةً وكتابةً. أي شخص على الإنترنت يستطيع تنزيل قاعدة بيانات الطلبات كاملة (أسماء، هواتف، عناوين) وتعديل حالة أي طلب. السلة قابلة للاختطاف الدائم. والنشر يجري على HTTP صريح.

---

## حرِج

### C-1 — تسريب كامل لقاعدة بيانات الطلبات دون مصادقة

**الموقع:** `backend/Features/OrderFeature/ListOrderPaginated/ListOrderPaginatedEndpoint.cs:14`

`[AllowAnonymous]` على `GET /api/orders` بلا أي تصفية ملكية في الـ handler.

**الاستغلال:**

    GET /api/orders?limit=999999

لا يوجد سقف على `limit` (الـ handler يفحص `<= 0` فقط)، فطلب واحد يُنزّل كل الطلبات. الاستجابة تحوي `ShippingFirstName`, `ShippingPhone`, `ShippingCity`, `ShippingGovernorate`, `Total`, `TrackingNumber` وكل `OrderItems`.

**أسوأ:** المعامل `search` يبحث في رقم الهاتف مباشرة:

    GET /api/orders?search=07701234567

أي استعلام موجَّه عن شخص بعينه. والمعامل `userId` يتيح التعداد لكل عميل على حدة.

**الأثر:** انتهاك بيانات شخصية كامل. في سوق يغلب عليه الدفع عند الاستلام، هذه قائمة بأسماء وعناوين وهواتف وقيم مشتريات — مادة جاهزة للاحتيال الهاتفي أو للبيع لمنافس.

---

### C-2 — IDOR على تفاصيل الطلب

**الموقع:** `backend/Features/OrderFeature/GetByIdOrder/GetByIdOrderEndpoint.cs:14`

`GET /api/orders/{id}` بـ `[AllowAnonymous]`، والـ handler يجلب بـ `o.Id == query.Id` فقط. المعرّفات `long` متسلسلة، فالتعداد تافه.

يُعيد أكثر مما يُعيده C-1: `ShippingLastName`, `ShippingAddress1`, `ShippingAddress2`, `CustomerNotes`، و**`Notes`** — وهي الملاحظات الداخلية للإدارة.

نفس المشكلة في `ListOrderItems` و `GetByIdOrderItem`.

---

### C-3 — كتابة على الطلبات دون مصادقة + تزوير سجل التدقيق

**الموقع:** `backend/Features/OrderFeature/UpdateOrder/UpdateOrderEndpoint.cs:15`

`PUT /api/orders/{id}` بـ `[AllowAnonymous]`.

**الاستغلال:**

    PUT /api/orders/123
    { "paymentStatus": "paid" }

احتيال مباشر: تعليم طلب غير مدفوع كمدفوع. أو إلغاء كل طلبات المتجر في حلقة:

    PUT /api/orders/{i}
    { "status": "cancelled" }

**الأخطر:** في `UpdateOrderHandler.cs` يُكتب سجل الحالة بـ `ChangedByType = "admin"` ثابتاً. أي أن تعديلات المهاجم تُسجَّل في `OrderStatusHistory` منسوبة إلى الإدارة. سجل التدقيق لا يفشل فحسب — بل يكذب بنشاط ويضلل أي تحقيق لاحق.

---

### C-4 — اختطاف دائم لسلة أي مستخدم

**الموقع:** `GetCart`, `AddCartItem`, `UpdateCartItem`, `RemoveCartItem`, `MergeCart` — الخمسة `[AllowAnonymous]`

الـ `userId` يأتي من العميل (query string أو body) دون أي تحقق. وفي `GetOrCreateCartAsync` (مكرَّرة في ثلاثة handlers):

```csharp
if (userId is not null)
    cart = await context.Carts.FirstOrDefaultAsync(c => c.UserId == userId, ...);
...
if (cart.SessionId != sessionId)
{
    cart.SessionId = sessionId;   // ← يُعاد ربط السلة بجلسة المهاجم
}
```

**الاستغلال:**

    GET /api/carts?userId=5
    X-Cart-Session: <جلسة المهاجم>

لا يقرأ سلة المستخدم 5 فقط، بل **يعيد ربطها بجلسة المهاجم بشكل دائم**. الضحية تفقد سلتها، والمهاجم يتحكم بمحتواها. وهذه كتابة تحدث داخل طلب `GET`.

`MergeCart` نفس الشيء عبر `request.UserId` في الـ body — والتعليق في الملف يقول "Keep AllowAnonymous until JWT is wired"، والـ JWT صار موصولاً منذ حين.

---

### C-5 — الإنتاج يعمل على HTTP صريح

**الموقع:** `docker-compose.yml`

الافتراضيات تشير إلى IP عام عبر `http://` مع نشر المنافذ `5101`, `3000`, `3001` مباشرة. كلمة مرور المسؤول والتوكن الإداري يسافران بنص واضح، قابلان للالتقاط على أي قفزة شبكية.

**هذا ما سيُصلحه Traefik.** ملاحظة مرتبطة: `Program.cs` فيه `UseHttpsRedirection()` بلا `UseForwardedHeaders` — سيسبب حلقات إعادة توجيه فور دخول أي reverse proxy.

---

### C-6 — تنفيذ طلب من سلة الغير

**الموقع:** `backend/Features/OrderFeature/CreateOrder/CreateOrderHandler.cs`

```csharp
c => c.SessionId == sessionId || (command.UserId != null && c.UserId == command.UserId)
```

`command.UserId` من العميل والـ endpoint `[AllowAnonymous]`. المهاجم يُمرّر `userId` الضحية فيُنفَّذ طلب من سلتها ويُفرّغها، مع عنوان شحن يختاره هو.

---

## عالٍ

### H-1 — الأدوار معرَّفة لكنها غير مُنفَّذة إطلاقاً
`AdminUser.Role` موجود ويُوضع في التوكن (`JwtTokenService.cs:24`)، لكن لا يوجد `[Authorize(Roles = ...)]` واحد في المشروع كله. أي حساب إداري — مهما كان دوره — يستطيع حذف المنتجات وتعديل الأسعار والوصول للوحة التحكم بالكامل.

### H-2 — التوكن الإداري مكشوف لـ JavaScript في المتصفح
`client/admin-panel/auth.ts` — الـ `session` callback يضع `accessToken` في كائن الجلسة المُعاد للعميل. أي XSS في اللوحة = سرقة توكن إداري صالح 8 ساعات.

### H-3 — لا يوجد rate limiting على تسجيل الدخول
`POST api/auth/login` بلا أي حد. الـ handler نفسه مكتوب جيداً (BCrypt، تخفيف timing attack، رسالة خطأ عامة، فحص `IsActive`) — لكن لا شيء يمنع تجربة ملايين كلمات المرور. **هذا ما سيُصلحه Redis.**

### H-4 — سباق تزامن على المخزون (TOCTOU)
`CreateOrderHandler` يفحص `item.Quantity > availableStock` ثم يُنقص `StockQuantity -= quantity` دون قفل أو معاملة معزولة. طلبات متزامنة تعني بيعاً أكثر من المخزون، ومخزوناً سالباً.

### H-5 — لا يوجد إبطال للتوكن
التوكن صالح 480 دقيقة بلا refresh ولا قائمة إبطال. تعطيل حساب مسؤول (`IsActive = false`) لا يُنهي جلسته — يبقى فاعلاً حتى 8 ساعات.

---

## متوسط

- **M-1** لا سقف على `limit` في أي endpoint في المشروع، ما يتيح استنزاف الموارد والسحب الجماعي للبيانات.
- **M-2** سر JWT للتطوير مرفوع في git: `backend/appsettings.Development.json`.
- **M-3** `TrackGuestOrder` قابل للتخمين: صيغة رقم الطلب `ORD-yyyyMMdd-{6 أرقام}` تعني 900,000 احتمال يومياً، بلا rate limiting.
- **M-4** `AllowedHosts: "*"` وغياب أي security headers (HSTS, CSP, X-Content-Type-Options).
- **M-5** `Order.IpAddress` يُؤخذ من `HttpContext.Connection.RemoteIpAddress` — صحيح حالياً، لكنه سيصبح IP الـ proxy بعد إدخال Traefik ما لم يُضبط `UseForwardedHeaders`.
- **M-6** `CouponCode` يُقبل ويُخزَّن في الطلب لكنه لا يُتحقق منه ولا يُطبَّق (`discountAmount = 0` ثابت). ليس ثغرة، لكنه سطح مضلل.

---

## ما فُحص ووُجد سليماً

توثيق هذا مهم بقدر توثيق الثغرات:

- **لا يوجد تلاعب بالأسعار.** `AddCartItemHandler` يحسب `unitPrice = variant?.Price ?? product.Price` من قاعدة البيانات؛ ولا يوجد حقل سعر في `AddCartItemCommand`. و`CreateOrderHandler` يعيد الحساب من الخادم. هذه أكثر ثغرات المتاجر شيوعاً، والمشروع سليم منها.
- **معالجة كلمات المرور سليمة.** BCrypt، مع hash وهمي لتخفيف هجوم التوقيت، ورسالة `unauthorized` عامة لا تفرّق بين بريد خاطئ وكلمة مرور خاطئة.
- **لا تسريب للاستثناءات.** `ExceptionHandlingMiddleware` يُعيد رسالة عامة ويُسجّل التفاصيل داخلياً فقط.
- **لا SQL injection.** كل الاستعلامات عبر EF Core LINQ؛ لا `FromSqlRaw`. الترتيب الديناميكي عبر `switch` على قيم ثابتة لا عبر تركيب نصي.
- **`TrackGuestOrder` لا يسرّب oracle** — يُعيد "Order not found" لرقم الهاتف الخاطئ، لا رسالة مختلفة.
- **`ResolveShippingAsync` يتحقق من ملكية العنوان** — `a.Id == command.AddressId && a.UserId == command.UserId`.
- **`FallbackPolicy` مضبوط بشكل صحيح** — الافتراضي هو المنع، والاستثناء يتطلب `[AllowAnonymous]` صريحاً. البنية سليمة؛ المشكلة أن `[AllowAnonymous]` وُضعت في مواضع خاطئة.

---

## الإصلاحات ذات الأولوية

| # | الإصلاح | الجهد |
|---|---|---|
| 1 | إزالة `[AllowAnonymous]` من `ListOrderPaginated`, `GetByIdOrder`, `UpdateOrder`, `ListOrderItems`, `GetByIdOrderItem` | دقائق |
| 2 | اشتقاق `userId` من `ClaimTypes.NameIdentifier` لا من العميل، في كل الـ CartFeature؛ وحذف إعادة ربط `SessionId` | ساعة |
| 3 | حذف `command.UserId` من شرط جلب السلة في `CreateOrder` | دقائق |
| 4 | سقف `limit` عند 100 في كل الـ handlers | ساعة |
| 5 | إخراج `accessToken` من كائن الجلسة في `auth.ts` | نصف ساعة |
| 6 | إضافة `[Authorize(Roles=...)]` حسب مصفوفة صلاحيات | نصف يوم |
| 7 | معاملة وقفل تفاؤلي على إنقاص المخزون | نصف يوم |

البندان 1 و3 وحدهما يغلقان أخطر خمس ثغرات، وهما تعديل أسطر معدودة.

**اختبار انحدار مطلوب لكل بند** في `backend.Tests` — طلب بلا توكن ينتج 401، وطلب بـ `userId` غريب ينتج 403 أو 404.

---

# ملحق: حالة الإصلاح — 2026-09-05

## أُصلح وتم التحقق منه

| المعرّف | الإصلاح | التحقق |
|---|---|---|
| C-1, C-2, C-3 | حُذف `[AllowAnonymous]` من `ListOrderPaginated`, `GetByIdOrder`, `UpdateOrder`, `ListOrderItems`, `GetByIdOrderItem` | 6 اختبارات في `OrderAccessControlTests` |
| C-4 | حُذف `userId` القادم من العميل من كل `CartFeature`؛ الملكية صارت من `X-Cart-Session` وحدها؛ وحُذفت إعادة ربط `SessionId`؛ و`MergeCart` صار يتطلب مصادقة | 5 اختبارات في `CartOwnershipTests` |
| C-6 | `CreateOrder` لم يعد يقبل `userId` أو `addressId` في الـ body، وجلب السلة صار بالجلسة وحدها | `POST_Order_WithForeignUserId_DoesNotUseAnotherSessionsCart` |
| M-1 | سقف `limit` عند 100 في الـ 14 handler كلها | — |

`GetOrCreateCartAsync` كانت مكرّرة في ثلاثة handlers بنفس الخلل؛ صارت الآن نسخة واحدة مشتركة في `GetCartHandler` مع تعليق يوضّح سبب المنع.

**نتيجة السويت:** 429 اختباراً ناجحاً، صفر إخفاق.

## إخفاق سابق عُثر عليه وأُصلح

`AdminUserIntegrationTests.GET_Categories_WithoutToken_Returns401` كان يفشل **قبل** هذا العمل: يفترض أن `/api/categories` محمي، بينما `[AllowAnonymous]` عليه موجود في HEAD عمداً لأجل المتجر. اختبار بائد من زمن ما قبل وجود الواجهة العامة. أُعيدت صياغته ليؤكد السلوك الموثّق.

## نتيجة جديدة من مخرجات البناء

**M-7 (عالٍ) — تبعيات ذات ثغرات معروفة:**
- `Microsoft.OpenApi` 2.0.0 — ثغرة عالية الخطورة، [GHSA-v5pm-xwqc-g5wc](https://github.com/advisories/GHSA-v5pm-xwqc-g5wc)
- `SSH.NET` 2024.2.0 (عبر Testcontainers) — ثغرة عالية الخطورة، [GHSA-q939-rpr3-3284](https://github.com/advisories/GHSA-q939-rpr3-3284)

تعالَج في مرحلة التبعيات.

## ما زال مفتوحاً

- **C-5** — HTTP الصريح في الإنتاج ⇒ الموجة ب (Traefik).
- **H-1** — الأدوار غير مُنفَّذة؛ يحتاج مصفوفة صلاحيات قبل التنفيذ.
- **H-2** — `accessToken` مكشوف لـ JavaScript في `auth.ts`.
- **H-3** — لا rate limiting ⇒ الموجة ب (Redis).
- **H-4** — سباق المخزون.
- **H-5** — لا إبطال للتوكن.
- **M-2 … M-7** — كما وردت أعلاه.
