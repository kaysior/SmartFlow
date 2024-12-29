using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartFlow.Models
{
    public class Transaction
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("Category")]
        [Display(Name = "Kategoria")]
        [Required(ErrorMessage = "Kategoria jest wymagana.")]
        public int CategoryId { get; set; }
        public Category? Category { get; set; }

        [Display(Name = "Kwota")]
        [Required(ErrorMessage = "Kwota jest wymagana.")]
        [Range(1, int.MaxValue, ErrorMessage = "Kwota powinna być większa niż 0.")]
        public int Amount { get; set; }

        [Display(Name = "Notatka")]
        [MaxLength(75, ErrorMessage = "Notatka nie może przekraczać 75 znaków.")]
        [Column(TypeName = "varchar(75)")]
        public string? Note { get; set; }

        [Display(Name = "Data")]
        [DataType(DataType.Date)]
        [Required(ErrorMessage = "Data jest wymagana.")]
        public DateTime Date { get; set; }

        [Display(Name = "Typ transakcji")]
        [Required(ErrorMessage = "Typ transakcji jest wymagany.")]
        [Column(TypeName = "varchar(10)")]
        [RegularExpression("^(Expense|Income)$", ErrorMessage = "Typ transakcji musi być 'Expense' lub 'Income'.")]
        public string Type { set; get; } = "Expense";

        public string? userId { get; set; }

        [ForeignKey("SavingsGoal")]
        public int? SavingsGoalId { get; set; }
        public SavingsGoal? SavingsGoal { get; set; }
    }
}
