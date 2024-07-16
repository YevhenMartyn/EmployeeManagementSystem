using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataAccessLayer.Entities
{
    public class DepartmentEntity : BaseEntity
    {
        public string Name { get; set; }
    }
}
