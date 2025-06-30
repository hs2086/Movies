using System.Diagnostics;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MovieApp.Models;
using MovieApp.Services;
using MovieApp.ViewModel;

namespace MovieApp.Controllers;

public class HomeController : Controller
{
    private readonly MovieService movieService;
private readonly ApplicationDbContext _context;

public HomeController(MovieService movieService, ApplicationDbContext context)
{
    this.movieService = movieService;
    this._context = context;
}

public IActionResult Index()
{
    // Geting Movies from the database
    AllMovieVM allMovieVM = new AllMovieVM();
    allMovieVM.Trending = movieService.GetTrendingMovies();
    allMovieVM.Popular = movieService.GetPopularMovies();
    allMovieVM.TopRated = movieService.GetTopRatedMovies();
    allMovieVM.Upcoming = movieService.GetUpComingMovies();
    allMovieVM.NowPlaying = movieService.GetNowPlayingMovies();
    allMovieVM.Latest = movieService.GetLatestMovies();
    return View(allMovieVM);
}

public IActionResult Show(int id) 
{
    MovieAndSomeMovieFromTheSameCategoryVM movieAndSomeMovieFromTheSameCategoryVM = new MovieAndSomeMovieFromTheSameCategoryVM();
    movieAndSomeMovieFromTheSameCategoryVM.movieVM = movieService.GetMovieById(id);
    movieAndSomeMovieFromTheSameCategoryVM.MoviesFromTheSameCategory = movieService.GetSameCategory(movieAndSomeMovieFromTheSameCategoryVM.movieVM.CategoryId, id);
    return View(movieAndSomeMovieFromTheSameCategoryVM);
}

public IActionResult AddList(int id)
{
    if (!User.Identity.IsAuthenticated)
    {
        return RedirectToAction("Login", "Account", new { returnUrl = Url.Action("AddList", new { id }) });
    }

    try
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (id <= 0)
        {
            TempData["AlertMessage"] = "Invalid movie ID";
            TempData["AlertType"] = "danger";
            return RedirectToAction("Show", "Home", new { id });
        }

        string message = movieService.AddToFavoriteList(userId, id);

        TempData["AlertMessage"] = message;
        TempData["AlertType"] = message.Contains("successfully") ? "success" : "warning";

        return RedirectToAction("Show", "Home", new { id });
    }
    catch (Exception ex)
    {
        TempData["AlertMessage"] = "An error occurred while adding the movie to your list";
        TempData["AlertType"] = "danger";
        return RedirectToAction("Show", "Home", new { id });
    }
}

[HttpPost]
[Authorize]
public async Task<IActionResult> RemoveFromFavorites(int movieId)
{
    try
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        // Find the favorite to remove
        var favorite = await _context.Favorites
            .FirstOrDefaultAsync(f => f.UserId == userId && f.MovieId == movieId);

        if (favorite == null)
        {
            TempData["AlertMessage"] = "Movie not found in your favorites";
            TempData["AlertType"] = "warning";
            return RedirectToAction("Profile", "Account");
        }

        // Remove the favorite
        _context.Favorites.Remove(favorite);
        await _context.SaveChangesAsync();

        TempData["AlertMessage"] = "Movie removed from favorites successfully";
        TempData["AlertType"] = "success";
    }
    catch (Exception ex)
    {
        TempData["AlertMessage"] = "An error occurred while removing the movie";
        TempData["AlertType"] = "danger";
    }

    return RedirectToAction("Profile", "Account");
}

[HttpGet]
public IActionResult Trending(int category, int page = 1, int pageSize = 10)
{
    ViewBag.CurrentCategory = category;

    var totalMovies = _context.Movies.Count(m => m.CategoryId == category);
    var totalPages = (int)Math.Ceiling(totalMovies / (double)pageSize);

    const int maxPagesToShow = 10;
    var startPage = Math.Max(1, page - maxPagesToShow / 2);
    var endPage = Math.Min(totalPages, startPage + maxPagesToShow - 1);
    startPage = Math.Max(1, endPage - maxPagesToShow + 1);

    var movies = _context.Movies
        .Where(m => m.CategoryId == category)
        .OrderByDescending(m => m.Rating)
        .Skip((page - 1) * pageSize)
        .Take(pageSize)
        .Select(m => new MovieVM
        {
            Id = m.Id,
            Name = m.Title,
            PosterUrl = m.PosterUrl,
            Rating = m.Rating,
            ReleaseDate = m.ReleaseDate
        })
        .ToList();

    var viewModel = new PaginatedMoviesVM
    {
        Movies = movies,
        PageIndex = page,
        TotalPages = totalPages,
        CategoryName = _context.Categories.FirstOrDefault(c => c.Id == category)?.Name ?? "Movies",
        StartPage = startPage,
        EndPage = endPage
    };

    return View(viewModel);
}

[HttpGet]
public async Task<IActionResult> SearchSuggestions(string term)
{
    var suggestions = await _context.Movies
        .Where(m => m.Title.Contains(term))
        .OrderByDescending(m => m.Rating)
        .Take(8)
        .Select(m => new {
            id = m.Id,
            title = m.Title,
            poster = m.PosterUrl
        })
        .ToListAsync();

    return Json(suggestions);
}
}
