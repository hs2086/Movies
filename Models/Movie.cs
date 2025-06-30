using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MovieApp.Models;
public class Movie
{
    [Key]
    public int Id { get; set; }
    public required string Title { get; set; }
    public required string Description { get; set; }
    public DateTime ReleaseDate { get; set; }
    public double Rating { get; set; }
    public required string PosterUrl { get; set; }
    [ForeignKey("Category")]
    public int CategoryId { get; set; } 
    public Category? Category { get; set; }  
}