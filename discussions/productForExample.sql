-- ============================================================
-- AL-NABAA — Seed Data
-- منتج: ASUS ROG Strix G16 (2025) G615JPR-RV103
-- المصدر: https://store.alnabaa.com/products/asus-rog-strix-g16-...
-- ============================================================

BEGIN;

-- ─────────────────────────────────────────────
-- 1. الإعدادات العامة
-- ─────────────────────────────────────────────
INSERT INTO settings (id, key, value, type, "group") VALUES
(1,  'store_name_en',           'AL-NABAA',                          'string',  'general'),
(2,  'store_name_ar',           'النبع',                             'string',  'general'),
(3,  'default_currency',        'IQD',                               'string',  'general'),
(4,  'free_shipping_threshold', '100000',                            'integer', 'shipping'),
(5,  'support_email',           'store@alnabaa.com',                 'string',  'general'),
(6,  'support_phone',           '6543',                              'string',  'general'),
(7,  'store_address',           'HQ – Al-Sinaa Street, Baghdad, Iraq','string', 'general'),
(8,  'warranty_default_months', '12',                                'integer', 'general'),
(9,  'delivery_note_en',        'Same-day to 2-day delivery',        'string',  'shipping'),
(10, 'delivery_note_ar',        'توصيل خلال يوم إلى يومين',         'string',  'shipping');

-- ─────────────────────────────────────────────
-- 2. حساب الإدارة الوحيد (MVP)
-- كلمة المرور: Admin@123  (bcrypt — غيّرها في الإنتاج)
-- ─────────────────────────────────────────────
INSERT INTO admin_users (id, name, email, password_hash, role, is_active, created_at) VALUES
(1, 'مدير النبع', 'admin@alnabaa.com',
 '$2b$10$placeholder_hash_replace_in_production', 'super_admin', true, NOW());

-- ─────────────────────────────────────────────
-- 3. التصنيفات
-- ─────────────────────────────────────────────
INSERT INTO categories (id, parent_id, name_en, name_ar, slug, is_active, sort_order, created_at) VALUES
(1, NULL, 'Computers',  'حاسبات',   'computers', true, 1, NOW()),
(2, 1,    'Laptops',    'لابتوبات', 'laptops',   true, 1, NOW());

-- ─────────────────────────────────────────────
-- 4. العلامة التجارية
-- ─────────────────────────────────────────────
INSERT INTO brands (id, name, slug, website_url, is_featured, is_active, sort_order, created_at) VALUES
(1, 'ASUS', 'asus', 'https://www.asus.com', true, true, 1, NOW());

-- ─────────────────────────────────────────────
-- 5. المنتج ⭐
-- ─────────────────────────────────────────────
INSERT INTO products (
  id, category_id, brand_id,
  name_en, name_ar, slug, sku,
  description_en, description_ar,
  short_description_en, short_description_ar,
  price, compare_at_price, currency,
  stock_quantity, low_stock_threshold, min_order_quantity,
  weight, status, is_featured, requires_shipping,
  warranty_months, warranty_description,
  meta_title, meta_description,
  created_at, updated_at
) VALUES (
  1, 2, 1,
  'ASUS ROG Strix G16 (2025) G615JPR-RV103 16" 165Hz - Core i7 14650HX - 16GB RAM - 512GB SSD - GN22-X6 GeForce RTX 5070 8GB - WIN 11 PRO K',
  'ASUS ROG Strix G16 (2025) G615JPR-RV103 16" 165Hz - Core i7 14650HX - 16GB RAM - 512GB SSD - GN22-X6 GeForce RTX 5070 8GB - WIN 11 PRO K',
  'asus-rog-strix-g16-2025-g615jpr-rv103-16-240hz-core-i7-14650hx-16gb-ram-512gb-ssd-gn22-x6-geforce-rtx-5070-8gb-win-11-pro-k',
  '90NR0L92-M004A0',
  E'• Intel® Core™ i7 Processor 14650HX\n• GN22-X6 GeForce RTX™ 5070 Laptop GPU ROG Boost: 1475MHz at 115W (1425MHz Boost Clock+50MHz OC, 100W+15W Dynamic Boost) 8GB GDDR7\n• 16GB DDR5-5600 SO-DIMM\n• 512GB PCIe® 4.0 NVMe™ M.2 SSD\n• 16-inch FHD+ 16:10 (1920 x 1200, WUXGA) IPS-level Anti-glare display\n• sRGB: 100% | Refresh Rate: 165Hz | Response Time: 3ms\n• G-Sync | MUX Switch + NVIDIA® Advanced Optimus\n• Dolby Vision HDR | Windows 11 Pro',
  E'• معالج Intel® Core™ i7 14650HX\n• بطاقة رسوميات GN22-X6 GeForce RTX™ 5070 (8GB GDDR7)\n• ذاكرة 16GB DDR5-5600 SO-DIMM\n• تخزين 512GB PCIe® 4.0 NVMe™ M.2 SSD\n• شاشة 16 بوصة FHD+ 16:10 (1920×1200 WUXGA) IPS مضاد للانعكاس\n• sRGB 100% | معدل تحديث 165Hz | زمن استجابة 3ms\n• G-Sync | MUX Switch + NVIDIA Advanced Optimus\n• Dolby Vision HDR | Windows 11 Pro',
  'Gaming laptop with Core i7 14650HX, RTX 5070 8GB, 16GB RAM, 512GB SSD, 16" 165Hz display.',
  'لابتوب ألعاب بمعالج Core i7 14650HX، RTX 5070 8GB، 16GB RAM، 512GB SSD، شاشة 16" 165Hz.',
  2550000.00, NULL, 'IQD',
  5, 2, 1,
  2.50, 'active', true, true,
  12, '1-Year AL-NABAA warranty',
  'ASUS ROG Strix G16 (2025) G615JPR-RV103 | AL-NABAA',
  'Buy ASUS ROG Strix G16 (2025) with Core i7 14650HX, RTX 5070, 16GB RAM, 512GB SSD. Free delivery on orders over 100,000 IQD.',
  NOW(), NOW()
);

