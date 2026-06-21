using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using backend.Domain.Entities;
using AttributeEntity = backend.Domain.Entities.Attribute;

namespace backend.Infrastructure.Persistence;

public partial class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<ActivityLog> ActivityLogs { get; set; }

    public virtual DbSet<Address> Addresses { get; set; }

    public virtual DbSet<AdminUser> AdminUsers { get; set; }

    public virtual DbSet<AttributeEntity> Attributes { get; set; }

    public virtual DbSet<AttributeGroup> AttributeGroups { get; set; }

    public virtual DbSet<BackInStockRequest> BackInStockRequests { get; set; }

    public virtual DbSet<Banner> Banners { get; set; }

    public virtual DbSet<Brand> Brands { get; set; }

    public virtual DbSet<Cart> Carts { get; set; }

    public virtual DbSet<CartItem> CartItems { get; set; }

    public virtual DbSet<CashCollection> CashCollections { get; set; }

    public virtual DbSet<Category> Categories { get; set; }

    public virtual DbSet<Collection> Collections { get; set; }

    public virtual DbSet<CollectionProduct> CollectionProducts { get; set; }

    public virtual DbSet<ContactMessage> ContactMessages { get; set; }

    public virtual DbSet<DeliveryAgent> DeliveryAgents { get; set; }

    public virtual DbSet<Discount> Discounts { get; set; }

    public virtual DbSet<DiscountCondition> DiscountConditions { get; set; }

    public virtual DbSet<DiscountUsage> DiscountUsages { get; set; }

    public virtual DbSet<FlashSale> FlashSales { get; set; }

    public virtual DbSet<FlashSaleProduct> FlashSaleProducts { get; set; }

    public virtual DbSet<InventoryMovement> InventoryMovements { get; set; }

    public virtual DbSet<Menu> Menus { get; set; }

    public virtual DbSet<MenuItem> MenuItems { get; set; }

    public virtual DbSet<Notification> Notifications { get; set; }

    public virtual DbSet<Order> Orders { get; set; }

    public virtual DbSet<OrderDelivery> OrderDeliveries { get; set; }

    public virtual DbSet<OrderItem> OrderItems { get; set; }

    public virtual DbSet<OrderStatusHistory> OrderStatusHistories { get; set; }

    public virtual DbSet<Page> Pages { get; set; }

    public virtual DbSet<PaymentMethod> PaymentMethods { get; set; }

    public virtual DbSet<PaymentTransaction> PaymentTransactions { get; set; }

    public virtual DbSet<Product> Products { get; set; }

    public virtual DbSet<ProductAttributeValue> ProductAttributeValues { get; set; }

    public virtual DbSet<ProductCompare> ProductCompares { get; set; }

    public virtual DbSet<ProductImage> ProductImages { get; set; }

    public virtual DbSet<ProductTag> ProductTags { get; set; }

    public virtual DbSet<ProductVariant> ProductVariants { get; set; }

    public virtual DbSet<PurchaseOrder> PurchaseOrders { get; set; }

    public virtual DbSet<PurchaseOrderItem> PurchaseOrderItems { get; set; }

    public virtual DbSet<Return> Returns { get; set; }

    public virtual DbSet<ReturnItem> ReturnItems { get; set; }

    public virtual DbSet<Review> Reviews { get; set; }

    public virtual DbSet<Setting> Settings { get; set; }

    public virtual DbSet<ShippingRate> ShippingRates { get; set; }

    public virtual DbSet<ShippingZone> ShippingZones { get; set; }

    public virtual DbSet<Supplier> Suppliers { get; set; }

    public virtual DbSet<Tag> Tags { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<WarrantyClaim> WarrantyClaims { get; set; }

    public virtual DbSet<Wishlist> Wishlists { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ActivityLog>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("activity_logs_pkey");

            entity.ToTable("activity_logs");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Action)
                .HasMaxLength(100)
                .HasColumnName("action");
            entity.Property(e => e.AdminUserId).HasColumnName("admin_user_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("now()")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.EntityId).HasColumnName("entity_id");
            entity.Property(e => e.EntityType)
                .HasMaxLength(100)
                .HasColumnName("entity_type");
            entity.Property(e => e.IpAddress)
                .HasMaxLength(50)
                .HasColumnName("ip_address");
            entity.Property(e => e.NewValues).HasColumnName("new_values");
            entity.Property(e => e.OldValues).HasColumnName("old_values");

            entity.HasOne(d => d.AdminUser).WithMany(p => p.ActivityLogs)
                .HasForeignKey(d => d.AdminUserId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("activity_logs_admin_user_id_fkey");
        });

        modelBuilder.Entity<Address>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("addresses_pkey");

            entity.ToTable("addresses");

            entity.HasIndex(e => e.UserId, "idx_addresses_user_id");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Address1).HasColumnName("address1");
            entity.Property(e => e.Address2).HasColumnName("address2");
            entity.Property(e => e.City)
                .HasMaxLength(100)
                .HasColumnName("city");
            entity.Property(e => e.Country)
                .HasMaxLength(100)
                .HasDefaultValueSql("'Iraq'::character varying")
                .HasColumnName("country");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("now()")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.FirstName)
                .HasMaxLength(100)
                .HasColumnName("first_name");
            entity.Property(e => e.Governorate)
                .HasMaxLength(100)
                .HasColumnName("governorate");
            entity.Property(e => e.IsDefault).HasColumnName("is_default");
            entity.Property(e => e.LastName)
                .HasMaxLength(100)
                .HasColumnName("last_name");
            entity.Property(e => e.Phone)
                .HasMaxLength(20)
                .HasColumnName("phone");
            entity.Property(e => e.UpdatedAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("updated_at");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.User).WithMany(p => p.Addresses)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("addresses_user_id_fkey");
        });

        modelBuilder.Entity<AdminUser>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("admin_users_pkey");

            entity.ToTable("admin_users");

            entity.HasIndex(e => e.Email, "admin_users_email_key").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("now()")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.Email)
                .HasMaxLength(255)
                .HasColumnName("email");
            entity.Property(e => e.IsActive)
                .HasDefaultValue(true)
                .HasColumnName("is_active");
            entity.Property(e => e.LastLoginAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("last_login_at");
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .HasColumnName("name");
            entity.Property(e => e.PasswordHash)
                .HasMaxLength(255)
                .HasColumnName("password_hash");
            entity.Property(e => e.Role)
                .HasMaxLength(50)
                .HasDefaultValueSql("'staff'::character varying")
                .HasColumnName("role");
            entity.Property(e => e.UpdatedAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("updated_at");
        });

        modelBuilder.Entity<AttributeEntity>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("attributes_pkey");

            entity.ToTable("attributes");

            entity.HasIndex(e => e.Slug, "attributes_slug_key").IsUnique();

            entity.HasIndex(e => e.GroupId, "idx_attributes_group_id");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("now()")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.GroupId).HasColumnName("group_id");
            entity.Property(e => e.InputType)
                .HasMaxLength(50)
                .HasDefaultValueSql("'text'::character varying")
                .HasColumnName("input_type");
            entity.Property(e => e.IsFilterable)
                .HasDefaultValue(true)
                .HasColumnName("is_filterable");
            entity.Property(e => e.NameAr)
                .HasMaxLength(255)
                .HasColumnName("name_ar");
            entity.Property(e => e.NameEn)
                .HasMaxLength(255)
                .HasColumnName("name_en");
            entity.Property(e => e.Slug)
                .HasMaxLength(255)
                .HasColumnName("slug");
            entity.Property(e => e.SortOrder).HasColumnName("sort_order");
            entity.Property(e => e.Unit)
                .HasMaxLength(50)
                .HasColumnName("unit");

            entity.HasOne(d => d.Group).WithMany(p => p.Attributes)
                .HasForeignKey(d => d.GroupId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("attributes_group_id_fkey");
        });

        modelBuilder.Entity<AttributeGroup>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("attribute_groups_pkey");

            entity.ToTable("attribute_groups");

            entity.HasIndex(e => e.Slug, "attribute_groups_slug_key").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("now()")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.IsFilterable)
                .HasDefaultValue(true)
                .HasColumnName("is_filterable");
            entity.Property(e => e.NameAr)
                .HasMaxLength(100)
                .HasColumnName("name_ar");
            entity.Property(e => e.NameEn)
                .HasMaxLength(100)
                .HasColumnName("name_en");
            entity.Property(e => e.Slug)
                .HasMaxLength(100)
                .HasColumnName("slug");
            entity.Property(e => e.SortOrder).HasColumnName("sort_order");
        });

        modelBuilder.Entity<BackInStockRequest>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("back_in_stock_requests_pkey");

            entity.ToTable("back_in_stock_requests");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("now()")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.Email)
                .HasMaxLength(255)
                .HasColumnName("email");
            entity.Property(e => e.IsActive)
                .HasDefaultValue(true)
                .HasColumnName("is_active");
            entity.Property(e => e.NotifiedAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("notified_at");
            entity.Property(e => e.ProductId).HasColumnName("product_id");
            entity.Property(e => e.UserId).HasColumnName("user_id");
            entity.Property(e => e.VariantId).HasColumnName("variant_id");

            entity.HasOne(d => d.Product).WithMany(p => p.BackInStockRequests)
                .HasForeignKey(d => d.ProductId)
                .HasConstraintName("back_in_stock_requests_product_id_fkey");

            entity.HasOne(d => d.User).WithMany(p => p.BackInStockRequests)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("back_in_stock_requests_user_id_fkey");

            entity.HasOne(d => d.Variant).WithMany(p => p.BackInStockRequests)
                .HasForeignKey(d => d.VariantId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("back_in_stock_requests_variant_id_fkey");
        });

        modelBuilder.Entity<Banner>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("banners_pkey");

            entity.ToTable("banners");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.ButtonTextAr)
                .HasMaxLength(100)
                .HasColumnName("button_text_ar");
            entity.Property(e => e.ButtonTextEn)
                .HasMaxLength(100)
                .HasColumnName("button_text_en");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("now()")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.EndsAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("ends_at");
            entity.Property(e => e.ImageUrl).HasColumnName("image_url");
            entity.Property(e => e.IsActive)
                .HasDefaultValue(true)
                .HasColumnName("is_active");
            entity.Property(e => e.LinkUrl).HasColumnName("link_url");
            entity.Property(e => e.MobileImageUrl).HasColumnName("mobile_image_url");
            entity.Property(e => e.Position)
                .HasMaxLength(50)
                .HasColumnName("position");
            entity.Property(e => e.SortOrder).HasColumnName("sort_order");
            entity.Property(e => e.StartsAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("starts_at");
            entity.Property(e => e.SubtitleAr)
                .HasMaxLength(500)
                .HasColumnName("subtitle_ar");
            entity.Property(e => e.SubtitleEn)
                .HasMaxLength(500)
                .HasColumnName("subtitle_en");
            entity.Property(e => e.TitleAr)
                .HasMaxLength(255)
                .HasColumnName("title_ar");
            entity.Property(e => e.TitleEn)
                .HasMaxLength(255)
                .HasColumnName("title_en");
        });

        modelBuilder.Entity<Brand>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("brands_pkey");

            entity.ToTable("brands");

            entity.HasIndex(e => e.Slug, "brands_slug_key").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.BannerUrl).HasColumnName("banner_url");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("now()")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.DescriptionAr).HasColumnName("description_ar");
            entity.Property(e => e.DescriptionEn).HasColumnName("description_en");
            entity.Property(e => e.IsActive)
                .HasDefaultValue(true)
                .HasColumnName("is_active");
            entity.Property(e => e.IsFeatured).HasColumnName("is_featured");
            entity.Property(e => e.LogoUrl).HasColumnName("logo_url");
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .HasColumnName("name");
            entity.Property(e => e.Slug)
                .HasMaxLength(255)
                .HasColumnName("slug");
            entity.Property(e => e.SortOrder).HasColumnName("sort_order");
            entity.Property(e => e.UpdatedAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("updated_at");
            entity.Property(e => e.WebsiteUrl)
                .HasMaxLength(255)
                .HasColumnName("website_url");
        });

        modelBuilder.Entity<Cart>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("carts_pkey");

            entity.ToTable("carts");

            entity.HasIndex(e => e.UserId, "idx_carts_user_id");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CouponCode)
                .HasMaxLength(100)
                .HasColumnName("coupon_code");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("now()")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.ExpiresAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("expires_at");
            entity.Property(e => e.Notes).HasColumnName("notes");
            entity.Property(e => e.SessionId)
                .HasMaxLength(255)
                .HasColumnName("session_id");
            entity.Property(e => e.UpdatedAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("updated_at");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.User).WithMany(p => p.Carts)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("carts_user_id_fkey");
        });

        modelBuilder.Entity<CartItem>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("cart_items_pkey");

            entity.ToTable("cart_items");

            entity.HasIndex(e => e.CartId, "idx_cart_items_cart_id");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CartId).HasColumnName("cart_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("now()")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.ProductId).HasColumnName("product_id");
            entity.Property(e => e.Quantity)
                .HasDefaultValue(1)
                .HasColumnName("quantity");
            entity.Property(e => e.UnitPrice)
                .HasPrecision(15, 2)
                .HasColumnName("unit_price");
            entity.Property(e => e.UpdatedAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("updated_at");
            entity.Property(e => e.VariantId).HasColumnName("variant_id");

            entity.HasOne(d => d.Cart).WithMany(p => p.CartItems)
                .HasForeignKey(d => d.CartId)
                .HasConstraintName("cart_items_cart_id_fkey");

            entity.HasOne(d => d.Product).WithMany(p => p.CartItems)
                .HasForeignKey(d => d.ProductId)
                .HasConstraintName("cart_items_product_id_fkey");

            entity.HasOne(d => d.Variant).WithMany(p => p.CartItems)
                .HasForeignKey(d => d.VariantId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("cart_items_variant_id_fkey");
        });

        modelBuilder.Entity<CashCollection>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("cash_collections_pkey");

            entity.ToTable("cash_collections");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CollectedAmount)
                .HasPrecision(15, 2)
                .HasColumnName("collected_amount");
            entity.Property(e => e.CollectionDate).HasColumnName("collection_date");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("now()")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.DeliveryAgentId).HasColumnName("delivery_agent_id");
            entity.Property(e => e.Difference)
                .HasPrecision(15, 2)
                .HasColumnName("difference");
            entity.Property(e => e.ExpectedAmount)
                .HasPrecision(15, 2)
                .HasColumnName("expected_amount");
            entity.Property(e => e.Notes).HasColumnName("notes");
            entity.Property(e => e.ReceivedAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("received_at");
            entity.Property(e => e.ReceivedBy).HasColumnName("received_by");
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .HasDefaultValueSql("'pending'::character varying")
                .HasColumnName("status");
            entity.Property(e => e.UpdatedAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("updated_at");

            entity.HasOne(d => d.DeliveryAgent).WithMany(p => p.CashCollections)
                .HasForeignKey(d => d.DeliveryAgentId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("cash_collections_delivery_agent_id_fkey");

            entity.HasOne(d => d.ReceivedByNavigation).WithMany(p => p.CashCollections)
                .HasForeignKey(d => d.ReceivedBy)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("cash_collections_received_by_fkey");
        });

        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("categories_pkey");

            entity.ToTable("categories");

            entity.HasIndex(e => e.Slug, "categories_slug_key").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.BannerUrl).HasColumnName("banner_url");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("now()")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.DescriptionAr).HasColumnName("description_ar");
            entity.Property(e => e.DescriptionEn).HasColumnName("description_en");
            entity.Property(e => e.ImageUrl).HasColumnName("image_url");
            entity.Property(e => e.IsActive)
                .HasDefaultValue(true)
                .HasColumnName("is_active");
            entity.Property(e => e.NameAr)
                .HasMaxLength(255)
                .HasColumnName("name_ar");
            entity.Property(e => e.NameEn)
                .HasMaxLength(255)
                .HasColumnName("name_en");
            entity.Property(e => e.ParentId).HasColumnName("parent_id");
            entity.Property(e => e.SeoDescription).HasColumnName("seo_description");
            entity.Property(e => e.SeoTitle)
                .HasMaxLength(255)
                .HasColumnName("seo_title");
            entity.Property(e => e.Slug)
                .HasMaxLength(255)
                .HasColumnName("slug");
            entity.Property(e => e.SortOrder).HasColumnName("sort_order");
            entity.Property(e => e.UpdatedAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("updated_at");

            entity.HasOne(d => d.Parent).WithMany(p => p.InverseParent)
                .HasForeignKey(d => d.ParentId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("categories_parent_id_fkey");
        });

        modelBuilder.Entity<Collection>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("collections_pkey");

            entity.ToTable("collections");

            entity.HasIndex(e => e.Slug, "collections_slug_key").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.BannerUrl).HasColumnName("banner_url");
            entity.Property(e => e.CollectionType)
                .HasMaxLength(50)
                .HasDefaultValueSql("'manual'::character varying")
                .HasColumnName("collection_type");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("now()")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.DescriptionAr).HasColumnName("description_ar");
            entity.Property(e => e.DescriptionEn).HasColumnName("description_en");
            entity.Property(e => e.ImageUrl).HasColumnName("image_url");
            entity.Property(e => e.IsActive)
                .HasDefaultValue(true)
                .HasColumnName("is_active");
            entity.Property(e => e.MetaDescription).HasColumnName("meta_description");
            entity.Property(e => e.MetaTitle)
                .HasMaxLength(255)
                .HasColumnName("meta_title");
            entity.Property(e => e.NameAr)
                .HasMaxLength(255)
                .HasColumnName("name_ar");
            entity.Property(e => e.NameEn)
                .HasMaxLength(255)
                .HasColumnName("name_en");
            entity.Property(e => e.Slug)
                .HasMaxLength(255)
                .HasColumnName("slug");
            entity.Property(e => e.SortOrder).HasColumnName("sort_order");
            entity.Property(e => e.UpdatedAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("updated_at");
        });

        modelBuilder.Entity<CollectionProduct>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("collection_products_pkey");

            entity.ToTable("collection_products");

            entity.HasIndex(e => e.CollectionId, "idx_collection_products_collection_id");

            entity.HasIndex(e => e.ProductId, "idx_collection_products_product_id");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CollectionId).HasColumnName("collection_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("now()")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.ProductId).HasColumnName("product_id");
            entity.Property(e => e.SortOrder).HasColumnName("sort_order");

            entity.HasOne(d => d.Collection).WithMany(p => p.CollectionProducts)
                .HasForeignKey(d => d.CollectionId)
                .HasConstraintName("collection_products_collection_id_fkey");

            entity.HasOne(d => d.Product).WithMany(p => p.CollectionProducts)
                .HasForeignKey(d => d.ProductId)
                .HasConstraintName("collection_products_product_id_fkey");
        });

        modelBuilder.Entity<ContactMessage>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("contact_messages_pkey");

            entity.ToTable("contact_messages");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("now()")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.Email)
                .HasMaxLength(255)
                .HasColumnName("email");
            entity.Property(e => e.IpAddress)
                .HasMaxLength(50)
                .HasColumnName("ip_address");
            entity.Property(e => e.Message).HasColumnName("message");
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .HasColumnName("name");
            entity.Property(e => e.Phone)
                .HasMaxLength(20)
                .HasColumnName("phone");
            entity.Property(e => e.RepliedAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("replied_at");
            entity.Property(e => e.RepliedBy).HasColumnName("replied_by");
            entity.Property(e => e.ReplyMessage).HasColumnName("reply_message");
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .HasDefaultValueSql("'new'::character varying")
                .HasColumnName("status");
            entity.Property(e => e.Subject)
                .HasMaxLength(255)
                .HasColumnName("subject");
            entity.Property(e => e.UpdatedAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("updated_at");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.RepliedByNavigation).WithMany(p => p.ContactMessages)
                .HasForeignKey(d => d.RepliedBy)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("contact_messages_replied_by_fkey");

            entity.HasOne(d => d.User).WithMany(p => p.ContactMessages)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("contact_messages_user_id_fkey");
        });

        modelBuilder.Entity<DeliveryAgent>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("delivery_agents_pkey");

            entity.ToTable("delivery_agents");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Area)
                .HasMaxLength(100)
                .HasColumnName("area");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("now()")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.IsActive)
                .HasDefaultValue(true)
                .HasColumnName("is_active");
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .HasColumnName("name");
            entity.Property(e => e.NationalId)
                .HasMaxLength(50)
                .HasColumnName("national_id");
            entity.Property(e => e.Notes).HasColumnName("notes");
            entity.Property(e => e.Phone)
                .HasMaxLength(20)
                .HasColumnName("phone");
            entity.Property(e => e.UpdatedAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("updated_at");
            entity.Property(e => e.Whatsapp)
                .HasMaxLength(20)
                .HasColumnName("whatsapp");
        });

        modelBuilder.Entity<Discount>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("discounts_pkey");

            entity.ToTable("discounts");

            entity.HasIndex(e => e.Code, "discounts_code_key").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.AppliesTo)
                .HasMaxLength(50)
                .HasColumnName("applies_to");
            entity.Property(e => e.Code)
                .HasMaxLength(100)
                .HasColumnName("code");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("now()")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.Currency)
                .HasMaxLength(10)
                .HasDefaultValueSql("'IQD'::character varying")
                .HasColumnName("currency");
            entity.Property(e => e.EndsAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("ends_at");
            entity.Property(e => e.IsActive)
                .HasDefaultValue(true)
                .HasColumnName("is_active");
            entity.Property(e => e.MinOrderValue)
                .HasPrecision(15, 2)
                .HasColumnName("min_order_value");
            entity.Property(e => e.MinQuantity).HasColumnName("min_quantity");
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .HasColumnName("name");
            entity.Property(e => e.PerCustomerLimit)
                .HasDefaultValue(1)
                .HasColumnName("per_customer_limit");
            entity.Property(e => e.StartsAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("starts_at");
            entity.Property(e => e.Type)
                .HasMaxLength(50)
                .HasColumnName("type");
            entity.Property(e => e.UpdatedAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("updated_at");
            entity.Property(e => e.UsageCount).HasColumnName("usage_count");
            entity.Property(e => e.UsageLimit).HasColumnName("usage_limit");
            entity.Property(e => e.Value)
                .HasPrecision(15, 2)
                .HasColumnName("value");
        });

        modelBuilder.Entity<DiscountCondition>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("discount_conditions_pkey");

            entity.ToTable("discount_conditions");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("now()")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.DiscountId).HasColumnName("discount_id");
            entity.Property(e => e.EntityId).HasColumnName("entity_id");
            entity.Property(e => e.EntityType)
                .HasMaxLength(50)
                .HasColumnName("entity_type");

            entity.HasOne(d => d.Discount).WithMany(p => p.DiscountConditions)
                .HasForeignKey(d => d.DiscountId)
                .HasConstraintName("discount_conditions_discount_id_fkey");
        });

        modelBuilder.Entity<DiscountUsage>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("discount_usages_pkey");

            entity.ToTable("discount_usages");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.AmountSaved)
                .HasPrecision(15, 2)
                .HasColumnName("amount_saved");
            entity.Property(e => e.DiscountId).HasColumnName("discount_id");
            entity.Property(e => e.OrderId).HasColumnName("order_id");
            entity.Property(e => e.UsedAt)
                .HasDefaultValueSql("now()")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("used_at");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.Discount).WithMany(p => p.DiscountUsages)
                .HasForeignKey(d => d.DiscountId)
                .HasConstraintName("discount_usages_discount_id_fkey");

            entity.HasOne(d => d.Order).WithMany(p => p.DiscountUsages)
                .HasForeignKey(d => d.OrderId)
                .HasConstraintName("discount_usages_order_id_fkey");

            entity.HasOne(d => d.User).WithMany(p => p.DiscountUsages)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("discount_usages_user_id_fkey");
        });

        modelBuilder.Entity<FlashSale>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("flash_sales_pkey");

            entity.ToTable("flash_sales");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.BannerUrl).HasColumnName("banner_url");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("now()")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.CreatedBy).HasColumnName("created_by");
            entity.Property(e => e.DescriptionAr).HasColumnName("description_ar");
            entity.Property(e => e.DescriptionEn).HasColumnName("description_en");
            entity.Property(e => e.DiscountType)
                .HasMaxLength(50)
                .HasComment("percentage, fixed_amount")
                .HasColumnName("discount_type");
            entity.Property(e => e.DiscountValue)
                .HasPrecision(10, 2)
                .HasColumnName("discount_value");
            entity.Property(e => e.EndsAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("ends_at");
            entity.Property(e => e.IsActive)
                .HasDefaultValue(true)
                .HasColumnName("is_active");
            entity.Property(e => e.MaxUses).HasColumnName("max_uses");
            entity.Property(e => e.NameAr)
                .HasMaxLength(255)
                .HasColumnName("name_ar");
            entity.Property(e => e.NameEn)
                .HasMaxLength(255)
                .HasColumnName("name_en");
            entity.Property(e => e.SortOrder).HasColumnName("sort_order");
            entity.Property(e => e.StartsAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("starts_at");
            entity.Property(e => e.UpdatedAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("updated_at");
            entity.Property(e => e.UsesCount).HasColumnName("uses_count");

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.FlashSales)
                .HasForeignKey(d => d.CreatedBy)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("flash_sales_created_by_fkey");
        });

        modelBuilder.Entity<FlashSaleProduct>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("flash_sale_products_pkey");

            entity.ToTable("flash_sale_products");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("now()")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.FlashSaleId).HasColumnName("flash_sale_id");
            entity.Property(e => e.MaxQuantityPerOrder).HasColumnName("max_quantity_per_order");
            entity.Property(e => e.OriginalPrice)
                .HasPrecision(15, 2)
                .HasColumnName("original_price");
            entity.Property(e => e.ProductId).HasColumnName("product_id");
            entity.Property(e => e.QuantityLimit).HasColumnName("quantity_limit");
            entity.Property(e => e.QuantitySold).HasColumnName("quantity_sold");
            entity.Property(e => e.SalePrice)
                .HasPrecision(15, 2)
                .HasColumnName("sale_price");
            entity.Property(e => e.SortOrder).HasColumnName("sort_order");
            entity.Property(e => e.VariantId).HasColumnName("variant_id");

            entity.HasOne(d => d.FlashSale).WithMany(p => p.FlashSaleProducts)
                .HasForeignKey(d => d.FlashSaleId)
                .HasConstraintName("flash_sale_products_flash_sale_id_fkey");

            entity.HasOne(d => d.Product).WithMany(p => p.FlashSaleProducts)
                .HasForeignKey(d => d.ProductId)
                .HasConstraintName("flash_sale_products_product_id_fkey");

            entity.HasOne(d => d.Variant).WithMany(p => p.FlashSaleProducts)
                .HasForeignKey(d => d.VariantId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("flash_sale_products_variant_id_fkey");
        });

        modelBuilder.Entity<InventoryMovement>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("inventory_movements_pkey");

            entity.ToTable("inventory_movements");

            entity.HasIndex(e => e.ProductId, "idx_inventory_movements_product_id");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.AdminUserId).HasColumnName("admin_user_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("now()")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.MovementType)
                .HasMaxLength(50)
                .HasComment("PURCHASE_IN, SALE_OUT, RETURN_IN, ADJUSTMENT_IN, ADJUSTMENT_OUT, DAMAGE_OUT")
                .HasColumnName("movement_type");
            entity.Property(e => e.Notes).HasColumnName("notes");
            entity.Property(e => e.ProductId).HasColumnName("product_id");
            entity.Property(e => e.Quantity).HasColumnName("quantity");
            entity.Property(e => e.QuantityAfter).HasColumnName("quantity_after");
            entity.Property(e => e.QuantityBefore).HasColumnName("quantity_before");
            entity.Property(e => e.ReferenceId).HasColumnName("reference_id");
            entity.Property(e => e.ReferenceType)
                .HasMaxLength(50)
                .HasComment("purchase_order, order, return, manual")
                .HasColumnName("reference_type");
            entity.Property(e => e.UnitCost)
                .HasPrecision(15, 2)
                .HasColumnName("unit_cost");
            entity.Property(e => e.VariantId).HasColumnName("variant_id");

            entity.HasOne(d => d.AdminUser).WithMany(p => p.InventoryMovements)
                .HasForeignKey(d => d.AdminUserId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("inventory_movements_admin_user_id_fkey");

            entity.HasOne(d => d.Product).WithMany(p => p.InventoryMovements)
                .HasForeignKey(d => d.ProductId)
                .HasConstraintName("inventory_movements_product_id_fkey");

            entity.HasOne(d => d.Variant).WithMany(p => p.InventoryMovements)
                .HasForeignKey(d => d.VariantId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("inventory_movements_variant_id_fkey");
        });

        modelBuilder.Entity<Menu>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("menus_pkey");

            entity.ToTable("menus");

            entity.HasIndex(e => e.Name, "menus_name_key").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("now()")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.IsActive)
                .HasDefaultValue(true)
                .HasColumnName("is_active");
            entity.Property(e => e.LabelAr)
                .HasMaxLength(100)
                .HasColumnName("label_ar");
            entity.Property(e => e.LabelEn)
                .HasMaxLength(100)
                .HasColumnName("label_en");
            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .HasColumnName("name");
        });

        modelBuilder.Entity<MenuItem>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("menu_items_pkey");

            entity.ToTable("menu_items");

            entity.HasIndex(e => e.MenuId, "idx_menu_items_menu_id");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("now()")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.Icon)
                .HasMaxLength(100)
                .HasColumnName("icon");
            entity.Property(e => e.IsActive)
                .HasDefaultValue(true)
                .HasColumnName("is_active");
            entity.Property(e => e.LabelAr)
                .HasMaxLength(100)
                .HasColumnName("label_ar");
            entity.Property(e => e.LabelEn)
                .HasMaxLength(100)
                .HasColumnName("label_en");
            entity.Property(e => e.LinkId).HasColumnName("link_id");
            entity.Property(e => e.LinkType)
                .HasMaxLength(50)
                .HasComment("custom, category, collection, page, product")
                .HasColumnName("link_type");
            entity.Property(e => e.MenuId).HasColumnName("menu_id");
            entity.Property(e => e.ParentId).HasColumnName("parent_id");
            entity.Property(e => e.SortOrder).HasColumnName("sort_order");
            entity.Property(e => e.Target)
                .HasMaxLength(20)
                .HasDefaultValueSql("'_self'::character varying")
                .HasColumnName("target");
            entity.Property(e => e.Url)
                .HasMaxLength(500)
                .HasColumnName("url");

            entity.HasOne(d => d.Menu).WithMany(p => p.MenuItems)
                .HasForeignKey(d => d.MenuId)
                .HasConstraintName("menu_items_menu_id_fkey");

            entity.HasOne(d => d.Parent).WithMany(p => p.InverseParent)
                .HasForeignKey(d => d.ParentId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("menu_items_parent_id_fkey");
        });

        modelBuilder.Entity<Notification>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("notifications_pkey");

            entity.ToTable("notifications");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Body).HasColumnName("body");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("now()")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.Data).HasColumnName("data");
            entity.Property(e => e.ReadAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("read_at");
            entity.Property(e => e.Title)
                .HasMaxLength(255)
                .HasColumnName("title");
            entity.Property(e => e.Type)
                .HasMaxLength(100)
                .HasColumnName("type");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.User).WithMany(p => p.Notifications)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("notifications_user_id_fkey");
        });

        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("orders_pkey");

            entity.ToTable("orders");

            entity.HasIndex(e => e.Status, "idx_orders_status");

            entity.HasIndex(e => e.UserId, "idx_orders_user_id");

            entity.HasIndex(e => e.OrderNumber, "orders_order_number_key").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CancelReason).HasColumnName("cancel_reason");
            entity.Property(e => e.CancelledAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("cancelled_at");
            entity.Property(e => e.CouponCode)
                .HasMaxLength(100)
                .HasColumnName("coupon_code");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("now()")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.Currency)
                .HasMaxLength(10)
                .HasDefaultValueSql("'IQD'::character varying")
                .HasColumnName("currency");
            entity.Property(e => e.CustomerNotes).HasColumnName("customer_notes");
            entity.Property(e => e.DeliveredAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("delivered_at");
            entity.Property(e => e.DiscountAmount)
                .HasPrecision(15, 2)
                .HasColumnName("discount_amount");
            entity.Property(e => e.DiscountId).HasColumnName("discount_id");
            entity.Property(e => e.FulfillmentStatus)
                .HasMaxLength(50)
                .HasDefaultValueSql("'unfulfilled'::character varying")
                .HasColumnName("fulfillment_status");
            entity.Property(e => e.IpAddress)
                .HasMaxLength(50)
                .HasColumnName("ip_address");
            entity.Property(e => e.Notes).HasColumnName("notes");
            entity.Property(e => e.OrderNumber)
                .HasMaxLength(50)
                .HasColumnName("order_number");
            entity.Property(e => e.PaymentMethod)
                .HasMaxLength(100)
                .HasColumnName("payment_method");
            entity.Property(e => e.PaymentStatus)
                .HasMaxLength(50)
                .HasDefaultValueSql("'pending'::character varying")
                .HasColumnName("payment_status");
            entity.Property(e => e.ShippedAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("shipped_at");
            entity.Property(e => e.ShippingAddress1).HasColumnName("shipping_address1");
            entity.Property(e => e.ShippingAddress2).HasColumnName("shipping_address2");
            entity.Property(e => e.ShippingAmount)
                .HasPrecision(15, 2)
                .HasColumnName("shipping_amount");
            entity.Property(e => e.ShippingCity)
                .HasMaxLength(100)
                .HasColumnName("shipping_city");
            entity.Property(e => e.ShippingCountry)
                .HasMaxLength(100)
                .HasDefaultValueSql("'Iraq'::character varying")
                .HasColumnName("shipping_country");
            entity.Property(e => e.ShippingFirstName)
                .HasMaxLength(100)
                .HasColumnName("shipping_first_name");
            entity.Property(e => e.ShippingGovernorate)
                .HasMaxLength(100)
                .HasColumnName("shipping_governorate");
            entity.Property(e => e.ShippingLastName)
                .HasMaxLength(100)
                .HasColumnName("shipping_last_name");
            entity.Property(e => e.ShippingPhone)
                .HasMaxLength(20)
                .HasColumnName("shipping_phone");
            entity.Property(e => e.ShippingZoneId).HasColumnName("shipping_zone_id");
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .HasDefaultValueSql("'pending'::character varying")
                .HasColumnName("status");
            entity.Property(e => e.Subtotal)
                .HasPrecision(15, 2)
                .HasColumnName("subtotal");
            entity.Property(e => e.TaxAmount)
                .HasPrecision(15, 2)
                .HasColumnName("tax_amount");
            entity.Property(e => e.Total)
                .HasPrecision(15, 2)
                .HasColumnName("total");
            entity.Property(e => e.TrackingNumber)
                .HasMaxLength(255)
                .HasColumnName("tracking_number");
            entity.Property(e => e.UpdatedAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("updated_at");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.Discount).WithMany(p => p.Orders)
                .HasForeignKey(d => d.DiscountId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("orders_discount_id_fkey");

            entity.HasOne(d => d.ShippingZone).WithMany(p => p.Orders)
                .HasForeignKey(d => d.ShippingZoneId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("orders_shipping_zone_id_fkey");

            entity.HasOne(d => d.User).WithMany(p => p.Orders)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("orders_user_id_fkey");
        });

        modelBuilder.Entity<OrderDelivery>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("order_deliveries_pkey");

            entity.ToTable("order_deliveries");

            entity.HasIndex(e => e.OrderId, "idx_order_deliveries_order_id");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.AssignedAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("assigned_at");
            entity.Property(e => e.AssignedBy).HasColumnName("assigned_by");
            entity.Property(e => e.AttemptedAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("attempted_at");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("now()")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.DeliveredAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("delivered_at");
            entity.Property(e => e.DeliveryAgentId).HasColumnName("delivery_agent_id");
            entity.Property(e => e.FailureReason).HasColumnName("failure_reason");
            entity.Property(e => e.Notes).HasColumnName("notes");
            entity.Property(e => e.OrderId).HasColumnName("order_id");
            entity.Property(e => e.PickedUpAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("picked_up_at");
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .HasDefaultValueSql("'assigned'::character varying")
                .HasColumnName("status");

            entity.HasOne(d => d.AssignedByNavigation).WithMany(p => p.OrderDeliveries)
                .HasForeignKey(d => d.AssignedBy)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("order_deliveries_assigned_by_fkey");

            entity.HasOne(d => d.DeliveryAgent).WithMany(p => p.OrderDeliveries)
                .HasForeignKey(d => d.DeliveryAgentId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("order_deliveries_delivery_agent_id_fkey");

            entity.HasOne(d => d.Order).WithMany(p => p.OrderDeliveries)
                .HasForeignKey(d => d.OrderId)
                .HasConstraintName("order_deliveries_order_id_fkey");
        });

        modelBuilder.Entity<OrderItem>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("order_items_pkey");

            entity.ToTable("order_items");

            entity.HasIndex(e => e.OrderId, "idx_order_items_order_id");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CompareAtPrice)
                .HasPrecision(15, 2)
                .HasColumnName("compare_at_price");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("now()")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.DiscountAmount)
                .HasPrecision(15, 2)
                .HasColumnName("discount_amount");
            entity.Property(e => e.OrderId).HasColumnName("order_id");
            entity.Property(e => e.ProductId).HasColumnName("product_id");
            entity.Property(e => e.ProductName)
                .HasMaxLength(500)
                .HasColumnName("product_name");
            entity.Property(e => e.Quantity).HasColumnName("quantity");
            entity.Property(e => e.RequiresShipping)
                .HasDefaultValue(true)
                .HasColumnName("requires_shipping");
            entity.Property(e => e.Sku)
                .HasMaxLength(100)
                .HasColumnName("sku");
            entity.Property(e => e.TotalPrice)
                .HasPrecision(15, 2)
                .HasColumnName("total_price");
            entity.Property(e => e.UnitPrice)
                .HasPrecision(15, 2)
                .HasColumnName("unit_price");
            entity.Property(e => e.VariantId).HasColumnName("variant_id");
            entity.Property(e => e.WarrantyMonths)
                .HasDefaultValue(12)
                .HasColumnName("warranty_months");

            entity.HasOne(d => d.Order).WithMany(p => p.OrderItems)
                .HasForeignKey(d => d.OrderId)
                .HasConstraintName("order_items_order_id_fkey");

            entity.HasOne(d => d.Product).WithMany(p => p.OrderItems)
                .HasForeignKey(d => d.ProductId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("order_items_product_id_fkey");

            entity.HasOne(d => d.Variant).WithMany(p => p.OrderItems)
                .HasForeignKey(d => d.VariantId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("order_items_variant_id_fkey");
        });

        modelBuilder.Entity<OrderStatusHistory>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("order_status_history_pkey");

            entity.ToTable("order_status_history");

            entity.HasIndex(e => e.OrderId, "idx_order_status_history_order_id");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.AdminUserId).HasColumnName("admin_user_id");
            entity.Property(e => e.ChangedByType)
                .HasMaxLength(20)
                .HasComment("admin, system, customer")
                .HasColumnName("changed_by_type");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("now()")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.FromStatus)
                .HasMaxLength(50)
                .HasColumnName("from_status");
            entity.Property(e => e.Note).HasColumnName("note");
            entity.Property(e => e.OrderId).HasColumnName("order_id");
            entity.Property(e => e.ToStatus)
                .HasMaxLength(50)
                .HasColumnName("to_status");

            entity.HasOne(d => d.AdminUser).WithMany(p => p.OrderStatusHistories)
                .HasForeignKey(d => d.AdminUserId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("order_status_history_admin_user_id_fkey");

            entity.HasOne(d => d.Order).WithMany(p => p.OrderStatusHistories)
                .HasForeignKey(d => d.OrderId)
                .HasConstraintName("order_status_history_order_id_fkey");
        });

        modelBuilder.Entity<Page>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("pages_pkey");

            entity.ToTable("pages");

            entity.HasIndex(e => e.Slug, "pages_slug_key").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.ContentAr).HasColumnName("content_ar");
            entity.Property(e => e.ContentEn).HasColumnName("content_en");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("now()")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.IsPublished)
                .HasDefaultValue(true)
                .HasColumnName("is_published");
            entity.Property(e => e.MetaDescription).HasColumnName("meta_description");
            entity.Property(e => e.MetaTitle)
                .HasMaxLength(255)
                .HasColumnName("meta_title");
            entity.Property(e => e.Slug)
                .HasMaxLength(255)
                .HasColumnName("slug");
            entity.Property(e => e.TitleAr)
                .HasMaxLength(255)
                .HasColumnName("title_ar");
            entity.Property(e => e.TitleEn)
                .HasMaxLength(255)
                .HasColumnName("title_en");
            entity.Property(e => e.UpdatedAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("updated_at");
        });

        modelBuilder.Entity<PaymentMethod>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("payment_methods_pkey");

            entity.ToTable("payment_methods");

            entity.HasIndex(e => e.Name, "payment_methods_name_key").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("now()")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.Gateway)
                .HasMaxLength(100)
                .HasColumnName("gateway");
            entity.Property(e => e.GatewayConfig).HasColumnName("gateway_config");
            entity.Property(e => e.IconUrl).HasColumnName("icon_url");
            entity.Property(e => e.InstructionsAr).HasColumnName("instructions_ar");
            entity.Property(e => e.InstructionsEn).HasColumnName("instructions_en");
            entity.Property(e => e.IsActive)
                .HasDefaultValue(true)
                .HasColumnName("is_active");
            entity.Property(e => e.LabelAr)
                .HasMaxLength(100)
                .HasColumnName("label_ar");
            entity.Property(e => e.LabelEn)
                .HasMaxLength(100)
                .HasColumnName("label_en");
            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .HasColumnName("name");
            entity.Property(e => e.SortOrder).HasColumnName("sort_order");
            entity.Property(e => e.Type)
                .HasMaxLength(50)
                .HasComment("offline, online")
                .HasColumnName("type");
            entity.Property(e => e.UpdatedAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("updated_at");
        });

        modelBuilder.Entity<PaymentTransaction>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("payment_transactions_pkey");

            entity.ToTable("payment_transactions");

            entity.HasIndex(e => e.OrderId, "idx_payment_transactions_order_id");

            entity.HasIndex(e => e.TransactionNumber, "payment_transactions_transaction_number_key").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Amount)
                .HasPrecision(15, 2)
                .HasColumnName("amount");
            entity.Property(e => e.CollectedAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("collected_at");
            entity.Property(e => e.CollectedBy).HasColumnName("collected_by");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("now()")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.Currency)
                .HasMaxLength(10)
                .HasDefaultValueSql("'IQD'::character varying")
                .HasColumnName("currency");
            entity.Property(e => e.FailureReason).HasColumnName("failure_reason");
            entity.Property(e => e.GatewayName)
                .HasMaxLength(100)
                .HasColumnName("gateway_name");
            entity.Property(e => e.GatewayResponse).HasColumnName("gateway_response");
            entity.Property(e => e.GatewayStatus)
                .HasMaxLength(100)
                .HasColumnName("gateway_status");
            entity.Property(e => e.GatewayTransactionId)
                .HasMaxLength(255)
                .HasColumnName("gateway_transaction_id");
            entity.Property(e => e.Notes).HasColumnName("notes");
            entity.Property(e => e.OrderId).HasColumnName("order_id");
            entity.Property(e => e.PaidAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("paid_at");
            entity.Property(e => e.PaymentMethodId).HasColumnName("payment_method_id");
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .HasDefaultValueSql("'pending'::character varying")
                .HasColumnName("status");
            entity.Property(e => e.TransactionNumber)
                .HasMaxLength(100)
                .HasColumnName("transaction_number");
            entity.Property(e => e.Type)
                .HasMaxLength(50)
                .HasDefaultValueSql("'charge'::character varying")
                .HasComment("charge, refund")
                .HasColumnName("type");
            entity.Property(e => e.UpdatedAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("updated_at");

            entity.HasOne(d => d.CollectedByNavigation).WithMany(p => p.PaymentTransactions)
                .HasForeignKey(d => d.CollectedBy)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("payment_transactions_collected_by_fkey");

            entity.HasOne(d => d.Order).WithMany(p => p.PaymentTransactions)
                .HasForeignKey(d => d.OrderId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("payment_transactions_order_id_fkey");

            entity.HasOne(d => d.PaymentMethod).WithMany(p => p.PaymentTransactions)
                .HasForeignKey(d => d.PaymentMethodId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("payment_transactions_payment_method_id_fkey");
        });

        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("products_pkey");

            entity.ToTable("products");

            entity.HasIndex(e => e.BrandId, "idx_products_brand_id");

            entity.HasIndex(e => e.CategoryId, "idx_products_category_id");

            entity.HasIndex(e => e.Status, "idx_products_status");

            entity.HasIndex(e => e.Sku, "products_sku_key").IsUnique();

            entity.HasIndex(e => e.Slug, "products_slug_key").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Barcode)
                .HasMaxLength(100)
                .HasColumnName("barcode");
            entity.Property(e => e.BrandId).HasColumnName("brand_id");
            entity.Property(e => e.CategoryId).HasColumnName("category_id");
            entity.Property(e => e.CompareAtPrice)
                .HasPrecision(15, 2)
                .HasColumnName("compare_at_price");
            entity.Property(e => e.CostPrice)
                .HasPrecision(15, 2)
                .HasColumnName("cost_price");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("now()")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.Currency)
                .HasMaxLength(10)
                .HasDefaultValueSql("'IQD'::character varying")
                .HasColumnName("currency");
            entity.Property(e => e.DescriptionAr).HasColumnName("description_ar");
            entity.Property(e => e.DescriptionEn).HasColumnName("description_en");
            entity.Property(e => e.IsFeatured).HasColumnName("is_featured");
            entity.Property(e => e.LowStockThreshold)
                .HasDefaultValue(5)
                .HasColumnName("low_stock_threshold");
            entity.Property(e => e.MetaDescription).HasColumnName("meta_description");
            entity.Property(e => e.MetaTitle)
                .HasMaxLength(255)
                .HasColumnName("meta_title");
            entity.Property(e => e.MinOrderQuantity)
                .HasDefaultValue(1)
                .HasColumnName("min_order_quantity");
            entity.Property(e => e.NameAr)
                .HasMaxLength(500)
                .HasColumnName("name_ar");
            entity.Property(e => e.NameEn)
                .HasMaxLength(500)
                .HasColumnName("name_en");
            entity.Property(e => e.Price)
                .HasPrecision(15, 2)
                .HasColumnName("price");
            entity.Property(e => e.RequiresShipping)
                .HasDefaultValue(true)
                .HasColumnName("requires_shipping");
            entity.Property(e => e.ShortDescriptionAr).HasColumnName("short_description_ar");
            entity.Property(e => e.ShortDescriptionEn).HasColumnName("short_description_en");
            entity.Property(e => e.Sku)
                .HasMaxLength(100)
                .HasColumnName("sku");
            entity.Property(e => e.Slug)
                .HasMaxLength(500)
                .HasColumnName("slug");
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .HasDefaultValueSql("'active'::character varying")
                .HasColumnName("status");
            entity.Property(e => e.StockQuantity).HasColumnName("stock_quantity");
            entity.Property(e => e.Tags).HasColumnName("tags");
            entity.Property(e => e.TotalSold).HasColumnName("total_sold");
            entity.Property(e => e.UpdatedAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("updated_at");
            entity.Property(e => e.ViewsCount).HasColumnName("views_count");
            entity.Property(e => e.WarrantyDescription).HasColumnName("warranty_description");
            entity.Property(e => e.WarrantyMonths)
                .HasDefaultValue(12)
                .HasColumnName("warranty_months");
            entity.Property(e => e.Weight)
                .HasPrecision(8, 2)
                .HasColumnName("weight");

            entity.HasOne(d => d.Brand).WithMany(p => p.Products)
                .HasForeignKey(d => d.BrandId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("products_brand_id_fkey");

            entity.HasOne(d => d.Category).WithMany(p => p.Products)
                .HasForeignKey(d => d.CategoryId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("products_category_id_fkey");
        });

        modelBuilder.Entity<ProductAttributeValue>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("product_attribute_values_pkey");

            entity.ToTable("product_attribute_values");

            entity.HasIndex(e => e.AttributeId, "idx_product_attribute_values_attribute_id");

            entity.HasIndex(e => e.ProductId, "idx_product_attribute_values_product_id");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.AttributeId).HasColumnName("attribute_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("now()")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.ProductId).HasColumnName("product_id");
            entity.Property(e => e.ValueBoolean).HasColumnName("value_boolean");
            entity.Property(e => e.ValueNumber)
                .HasPrecision(10, 2)
                .HasColumnName("value_number");
            entity.Property(e => e.ValueText)
                .HasMaxLength(500)
                .HasColumnName("value_text");

            entity.HasOne(d => d.Attribute).WithMany(p => p.ProductAttributeValues)
                .HasForeignKey(d => d.AttributeId)
                .HasConstraintName("product_attribute_values_attribute_id_fkey");

            entity.HasOne(d => d.Product).WithMany(p => p.ProductAttributeValues)
                .HasForeignKey(d => d.ProductId)
                .HasConstraintName("product_attribute_values_product_id_fkey");
        });

        modelBuilder.Entity<ProductCompare>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("product_compares_pkey");

            entity.ToTable("product_compares");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("now()")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.ProductId).HasColumnName("product_id");
            entity.Property(e => e.SessionId)
                .HasMaxLength(255)
                .HasColumnName("session_id");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.Product).WithMany(p => p.ProductCompares)
                .HasForeignKey(d => d.ProductId)
                .HasConstraintName("product_compares_product_id_fkey");

            entity.HasOne(d => d.User).WithMany(p => p.ProductCompares)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("product_compares_user_id_fkey");
        });

        modelBuilder.Entity<ProductImage>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("product_images_pkey");

            entity.ToTable("product_images");

            entity.HasIndex(e => e.ProductId, "idx_product_images_product_id");

            entity.HasIndex(e => e.VariantId, "idx_product_images_variant_id");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.AltText)
                .HasMaxLength(255)
                .HasColumnName("alt_text");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("now()")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.ImageUrl).HasColumnName("image_url");
            entity.Property(e => e.IsPrimary).HasColumnName("is_primary");
            entity.Property(e => e.ProductId).HasColumnName("product_id");
            entity.Property(e => e.SortOrder).HasColumnName("sort_order");
            entity.Property(e => e.VariantId).HasColumnName("variant_id");

            entity.HasOne(d => d.Product).WithMany(p => p.ProductImages)
                .HasForeignKey(d => d.ProductId)
                .HasConstraintName("product_images_product_id_fkey");

            entity.HasOne(d => d.Variant).WithMany(p => p.ProductImages)
                .HasForeignKey(d => d.VariantId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("product_images_variant_id_fkey");
        });

        modelBuilder.Entity<ProductTag>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("product_tags_pkey");

            entity.ToTable("product_tags");

            entity.HasIndex(e => e.ProductId, "idx_product_tags_product_id");

            entity.HasIndex(e => e.TagId, "idx_product_tags_tag_id");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("now()")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.ProductId).HasColumnName("product_id");
            entity.Property(e => e.TagId).HasColumnName("tag_id");

            entity.HasOne(d => d.Product).WithMany(p => p.ProductTags)
                .HasForeignKey(d => d.ProductId)
                .HasConstraintName("product_tags_product_id_fkey");

            entity.HasOne(d => d.Tag).WithMany(p => p.ProductTags)
                .HasForeignKey(d => d.TagId)
                .HasConstraintName("product_tags_tag_id_fkey");
        });

        modelBuilder.Entity<ProductVariant>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("product_variants_pkey");

            entity.ToTable("product_variants");

            entity.HasIndex(e => e.ProductId, "idx_product_variants_product_id");

            entity.HasIndex(e => e.Sku, "product_variants_sku_key").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Barcode)
                .HasMaxLength(100)
                .HasColumnName("barcode");
            entity.Property(e => e.CompareAtPrice)
                .HasPrecision(15, 2)
                .HasColumnName("compare_at_price");
            entity.Property(e => e.CostPrice)
                .HasPrecision(15, 2)
                .HasColumnName("cost_price");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("now()")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.IsActive)
                .HasDefaultValue(true)
                .HasColumnName("is_active");
            entity.Property(e => e.Price)
                .HasPrecision(15, 2)
                .HasColumnName("price");
            entity.Property(e => e.ProductId).HasColumnName("product_id");
            entity.Property(e => e.Sku)
                .HasMaxLength(100)
                .HasColumnName("sku");
            entity.Property(e => e.SortOrder).HasColumnName("sort_order");
            entity.Property(e => e.StockQuantity).HasColumnName("stock_quantity");
            entity.Property(e => e.Title)
                .HasMaxLength(255)
                .HasColumnName("title");
            entity.Property(e => e.UpdatedAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("updated_at");
            entity.Property(e => e.Weight)
                .HasPrecision(8, 2)
                .HasColumnName("weight");

            entity.HasOne(d => d.Product).WithMany(p => p.ProductVariants)
                .HasForeignKey(d => d.ProductId)
                .HasConstraintName("product_variants_product_id_fkey");
        });

        modelBuilder.Entity<PurchaseOrder>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("purchase_orders_pkey");

            entity.ToTable("purchase_orders");

            entity.HasIndex(e => e.PoNumber, "purchase_orders_po_number_key").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.AdminUserId).HasColumnName("admin_user_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("now()")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.Currency)
                .HasMaxLength(10)
                .HasDefaultValueSql("'USD'::character varying")
                .HasColumnName("currency");
            entity.Property(e => e.ExchangeRate)
                .HasPrecision(10, 4)
                .HasDefaultValue(1m)
                .HasColumnName("exchange_rate");
            entity.Property(e => e.ExpectedDate).HasColumnName("expected_date");
            entity.Property(e => e.Notes).HasColumnName("notes");
            entity.Property(e => e.OrderDate).HasColumnName("order_date");
            entity.Property(e => e.PaidAmount)
                .HasPrecision(15, 2)
                .HasColumnName("paid_amount");
            entity.Property(e => e.PaymentStatus)
                .HasMaxLength(50)
                .HasDefaultValueSql("'unpaid'::character varying")
                .HasColumnName("payment_status");
            entity.Property(e => e.PoNumber)
                .HasMaxLength(50)
                .HasColumnName("po_number");
            entity.Property(e => e.ReceivedDate).HasColumnName("received_date");
            entity.Property(e => e.ShippingCost)
                .HasPrecision(15, 2)
                .HasColumnName("shipping_cost");
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .HasDefaultValueSql("'draft'::character varying")
                .HasColumnName("status");
            entity.Property(e => e.Subtotal)
                .HasPrecision(15, 2)
                .HasColumnName("subtotal");
            entity.Property(e => e.SupplierId).HasColumnName("supplier_id");
            entity.Property(e => e.TaxAmount)
                .HasPrecision(15, 2)
                .HasColumnName("tax_amount");
            entity.Property(e => e.Total)
                .HasPrecision(15, 2)
                .HasColumnName("total");
            entity.Property(e => e.UpdatedAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("updated_at");

            entity.HasOne(d => d.AdminUser).WithMany(p => p.PurchaseOrders)
                .HasForeignKey(d => d.AdminUserId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("purchase_orders_admin_user_id_fkey");

            entity.HasOne(d => d.Supplier).WithMany(p => p.PurchaseOrders)
                .HasForeignKey(d => d.SupplierId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("purchase_orders_supplier_id_fkey");
        });

        modelBuilder.Entity<PurchaseOrderItem>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("purchase_order_items_pkey");

            entity.ToTable("purchase_order_items");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("now()")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.Notes).HasColumnName("notes");
            entity.Property(e => e.ProductId).HasColumnName("product_id");
            entity.Property(e => e.ProductName)
                .HasMaxLength(500)
                .HasColumnName("product_name");
            entity.Property(e => e.PurchaseOrderId).HasColumnName("purchase_order_id");
            entity.Property(e => e.QuantityOrdered).HasColumnName("quantity_ordered");
            entity.Property(e => e.QuantityReceived).HasColumnName("quantity_received");
            entity.Property(e => e.Sku)
                .HasMaxLength(100)
                .HasColumnName("sku");
            entity.Property(e => e.TotalCost)
                .HasPrecision(15, 2)
                .HasColumnName("total_cost");
            entity.Property(e => e.UnitCost)
                .HasPrecision(15, 2)
                .HasColumnName("unit_cost");
            entity.Property(e => e.VariantId).HasColumnName("variant_id");

            entity.HasOne(d => d.Product).WithMany(p => p.PurchaseOrderItems)
                .HasForeignKey(d => d.ProductId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("purchase_order_items_product_id_fkey");

            entity.HasOne(d => d.PurchaseOrder).WithMany(p => p.PurchaseOrderItems)
                .HasForeignKey(d => d.PurchaseOrderId)
                .HasConstraintName("purchase_order_items_purchase_order_id_fkey");

            entity.HasOne(d => d.Variant).WithMany(p => p.PurchaseOrderItems)
                .HasForeignKey(d => d.VariantId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("purchase_order_items_variant_id_fkey");
        });

        modelBuilder.Entity<Return>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("returns_pkey");

            entity.ToTable("returns");

            entity.HasIndex(e => e.OrderId, "idx_returns_order_id");

            entity.HasIndex(e => e.ReturnNumber, "returns_return_number_key").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.AdminNotes).HasColumnName("admin_notes");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("now()")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.OrderId).HasColumnName("order_id");
            entity.Property(e => e.Reason)
                .HasMaxLength(255)
                .HasColumnName("reason");
            entity.Property(e => e.ReasonDetails).HasColumnName("reason_details");
            entity.Property(e => e.ReceivedAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("received_at");
            entity.Property(e => e.RefundAmount)
                .HasPrecision(15, 2)
                .HasColumnName("refund_amount");
            entity.Property(e => e.RefundMethod)
                .HasMaxLength(50)
                .HasComment("original_payment, store_credit, bank_transfer")
                .HasColumnName("refund_method");
            entity.Property(e => e.RefundStatus)
                .HasMaxLength(50)
                .HasDefaultValueSql("'pending'::character varying")
                .HasColumnName("refund_status");
            entity.Property(e => e.ReturnNumber)
                .HasMaxLength(50)
                .HasColumnName("return_number");
            entity.Property(e => e.ReturnType)
                .HasMaxLength(50)
                .HasComment("return, exchange, warranty_repair")
                .HasColumnName("return_type");
            entity.Property(e => e.ReviewedAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("reviewed_at");
            entity.Property(e => e.ReviewedBy).HasColumnName("reviewed_by");
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .HasDefaultValueSql("'requested'::character varying")
                .HasColumnName("status");
            entity.Property(e => e.UpdatedAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("updated_at");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.Order).WithMany(p => p.Returns)
                .HasForeignKey(d => d.OrderId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("returns_order_id_fkey");

            entity.HasOne(d => d.ReviewedByNavigation).WithMany(p => p.Returns)
                .HasForeignKey(d => d.ReviewedBy)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("returns_reviewed_by_fkey");

            entity.HasOne(d => d.User).WithMany(p => p.Returns)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("returns_user_id_fkey");
        });

        modelBuilder.Entity<ReturnItem>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("return_items_pkey");

            entity.ToTable("return_items");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Condition)
                .HasMaxLength(50)
                .HasComment("new, good, damaged, defective")
                .HasColumnName("condition");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("now()")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.Notes).HasColumnName("notes");
            entity.Property(e => e.OrderItemId).HasColumnName("order_item_id");
            entity.Property(e => e.ProductId).HasColumnName("product_id");
            entity.Property(e => e.Quantity).HasColumnName("quantity");
            entity.Property(e => e.RefundAmount)
                .HasPrecision(15, 2)
                .HasColumnName("refund_amount");
            entity.Property(e => e.ReturnId).HasColumnName("return_id");
            entity.Property(e => e.ReturnToStock).HasColumnName("return_to_stock");
            entity.Property(e => e.UnitPrice)
                .HasPrecision(15, 2)
                .HasColumnName("unit_price");
            entity.Property(e => e.VariantId).HasColumnName("variant_id");

            entity.HasOne(d => d.OrderItem).WithMany(p => p.ReturnItems)
                .HasForeignKey(d => d.OrderItemId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("return_items_order_item_id_fkey");

            entity.HasOne(d => d.Product).WithMany(p => p.ReturnItems)
                .HasForeignKey(d => d.ProductId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("return_items_product_id_fkey");

            entity.HasOne(d => d.Return).WithMany(p => p.ReturnItems)
                .HasForeignKey(d => d.ReturnId)
                .HasConstraintName("return_items_return_id_fkey");

            entity.HasOne(d => d.Variant).WithMany(p => p.ReturnItems)
                .HasForeignKey(d => d.VariantId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("return_items_variant_id_fkey");
        });

        modelBuilder.Entity<Review>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("reviews_pkey");

            entity.ToTable("reviews");

            entity.HasIndex(e => e.ProductId, "idx_reviews_product_id");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Body).HasColumnName("body");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("now()")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.HelpfulCount).HasColumnName("helpful_count");
            entity.Property(e => e.IsVerifiedPurchase).HasColumnName("is_verified_purchase");
            entity.Property(e => e.OrderItemId).HasColumnName("order_item_id");
            entity.Property(e => e.ProductId).HasColumnName("product_id");
            entity.Property(e => e.Rating).HasColumnName("rating");
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .HasDefaultValueSql("'pending'::character varying")
                .HasColumnName("status");
            entity.Property(e => e.Title)
                .HasMaxLength(255)
                .HasColumnName("title");
            entity.Property(e => e.UpdatedAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("updated_at");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.OrderItem).WithMany(p => p.Reviews)
                .HasForeignKey(d => d.OrderItemId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("reviews_order_item_id_fkey");

            entity.HasOne(d => d.Product).WithMany(p => p.Reviews)
                .HasForeignKey(d => d.ProductId)
                .HasConstraintName("reviews_product_id_fkey");

            entity.HasOne(d => d.User).WithMany(p => p.Reviews)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("reviews_user_id_fkey");
        });

        modelBuilder.Entity<Setting>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("settings_pkey");

            entity.ToTable("settings");

            entity.HasIndex(e => e.Key, "settings_key_key").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("now()")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.Group)
                .HasMaxLength(100)
                .HasColumnName("group");
            entity.Property(e => e.Key)
                .HasMaxLength(255)
                .HasColumnName("key");
            entity.Property(e => e.Type)
                .HasMaxLength(50)
                .HasDefaultValueSql("'string'::character varying")
                .HasColumnName("type");
            entity.Property(e => e.UpdatedAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("updated_at");
            entity.Property(e => e.Value).HasColumnName("value");
        });

        modelBuilder.Entity<ShippingRate>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("shipping_rates_pkey");

            entity.ToTable("shipping_rates");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("now()")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.EstimatedDaysMax).HasColumnName("estimated_days_max");
            entity.Property(e => e.EstimatedDaysMin).HasColumnName("estimated_days_min");
            entity.Property(e => e.FreeShippingThreshold)
                .HasPrecision(15, 2)
                .HasColumnName("free_shipping_threshold");
            entity.Property(e => e.IsActive)
                .HasDefaultValue(true)
                .HasColumnName("is_active");
            entity.Property(e => e.MaxOrderValue)
                .HasPrecision(15, 2)
                .HasColumnName("max_order_value");
            entity.Property(e => e.MinOrderValue)
                .HasPrecision(15, 2)
                .HasColumnName("min_order_value");
            entity.Property(e => e.NameAr)
                .HasMaxLength(100)
                .HasColumnName("name_ar");
            entity.Property(e => e.NameEn)
                .HasMaxLength(100)
                .HasColumnName("name_en");
            entity.Property(e => e.Price)
                .HasPrecision(15, 2)
                .HasColumnName("price");
            entity.Property(e => e.RateType)
                .HasMaxLength(20)
                .HasDefaultValueSql("'flat'::character varying")
                .HasColumnName("rate_type");
            entity.Property(e => e.ShippingZoneId).HasColumnName("shipping_zone_id");

            entity.HasOne(d => d.ShippingZone).WithMany(p => p.ShippingRates)
                .HasForeignKey(d => d.ShippingZoneId)
                .HasConstraintName("shipping_rates_shipping_zone_id_fkey");
        });

        modelBuilder.Entity<ShippingZone>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("shipping_zones_pkey");

            entity.ToTable("shipping_zones");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("now()")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.Governorates).HasColumnName("governorates");
            entity.Property(e => e.IsActive)
                .HasDefaultValue(true)
                .HasColumnName("is_active");
            entity.Property(e => e.NameAr)
                .HasMaxLength(100)
                .HasColumnName("name_ar");
            entity.Property(e => e.NameEn)
                .HasMaxLength(100)
                .HasColumnName("name_en");
        });

        modelBuilder.Entity<Supplier>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("suppliers_pkey");

            entity.ToTable("suppliers");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Address).HasColumnName("address");
            entity.Property(e => e.City)
                .HasMaxLength(100)
                .HasColumnName("city");
            entity.Property(e => e.CompanyName)
                .HasMaxLength(255)
                .HasColumnName("company_name");
            entity.Property(e => e.Country)
                .HasMaxLength(100)
                .HasColumnName("country");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("now()")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.Currency)
                .HasMaxLength(10)
                .HasDefaultValueSql("'USD'::character varying")
                .HasColumnName("currency");
            entity.Property(e => e.Email)
                .HasMaxLength(255)
                .HasColumnName("email");
            entity.Property(e => e.IsActive)
                .HasDefaultValue(true)
                .HasColumnName("is_active");
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .HasColumnName("name");
            entity.Property(e => e.Notes).HasColumnName("notes");
            entity.Property(e => e.PaymentTerms)
                .HasMaxLength(255)
                .HasColumnName("payment_terms");
            entity.Property(e => e.Phone)
                .HasMaxLength(20)
                .HasColumnName("phone");
            entity.Property(e => e.UpdatedAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("updated_at");
            entity.Property(e => e.Whatsapp)
                .HasMaxLength(20)
                .HasColumnName("whatsapp");
        });

        modelBuilder.Entity<Tag>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("tags_pkey");

            entity.ToTable("tags");

            entity.HasIndex(e => e.Name, "tags_name_key").IsUnique();

            entity.HasIndex(e => e.Slug, "tags_slug_key").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("now()")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .HasColumnName("name");
            entity.Property(e => e.Slug)
                .HasMaxLength(100)
                .HasColumnName("slug");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("users_pkey");

            entity.ToTable("users");

            entity.HasIndex(e => e.Email, "users_email_key").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.AcceptsMarketing).HasColumnName("accepts_marketing");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("now()")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.Email)
                .HasMaxLength(255)
                .HasColumnName("email");
            entity.Property(e => e.EmailVerifiedAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("email_verified_at");
            entity.Property(e => e.FirstName)
                .HasMaxLength(100)
                .HasColumnName("first_name");
            entity.Property(e => e.IsActive)
                .HasDefaultValue(true)
                .HasColumnName("is_active");
            entity.Property(e => e.LastName)
                .HasMaxLength(100)
                .HasColumnName("last_name");
            entity.Property(e => e.Notes).HasColumnName("notes");
            entity.Property(e => e.PasswordHash)
                .HasMaxLength(255)
                .HasColumnName("password_hash");
            entity.Property(e => e.Phone)
                .HasMaxLength(20)
                .HasColumnName("phone");
            entity.Property(e => e.StoreCredit)
                .HasPrecision(15, 2)
                .HasComment("رصيد المتجر للاسترداد")
                .HasColumnName("store_credit");
            entity.Property(e => e.UpdatedAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("updated_at");
        });

        modelBuilder.Entity<WarrantyClaim>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("warranty_claims_pkey");

            entity.ToTable("warranty_claims");

            entity.HasIndex(e => e.ClaimNumber, "warranty_claims_claim_number_key").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.AdminNotes).HasColumnName("admin_notes");
            entity.Property(e => e.ClaimNumber)
                .HasMaxLength(50)
                .HasColumnName("claim_number");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("now()")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.HandledBy).HasColumnName("handled_by");
            entity.Property(e => e.IssueDescription).HasColumnName("issue_description");
            entity.Property(e => e.OrderItemId).HasColumnName("order_item_id");
            entity.Property(e => e.ProductId).HasColumnName("product_id");
            entity.Property(e => e.ReceivedAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("received_at");
            entity.Property(e => e.Resolution)
                .HasMaxLength(50)
                .HasComment("repair, replacement, refund")
                .HasColumnName("resolution");
            entity.Property(e => e.ResolvedAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("resolved_at");
            entity.Property(e => e.ReturnId).HasColumnName("return_id");
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .HasDefaultValueSql("'submitted'::character varying")
                .HasColumnName("status");
            entity.Property(e => e.UpdatedAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("updated_at");
            entity.Property(e => e.UserId).HasColumnName("user_id");
            entity.Property(e => e.WarrantyExpiresAt).HasColumnName("warranty_expires_at");

            entity.HasOne(d => d.HandledByNavigation).WithMany(p => p.WarrantyClaims)
                .HasForeignKey(d => d.HandledBy)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("warranty_claims_handled_by_fkey");

            entity.HasOne(d => d.OrderItem).WithMany(p => p.WarrantyClaims)
                .HasForeignKey(d => d.OrderItemId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("warranty_claims_order_item_id_fkey");

            entity.HasOne(d => d.Product).WithMany(p => p.WarrantyClaims)
                .HasForeignKey(d => d.ProductId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("warranty_claims_product_id_fkey");

            entity.HasOne(d => d.Return).WithMany(p => p.WarrantyClaims)
                .HasForeignKey(d => d.ReturnId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("warranty_claims_return_id_fkey");

            entity.HasOne(d => d.User).WithMany(p => p.WarrantyClaims)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("warranty_claims_user_id_fkey");
        });

        modelBuilder.Entity<Wishlist>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("wishlists_pkey");

            entity.ToTable("wishlists");

            entity.HasIndex(e => e.UserId, "idx_wishlists_user_id");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("now()")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.ProductId).HasColumnName("product_id");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.Product).WithMany(p => p.Wishlists)
                .HasForeignKey(d => d.ProductId)
                .HasConstraintName("wishlists_product_id_fkey");

            entity.HasOne(d => d.User).WithMany(p => p.Wishlists)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("wishlists_user_id_fkey");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
