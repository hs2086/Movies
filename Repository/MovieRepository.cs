using Microsoft.EntityFrameworkCore;
using MovieApp.Models;
namespace MovieApp.Repository;

public class MovieRepository : IMovieRepository
{
    private readonly ApplicationDbContext _context;

    public MovieRepository(ApplicationDbContext context)
    {
        this._context = context;
    }
    public Movie? GetById(int? id)
    {
        Movie? movie = _context.Movies
                            .Include(m => m.Category)
                            .FirstOrDefault(m => m.Id == id);
        return movie;
    }

    public List<Movie> GetMoviesByCategory(int? categoryId)
    {
        List<Movie> movies = _context.Movies
                                    .Include(m => m.Category)
                                    .Where(m => m.CategoryId == categoryId)
                                    .OrderByDescending(m => m.Rating)
                                    .AsEnumerable() // switch to LINQ-to-Objects here
                                    .DistinctBy(m => m.Title)
                                    .Take((categoryId == 5) ? 20 : 40)
                                    .ToList();
        return movies;
    }

    public List<Movie> GetSameCategory(int? categoryId, int? id)
    {
        List<Movie> movies = _context.Movies
                                    .Include(m => m.Category)
                                    .Where(m => m.CategoryId == categoryId && m.Id != id)
                                    .OrderByDescending(m => m.Rating)
                                    .AsEnumerable() // switch to LINQ-to-Objects here
                                    .DistinctBy(m => m.Title)
                                    .Take((categoryId == 5) ? 20 : 40)
                                    .ToList();
        return movies;
    }
    public List<Movie> GetLatestMovies()
    {
        List<Movie> movies = _context.Movies
                                    .Include(m => m.Category)
                                    .Where(m => m.ReleaseDate < DateTime.Now) // filter for the last 30 days
                                    .OrderByDescending(m => m.ReleaseDate)
                                    .AsEnumerable() // switch to LINQ-to-Objects here
                                    .DistinctBy(m => m.Title)
                                    .Take(40)
                                    .ToList();
        return movies;
    }
    public bool GetFavoriteByUserIdAndMovieId(string? userId, int? movieId)
    {
        var favorite = _context.Favorites
            .FirstOrDefault(f => f.UserId == userId && f.MovieId == movieId);
        return favorite != null;
    }

    public bool AddToFavoriteList(Favorite? favorite)
    {
        try
        {
            if (favorite == null)
            {
                return false;
            }
            _context.Favorites.Add(favorite);
            _context.SaveChanges();
            return true;
        }
        catch
        {
            return false;
        }
    }
    public List<Favorite> GetFavorite(string? id)
    {
        var favorites = _context.Favorites
        .Where(f => f.UserId == id)
        .Include(f => f.Movie)
        .OrderByDescending(f => f.DateAdded)
        .ToList();
        return favorites;
    }
}