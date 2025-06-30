namespace MovieApp.ViewModel
{
    public class MovieVM
    {
        public int Id { get; set; } 
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime ReleaseDate { get; set; }
        public double Rating { get; set; }
        public string PosterUrl { get; set; }
        public string Category { get; set; }
        public int? CategoryId { get; set; } 
    }
}