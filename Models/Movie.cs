using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MoviePoint.Models
{
    public class Movie
    {
        public int Id { get; set; }
        [Required]
        [MinLength(3)]
        [MaxLength(50)]
        public string Name { get; set; }

        [Required]
        [MinLength(3)]
         
        public string? Description { get; set; }
        [Required]
        [Range(0,100)]
        public double Price { get; set; }
        
        public string? ImgUrl { get; set; }
        public string? TrailerUrl { get; set; }

        [Required]
        
        public DateOnly StartDate { get; set; }

        [Required]
        
        public DateOnly EndDate { get; set; }
        [Required]
         
        public int MovieStatus { get; set; }
        [ValidateNever]
        public int CinemaId { get; set; }
        [ValidateNever]
        public Cinema Cinema { get; set; }

        [ValidateNever]
        public int CategoryId { get; set; }
        [ValidateNever]
        public Category Category { get; set; }

        [ValidateNever]
        public ICollection<ActorMovie> ActorMovies { get; set; }



    }
}
