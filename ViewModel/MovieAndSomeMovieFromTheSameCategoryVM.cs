 namespace MovieApp.ViewModel
{
    public class MovieAndSomeMovieFromTheSameCategoryVM
    {
        public MovieVM movieVM { get; set; }
        public List<MovieVM> MoviesFromTheSameCategory { get; set; }
    }
}