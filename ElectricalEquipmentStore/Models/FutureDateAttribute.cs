using System.ComponentModel.DataAnnotations;

namespace ElectricalEquipmentStore.Models
{
    public class FutureDateAttribute : ValidationAttribute
    {
        protected override System.ComponentModel.DataAnnotations.ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value is DateTime date)
            {
                if (date > DateTime.Now)
                {
                    return System.ComponentModel.DataAnnotations.ValidationResult.Success;
                }
                else
                {
                    return new System.ComponentModel.DataAnnotations.ValidationResult("Дата должна быть в будущем");
                }
            }
            return new System.ComponentModel.DataAnnotations.ValidationResult("Некорректная дата");
        }
    }
}
