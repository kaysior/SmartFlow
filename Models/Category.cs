using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace SmartFlow.Models
{
    public class Category
    {
        [Key]
        public int CategoryId { get; set; }

        [Display(Name = "Tytuł")]
        [Required(ErrorMessage = "Tytuł jest wymagany.")]
        [MaxLength(50, ErrorMessage = "Tytuł nie może przekraczać 50 znaków.")]
        [Column(TypeName = "varchar(50)")]
        public string Title { set; get; }

        [Display(Name = "Ikona")]
        [MaxLength(50, ErrorMessage = "Ikona nie może przekraczać 50 znaków.")]
        [Column(TypeName = "varchar(50)")]
        public string Icon { set; get; } = "";

        [NotMapped]
        [Display(Name = "Tytuł z ikoną")]
        public string TitleWithIcon
        {
            get
            {
                return this.Icon + " " + this.Title;
            }
        }

        public string? userId { set; get; }

        public IdentityUser? User { set; get; }
    }
}
