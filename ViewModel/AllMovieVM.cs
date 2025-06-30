namespace MovieApp.ViewModel
{
    public class AllMovieVM
    {
        public List<MovieVM> Trending { get; set; }
        public List<MovieVM> Popular { get; set; }
        public List<MovieVM> TopRated { get; set; }
        public List<MovieVM> Upcoming { get; set; }
        public List<MovieVM> NowPlaying { get; set; }
        public List<MovieVM> Latest { get; set; }
    }
}