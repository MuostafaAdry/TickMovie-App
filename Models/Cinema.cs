using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;

namespace MoviePoint.Models
{
    public class Cinema
    {
        public int Id { get; set; }
        [Required]
        [MinLength(3)]
        [MaxLength(50)]
        public string Name { get; set; }


        [Required]
        [MinLength(3)]
       
        public string? Description { get; set; }  
        public string? CinemaLogo { get; set; }
        [Required]
        [MinLength(3)]
        [MaxLength(255)]
        public string Address { get; set; }
        [ValidateNever]
        public List<Movie> Moives { get; set; }
    }
}

