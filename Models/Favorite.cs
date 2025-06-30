using System.ComponentModel.DataAnnotations;

namespace MovieApp.Models;
public class Favorite
{
    [Key]
    public int Id { get; set; }
    public required string UserId { get; set; }
    public int MovieId { get; set; }
    public DateTime DateAdded { get; set; } = DateTime.UtcNow;
    public virtual ApplicationUser? User { get; set; }
    public virtual Movie? Movie { get; set; }
}