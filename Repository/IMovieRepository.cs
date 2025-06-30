using MovieApp.Models;

namespace MovieApp.Repository;
public interface IMovieRepository
{
    Movie? GetById(int? id);
    public List<Movie> GetSameCategory(int? categoryId, int? id);
    List<Movie> GetMoviesByCategory(int? categoryId);
    List<Movie> GetLatestMovies();
    bool GetFavoriteByUserIdAndMovieId(string? userId, int? movieId);
    bool AddToFavoriteList(Favorite? favorite);
    List<Favorite> GetFavorite(string? id);
}