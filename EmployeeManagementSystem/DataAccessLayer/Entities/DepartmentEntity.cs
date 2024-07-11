using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataAccessLayer.Entities
{
    public class DepartmentEntity
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}
