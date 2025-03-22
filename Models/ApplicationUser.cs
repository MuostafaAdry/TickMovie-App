using Microsoft.AspNetCore.Identity;

namespace MoviePoint.Models
{
    public class ApplicationUser:IdentityUser
    {
        public string? Address { get; set; }
        public string? PhoneNumber { get; set; }

        public string? ImgProfile { get; set; }
    }
}
