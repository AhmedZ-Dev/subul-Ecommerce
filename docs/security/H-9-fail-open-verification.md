# H-9 — «الـ rate limiter يفشل مفتوحاً بصمت»: مستند تحقّق

> **الغرض:** هذا المستند مكتوب ليُراجَع من طرف مستقل (مهندس أو نموذج ذكاء اصطناعي آخر).
> كل ادّعاء فيه مقترن بموضعه في الشيفرة وبأمر يُعيد إنتاج النتيجة. المطلوب من المراجع
> أن **يتحقق أو يُفنِّد**، لا أن يوافق.
>
> **المشروع:** subul-Ecommerce · **الفرع:** `securitycheck` · **التاريخ:** 2026-09-06
> **الملف محل الفحص:** `backend/Common/RateLimiting/RedisRateLimiter.cs`

---

## 1. ما لا يدّعيه هذا المستند

يجب توضيح هذا أولاً لأن الالتباس وقع فعلاً في نقاش سابق:

- ❌ **لا يُدّعى** أن الـ rate limiting غير مُطبَّق.
- ❌ **لا يُدّعى** أنه غير مُسجَّل في الـ pipeline.
- ❌ **لا يُدّعى** أنه معطّل في البيئات المُعدَّة حالياً.

الميزة **مُطبَّقة، ومُسجَّلة، وفعّالة** في بيئتَي التطوير والإنتاج (Docker) كما هما مضبوطتان اليوم. أُضيفت في الـ commit `3adef9c`، وتنفيذها جيد تقنياً (سكربت Lua ذرّي، تجزئة SHA-256 لمُعرّف العميل، سياسة منفصلة لتسجيل الدخول، ترويسات `X-RateLimit-*` و`Retry-After`).

---

## 2. الادّعاء محل التحقق

> **الادّعاء:** حين يغيب المحدِّد أو يتعطّل، يستمر النظام في **قبول** الطلبات (fail open) **دون أي إشارة مرئية** (بصمت). ولا يوجد فرق ملحوظ عند الإقلاع أو في السجل بين «الحماية تعمل» و«الحماية غائبة تماماً».

الادّعاء مركّب من جزأين، ويُتحقق من كلٍّ منهما على حدة:

| # | الجزء | معناه |
|---|---|---|
| أ | **يفشل مفتوحاً** | عند تعطّل مكوّن الحماية، يمر الطلب بدل أن يُرفض |
| ب | **بصمت** | لا خطأ، ولا رفض إقلاع، ولا سجل (في إحدى الحالتين) |

---

## 3. الأدلّة

### 3-أ. التسجيل في الـ DI مشروط باتصال Redis

`backend/DependencyInjection/ServiceCollectionExtensions.cs:37,47,51`

```csharp
var redisConnectionString = configuration.GetConnectionString("Redis");   // سطر 37
...
if (!string.IsNullOrWhiteSpace(redisConnectionString))                    // سطر 47
{
    services.AddSingleton<IConnectionMultiplexer>(_ =>
        ConnectionMultiplexer.Connect(redisConnectionString));
    services.AddSingleton<RedisRateLimiter>();                            // سطر 51
}
```

**النتيجة:** إن غاب `ConnectionStrings:Redis` فلن يُسجَّل `RedisRateLimiter` إطلاقاً.

**ملاحظة مهمة:** `ValidateOnStart()` في السطر 45 يتحقق من قيم `RedisRateLimit` (أن `PermitLimit > 0` إلخ) — أي أنه يتحقق من **الأرقام**، ولا يتحقق من **وجود اتصال Redis**. فهو لا يمنع الإقلاع في هذه الحالة.

---

### 3-ب. الـ middleware يمرّر الطلب حين يغيب المحدِّد — بلا سجل

`backend/Common/RateLimiting/RedisRateLimiter.cs:85-89`

```csharp
var rateLimiter = context.RequestServices.GetService<RedisRateLimiter>();  // سطر 85
if (rateLimiter is null)                                                   // سطر 86
{
    await next(context);                                                   // سطر 88
    return;
}
```

`GetService<T>` (لا `GetRequiredService<T>`) تُعيد `null` بدل أن ترمي استثناءً. ثم `await next(context)` تُمرِّر الطلب إلى بقية الـ pipeline.

