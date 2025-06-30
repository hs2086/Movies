using MovieApp.Models;

namespace MovieApp.ViewModel
{
    public class ProfileViewModel
    {
        public string Name { get; set; }    
        public string Username { get; set; }
        public DateTime MemberSince { get; set; }
        public List<Favorite> FavoriteMovies { get; set; } = new List<Favorite>();
        public int TotalMoviesInList { get; set; }
    }
}