-- ─────────────────────────────────────────────
-- 6. صور المنتج (من CDN متجر النبع)
-- ─────────────────────────────────────────────
INSERT INTO product_images (id, product_id, variant_id, image_url, alt_text, sort_order, is_primary, created_at) VALUES
(1,  1, NULL, 'https://store.alnabaa.com/cdn/shop/files/h732_666c8ded-b1d7-4bd6-a9da-6c648683ba33.png?v=1752478644', 'ASUS ROG Strix G16 - Front',        0,  true,  NOW()),
(2,  1, NULL, 'https://store.alnabaa.com/cdn/shop/files/h732_666c8ded-b1d7-4bd6-a9da-6c648683ba33.png?v=1752478644', 'ASUS ROG Strix G16 - Angle 1',      1,  false, NOW()),
(3,  1, NULL, 'https://store.alnabaa.com/cdn/shop/files/h732_666c8ded-b1d7-4bd6-a9da-6c648683ba33.png?v=1752478644', 'ASUS ROG Strix G16 - Angle 2',      2,  false, NOW()),
(4,  1, NULL, 'https://store.alnabaa.com/cdn/shop/files/h732_666c8ded-b1d7-4bd6-a9da-6c648683ba33.png?v=1752478644', 'ASUS ROG Strix G16 - Keyboard',     3,  false, NOW()),
(5,  1, NULL, 'https://store.alnabaa.com/cdn/shop/files/h732_666c8ded-b1d7-4bd6-a9da-6c648683ba33.png?v=1752478644', 'ASUS ROG Strix G16 - Ports',        4,  false, NOW());

-- ─────────────────────────────────────────────
-- 7. الخصائص التقنية (Groups + Attributes + Values)
-- ─────────────────────────────────────────────
INSERT INTO attribute_groups (id, name_en, name_ar, slug, sort_order, is_filterable) VALUES
(1, 'Processor', 'المعالج',   'processor', 1, true),
(2, 'Display',   'الشاشة',    'display',   2, true),
(3, 'Memory',    'الذاكرة',   'memory',    3, true),
(4, 'Graphics',  'الرسوميات', 'graphics',  4, true);

INSERT INTO attributes (id, group_id, name_en, name_ar, slug, unit, input_type, is_filterable, sort_order) VALUES
(1, 1, 'Processor',              'المعالج',           'processor',              NULL,  'text',    true, 1),
(2, 2, 'Display Size',           'حجم الشاشة',        'display-size',           'inch', 'text',   true, 1),
(3, 2, 'Monitor Resolution',     'دقة الشاشة',        'monitor-resolution',     NULL,  'text',    true, 2),
(4, 2, 'Refresh Rate',           'معدل التحديث',      'refresh-rate',           'Hz',  'number',  true, 3),
(5, 2, 'Touch Screen',           'شاشة لمس',          'touch-screen',           NULL,  'boolean', true, 4),
(6, 3, 'RAM',                    'الذاكرة RAM',       'ram',                    'GB',  'text',    true, 1),
(7, 3, 'Storage Capacity',       'سعة التخزين',       'storage-capacity',       'GB',  'text',    true, 2),
(8, 3, 'Storage Type',           'نوع التخزين',       'storage-type',           NULL,  'text',    true, 3),
(9, 4, 'GPU',                    'بطاقة الرسوميات',   'gpu',                    NULL,  'text',    true, 1),
(10, 4, 'Graphic Card Memory',    'ذاكرة الرسوميات', 'graphic-card-memory',    'GB',  'text',    true, 2);

