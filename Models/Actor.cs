using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;

namespace MoviePoint.Models
{
    public class Actor
    {
        public int Id { get; set; }

        [Required]
        [MinLength(3)]
        [MaxLength(20)]
        public string FirstName { get; set; }
        [Required]
        [MinLength(3)]
        [MaxLength(20)]
        public string LastName { get; set; }
        [Required]
        [MinLength(3)]
       
        public string Bio { get; set; }
        public string? ProfilePicture { get; set; }
        [Required]
        [MinLength(3)]
      
        public string News { get; set; }
        [ValidateNever]
        public ICollection<ActorMovie> ActorMovies { get; set; }
    }
}
