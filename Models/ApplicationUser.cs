using Microsoft.AspNetCore.Identity;
using MovieApp.Models;
namespace MovieApp.Models;
public class ApplicationUser : IdentityUser
{
    public required string Address { get; set; }
    public DateTime JoinedAt { get; set; }
}