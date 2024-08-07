﻿using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataAccessLayer.Entities.Configurations
{
    public class DepartmentConfiguration : IEntityTypeConfiguration<DepartmentEntity>
    {
        public void Configure(EntityTypeBuilder<DepartmentEntity> builder)
        {
            builder.HasKey(d => d.Id);
            builder.Property(d => d.Id)
                .ValueGeneratedOnAdd();

            builder.Property(d => d.Name)
                .IsRequired()
                .HasMaxLength(30);

            builder.HasData(
                new DepartmentEntity()
                {
                    Id = 1,
                    Name = "d1"
                },
                new DepartmentEntity()
                {
                    Id = 2,
                    Name = "d2"
                }
            );
        }
    }
}
