using System.ComponentModel.DataAnnotations.Schema;

namespace Models.Entities
{
    //[Table("Product")]
    public class Product
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string SKU { get; set; }

        public int CategoryId { get; set; }

        [ForeignKey("CategoryId")]
        public Category Category { get; set; }

        public decimal BasePrice { get; set; }

        public decimal MRP { get; set; }

        public string Description { get; set; }

        public string Currency { get; set; }

        public DateTime ManufactureDate { get; set; }

        public DateTime ExpireDate { get; set; }
    }
}