INSERT INTO product_attribute_values (id, product_id, attribute_id, value_text, value_number, value_boolean) VALUES
(1,  1, 1,  'Intel Core i7-14650HX',                              NULL, NULL),
(2,  1, 2,  '16',                                                 16.00, NULL),
(3,  1, 3,  '1920 x 1200 WUXGA FHD+ 16:10',                       NULL, NULL),
(4,  1, 4,  NULL,                                                 165.00, NULL),
(5,  1, 5,  NULL,                                                 NULL, false),
(6,  1, 6,  '16GB DDR5-5600',                                     NULL, NULL),
(7,  1, 7,  '512',                                                512.00, NULL),
(8,  1, 8,  'PCIe 4.0 NVMe M.2 SSD',                              NULL, NULL),
(9,  1, 9,  'NVIDIA GeForce RTX 5070 (GN22-X6) ROG Boost 115W',  NULL, NULL),
(10, 1, 10, '8GB GDDR7',                                          NULL, NULL);

-- ─────────────────────────────────────────────
-- 8. التاغات
-- ─────────────────────────────────────────────
INSERT INTO tags (id, name, slug) VALUES
(1, 'Gaming',        'gaming'),
(2, 'ROG',           'rog'),
(3, 'RTX 5070',      'rtx-5070');

INSERT INTO product_tags (id, product_id, tag_id) VALUES
(1, 1, 1),
(2, 1, 2),
(3, 1, 3);

-- ─────────────────────────────────────────────
-- 9. مجموعة تسويقية (اختياري — للصفحة الرئيسية)
-- ─────────────────────────────────────────────
INSERT INTO collections (id, name_en, name_ar, slug, collection_type, is_active, sort_order, created_at) VALUES
(1, 'Gaming Laptops', 'لابتوبات الألعاب', 'gaming-laptops', 'manual', true, 1, NOW());

INSERT INTO collection_products (id, collection_id, product_id, sort_order, created_at) VALUES
(1, 1, 1, 1, NOW());

-- ─────────────────────────────────────────────
-- 10. قائمة التنقل
-- ─────────────────────────────────────────────
INSERT INTO menus (id, name, label_en, label_ar, is_active, created_at) VALUES
(1, 'main_header', 'Main Navigation', 'القائمة الرئيسية', true, NOW());

INSERT INTO menu_items (id, menu_id, parent_id, label_en, label_ar, url, link_type, link_id, sort_order, is_active) VALUES
(1, 1, NULL, 'Computers', 'حاسبات', '/categories/computers', 'category', 1, 1, true),
(2, 1, 1,    'Laptops',   'لابتوبات', '/categories/laptops',   'category', 2, 1, true);

-- ─────────────────────────────────────────────
-- 11. الشحن
-- ─────────────────────────────────────────────
INSERT INTO shipping_zones (id, name_en, name_ar, governorates, is_active, created_at) VALUES
(1, 'All Iraq', 'جميع المحافظات', 'Baghdad,Basra,Nineveh,Erbil,Sulaymaniyah,Najaf,Karbala,Anbar,Diyala,Dhi Qar,Maysan,Wasit,Salah al-Din,Kirkuk,Babil,Diwaniyah,Muthanna,Qadisiyyah', true, NOW());

INSERT INTO shipping_rates (id, shipping_zone_id, name_en, name_ar, rate_type, price, free_shipping_threshold, estimated_days_min, estimated_days_max, is_active, created_at) VALUES
(1, 1, 'Standard Delivery', 'توصيل عادي', 'flat', 15000.00, 100000.00, 1, 2, true, NOW());

-- ─────────────────────────────────────────────
-- 12. طريقة الدفع (COD فقط في MVP)
-- ─────────────────────────────────────────────
INSERT INTO payment_methods (id, name, label_en, label_ar, type, instructions_en, instructions_ar, is_active, sort_order, created_at) VALUES
(1, 'cod', 'Cash on Delivery', 'الدفع عند الاستلام', 'offline',
 'Pay safely at your doorstep. No online payment or card details needed.',
 'ادفع بأمان عند باب منزلك. لا حاجة لدفع إلكتروني أو بيانات بطاقة.',
 true, 1, NOW());