**هذا هو المسار الأول: مفتوح + صامت.** لا استثناء، ولا `LogWarning`، ولا أي أثر.

---

### 3-ج. الـ middleware يمرّر الطلب حين يتعطّل Redis — مع سجل تحذيري

`backend/Common/RateLimiting/RedisRateLimiter.cs:113-117`

```csharp
catch (Exception exception) when (exception is RedisException or TimeoutException)  // سطر 113
{
    logger.LogWarning(exception, "Redis rate limiter is unavailable; allowing the request");  // سطر 115
    await next(context);                                                             // سطر 116
    return;
}
```

**هذا هو المسار الثاني: مفتوح، لكنه ليس صامتاً تماماً** — يوجد `LogWarning` على الأقل. القرار مع ذلك هو التمرير.

---

### 3-د. `appsettings.json` الأساسي يقول `Enabled: true` بلا اتصال Redis

`backend/appsettings.json` يحوي:

```json
"RedisRateLimit": {
  "Enabled": true,
  "PermitLimit": 120,
  "WindowSeconds": 60,
  "LoginPermitLimit": 10,
  "LoginWindowSeconds": 300
}
```

ولا يحوي `ConnectionStrings:Redis`. الاتصال يأتي حصراً من ملفَّي التجاوز:

| المصدر | السطر | القيمة |
|---|---|---|
| `backend/appsettings.Development.json` | 18 | `localhost:6379,password=redis,abortConnect=false` |
| `docker-compose.yml` | 46 | `ConnectionStrings__Redis: redis:6379,abortConnect=false` |

**النتيجة:** أي نشر لا يمر عبر هذين المسارين (تشغيل مباشر بـ `ASPNETCORE_ENVIRONMENT=Production`، أو منسّق آخر، أو نسيان متغيّر بيئة واحد) يعمل بلا تحديد معدّل — بينما الإعداد يعلن `"Enabled": true`.

---

### 3-هـ. `abortConnect=false` يمنع الانفجار عند الإقلاع

سلسلتا الاتصال تحتويان `abortConnect=false`. هذا يجعل `ConnectionMultiplexer.Connect` **لا ترمي** استثناءً إن كان Redis غير متاح، بل تُعيد multiplexer يعيد المحاولة في الخلفية.

**النتيجة:** التطبيق يقلع بنجاح حتى مع Redis معطّل، ثم تسقط الطلبات في مسار 3-ج.

---

### 3-و. دليل عملي: بيئة الاختبار نفسها تسلك المسار المفتوح

`backend.Tests/Infrastructure/TestWebApplicationFactory.cs` يضبط `ConnectionStrings:DefaultConnection` فقط، ولا يضبط `Redis` إطلاقاً.

**النتيجة:** الـ 450 اختباراً كلها تمر عبر فرع `rateLimiter is null` في السطر 86. أي أن المسار المفتوح ليس افتراضاً نظرياً — إنه المسار المسلوك افتراضياً في بيئة كاملة قائمة اليوم، دون أن يلاحظه أحد. وهذا في ذاته تجسيد للادّعاء.

---

### 3-ز. لا يوجد أي اختبار يغطي الـ rate limiting

بحث عن `RateLimit` أو `429` أو `TooManyRequests` في `backend.Tests/` يُعيد **صفر نتيجة**. لا شيء يكشف لو انكسرت الميزة أو غابت.

---

## 4. أوامر إعادة إنتاج النتائج

```bash
# 3-أ  التسجيل المشروط في الـ DI
grep -n "redisConnectionString\|AddSingleton<RedisRateLimiter>\|ValidateOnStart" \
  backend/DependencyInjection/ServiceCollectionExtensions.cs

# 3-ب و 3-ج  مساري التمرير في الـ middleware
sed -n '75,120p' backend/Common/RateLimiting/RedisRateLimiter.cs

# 3-د  أين يُعرَّف اتصال Redis (ولاحظ غيابه عن appsettings.json)
grep -rn "Redis" backend/appsettings*.json docker-compose.yml

# 3-و  بيئة الاختبار لا تضبط Redis
grep -n "UseSetting\|ConnectionStrings" backend.Tests/Infrastructure/TestWebApplicationFactory.cs

# 3-ز  غياب اختبارات الـ rate limiting
grep -rln "RateLimit\|429\|TooManyRequests" backend.Tests/ --include=*.cs

# الـ middleware مُسجَّل فعلاً وفي الموضع الصحيح (دليل أن الميزة تعمل)
grep -n "UseAuthentication\|RedisRateLimitMiddleware\|UseAuthorization" backend/Program.cs
```

