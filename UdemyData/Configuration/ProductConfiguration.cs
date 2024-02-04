﻿using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UdemyCore.Models;

namespace UdemyData.Configuration
{
    public class ProductConfiguration : IEntityTypeConfiguration<Product>
    {
        public void Configure(EntityTypeBuilder<Product> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Name).IsRequired().HasMaxLength(200);
            builder.Property(x => x.Stock).IsRequired();
            builder.Property(x=>x.Price).HasColumnType("decimal(18,2)"); // toplam 18 virgülden sonra 2 yani 16-2 
            builder.Property(x => x.UserId).IsRequired();
        }
    }
}
