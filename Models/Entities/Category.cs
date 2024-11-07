using System.ComponentModel.DataAnnotations;

namespace Models.Entities
{
    public class Category
    {
        [Key]
        public int Id { get; set; }
    
        public string Name { get; set; }

        public bool IsActive { get; set; }
    }
}
