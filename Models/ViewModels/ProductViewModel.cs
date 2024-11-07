using System.ComponentModel.DataAnnotations;

namespace Models.ViewModels
{
    public class ProductViewModel
    {
        [Key]
        public int ProductId { get; set; }
        
        [Required(ErrorMessage = "Name is required.")]
        [StringLength(256, ErrorMessage = "Name must not be more than 256 characters.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "SKU is required.")]
        [StringLength(6, ErrorMessage = "SKU must be of exact 6 characters.")]
        public string SKU { get; set; }
        
        [Required(ErrorMessage = "Category is required.")]
        public int CategoryId { get; set; }  // Foreign Key

        [Required(ErrorMessage = "BasePrice is required.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Base Price must be a positive value.")]
        public decimal BasePrice { get; set; }

        [Required(ErrorMessage = "MRP is required.")]
        [Range(0.1, double.MaxValue, ErrorMessage = "MRP must be a positive value.")]
        public decimal MRP { get; set; }

        [Required(ErrorMessage = "Currency is required.")]
        public CurrencyEnum Currency { get; set; } 

        public string Description { get; set; }

        [Required(ErrorMessage = "Manufacture Date is required.")]
        public DateTime ManufactureDate { get; set; }

        [Required(ErrorMessage = "Expire Date is required.")]
        [CustomValidation("ManufactureDate")]
        public DateTime ExpireDate { get; set; }

    }

    public enum CurrencyEnum
    {
        INR,
        USD,
        
    }

    public class CustomValidationAttribute : ValidationAttribute
    {
        private readonly string _name;

        public CustomValidationAttribute(string mfdt)
        {
            _name = mfdt;
        }

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            var mfdtProperty = validationContext.ObjectType.GetProperty(_name);

            if (mfdtProperty == null) 
            {
                return new ValidationResult($"Invalid property name {_name} as given in custom validation attribute.");
            }

            var mfDate = (DateTime?)mfdtProperty.GetValue(validationContext.ObjectInstance);
            var expDate = (DateTime?)value;

            if (mfDate.HasValue && expDate.HasValue && expDate.Value < mfDate.Value)
            {
                return new ValidationResult("The Expire Date must be after The Manufacture Date.");
            }

            return ValidationResult.Success;
        }
    }
}