### فحص بيئة حيّة

الترويسات تُضاف فقط حين يعمل المحدِّد فعلاً (`RedisRateLimiter.cs:124`)، فوجودها دليل التشغيل وغيابها دليل التعطّل:

```bash
curl -sSI http://<الخادم>:5101/api/products | grep -i "x-ratelimit"
```

- ظهور `X-RateLimit-Limit: 120` ⇒ المحدِّد يعمل.
- خروج فارغ ⇒ المحدِّد غير فاعل على تلك البيئة.

---

## 5. ما الذي يُفنِّد هذا الادّعاء

على المراجع أن يبحث عن أيٍّ مما يلي؛ وجود واحد منها يُضعف الادّعاء أو يُسقطه:

1. **رفض إقلاع** حين يكون `RedisRateLimit:Enabled = true` بلا `ConnectionStrings:Redis` — لم أجده؛ إن وُجد فالجزء «بصمت» ساقط في المسار الأول.
2. **طبقة تحديد معدّل أخرى** خارج هذا الـ middleware (مثل `AddRateLimiter` المدمج في ASP.NET، أو WAF، أو حد على مستوى Nginx/Cloudflare/Traefik) — لم أجد شيئاً منها في المستودع، لكن **قد توجد على مستوى البنية التحتية خارج هذا المستودع**، وهذا خارج ما تستطيع الشيفرة إثباته. إن وُجد حدّ على مستوى الشبكة فالأثر العملي أقل بكثير مما وصفتُه.
3. **`ConnectionStrings:Redis` مضبوط عبر مصدر لم أفحصه** — مثل user-secrets، أو Azure Key Vault، أو متغيّر بيئة على الخادم. فحصتُ ملفات `appsettings*.json` و`docker-compose.yml` فقط.
4. **`GetRequiredService` بدل `GetService`** في السطر 85 — عندها يصير الفشل مغلقاً (استثناء ⇒ 500). النص الحالي `GetService`.

---

## 6. سؤالان محدّدان للمراجع

1. **هل التوصيف دقيق؟** أي: هل صحيح أن غياب `ConnectionStrings:Redis` يُنتج نظاماً يقلع بنجاح، ويعلن `"Enabled": true`، ولا يحدّ أي طلب، ولا يُصدر أي تحذير؟
2. **هل الإصلاح المقترح متناسب؟** المقترح:
   - `ValidateOnStart` يرفض الإقلاع إن كان `Enabled = true` بلا اتصال Redis (يعالج «بصمت» في المسار الأول)؛
   - **fail-closed على `/api/auth/login` وحده** عند تعذّر الفحص (429)، مع **إبقاء fail-open لبقية المسارات** حفاظاً على توفّر المتجر.

   التساؤل المطروح للنقاش: هل التمييز بين المسارين مبرَّر، أم يجب توحيد السلوك؟ الحجة للتمييز أن تعطّل Redis يجب ألّا يُسقط المتجر كله، بينما تعطيل حماية تسجيل الدخول تحديداً هو ما يريده المهاجم قبل هجوم التخمين.

---

## 7. سياق: أين يقع H-9 من التقرير الكامل

هذا المستند يعالج بنداً واحداً. التقرير الكامل — بما فيه بنود أعلى خطورة مثل تسريب `gateway_config` وXSS المخزّن — في:

- `docs/security/SECURITY-AUDIT-WAVE-B.md` (الحالي)
- `docs/security/SECURITY-AUDIT-WAVE-A.md` (السابق)

H-9 مصنّف **عالٍ**، ومرتبته **الخامسة** في جدول الأولويات — بعد أربعة بنود نُفِّذت بالفعل، وهي: تسريب بيانات اعتماد بوابة الدفع (حرِج)، وXSS المخزّن، وثغرة `next-auth` الحرِجة، وتعداد الكتالوج غير المنشور.
