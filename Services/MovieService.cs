
using MovieApp.Models;
using MovieApp.Repository;
using MovieApp.ViewModel;

namespace MovieApp.Services;
public class MovieService
{
    private readonly IMovieRepository movieRepository;

    public MovieService(IMovieRepository movieRepository)
    {
        this.movieRepository = movieRepository;
    }

    public MovieVM? GetMovieById(int id)
    {
        Movie? movie = movieRepository.GetById(id);
        if (movie == null)
        {
            return null;
        }
        MovieVM movieVM = new MovieVM
        {
            Id = movie.Id,
            Name = movie.Title,
            Description = movie.Description,
            ReleaseDate = movie.ReleaseDate,
            Rating = movie.Rating,
            PosterUrl = movie.PosterUrl,
            Category = movie?.Category?.Name ?? "",
            CategoryId = movie?.CategoryId
        };
        return movieVM;
    }

    public List<MovieVM> GetSameCategory(int? categoryId, int id)
    {
        List<MovieVM> movies = movieRepository.GetSameCategory(categoryId, id)
                                              .Select(m => new MovieVM
                                              {
                                                  Id = m.Id,
                                                  Name = m.Title,
                                                  Description = m.Description,
                                                  ReleaseDate = m.ReleaseDate,
                                                  Rating = m.Rating,
                                                  PosterUrl = m.PosterUrl,
                                                  Category = m?.Category?.Name ?? ""
                                              }).ToList();
        return movies;
    }

    public List<MovieVM> GetTrendingMovies(int categoryId = 5)
    {
        List<MovieVM> movieVMs = movieRepository.GetMoviesByCategory(5).Select(m => new MovieVM
        {
            Id = m.Id,
            Name = m.Title,
            Description = m.Description,
            ReleaseDate = m.ReleaseDate,
            Rating = m.Rating,
            PosterUrl = m.PosterUrl,
            Category = m?.Category?.Name ?? ""
        }
        ).ToList();
        return movieVMs;
    }

    public List<MovieVM> GetPopularMovies(int categoryId = 1)
    {
        List<MovieVM> movieVMs = movieRepository.GetMoviesByCategory(categoryId).Select(m => new MovieVM
        {
            Id = m.Id,
            Name = m.Title,
            Description = m.Description,
            ReleaseDate = m.ReleaseDate,
            Rating = m.Rating,
            PosterUrl = m.PosterUrl,
            Category = m?.Category?.Name ?? ""
        }
        ).ToList();
        return movieVMs;
    }

    public List<MovieVM> GetUpComingMovies(int categoryId = 3)  
    {
        List<MovieVM> movieVMs = movieRepository.GetMoviesByCategory(categoryId).Select(m => new MovieVM
        {
            Id = m.Id,
            Name = m.Title,
            Description = m.Description,
            ReleaseDate = m.ReleaseDate,
            Rating = m.Rating,
            PosterUrl = m.PosterUrl,
            Category = m?.Category?.Name ?? ""
        }
        ).ToList();
        return movieVMs;
    }

    public List<MovieVM> GetTopRatedMovies(int categoryId = 2)  
    {
        List<MovieVM> movieVMs = movieRepository.GetMoviesByCategory(categoryId).Select(m => new MovieVM
        {
            Id = m.Id,
            Name = m.Title,
            Description = m.Description,
            ReleaseDate = m.ReleaseDate,
            Rating = m.Rating,
            PosterUrl = m.PosterUrl,
            Category = m?.Category?.Name ?? ""
        }
        ).ToList();
        return movieVMs;
    }

    public List<MovieVM> GetNowPlayingMovies(int categoryId = 4)  
    {
        List<MovieVM> movieVMs = movieRepository.GetMoviesByCategory(categoryId).Select(m => new MovieVM
        {
            Id = m.Id,
            Name = m.Title,
            Description = m.Description,
            ReleaseDate = m.ReleaseDate,
            Rating = m.Rating,
            PosterUrl = m.PosterUrl,
            Category = m?.Category?.Name ?? ""
        }
        ).ToList();
        return movieVMs;
    }
    

    public List<MovieVM> GetLatestMovies()  
    {
        List<MovieVM> movieVMs = movieRepository.GetLatestMovies().Select(m => new MovieVM
        {
            Id = m.Id,
            Name = m.Title,
            Description = m.Description,
            ReleaseDate = m.ReleaseDate,
            Rating = m.Rating,
            PosterUrl = m.PosterUrl,
            Category = m?.Category?.Name ?? ""
        }
        ).ToList();
        return movieVMs;
    }

    public string AddToFavoriteList(string userId, int movieId)
    {
        // Check if the movie is already in the user's list
        bool existingFavorite = movieRepository.GetFavoriteByUserIdAndMovieId(userId, movieId);  
        if (existingFavorite)
        {
            return "Movie already in the favorite list.";
        }
        // Create a new Favorite object and add it to the database
        var favorite = new Favorite
        {
            UserId = userId,
            MovieId = movieId,
            DateAdded = DateTime.UtcNow
        };
        bool stored = movieRepository.AddToFavoriteList(favorite);
        if (stored)
        {
            return "Movie added successfully";
        }
        else
        {
            return "Failed to add movie to the favorite list.";
        }
    }

    public List<Favorite> GetFavoriteMovies(string userId)
    {
        return movieRepository.GetFavorite(userId);
    }
}