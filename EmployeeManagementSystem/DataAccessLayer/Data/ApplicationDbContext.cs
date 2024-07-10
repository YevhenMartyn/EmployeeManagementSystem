using DataAccessLayer.Entities;
using Microsoft.EntityFrameworkCore;

namespace DataAccessLayer.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<Department> Departments { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Department>().HasData(
                new Department()
                {
                    Id = -1,
                    Name = "No Department"
                },
                new Department()
                {
                    Id = 1,
                    Name = "d1"
                },
                new Department()
                {
                    Id = 2,
                    Name = "d2"
                }
                );

            modelBuilder.Entity<Employee>().HasData(
                new Employee()
                {
                    Id = 1,
                    Name = "n1",
                    Position = "p1",
                    DepartmentId = 1,
                    StartDate = new DateTime(2023, 4, 7),
                },
                new Employee()
                {
                    Id = 2,
                    Name = "n2",
                    Position = "p2",
                    DepartmentId = 2,
                    StartDate = new DateTime(2023, 7, 15),
                },
                new Employee()
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
