using DataAccessLayer.Entities;
using Microsoft.EntityFrameworkCore;

namespace DataAccessLayer.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }
        public DbSet<EmployeeEntity> Employees { get; set; }
        public DbSet<DepartmentEntity> Departments { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<DepartmentEntity>().HasData(
                new DepartmentEntity()
                {
                    Id = -1,
                    Name = "No Department"
                },
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

            modelBuilder.Entity<EmployeeEntity>().HasData(
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
