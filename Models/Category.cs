using System.ComponentModel.DataAnnotations;

namespace MovieApp.Models;
public class Category
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public List<Movie>? Movies { get; set; } 
}