-- ─────────────────────────────────────────────
-- 13. بنر + صفحات ثابتة
-- ─────────────────────────────────────────────
INSERT INTO banners (id, title_en, title_ar, subtitle_en, subtitle_ar, image_url, link_url, button_text_en, button_text_ar, position, sort_order, is_active, starts_at) VALUES
(1,
 'ASUS ROG Strix G16 (2025)',
 'ASUS ROG Strix G16 (2025)',
 'Core i7 14650HX · RTX 5070 · 16GB · 512GB',
 'Core i7 14650HX · RTX 5070 · 16GB · 512GB',
 'https://store.alnabaa.com/cdn/shop/files/h732_666c8ded-b1d7-4bd6-a9da-6c648683ba33.png?v=1752478644',
 '/products/asus-rog-strix-g16-2025-g615jpr-rv103-16-240hz-core-i7-14650hx-16gb-ram-512gb-ssd-gn22-x6-geforce-rtx-5070-8gb-win-11-pro-k',
 'Shop Now', 'تسوق الآن', 'hero', 1, true, NOW());

INSERT INTO pages (id, title_en, title_ar, slug, content_en, content_ar, is_published) VALUES
(1, 'About AL-NABAA', 'عن النبع', 'about',
 'Since 1989 — Iraq''s trusted electronics destination.',
 'منذ 1989 — وجهتك الموثوقة للإلكترونيات في العراق.',
 true),
(2, 'Refund Policy', 'سياسة الإرجاع', 'refund-policy',
 'Minimum 1-year warranty on all products.',
 'ضمان لا يقل عن سنة واحدة على جميع المنتجات.',
 true);

-- ─────────────────────────────────────────────
-- 14. سجل نشاط (إضافة المنتج)
-- ─────────────────────────────────────────────
INSERT INTO activity_logs (id, admin_user_id, action, entity_type, entity_id, new_values, ip_address, created_at) VALUES
(1, 1, 'create', 'product', 1,
 '{"name_en":"ASUS ROG Strix G16 (2025)","sku":"90NR0L92-M004A0","price":2550000}',
 '127.0.0.1', NOW());

-- ─────────────────────────────────────────────
-- تحديث Sequences (PostgreSQL)
-- ─────────────────────────────────────────────
SELECT setval('settings_id_seq',              (SELECT MAX(id) FROM settings));
SELECT setval('admin_users_id_seq',         (SELECT MAX(id) FROM admin_users));
SELECT setval('categories_id_seq',          (SELECT MAX(id) FROM categories));
SELECT setval('brands_id_seq',              (SELECT MAX(id) FROM brands));
SELECT setval('products_id_seq',            (SELECT MAX(id) FROM products));
SELECT setval('product_images_id_seq',      (SELECT MAX(id) FROM product_images));
SELECT setval('attribute_groups_id_seq',    (SELECT MAX(id) FROM attribute_groups));
SELECT setval('attributes_id_seq',          (SELECT MAX(id) FROM attributes));
SELECT setval('product_attribute_values_id_seq', (SELECT MAX(id) FROM product_attribute_values));
SELECT setval('tags_id_seq',                (SELECT MAX(id) FROM tags));
SELECT setval('product_tags_id_seq',        (SELECT MAX(id) FROM product_tags));
SELECT setval('collections_id_seq',         (SELECT MAX(id) FROM collections));
SELECT setval('collection_products_id_seq', (SELECT MAX(id) FROM collection_products));
SELECT setval('menus_id_seq',               (SELECT MAX(id) FROM menus));
SELECT setval('menu_items_id_seq',          (SELECT MAX(id) FROM menu_items));
SELECT setval('shipping_zones_id_seq',      (SELECT MAX(id) FROM shipping_zones));
SELECT setval('shipping_rates_id_seq',      (SELECT MAX(id) FROM shipping_rates));
SELECT setval('payment_methods_id_seq',     (SELECT MAX(id) FROM payment_methods));
SELECT setval('banners_id_seq',             (SELECT MAX(id) FROM banners));
SELECT setval('pages_id_seq',               (SELECT MAX(id) FROM pages));
SELECT setval('activity_logs_id_seq',       (SELECT MAX(id) FROM activity_logs));

COMMIT;