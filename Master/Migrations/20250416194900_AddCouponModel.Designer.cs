﻿// <auto-generated />
using System;
using Master.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace Master.Migrations
{
    [DbContext(typeof(MyDbContext))]
    [Migration("20250416194900_AddCouponModel")]
    partial class AddCouponModel
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "9.0.4")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("Master.Models.Cart", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("id");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("Quantity")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasDefaultValue(1)
                        .HasColumnName("quantity");

                    b.Property<int?>("UserId")
                        .HasColumnType("int")
                        .HasColumnName("user_id");

                    b.HasKey("Id")
                        .HasName("cart_id_primary");

                    b.HasIndex("UserId");

                    b.ToTable("Cart", (string)null);
                });

            modelBuilder.Entity("Master.Models.CartItem", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("CartId")
                        .HasColumnType("int");

                    b.Property<DateTime?>("CreatedAt")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime")
                        .HasDefaultValueSql("(getdate())");

                    b.Property<int>("ProductId")
                        .HasColumnType("int");

                    b.Property<int>("Quantity")
                        .HasColumnType("int");

                    b.Property<decimal?>("TotalPrice")
                        .ValueGeneratedOnAddOrUpdate()
                        .HasColumnType("decimal(21, 2)")
                        .HasComputedColumnSql("([Quantity]*[UnitPrice])", true);

                    b.Property<decimal>("UnitPrice")
                        .HasColumnType("decimal(10, 2)");

                    b.HasKey("Id")
                        .HasName("PK__CartItem__3214EC07011A0A7B");

                    b.HasIndex("CartId");

                    b.HasIndex("ProductId");

                    b.ToTable("CartItem", (string)null);
                });

            modelBuilder.Entity("Master.Models.Category", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("id");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("CategoryName")
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)")
                        .HasColumnName("categoryName");

                    b.HasKey("Id");

                    b.ToTable("Category", (string)null);
                });

            modelBuilder.Entity("Master.Models.Company", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("id");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("CompanyName")
                        .IsRequired()
                        .HasMaxLength(255)
                        .IsUnicode(false)
                        .HasColumnType("varchar(255)")
                        .HasColumnName("company_name");

                    b.Property<int>("OwnerId")
                        .HasColumnType("int")
                        .HasColumnName("owner_Id");

                    b.HasKey("Id")
                        .HasName("company_id_primary");

                    b.HasIndex("OwnerId");

                    b.ToTable("Company", (string)null);
                });

            modelBuilder.Entity("Master.Models.Coupon", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Code")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<decimal>("DiscountPercentage")
                        .HasColumnType("decimal(18,2)");

                    b.Property<DateTime?>("ExpiryDate")
                        .HasColumnType("datetime2");

                    b.Property<bool>("IsActive")
                        .HasColumnType("bit");

                    b.HasKey("Id");

                    b.ToTable("Coupons");
                });

            modelBuilder.Entity("Master.Models.Delivery", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("id");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<DateTime>("DeliveryTime")
                        .HasColumnType("datetime2")
                        .HasColumnName("delivery_time");

                    b.Property<string>("Location")
                        .IsRequired()
                        .HasMaxLength(255)
                        .IsUnicode(false)
                        .HasColumnType("varchar(255)")
                        .HasColumnName("location");

                    b.Property<bool?>("PaidDelivery")
                        .HasColumnType("bit")
                        .HasColumnName("paid_delivery");

                    b.Property<int?>("PaymentId")
                        .HasColumnType("int")
                        .HasColumnName("payment_id");

                    b.Property<string>("Phone")
                        .IsRequired()
                        .HasMaxLength(20)
                        .IsUnicode(false)
                        .HasColumnType("varchar(20)")
                        .HasColumnName("phone");

                    b.Property<string>("ReceiverName")
                        .IsRequired()
                        .HasMaxLength(255)
                        .IsUnicode(false)
                        .HasColumnType("varchar(255)")
                        .HasColumnName("receiver_name");

                    b.Property<int?>("RequestId")
                        .HasColumnType("int")
                        .HasColumnName("request_id");

                    b.HasKey("Id")
                        .HasName("deliveries_id_primary");

                    b.HasIndex("PaymentId");

                    b.HasIndex("RequestId");

                    b.ToTable("Deliveries");
                });

            modelBuilder.Entity("Master.Models.Order", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("id");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Address1")
                        .HasMaxLength(255)
                        .HasColumnType("nchar(255)")
                        .IsFixedLength();

                    b.Property<string>("Address2")
                        .HasMaxLength(255)
                        .HasColumnType("nchar(255)")
                        .IsFixedLength();

                    b.Property<DateTime?>("CreatedAt")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime2")
                        .HasColumnName("created_at")
                        .HasDefaultValueSql("(getdate())");

                    b.Property<string>("DeliveryAddress")
                        .IsRequired()
                        .HasMaxLength(255)
                        .IsUnicode(false)
                        .HasColumnType("varchar(255)")
                        .HasColumnName("delivery_address");

                    b.Property<DateTime>("DeliveryTime")
                        .HasColumnType("datetime2")
                        .HasColumnName("delivery_time");

                    b.Property<string>("OrderStatus")
                        .ValueGeneratedOnAdd()
                        .HasMaxLength(50)
                        .IsUnicode(false)
                        .HasColumnType("varchar(50)")
                        .HasDefaultValue("Pending")
                        .HasColumnName("order_status");

                    b.Property<int?>("PaymentId")
                        .HasColumnType("int")
                        .HasColumnName("payment_id");

                    b.Property<int?>("Phone")
                        .HasColumnType("int")
                        .HasColumnName("phone");

                    b.Property<decimal>("TotalAmount")
                        .HasColumnType("decimal(10, 2)")
                        .HasColumnName("total_amount");

                    b.Property<int?>("UserId")
                        .HasColumnType("int")
                        .HasColumnName("user_id");

                    b.HasKey("Id")
                        .HasName("orders_id_primary");

                    b.HasIndex("PaymentId");

                    b.HasIndex("UserId");

                    b.ToTable("Orders");
                });

            modelBuilder.Entity("Master.Models.OrderItem", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("id");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int?>("OrderId")
                        .HasColumnType("int")
                        .HasColumnName("order_id");

                    b.Property<decimal>("Price")
                        .HasColumnType("decimal(10, 2)")
                        .HasColumnName("price");

                    b.Property<int?>("ProductId")
                        .HasColumnType("int")
                        .HasColumnName("product_id");

                    b.Property<int>("Quantity")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasDefaultValue(1)
                        .HasColumnName("quantity");

                    b.HasKey("Id")
                        .HasName("orderitems_id_primary");

                    b.HasIndex("OrderId");

                    b.HasIndex("ProductId");

                    b.ToTable("OrderItems");
                });

            modelBuilder.Entity("Master.Models.Payment", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("id");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<decimal>("Amount")
                        .HasColumnType("decimal(10, 2)")
                        .HasColumnName("amount");

                    b.Property<DateTime>("CreatedAt")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime2")
                        .HasColumnName("created_at")
                        .HasDefaultValueSql("(getdate())");

                    b.Property<string>("PaymentMethod")
                        .IsRequired()
                        .HasMaxLength(50)
                        .IsUnicode(false)
                        .HasColumnType("varchar(50)")
                        .HasColumnName("payment_method");

                    b.Property<string>("PaymentType")
                        .IsRequired()
                        .HasMaxLength(255)
                        .IsUnicode(false)
                        .HasColumnType("varchar(255)")
                        .HasColumnName("payment_type");

                    b.Property<string>("Status")
                        .IsRequired()
                        .ValueGeneratedOnAdd()
                        .HasMaxLength(50)
                        .IsUnicode(false)
                        .HasColumnType("varchar(50)")
                        .HasDefaultValue("Pending")
                        .HasColumnName("status");

                    b.Property<int?>("UserId")
                        .HasColumnType("int")
                        .HasColumnName("user_id");

                    b.HasKey("Id")
                        .HasName("payments_id_primary");

                    b.HasIndex("UserId");

                    b.ToTable("Payments");
                });

            modelBuilder.Entity("Master.Models.Product", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("id");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int?>("CategoryId")
                        .HasColumnType("int")
                        .HasColumnName("Category_id");

                    b.Property<string>("CompanyName")
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)")
                        .HasColumnName("company_name");

                    b.Property<DateTime?>("CreatedAt")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime2")
                        .HasColumnName("created_at")
                        .HasDefaultValueSql("(getdate())");

                    b.Property<string>("Description")
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)")
                        .HasColumnName("description");

                    b.Property<string>("ImageUrl")
                        .HasMaxLength(255)
                        .IsUnicode(false)
                        .HasColumnType("varchar(255)")
                        .HasColumnName("image_url");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)")
                        .HasColumnName("name");

                    b.Property<decimal>("Price")
                        .HasColumnType("decimal(10, 2)")
                        .HasColumnName("price");

                    b.Property<int>("Stock")
                        .HasColumnType("int")
                        .HasColumnName("stock");

                    b.Property<bool?>("Type")
                        .HasColumnType("bit")
                        .HasColumnName("type");

                    b.HasKey("Id")
                        .HasName("products_id_primary");

                    b.HasIndex("CategoryId");

                    b.ToTable("Products");
                });

            modelBuilder.Entity("Master.Models.RecyclingRequest", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("id");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<DateTime?>("CreatedAt")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime2")
                        .HasColumnName("created_at")
                        .HasDefaultValueSql("(getdate())");

                    b.Property<string>("ImageUrl")
                        .HasMaxLength(255)
                        .IsUnicode(false)
                        .HasColumnType("varchar(255)")
                        .HasColumnName("image_url");

                    b.Property<string>("ItemCondition")
                        .HasMaxLength(50)
                        .IsUnicode(false)
                        .HasColumnType("varchar(50)");

                    b.Property<string>("MaterialName")
                        .IsRequired()
                        .HasMaxLength(255)
                        .IsUnicode(false)
                        .HasColumnType("varchar(255)")
                        .HasColumnName("material_name");

                    b.Property<string>("MaterialType")
                        .IsRequired()
                        .HasMaxLength(255)
                        .IsUnicode(false)
                        .HasColumnType("varchar(255)")
                        .HasColumnName("material_type");

                    b.Property<string>("Notes")
                        .HasMaxLength(255)
                        .IsUnicode(false)
                        .HasColumnType("varchar(255)")
                        .HasColumnName("notes");

                    b.Property<int>("Quantity")
                        .HasColumnType("int")
                        .HasColumnName("quantity");

                    b.Property<int?>("UserId")
                        .HasColumnType("int")
                        .HasColumnName("user_id");

                    b.HasKey("Id")
                        .HasName("recyclingrequests_id_primary");

                    b.HasIndex("UserId");

                    b.ToTable("RecyclingRequests");
                });

            modelBuilder.Entity("Master.Models.Subscription", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("id");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("DayOfWeek")
                        .HasMaxLength(20)
                        .IsUnicode(false)
                        .HasColumnType("varchar(20)")
                        .HasColumnName("day_of_week");

                    b.Property<string>("Location")
                        .IsRequired()
                        .HasMaxLength(255)
                        .IsUnicode(false)
                        .HasColumnType("varchar(255)")
                        .HasColumnName("location");

                    b.Property<string>("Notes")
                        .HasMaxLength(255)
                        .IsUnicode(false)
                        .HasColumnType("varchar(255)")
                        .HasColumnName("notes");

                    b.Property<int?>("PaymentId")
                        .HasColumnType("int")
                        .HasColumnName("payment_id");

                    b.Property<string>("SubscriptionType")
                        .IsRequired()
                        .HasMaxLength(50)
                        .IsUnicode(false)
                        .HasColumnType("varchar(50)")
                        .HasColumnName("subscription_type");

                    b.Property<TimeOnly>("Time")
                        .HasColumnType("time")
                        .HasColumnName("time");

                    b.Property<int?>("UserId")
                        .HasColumnType("int")
                        .HasColumnName("user_id");

                    b.HasKey("Id")
                        .HasName("subscriptions_id_primary");

                    b.HasIndex("PaymentId");

                    b.HasIndex("UserId");

                    b.ToTable("Subscriptions");
                });

            modelBuilder.Entity("Master.Models.User", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("id");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<DateOnly?>("BirthDate")
                        .HasColumnType("date")
                        .HasColumnName("birth_date");

                    b.Property<string>("City")
                        .HasMaxLength(255)
                        .IsUnicode(false)
                        .HasColumnType("varchar(255)")
                        .HasColumnName("city");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasMaxLength(255)
                        .IsUnicode(false)
                        .HasColumnType("varchar(255)")
                        .HasColumnName("email");

                    b.Property<string>("Gender")
                        .HasMaxLength(50)
                        .IsUnicode(false)
                        .HasColumnType("varchar(50)")
                        .HasColumnName("gender");

                    b.Property<string>("Image")
                        .HasMaxLength(255)
                        .IsUnicode(false)
                        .HasColumnType("varchar(255)")
                        .HasColumnName("image");

                    b.Property<string>("Location")
                        .IsRequired()
                        .HasMaxLength(255)
                        .IsUnicode(false)
                        .HasColumnType("varchar(255)")
                        .HasColumnName("location");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(255)
                        .IsUnicode(false)
                        .HasColumnType("varchar(255)")
                        .HasColumnName("name");

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasMaxLength(255)
                        .IsUnicode(false)
                        .HasColumnType("varchar(255)")
                        .HasColumnName("password");

                    b.Property<string>("Phone")
                        .IsRequired()
                        .HasMaxLength(20)
                        .IsUnicode(false)
                        .HasColumnType("varchar(20)")
                        .HasColumnName("phone");

                    b.Property<int?>("Points")
                        .HasColumnType("int")
                        .HasColumnName("points");

                    b.Property<string>("UserType")
                        .IsRequired()
                        .HasMaxLength(50)
                        .IsUnicode(false)
                        .HasColumnType("varchar(50)")
                        .HasColumnName("user_type");

                    b.HasKey("Id")
                        .HasName("users_id_primary");

                    b.HasIndex(new[] { "Email" }, "users_email_unique")
                        .IsUnique();

                    b.ToTable("Users");
                });

            modelBuilder.Entity("Master.Models.Cart", b =>
                {
                    b.HasOne("Master.Models.User", "User")
                        .WithMany("Carts")
                        .HasForeignKey("UserId")
                        .HasConstraintName("cart_user_id_foreign");

                    b.Navigation("User");
                });

            modelBuilder.Entity("Master.Models.CartItem", b =>
                {
                    b.HasOne("Master.Models.Cart", "Cart")
                        .WithMany("CartItems")
                        .HasForeignKey("CartId")
                        .IsRequired()
                        .HasConstraintName("FK_CartItem_Cart");

                    b.HasOne("Master.Models.Product", "Product")
                        .WithMany("CartItems")
                        .HasForeignKey("ProductId")
                        .IsRequired()
                        .HasConstraintName("FK_CartItem_Product");

                    b.Navigation("Cart");

                    b.Navigation("Product");
                });

            modelBuilder.Entity("Master.Models.Company", b =>
                {
                    b.HasOne("Master.Models.User", "Owner")
                        .WithMany("Companies")
                        .HasForeignKey("OwnerId")
                        .IsRequired()
                        .HasConstraintName("company_owner_id_foreign");

                    b.Navigation("Owner");
                });

            modelBuilder.Entity("Master.Models.Delivery", b =>
                {
                    b.HasOne("Master.Models.Payment", "Payment")
                        .WithMany("Deliveries")
                        .HasForeignKey("PaymentId")
                        .HasConstraintName("deliveries_payment_id_foreign");

                    b.HasOne("Master.Models.RecyclingRequest", "Request")
                        .WithMany("Deliveries")
                        .HasForeignKey("RequestId")
                        .HasConstraintName("deliveries_request_id_foreign");

                    b.Navigation("Payment");

                    b.Navigation("Request");
                });

            modelBuilder.Entity("Master.Models.Order", b =>
                {
                    b.HasOne("Master.Models.Payment", "Payment")
                        .WithMany("Orders")
                        .HasForeignKey("PaymentId")
                        .HasConstraintName("orders_payment_id_foreign");

                    b.HasOne("Master.Models.User", "User")
                        .WithMany("Orders")
                        .HasForeignKey("UserId")
                        .HasConstraintName("orders_user_id_foreign");

                    b.Navigation("Payment");

                    b.Navigation("User");
                });

            modelBuilder.Entity("Master.Models.OrderItem", b =>
                {
                    b.HasOne("Master.Models.Order", "Order")
                        .WithMany("OrderItems")
                        .HasForeignKey("OrderId")
                        .HasConstraintName("orderitems_order_id_foreign");

                    b.HasOne("Master.Models.Product", "Product")
                        .WithMany("OrderItems")
                        .HasForeignKey("ProductId")
                        .HasConstraintName("orderitems_product_id_foreign");

                    b.Navigation("Order");

                    b.Navigation("Product");
                });

            modelBuilder.Entity("Master.Models.Payment", b =>
                {
                    b.HasOne("Master.Models.User", "User")
                        .WithMany("Payments")
                        .HasForeignKey("UserId")
                        .HasConstraintName("payments_user_id_foreign");

                    b.Navigation("User");
                });

            modelBuilder.Entity("Master.Models.Product", b =>
                {
                    b.HasOne("Master.Models.Category", "Category")
                        .WithMany("Products")
                        .HasForeignKey("CategoryId")
                        .HasConstraintName("FK_Products_Category");

                    b.Navigation("Category");
                });

            modelBuilder.Entity("Master.Models.RecyclingRequest", b =>
                {
                    b.HasOne("Master.Models.User", "User")
                        .WithMany("RecyclingRequests")
                        .HasForeignKey("UserId")
                        .HasConstraintName("recyclingrequests_user_id_foreign");

                    b.Navigation("User");
                });

            modelBuilder.Entity("Master.Models.Subscription", b =>
                {
                    b.HasOne("Master.Models.Payment", "Payment")
                        .WithMany("Subscriptions")
                        .HasForeignKey("PaymentId")
                        .HasConstraintName("subscriptions_payment_id_foreign");

                    b.HasOne("Master.Models.User", "User")
                        .WithMany("Subscriptions")
                        .HasForeignKey("UserId")
                        .HasConstraintName("subscriptions_user_id_foreign");

                    b.Navigation("Payment");

                    b.Navigation("User");
                });

            modelBuilder.Entity("Master.Models.Cart", b =>
                {
                    b.Navigation("CartItems");
                });

            modelBuilder.Entity("Master.Models.Category", b =>
                {
                    b.Navigation("Products");
                });

            modelBuilder.Entity("Master.Models.Order", b =>
                {
                    b.Navigation("OrderItems");
                });

            modelBuilder.Entity("Master.Models.Payment", b =>
                {
                    b.Navigation("Deliveries");

                    b.Navigation("Orders");

                    b.Navigation("Subscriptions");
                });

            modelBuilder.Entity("Master.Models.Product", b =>
                {
                    b.Navigation("CartItems");

                    b.Navigation("OrderItems");
                });

            modelBuilder.Entity("Master.Models.RecyclingRequest", b =>
                {
                    b.Navigation("Deliveries");
                });

            modelBuilder.Entity("Master.Models.User", b =>
                {
                    b.Navigation("Carts");

                    b.Navigation("Companies");

                    b.Navigation("Orders");

                    b.Navigation("Payments");

                    b.Navigation("RecyclingRequests");

                    b.Navigation("Subscriptions");
                });
#pragma warning restore 612, 618
        }
    }
}
