using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MovieApp.Models;
using MovieApp.Services;
using MovieApp.ViewModel;

namespace MovieApp.Controllers;

public class AccountController : Controller
{
    private readonly IEmailService _emailService;
    private readonly UserManager<ApplicationUser> userManager;
    private readonly SignInManager<ApplicationUser> signInManager;
    private readonly MovieService movieService;

    public AccountController(IEmailService emailService, UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, MovieService movieService)
    {
        this._emailService = emailService;
        this.userManager = userManager;
        this.signInManager = signInManager;
        this.movieService = movieService;
    }

    [HttpGet]
    public IActionResult Register()
    {
        return View(model: new RegisterVM { Step = 1 });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(RegisterVM model)
    {
        // Step-specific validation
        switch (model.Step)
        {
            case 1:
                ModelState.Remove("Password");
                ModelState.Remove("ConfirmPassword");
                ModelState.Remove("VerificationCode");

                var existingUser = await userManager.FindByEmailAsync(model.Email);
                if (existingUser != null)
                {
                    ModelState.AddModelError("Email", "This email is already in use");
                }
                break;

            case 2:
                // Clear validation for other steps' fields
                ModelState.Remove("Password");
                ModelState.Remove("ConfirmPassword");
                ModelState.Remove("FirstName");
                ModelState.Remove("LastName");
                ModelState.Remove("Email");
                ModelState.Remove("PhoneNumber");
                ModelState.Remove("Address");

                if (string.IsNullOrEmpty(model.VerificationCode))
                {
                    ModelState.AddModelError("VerificationCode", "Verification code is required");
                }
                break;

            case 3:
                ModelState.Remove("VerificationCode");
                ModelState.Remove("FirstName");
                ModelState.Remove("LastName");
                ModelState.Remove("Email");
                ModelState.Remove("PhoneNumber");
                ModelState.Remove("Address");
                break;
        }

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        switch (model.Step)
        {
            case 1:
                var verificationCode = GenerateVerificationCode();
                await _emailService.SendEmail(model.Email, verificationCode, model.FirstName);

                // Store all data in TempData
                TempData["RegisterData"] = JsonSerializer.Serialize(model);
                TempData["VerificationCode"] = verificationCode;

                // Explicitly keep TempData
                TempData.Keep("RegisterData");
                TempData.Keep("VerificationCode");

                // Move to step 2
                return View(new RegisterVM
                {
                    Step = 2,
                    Email = model.Email  // Preserve email for display
                });

            case 2:
                // Keep TempData before accessing
                TempData.Keep("RegisterData");
                TempData.Keep("VerificationCode");

                var storedData = JsonSerializer.Deserialize<RegisterVM>(TempData["RegisterData"].ToString());
                var storedCode = TempData["VerificationCode"]?.ToString();

                if (model.VerificationCode != storedCode)
                {
                    ModelState.AddModelError("VerificationCode", "Invalid verification code");
                    return View(new RegisterVM
                    {
                        Step = 2,
                        Email = storedData.Email,
                        VerificationCode = model.VerificationCode
                    });
                }

                // Move to step 3 with stored data
                return View(new RegisterVM
                {
                    Step = 3,
                    Email = storedData.Email
                });

            case 3:
                // Keep TempData before accessing
                TempData.Keep("RegisterData");

                var registerData = JsonSerializer.Deserialize<RegisterVM>(TempData["RegisterData"].ToString());

                ApplicationUser user = new ApplicationUser
                {
                    UserName = registerData.Email,
                    Email = registerData.Email,
                    PhoneNumber = registerData.PhoneNumber,
                    Address = registerData.Address + "|" + registerData.FirstName + "|" + registerData.LastName
                };

                var result = await userManager.CreateAsync(user, model.Password);

                if (!result.Succeeded)
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                    return View(new RegisterVM
                    {
                        Step = 3,
                        Email = registerData.Email
                    });
                }

                await signInManager.SignInAsync(user, isPersistent: false);
                return RedirectToAction("Index", "Home");

            default:
                return RedirectToAction("Register");
        }
    }

    private string GenerateVerificationCode()
    {
        return new Random().Next(100000, 999999).ToString();
    }

    [AcceptVerbs("GET", "POST")]
    public async Task<IActionResult> IsEmailInUse(string email)
    {
        var user = await userManager.FindByEmailAsync(email);
        return Json(user == null);
    }

