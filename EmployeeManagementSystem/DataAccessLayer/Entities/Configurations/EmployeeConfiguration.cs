using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace DataAccessLayer.Entities.Configurations
{
    public class EmployeeConfiguration : IEntityTypeConfiguration<EmployeeEntity>
    {
        public void Configure(EntityTypeBuilder<EmployeeEntity> builder)
        {
            builder.HasKey(e => e.Id);
            builder.Property(e => e.Id)
                .ValueGeneratedOnAdd();

            builder.Property(e => e.Name)
                .IsRequired()
                .HasMaxLength(30);

            builder.Property(e => e.Position)
                .IsRequired()
                .HasMaxLength(30);

            builder.Property(e => e.StartDate)
                .IsRequired();

            builder.HasOne<DepartmentEntity>()
                .WithMany()
                .HasForeignKey(e => e.DepartmentId)
                .OnDelete(DeleteBehavior.SetNull);

            builder.HasData(
                new EmployeeEntity()
                {
                    Id = 1,
                    Name = "n1",
                    Position = "p1",
                    DepartmentId = 1,
                    StartDate = new DateTime(2023, 4, 7),
                },
                new EmployeeEntity()
                {
                    Id = 2,
                    Name = "n2",
                    Position = "p2",
                    DepartmentId = 2,
                    StartDate = new DateTime(2023, 7, 15),
                },
                new EmployeeEntity()
                {
                    Id = 3,
                    Name = "n2",
                    Position = "p2",
                    DepartmentId = 1,
                    StartDate = new DateTime(2023, 9, 18),
                }
                );
        }
    }
}