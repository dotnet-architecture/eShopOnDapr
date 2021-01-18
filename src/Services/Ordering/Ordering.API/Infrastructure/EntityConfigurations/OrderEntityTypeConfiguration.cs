﻿using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.eShopOnContainers.Services.Ordering.API.Infrastructure;
using Microsoft.eShopOnContainers.Services.Ordering.API.Model;

namespace Microsoft.eShopOnContainers.Services.Ordering.Infrastructure.EntityConfigurations
{
    class OrderEntityTypeConfiguration : IEntityTypeConfiguration<Order>
    {
        public void Configure(EntityTypeBuilder<Order> orderConfiguration)
        {
            orderConfiguration.ToTable("orders", OrderingContext.DEFAULT_SCHEMA);

            orderConfiguration.HasKey(o => o.Id);

            orderConfiguration.HasAlternateKey(o => o.OrderNumber);

            orderConfiguration.Property(o => o.OrderNumber)
                .UseHiLo("orderseq", OrderingContext.DEFAULT_SCHEMA);

            orderConfiguration
                .OwnsOne(o => o.Address, a =>
                {
                    a.WithOwner();
                });

            //orderConfiguration
            //    .Property(o => o.OrderStatus)
            //    .IsRequired();

    //        orderConfiguration.HasOne(o => o.OrderStatus)
    //.WithMany();

            //orderConfiguration
            //    .Property<int?>("_paymentMethodId")
            //    .UsePropertyAccessMode(PropertyAccessMode.Field)
            //    .HasColumnName("PaymentMethodId")
            //    .IsRequired(false);

            //orderConfiguration
            //    .Property<string>("Description")
            //    .IsRequired(false);

            //var navigation = orderConfiguration.Metadata.FindNavigation(nameof(Order.OrderItems));

            //// DDD Patterns comment:
            ////Set as field (New since EF 1.1) to access the OrderItem collection property through its field
            //navigation.SetPropertyAccessMode(PropertyAccessMode.Field);

            //orderConfiguration.HasOne<PaymentMethod>()
            //    .WithMany()
            //    // .HasForeignKey("PaymentMethodId")
            //    .HasForeignKey("_paymentMethodId")
            //    .IsRequired(false)
            //    .OnDelete(DeleteBehavior.Restrict);

            //orderConfiguration.HasOne<Buyer>()
            //    .WithMany()
            //    .IsRequired(false)
            //    // .HasForeignKey("BuyerId");
            //    .HasForeignKey("_buyerId");


            // .HasForeignKey("OrderStatusId");
            //.HasForeignKey("_orderStatusId");
        }
    }
}