    [HttpGet]
    public IActionResult Login(string returnUrl = null)
    {
        ViewData["ReturnUrl"] = returnUrl;
        return View(new LoginVM());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginVM model, string returnUrl = null)
    {
        ViewData["ReturnUrl"] = returnUrl;

        if (ModelState.IsValid)
        {
            ApplicationUser? user = await userManager.FindByNameAsync(model.Email);
            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                return View(model);
            }
            
            bool rightPassword = await userManager.CheckPasswordAsync(user, model.Password);
            if (rightPassword)
            {
                await signInManager.SignInAsync(user, isPersistent: model.RememberMe);
                return RedirectToLocal(returnUrl);
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                return View(model);
            }
        }
        return View(model);
    }

    public async Task<IActionResult> Logout()
    {
        await signInManager.SignOutAsync();
        return RedirectToAction("Index", "Home");
    }

    private IActionResult RedirectToLocal(string returnUrl)
    {
        if (Url.IsLocalUrl(returnUrl))
        {
            return Redirect(returnUrl);
        }
        else
        {
            return RedirectToAction("Index", "Home");
        }
    }

    [HttpGet]
    public IActionResult Forgot()
    {
        return View(new ResetPasswordVM { Step = 1 });
    }

    [HttpPost]
    public async Task<IActionResult> Forgot(ResetPasswordVM model)
    {
        switch (model.Step)
        {
            case 1:
                ModelState.Remove("VerificationCode");
                ModelState.Remove("NewPassword");
                ModelState.Remove("ConfirmPassword");

                var user = await userManager.FindByEmailAsync(model.Email);
                if (user == null)
                {
                    ModelState.AddModelError("Email", "No account found with this email address");
                }
                break;

            case 2: 
                ModelState.Remove("Email");
                ModelState.Remove("NewPassword");
                ModelState.Remove("ConfirmPassword");
                if (string.IsNullOrEmpty(model.VerificationCode))
                {
                    ModelState.AddModelError("VerificationCode", "Verification code is required");
                }
                else if (model.VerificationCode.Length != 6)
                {
                    ModelState.AddModelError("VerificationCode", "Code must be 6 digits");
                }
                break;

            case 3:
                ModelState.Remove("Email");
                ModelState.Remove("VerificationCode");
                if (!string.IsNullOrEmpty(model.NewPassword) && model.NewPassword.Length < 8)
                {
                    ModelState.AddModelError("NewPassword", "Password must be at least 8 characters");
                }
                break;
        }
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        switch (model.Step)
        {
            case 1: 
                var code = GenerateVerificationCode();

                HttpContext.Session.SetString("ResetCode", code);
                HttpContext.Session.SetString("ResetEmail", model.Email);

                await _emailService.SendEmail(model.Email, code, model.Email);

                model.Step = 2;
                return View(model);

            case 2: 
                var storedCode = HttpContext.Session.GetString("ResetCode");
                var storedEmail = HttpContext.Session.GetString("ResetEmail");

                if (model.VerificationCode != storedCode || model.Email != storedEmail)
                {
                    ModelState.AddModelError("VerificationCode", "Invalid verification code");
                    return View(model);
                }

                model.Step = 3;
                return View(model);

            case 3: 
                var user = await userManager.FindByEmailAsync(model.Email);
                if (user != null)
                {
                    var token = await userManager.GeneratePasswordResetTokenAsync(user);
                    var result = await userManager.ResetPasswordAsync(user, token, model.NewPassword);

                    if (result.Succeeded)
                    {
                        HttpContext.Session.Remove("ResetCode");
                        HttpContext.Session.Remove("ResetEmail");

                        return RedirectToAction("Login", new { message = "Password reset successfully" });
                    }
                }

                ModelState.AddModelError("", "Failed to reset password");
                return View(model);

            default:
                return RedirectToAction("Forgot");
        }
    }

    [Authorize]
    public async Task<IActionResult> Profile()
    {
        var user = await userManager.GetUserAsync(User);
        if (user == null)
        {
            return NotFound($"Unable to load user with ID '{userManager.GetUserId(User)}'.");
        }

        var favoriteMovies = movieService.GetFavoriteMovies(user.Id);

        var model = new ProfileViewModel
        {
            Name = user.Address.Split("|")[1] + " " + user.Address.Split("|")[2],
            Username = user.UserName,
            MemberSince = user.JoinedAt, // Add this property to your ApplicationUser
            FavoriteMovies = favoriteMovies,
            TotalMoviesInList = favoriteMovies.Count
        };

        return View(model);
    }
}