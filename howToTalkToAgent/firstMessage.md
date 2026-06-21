قم بتحليل المشروع الحالي بالكامل ثم أنشئ AI documentation و project rules احترافية تساعد Agents على العمل بدقة واستهلاك Context أقل.

أنشئ المجلدات والملفات التالية:

.cursor/rules/
docs/ai/

أريد ملفات منظمة تشمل:

1. architecture.mdc

* شرح Architecture المشروع
* naming conventions
* folder structure
* data flow
* API patterns
* state management
* reusable patterns

2. frontend.mdc

* قواعد React / Next.js
* form handling
* validation
* UI conventions
* Tailwind usage
* server/client component rules

3. backend.mdc

* service patterns
* API structure
* DTO handling
* database access rules
* error handling

4. security.mdc

* XSS prevention
* validation rules
* auth rules
* secret handling
* secure coding practices
* OWASP/CWE aligned rules

5. testing.mdc

* testing strategy
* unit/integration patterns
* mocking conventions

6. task-template.md

* workflow للـAgents قبل أي تعديل
* منع scanning كامل المشروع
* minimal changes philosophy
* explain-plan-before-code workflow

7. feature-workflow.md

* كيف يتم إنشاء CRUD modules
* patterns المستخدمة بالمشروع
* reusable feature structure

المطلوب:

* اجعل الملفات قصيرة وعملية وليست generic
* استنتج القواعد من الكود الحالي
* لا تكتب best practices عامة غير مستخدمة بالمشروع
* استخدم project-specific conventions فقط
* لا تنشئ قواعد غير ضرورية
* اجعل rules تقلل context usage وتحسن accuracy
* أضف examples حقيقية من المشروع الحالي إن أمكن

قبل إنشاء الملفات:

1. حلل المشروع
2. استنتج architecture الحقيقي
3. ثم أنشئ الملفات
