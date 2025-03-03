using MoviePoint.Models;

public class Movie
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public double Price { get; set; }
    public string? ImgUrl { get; set; }
    public string? TrailerUrl { get; set; }
    public DateOnly StartDate { get; set; }
    public DateOnly EndDate { get; set; }
    public int MovieStatus { get; set; }

    public int CinemaId   { get; set; }
    public Cinema Cinema { get; set; }
    public int CategoryId { get; set; }
    public Category Category { get; set; }
    public ICollection<ActorMovie> ActorMovies { get; set; }
}
