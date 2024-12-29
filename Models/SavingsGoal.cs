using Microsoft.AspNetCore.Identity;
using System;
using System.ComponentModel.DataAnnotations;

namespace SmartFlow.Models
{
    public class SavingsGoal
    {
        public int Id { get; set; }

        [Display(Name = "Nazwa celu")]
        [Required(ErrorMessage = "Nazwa celu jest wymagana.")]
        [MaxLength(100, ErrorMessage = "Nazwa celu nie może przekraczać 100 znaków.")]
        public string GoalName { get; set; }

        [Display(Name = "Docelowa kwota")]
        [Required(ErrorMessage = "Docelowa kwota jest wymagana.")]
        [Range(0.01, 1000000, ErrorMessage = "Docelowa kwota musi być większa niż 0.")]
        public decimal TargetAmount { get; set; }

        [Display(Name = "Aktualna kwota")]
        [Range(0, double.MaxValue, ErrorMessage = "Aktualna kwota nie może być ujemna.")]
        public decimal CurrentAmount { get; set; }

        [Display(Name = "Data realizacji celu")]
        [DataType(DataType.Date)]
        [Required(ErrorMessage = "Data realizacji celu jest wymagana.")]
        [FutureDate(ErrorMessage = "Data realizacji celu musi być w przyszłości.")]
        public DateTime TargetDate { get; set; }

        public string? userId { set; get; }

        public IdentityUser? User { set; get; }
    }

    public class FutureDateAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value is DateTime dateValue)
            {
                if (dateValue.Date <= DateTime.Today)
                {
                    return new ValidationResult(ErrorMessage ?? "Data musi być późniejsza niż dzisiaj.");
                }
            }

            return ValidationResult.Success;
        }
    }
}
