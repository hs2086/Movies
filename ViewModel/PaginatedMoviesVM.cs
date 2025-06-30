using MovieApp.ViewModel;

namespace MovieApp.ViewModel
{
    public class PaginatedMoviesVM
    {
        public List<MovieVM> Movies { get; set; }
        public int PageIndex { get; set; }
        public int TotalPages { get; set; }
        public string CategoryName { get; set; }
        public int StartPage { get; set; }
        public int EndPage { get; set; }
        public bool HasPreviousPage => PageIndex > 1;
        public bool HasNextPage => PageIndex < TotalPages;
    }
}